using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Block : MonoBehaviour
{
    // Static Variable
    public enum BlockType
    {
        Cube,
        Cylinder,
        Pillar,
        Sphere
    }

    public const int EMPTYID = -1;
    public const int DUMMYID = -2;
    static int idCount = 0;

    // Variable
    [SerializeField] private BlockType blockType;
    [SerializeField] private List<BlockType> linkableBlockType;
    [SerializeField] private GameObject deathParticlePrefab;
    private GameObject deathParticle;
    public int id { get; private set; }         // id is set automatically in Start(). Readonly. (Note: id starts from 0)
    public bool isCore;                         // core or not (related to clear check)
    [HideInInspector] public int maxPairs;
    public List<int> pairs;                     // store pairs by block's id
    [HideInInspector] public bool isMoving;     // animating now
    // alone or linking boss
    private bool isAlone;
    public bool IsAlone
    {
        get { return isAlone; }
        set { 
            isAlone = value;
            if(value)
            {
                this.transform.parent = null;
            }
            else
            {
                this.transform.parent = manager.boss.transform;
            }
        }
    }
    private BoxCollider mycollider;
    private List<int> emptyIndex;

    // Link Animation
    Block targetBlock;
    Transform targetBlockTransform;
    float moveSpeed;
    Transform connectPointThis;
    Transform connectPointTarget;

    // Alone Animation
    private float randomSeed;
    private Vector3 direction;

    private BlockManager manager;
    private BossController bossController;

    // DEBUG
    bool DEBUG_MODE = false;

    // Method
    void Awake()
    {
        // Initialize variables
        manager = GameObject.Find("BlockManager").GetComponent<BlockManager>();
        bossController = GameObject.Find("Boss").GetComponent<BossController>();
        mycollider = GetComponent<BoxCollider>();
        randomSeed = Random.value;
        emptyIndex = new List<int>();
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

        //deathParticle = Instantiate(deathParticlePrefab);
        //deathParticle.SetActive(false);
        id = idCount++;
        isMoving = false;
        isAlone = true;
        direction = new Vector3(Mathf.Cos(Random.value * 2 * Mathf.PI), 0, Mathf.Sin(Random.value * 2 * Mathf.PI)).normalized;
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
        emptyIndex.Clear();
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

        if(DEBUG_MODE) Debug.Log($"Block[id:{this.id}] is linked to block[id:{block.id}].");
    }

    /// <summary>
    /// Cut block linking
    /// </summary>
    /// <param name="block"></param>
    public void CutLinkBlockTo(Block block)
    {
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

        if (GetPairsCount() == 0) IsAlone = true;
        if (block.GetPairsCount() == 0) block.IsAlone = true;
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

    public void MoveTo(Block block, float moveSpeed)
    {
        targetBlock = block;
        targetBlockTransform = block.transform;
        this.moveSpeed = moveSpeed;
        connectPointThis = GetConnectPoint(this, block.id);   // Get the connect point in this block
        connectPointTarget = GetConnectPoint(block, this.id); // Get the connect point in the target block

        isMoving = true;
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
        isMoving = false;
        IsAlone = true;
    }

    /// <summary>
    /// Check if this block overlaps with another block
    /// </summary>
    /// <returns>True : overlap, False : not overlap</returns>
    public bool CheckOverlap()
    {
        Vector3 castPos = transform.position + new Vector3(0, 3.0f, 0);
        RaycastHit[] hits = Physics.BoxCastAll(castPos, (mycollider.size - new Vector3(manager.overlapMargin, manager.overlapMargin, manager.overlapMargin)) * 0.5f, new Vector3(0, -1, 0), transform.rotation, Mathf.Infinity, LayerMask.GetMask("Enemy"));
        foreach (var hit in hits)
        {
            //Debug.Log("Hit Info: " + hit.collider.name, hit.collider);
            if (hit.collider.gameObject == this.gameObject) continue;       // ignore myself
            Block b = hit.collider.GetComponent<Block>();
            if (b.isMoving || b.IsAlone) continue;      // ignore moving block
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
        //deathParticle.transform.position = transform.position;
        //deathParticle.SetActive(true);
        deathParticle = Instantiate(deathParticlePrefab,this.transform.position,Quaternion.identity);
        manager.DeathBlock(this);
        Destroy(this.gameObject);
    }

    /*--------------------------------------- 
     *  Animation 
     * ------------------------------------*/

    void Update()
    {
        if (!IsAlone && !isMoving)
        {
        }
        // when block is alone
        else if(IsAlone && !isMoving)
        {
            AloneAnimation();
        }
        else if(isMoving)
        {
            LinkAnimation();
        }
        else
        {
        }

    }

    void OnEnable()
    {
        transform.GetComponentInChildren<BlockAnimator>().enabled = true;
        transform.GetComponentInChildren<SpammerController>().enabled = true;
    }
    void OnDisable()
    {
        transform.GetComponentInChildren<BlockAnimator>().enabled = false;
        transform.GetComponentInChildren<SpammerController>().enabled = false;
    }

    void AloneAnimation()
    {
        float rand = Mathf.PerlinNoise(Time.time * 0.1f, randomSeed * 256);
        float angle = (rand * 2f - 1f) * manager.randomWalkCurveStrength;
        Vector3 randomVector = Quaternion.Euler(0, angle, 0) * direction;
        direction = (direction + randomVector + GetRepulsiveWallVector()).normalized;
        transform.position += (direction * manager.randomWalkSpeed) * Time.deltaTime;
    }

    Vector3 GetRepulsiveWallVector()
    {
        Vector3 velocity = Vector3.zero;
        Vector3 pos = transform.position;
        float posX = pos.x - bossController.center.x;
        float posZ = pos.z - bossController.center.z;
        float halfLengthX = bossController.lengthX / 2;
        float halfLengthZ = bossController.lengthZ / 2;

        if (posX > 0) velocity.x -= manager.repulsiveWallStrength / ((halfLengthX - posX) / halfLengthX);
        if (posX < 0) velocity.x -= manager.repulsiveWallStrength / ((-halfLengthX - posX) / halfLengthX);
        if (posZ > 0) velocity.z -= manager.repulsiveWallStrength / ((halfLengthZ - posZ) / halfLengthZ);
        if (posZ < 0) velocity.z -= manager.repulsiveWallStrength / ((-halfLengthZ - posZ) / halfLengthZ);

        return velocity;
    }

    void LinkAnimation()
    {
        // Decide Position
        float distance = Vector3.Distance(transform.position, connectPointThis.position);
        Vector3 p2cTarget = (connectPointTarget.position - targetBlockTransform.position).normalized;
        Vector3 toPosition = connectPointTarget.position + p2cTarget * distance;

        // Decide Rotation
        Quaternion toRotation = Quaternion.FromToRotation(connectPointThis.position - transform.position, connectPointTarget.position - toPosition) * transform.rotation;
        if (toRotation.eulerAngles.z > 1)
        {
            toRotation = Quaternion.AngleAxis(180, transform.forward) * toRotation;
        }

        // Move and Rotation
        float dis = Vector3.Distance(transform.position, toPosition);
        float present = (Time.deltaTime * moveSpeed) / dis;

        transform.position = Vector3.Lerp(transform.position, toPosition, present);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, present);

        if(present > 1)
        {
            isMoving = false;

            //check overlap
            if (CheckOverlap())
            {
                IsAlone = true;
                targetBlock.AddDummyBlock(this);
            }
            else
            {
                IsAlone = false;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (this.isCore)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, 4, 0), transform.localScale.x);
        }
    }
}
