using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Block : MonoBehaviour
{
    // Static Variable
    public enum BlockType
    {
        STANDARD,
        ATTACK,
        SPHERE
    }
    public const int EMPTYID = -1;
    public const int DUMMYID = -2;
    static int idCount = 0;

    // Variable
    [SerializeField] private BlockType blockType;
    [SerializeField] private List<BlockType> linkableBlockType;
    public int id { get; private set; }         // id is set automatically in Start(). Readonly. (Note: id starts from 0)
    [HideInInspector] public int maxPairs;
    [HideInInspector] public List<int> pairs;                     // store pairs by block's id
    [HideInInspector] public bool isMoving;                       // animating now
    [HideInInspector] public bool isAlone;                        // alone or linking boss

    // Boss Animation
    public float distance;
    public float duration;
    public Ease ease;
    private Tween idleTween;

    private BlockManager manager;
    private Sequence movingSeqence;
    private Block parent;

    // DEBUG
    bool DEBUG_MODE = false;

    // Method
    void Start()
    {
        // Initialize variables
        maxPairs = 0;
        foreach (Transform child in transform)
        {
            if (child.CompareTag("ConnectPoint")) maxPairs++;
        }

        pairs = new List<int>();
        for (int i = 0; i < maxPairs; i++)
        {
            pairs.Add(EMPTYID);
        }

        id = idCount++;
        isMoving = false;
        isAlone = true;
        manager = GameObject.Find("BlockManager").GetComponent<BlockManager>();
    }

    /// <summary>
    /// This function link this block to target block.
    /// </summary>
    /// <param name="block">target block</param>
    public void LinkBlockTo(Block block)
    {
        // check if this block can be linked to the block
        if (!this.CheckLinkable(block) || !block.CheckLinkable(this)) return;

        // link at random point
        var emptyIndex = new List<int>();
        for (int i = 0; i < pairs.Count; i++)
        {
            if (pairs[i] == EMPTYID) emptyIndex.Add(i);
        }
        int rand = Random.Range(0, emptyIndex.Count);
        pairs[emptyIndex[rand]] = block.id;

        emptyIndex.Clear();
        for (int i = 0; i < block.pairs.Count; i++)
        {
            if (block.pairs[i] == EMPTYID) emptyIndex.Add(i);
        }
        rand = Random.Range(0, emptyIndex.Count);
        block.pairs[emptyIndex[rand]] = this.id;

        // set parent
        parent = block;
        
        if(DEBUG_MODE) Debug.Log($"Block[id:{this.id}] is linked to block[id:{block.id}].");
    }

    /// <summary>
    /// Cut block linking
    /// </summary>
    /// <param name="block"></param>
    public void CutLinkBlockTo(Block block)
    {
        if (block == parent) parent = null;

        int index = pairs.FindIndex(id => id == block.id);
        if (index == -1)
        {
            Debug.Log("Cannnot find the id in the pairs.");
            return;
        }
        pairs[index] = EMPTYID;

        index = block.pairs.FindIndex(id => id == this.id);
        if (index == -1)
        {
            Debug.Log("Cannnot find the id in the pairs.");
            return;
        }
        block.pairs[index] = EMPTYID;
    }

    /// <summary>
    /// Check if this block can link another one.
    /// </summary>
    /// <returns></returns>
    public bool CheckLinkable(Block block)
    {
        int count = GetPairsCount();
        if (count >= maxPairs)
        {
            if(DEBUG_MODE) Debug.Log("This Block is full. [ID : " + this.id + " ]");
            return false;
        }

        if(!linkableBlockType.Contains(block.blockType))
        {
            if(DEBUG_MODE) Debug.Log("This Block [Type: " + this.blockType + "] cannot be linked to the block [Type: " + block.blockType + "].");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Add dummy block at the connectPoint because the position is overlapped by another block
    /// </summary>
    /// <param name="block">find the block from pairs and exchange it to dummy block</param>
    public void AddDummyBlock(Block block)
    {
        int index = pairs.FindIndex(id => id == block.id);
        if(index == -1)
        {
            Debug.Log("Cannnot find the id in the pairs.");
            return;
        }
        pairs[index] = DUMMYID;
        
        if(DEBUG_MODE) Debug.Log("Add Dummy to Block[id:" + this.id + "]");

        index = block.pairs.FindIndex(id => id == this.id);
        if (index == -1)
        {
            Debug.Log("Cannnot find the id in the pairs.");
            return;
        }
        block.pairs[index] = EMPTYID;
    }

    public void MoveTo(Block block, float speed)
    {
        
        Transform connectPointThis = GetConnectPoint(this, block.id);   // Get the connect point in this block
        Transform connectPointTarget = GetConnectPoint(block, this.id); // Get the connect point in the target block

        // Calculate relative position and rotation from Child to Parent
        Vector3 c2pPos = - connectPointThis.localPosition;
        Quaternion c2pRot = Quaternion.Inverse(connectPointThis.localRotation);

        // Move parent's position to a point where both connection points match
        Vector3 toPosition = connectPointTarget.position + Quaternion.AngleAxis(180, transform.up) * connectPointTarget.rotation * c2pRot * c2pPos;
        Vector3 toRotation = (Quaternion.AngleAxis(180, transform.up) * connectPointTarget.rotation * c2pRot).eulerAngles;

        isMoving = true;
        movingSeqence = DOTween.Sequence();
        movingSeqence.Append(
            this.transform.DOMove(toPosition, speed)
        );
        movingSeqence.Join(
            this.transform.DORotate(toRotation, speed)
        );
        movingSeqence.OnComplete(() =>
        {
            isMoving = false;

            // check overlap
            if (CheckOverlap())
            {
                isAlone = true;
                block.AddDummyBlock(this);
            }
            else
            {
                isAlone = false;

                Transform point = GetConnectPoint(this, parent.id);
                idleTween = point.DOLocalMoveZ(point.localPosition.z + distance, duration).SetEase(ease).SetLoops(-1, LoopType.Yoyo);
                idleTween.Pause();
            }
        });
    }

    /// <summary>
    /// Get connect point's transform
    /// </summary>
    /// <param name="block">block which has connect points</param>
    /// <param name="id">target block's id</param>
    /// <returns>connect point's transform</returns>
    private Transform GetConnectPoint(Block block, int id)
    {
        int connectPointIndex = block.pairs.FindIndex(i => i == id);
        if (connectPointIndex == -1)
        {
            Debug.Log("Cannnot find the target block id in the pairs.");
            return null;
        }
        var connectPoints = new List<Transform>();
        foreach (Transform child in block.GetComponent<Transform>())
        {
            if (child.CompareTag("ConnectPoint")) connectPoints.Add(child);
        }
        return connectPoints[connectPointIndex];
    }

    public void StopMoving()
    {
        movingSeqence.Kill();
        isMoving = false;
        isAlone = true;
    }

    /// <summary>
    /// Check if this block overlaps with another block
    /// </summary>
    /// <returns>True : overlap, False : not overlap</returns>
    public bool CheckOverlap()
    {
        Vector3 castPos = transform.position + new Vector3(0, 3.0f, 0);
        RaycastHit[] hits = Physics.BoxCastAll(castPos, transform.localScale * 0.5f - new Vector3(manager.overlapMargin, manager.overlapMargin, manager.overlapMargin), new Vector3(0, -1, 0), Quaternion.identity, Mathf.Infinity, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            //Debug.Log("Hit Info: " + hit.collider.name, hit.collider);
            if (hit.collider.gameObject == this.gameObject) continue;       // ignore myself
            if (hit.collider.GetComponent<Block>().isMoving) continue;      // ignore moving block
            return true;
        }
        return false;
    }

    /// <summary>
    /// Get how many blocks link to this block
    /// </summary>
    /// <returns>block count</returns>
    public int GetPairsCount()
    {
        int count = 0;
        foreach (var id in pairs)
        {
            if (id != EMPTYID) count++;
        }
        return count;
    }

    /// <summary>
    /// Get how many blocks can link to this block left
    /// </summary>
    /// <returns>capacity</returns>
    public int GetCapacity()
    {
        return maxPairs - GetPairsCount();
    }

    public void Death()
    {
        StopMoving();
        manager.DeathBlock(this);
    }

    /*--------------------------------------- 
     *  Animation 
     * ------------------------------------*/

    void Update()
    {
        if (!isAlone && !isMoving)
        {
            FollowParent();
            idleTween.Play();
        }
        else
        {
            idleTween.Pause();
        }

    }

    void FollowParent()
    {
        if (parent == null) return;

        Transform connectPointThis = GetConnectPoint(this, parent.id);
        Transform connectPointTarget = GetConnectPoint(parent, this.id);

        // Calculate relative position and rotation from Child to Parent
        Vector3 c2pPos = -connectPointThis.localPosition;
        Quaternion c2pRot = Quaternion.Inverse(connectPointThis.localRotation);

        // Move parent's position to a point where both connection points match
        Vector3 toPosition = connectPointTarget.position + Quaternion.AngleAxis(180, transform.up) * connectPointTarget.rotation * c2pRot * c2pPos;
        Quaternion toRotation = Quaternion.AngleAxis(180, transform.up) * connectPointTarget.rotation * c2pRot;

        transform.position = toPosition;
        transform.rotation = toRotation;
    }
}
