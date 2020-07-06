using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Block : MonoBehaviour
{
    public enum BlockType
    {
        STANDARD,
        ATTACK
    }

    // Variable
    [SerializeField] public BlockParameter param;

    int id;
    BlockType blockType;
    List<int> pairs;        // store pairs by block's id

    // Method

    /// <summary>
    /// This function link this block to target block.
    /// </summary>
    /// <param name="block">target block</param>
    void LinkBlockTo(Block block)
    {
        if(!this.CheckLinkable())
        {
            Debug.Log("This Block is full. [ID : " + this.id + " ]");
            return;
        }

        if(!block.CheckLinkable())
        {
            Debug.Log("Target Block is full. [ID : " + block.id + " ]");
            return;
        }

        this.pairs.Add(block.id);
        block.pairs.Add(this.id);
    }

    bool CheckLinkable()
    {
        return pairs.Count < param.maxPairs;
    }

    protected void Start()
    {
        pairs = new List<int>();

    }

    void Update()
    {
        
    }
}
