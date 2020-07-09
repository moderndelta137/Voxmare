using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Block : MonoBehaviour
{
    // Static Variable
    public enum BlockType
    {
        STANDARD,
        ATTACK
    }

    static int idCount = 0;


    // Variable
    [SerializeField] public BlockParameter param;

    public int id { get; private set; }       // id is set automatically in Start(). Readonly. (Note: id starts from 0)
    protected BlockType blockType;          // blockType is set in Child Class's Start().
    public List<int> pairs;                        // store pairs by block's id

    // Method

    /// <summary>
    /// This function link this block to target block.
    /// </summary>
    /// <param name="block">target block</param>
    public void LinkBlockTo(Block block)
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

    /// <summary>
    /// Check if this block can link another one.
    /// </summary>
    /// <returns></returns>
    bool CheckLinkable()
    {
        return pairs.Count < param.maxPairs;
    }

    public void MoveTo(Block block)
    {
        // Get the connect point in this block
        int connectPointIndex = pairs.FindIndex(id => id == block.id);
        if(connectPointIndex == -1)
        {
            Debug.Log("Cannnot find the target block id in the pairs.");
            return;
        }
        Transform connectPointThis = this.transform.GetChild(connectPointIndex);


        // Get the connect point in the target block
        connectPointIndex = block.pairs.FindIndex(id => id == this.id);
        if (connectPointIndex == -1)
        {
            Debug.Log("Cannnot find the block id in the taget block's pairs.");
            return;
        }
        Transform connectPointTarget = block.transform.GetChild(connectPointIndex);

        // Move this block
        // Calculate relative position and rotation from Child to Parent
        Vector3 c2pPos = - connectPointThis.localPosition;
        Quaternion c2pRot = Quaternion.Inverse(connectPointThis.localRotation);

        // Move parent's position to a point where both connection points match
        this.transform.position = connectPointTarget.position + Quaternion.AngleAxis(180, transform.up) * connectPointTarget.rotation * c2pRot * c2pPos;
        this.transform.rotation = Quaternion.AngleAxis(180, transform.up) * connectPointTarget.rotation * c2pRot;
    }

    virtual protected void Start()
    {
        pairs = new List<int>();
        this.id = idCount++;
    }

    void Update()
    {
        
    }
}
