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
    const int EMPTYID = -1;
    const int DUMMYID = -2;


    static int idCount = 0;



    // Variable
    [SerializeField] public SpammerController spammer;
    [SerializeField] public BlockParameter param;

    public int id { get; private set; }         // id is set automatically in Start(). Readonly. (Note: id starts from 0)
    protected BlockType blockType;              // blockType is set in Child Class's Start().
    private int hp;                             // HP
    public List<int> pairs;                     // store pairs by block's id

    // Method

    /// <summary>
    /// This function link this block to target block.
    /// </summary>
    /// <param name="block">target block</param>
    public void LinkBlockTo(Block block)
    {
        // check if this block can be linked to the block
        if (!this.CheckLinkable(block) || !block.CheckLinkable(this)) return;

        // link
        this.pairs.Add(block.id);
        block.pairs.Add(this.id);
        Debug.Log($"Block[id:{this.id}] is linked to block[id:{block.id}].");
    }

    /// <summary>
    /// Check if this block can link another one.
    /// </summary>
    /// <returns></returns>
    public bool CheckLinkable(Block block)
    {
        if(pairs.Count >= param.maxPairs)
        {
            Debug.Log("This Block is full. [ID : " + this.id + " ]");
            return false;
        }

        if(!param.linkableBlockType.Contains(block.blockType))
        {
            Debug.Log("This Block [Type: " + this.blockType + "] cannot be linked to the block [Type: " + block.blockType + "].");
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

        index = block.pairs.FindIndex(id => id == this.id);
        if (index == -1)
        {
            Debug.Log("Cannnot find the id in the pairs.");
            return;
        }
        block.pairs.RemoveAt(index);
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

    /// <summary>
    /// Check if this block overlaps with another block
    /// </summary>
    /// <returns>True : overlap, False : not overlap</returns>
    public bool CheckOverlap()
    {
        Vector3 castPos = transform.position + new Vector3(0, 3.0f, 0);
        RaycastHit[] hits = Physics.BoxCastAll(castPos, transform.localScale * 0.5f - new Vector3(0.1f,0.1f, 0.1f), new Vector3(0, -1, 0), Quaternion.identity, Mathf.Infinity, layerMask: LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            //Debug.Log("Hit Info: " + hit.collider.name, hit.collider);
            if (hit.collider.gameObject == this.gameObject) continue;
            return true;
        }
        return false;
    }

    virtual protected void Start()
    {
        pairs = new List<int>();
        //for (int i = 0; i < param.maxPairs; i++)
        //{
        //    pairs.Add(-1);
        //}
        this.id = idCount++;
    }

}
