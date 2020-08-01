﻿using System.Collections;
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
    public List<int> pairs;                     // store pairs by block's id
    public bool isMoving;                       // animating now

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

    private Collider mycollider;

    // Boss Animation
    public float distance;
    public float duration;
    public Ease ease;
    private Tween idleTween;

    // Alone Animation
    private float randomSeed;

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
        IsAlone = true;
        manager = GameObject.Find("BlockManager").GetComponent<BlockManager>();
        randomSeed = Random.value;
        mycollider = GetComponent<Collider>();
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
        if (block.parent == this) block.parent = null;

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

    public void MoveTo(Block block, float speed)
    {
        
        Transform connectPointThis = GetConnectPoint(this, block.id);   // Get the connect point in this block
        Transform connectPointTarget = GetConnectPoint(block, this.id); // Get the connect point in the target block

        // Decide Position
        float distance = (transform.position - connectPointThis.position).magnitude;
        Vector3 p2cTarget = (connectPointTarget.position - block.transform.position).normalized;
        Vector3 toPosition = connectPointTarget.position + p2cTarget * distance;

        // Decide Rotation
        Quaternion toRotation = Quaternion.FromToRotation(connectPointThis.position - transform.position, connectPointTarget.position - toPosition) * transform.rotation;
        if(toRotation.eulerAngles.z > 1)
        {
            toRotation = Quaternion.AngleAxis(180, transform.forward) * toRotation;
        }

        isMoving = true;
        movingSeqence = DOTween.Sequence();
        movingSeqence.Append(
            this.transform.DOMove(toPosition, speed)
        );
        movingSeqence.Join(
            this.transform.DORotate(toRotation.eulerAngles, speed)
        );
        movingSeqence.OnComplete(() =>
        {
            isMoving = false;

            // check overlap
            if (CheckOverlap())
            {
                IsAlone = true;
                block.AddDummyBlock(this);
            }
            else
            {
                IsAlone = false;

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
        IsAlone = true;
    }

    /// <summary>
    /// Check if this block overlaps with another block
    /// </summary>
    /// <returns>True : overlap, False : not overlap</returns>
    public bool CheckOverlap()
    {
        Vector3 castPos = transform.position + new Vector3(0, 3.0f, 0);
        RaycastHit[] hits = Physics.BoxCastAll(castPos, (mycollider.bounds.size - new Vector3(manager.overlapMargin, manager.overlapMargin, manager.overlapMargin)) * 0.5f, new Vector3(0, -1, 0), Quaternion.identity, Mathf.Infinity, LayerMask.GetMask("Enemy"));

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
        manager.DeathBlock(this);
    }

    /*--------------------------------------- 
     *  Animation 
     * ------------------------------------*/

    void Update()
    {
        if (!IsAlone && !isMoving)
        {
            FollowParent();
            //idleTween.Play();
        }
        // when block is alone
        else if(IsAlone && !isMoving)
        {
            idleTween.Pause();
            float radius = manager.radius + manager.fluctuation * (Mathf.PerlinNoise(Time.time * manager.rotateSpeed, randomSeed * 255) - 0.5f);
            float theta = Time.time * manager.rotateSpeed + randomSeed * 2 * Mathf.PI / manager.rotateSpeed;

            float x = radius * Mathf.Sin(theta);
            float y = transform.position.y;
            float z = radius * Mathf.Cos(theta);

            Vector3 toPos = manager.center + new Vector3(x, y, z);
            Vector3 toVec = (toPos - transform.position);
            transform.position += toVec * manager.speed;
            //transform.position = toPos;
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

        // Decide Position
        float distance = (transform.position - connectPointThis.position).magnitude;
        Vector3 p2cTarget = (connectPointTarget.position - parent.transform.position).normalized;
        Vector3 toPosition = connectPointTarget.position + p2cTarget * distance;

        // Decide Rotation
        Quaternion toRotation = Quaternion.FromToRotation(connectPointThis.position - transform.position, connectPointTarget.position - toPosition) * transform.rotation;
        if (toRotation.eulerAngles.z > 1)
        {
            toRotation = Quaternion.AngleAxis(180, transform.forward) * toRotation;
        }

        transform.position = toPosition;
        transform.rotation = toRotation;
    }
}
