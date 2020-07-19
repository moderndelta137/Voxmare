using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [SerializeField] GameObject target = null;
    [SerializeField] GameObject stdBlock = null;
    [SerializeField] GameObject atkBlock = null;

    [Header("Generate")]
    [SerializeField] int generateCount = 2;

    List<Block> blocks;
    List<bool> visited;
    Stack<int> searchStack;

    // Start is called before the first frame update
    void Start()
    {
        blocks = new List<Block>();

        // Generate Blocks
        GenerateBlock(generateCount);


        visited = new List<bool>();
        for (int i = 0; i < blocks.Count; i++)
        {
            visited.Add(false);
        }

        searchStack = new Stack<int>();

        
    }

    // Update is called once per frame
    void Update()
    {
        // Link
        if(Input.GetKeyDown(KeyCode.L))
        {
            LinkAllBlock();
            //Debug.Log("Link block(id:" + blocks[0].id + ") and block(id:" + blocks[1].id + ")");
            //blocks[0].LinkBlockTo(blocks[1]);
            ////Debug.Log("Link block(id:" + blocks[1].id + ") and block(id:" + blocks[2].id + ")");
            //blocks[1].LinkBlockTo(blocks[2]);
            //blocks[1].LinkBlockTo(blocks[2]);
        }

        // Move
        if(Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Move");
            MoveAllBlock();
        }
    }

    void GenerateBlock(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int randomValue = Random.Range(0, (int)Block.BlockType.ATTACK + 1);
            GameObject block;

            switch (randomValue)
            {
                case 0:
                    block = GameObject.Instantiate(stdBlock);
                    break;
                case 1:
                    block = GameObject.Instantiate(atkBlock);
                    break;
                default:
                    Debug.Log("randomValue is out of range.");
                    return;
            }

            block.GetComponent<Block>().spammer.Target = this.target;
            block.transform.position = new Vector3((i % 10) * 4, 0, i / 10 * 4); // Line up 10 blocks of each row.

            blocks.Add(block.GetComponent<Block>());                             // The blocks are ordered by id.
        }
    }

    void LinkAllBlock()
    {
        // Initialization
        int capacity = 0;                   // how many boss can be linked left
        var leftBlocks = new List<int>();   // blocks which is not linked to boss yet
        var bossBlocks = new List<int>();   // blocks which blong to boss
        var eachCapacity = new List<int>(); // capacities of each block

        foreach (var block in blocks) leftBlocks.Add(block.id);
        foreach (var block in blocks) eachCapacity.Add(block.param.maxPairs - block.pairs.Count);

        // Start linking blocks from id 0
        TransferItem(leftBlocks[0], leftBlocks, bossBlocks);
        capacity += blocks[0].param.maxPairs - blocks[0].pairs.Count;

        while (leftBlocks.Count > 0 && capacity > 0)
        {
            int leftBlock = -1;     // target block in left blocks
            int bossBlock = -1;     // target block in boss blocks
            int lastBlock = -1;     // last target block in left blocks

            // find good block in left blocks
            foreach(int idLeft in leftBlocks)
            {
                if (capacity + eachCapacity[idLeft] - 2 < 0) continue;

                if (capacity + eachCapacity[idLeft] - 2 == 0)
                {
                    lastBlock = idLeft;
                    continue;
                }

                leftBlock = idLeft;
                // find good block from boss blocks
                foreach (var idBoss in bossBlocks)
                {
                    if (!blocks[idBoss].CheckLinkable(blocks[leftBlock]) || !blocks[leftBlock].CheckLinkable(blocks[idBoss])) continue;
                    
                    bossBlock = idBoss;
                    break;
                }

                if (bossBlock != -1) break;
            }

            // Cannot find good block
            if(bossBlock == -1)
            {
                // Check last block
                foreach (var idBoss in bossBlocks)
                {
                    if (eachCapacity[idBoss] <= 0) continue;
                    if (!blocks[idBoss].CheckLinkable(blocks[lastBlock]) || !blocks[lastBlock].CheckLinkable(blocks[idBoss])) continue;

                    bossBlock = idBoss;
                    break;
                }

                if (bossBlock == -1) return;
                leftBlock = lastBlock;
            }

            // link
            blocks[bossBlock].LinkBlockTo(blocks[leftBlock]);
            capacity = capacity + eachCapacity[leftBlock] - 2;
            eachCapacity[bossBlock]--;
            eachCapacity[leftBlock]--;
            TransferItem(leftBlock, leftBlocks, bossBlocks);
        }
    }

    /// <summary>
    /// Transfer item from fromList to toList
    /// </summary>
    /// <param name="item"></param>
    /// <param name="fromList"></param>
    /// <param name="toList"></param>
    void TransferItem(int item, List<int> fromList, List<int> toList)
    {
        if(!fromList.Contains(item))
        {
            Debug.Log("[Error] cannnot find item in fromList.");
            return;
        }

        fromList.Remove(item);
        toList.Add(item);
    }

    void MoveAllBlock()
    {
        // Initialization
        searchStack.Clear();
        for (int i = 0; i < visited.Count; i++)
        {
            visited[i] = false;
        }

        // Start moving blocks from id 0
        searchStack.Push(0);

        while (searchStack.Count != 0)
        {
            int current = searchStack.Pop();
            visited[current] = true;

            foreach (var nextBlock in blocks[current].pairs)
            {
                if (visited[nextBlock]) continue;

                blocks[nextBlock].MoveTo(blocks[current]);

                searchStack.Push(nextBlock);
            }
        }
    }
}
