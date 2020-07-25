using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [SerializeField] GameObject stdBlock = null;
    [SerializeField] GameObject atkBlock = null;

    [Header("Generate")]
    [SerializeField] int generateCount = 2;

    [Header("Link Animation")]
    [SerializeField] float interval;
    [SerializeField] float moveSpeed;

    List<Block> blocks;

    // Start is called before the first frame update
    void Start()
    {
        blocks = new List<Block>();

        // Generate Blocks
        GenerateBlock(generateCount);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            // Link and Move
            StartCoroutine("LinkAllBlock");
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

            block.transform.position = new Vector3((i % 10) * 4, 0, i / 10 * 4); // Line up 10 blocks of each row.

            blocks.Add(block.GetComponent<Block>());                             // The blocks are ordered by id.
        }
    }

    IEnumerator LinkAllBlock()
    {
        // Initialization
        int bossCapacity = 0;                   // how many boss can be linked left
        int linkingCount = 0;                   // how many times block is linking in this time
        bool allBlocksStop = true;
        var aloneBlocks = new List<int>();   // blocks which is not linked to boss yet
        var bossBlocks = new List<int>();   // blocks which blong to boss
        var eachCapacity = new List<int>(); // capacities of each block

        foreach (var block in blocks)
        {
            eachCapacity.Add(block.maxPairs - block.GetPairsCount());

            // ignore a moving block
            if (block.isMoving)
            {
                allBlocksStop = false;
                continue;
            }

            // check if the block is alone
            if (block.isAlone)
            {
                aloneBlocks.Add(block.id);
            }
            else
            {
                bossBlocks.Add(block.id);
                bossCapacity += block.maxPairs - block.GetPairsCount();
            }
        }

        // if this is first time, block whose id is 0 changes to initial boss block
        if (bossBlocks.Count == 0)
        {
            TransferItem(aloneBlocks[0], aloneBlocks, bossBlocks);
            blocks[bossBlocks[0]].isAlone = false;
            bossCapacity += blocks[0].maxPairs - blocks[0].GetPairsCount();
        }

        int bossCapacityBg = bossCapacity;
        while (bossCapacity > 0)
        {
            int aloneBlock = -1;     // target block in alone blocks
            int bossBlock = -1;     // target block in boss blocks

            // find good block in alone blocks
            foreach(int idAlone in aloneBlocks)
            {
                if (bossCapacityBg + eachCapacity[idAlone] - 2 <= 0) continue;

                aloneBlock = idAlone;

                // find good block from boss blocks
                foreach (var idBoss in bossBlocks)
                {
                    if (!blocks[idBoss].CheckLinkable(blocks[aloneBlock]) || !blocks[aloneBlock].CheckLinkable(blocks[idBoss])) continue;
                    
                    bossBlock = idBoss;
                    break;
                }

                if (bossBlock != -1) break;
            }

            // Cannot find good block
            if(bossBlock == -1) break;

            // link
            blocks[bossBlock].LinkBlockTo(blocks[aloneBlock]);

            // move
            blocks[aloneBlock].MoveTo(blocks[bossBlock], moveSpeed);

            linkingCount++;
            bossCapacity--;
            bossCapacityBg = bossCapacityBg + eachCapacity[aloneBlock] - 2;
            eachCapacity[bossBlock]--;
            eachCapacity[aloneBlock]--;
            aloneBlocks.Remove(aloneBlock);

            yield return new WaitForSeconds(interval);
        }

        // if there is a moving block, retry this coroutine 
        if (allBlocksStop && linkingCount == 0)
        {
            StartCoroutine("LinkLastBlock");
            yield break;
        }

        yield return new WaitForSeconds(0.1f);
        StartCoroutine("LinkAllBlock");
    }

    IEnumerator LinkLastBlock()
    {
        // Initialization
        int bossCapacity = 0;                   // how many boss can be linked left
        bool allBlocksStop = true;
        var aloneBlocks = new List<int>();   // blocks which is not linked to boss yet
        var bossBlocks = new List<int>();   // blocks which blong to boss
        var eachCapacity = new List<int>(); // capacities of each block

        foreach (var block in blocks)
        {
            eachCapacity.Add(block.maxPairs - block.GetPairsCount());

            if (block.isAlone)
            {
                aloneBlocks.Add(block.id);
            }
            else
            {
                bossBlocks.Add(block.id);
                bossCapacity += block.maxPairs - block.GetPairsCount();
            }
        }

        // Wait Animation
        if(!allBlocksStop)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine("LinkLastBlock");
            yield break;
        }

        // Finish condition 1 (Complete linking last block)
        if (bossCapacity != 1) yield break;

        // Link last block
        int aloneBlock = -1;     // target block in alone blocks
        int bossBlock = -1;     // target block in boss blocks
        foreach (var idAlone in aloneBlocks)
        {
            if (eachCapacity[idAlone] != 1) continue;

            aloneBlock = idAlone;

            // Search good block in boss blocks
            foreach (var idBoss in bossBlocks)
            {
                if (eachCapacity[idBoss] <= 0) continue;
                if (!blocks[idBoss].CheckLinkable(blocks[idAlone]) || !blocks[idAlone].CheckLinkable(blocks[idBoss])) continue;

                bossBlock = idBoss;
                break;
            }

            if (bossBlock != -1) break;
        }

        // Finish condition 2 (There are no good block)
        if (bossBlock == -1) yield break;

        // link
        blocks[bossBlock].LinkBlockTo(blocks[aloneBlock]);

        // move
        blocks[aloneBlock].MoveTo(blocks[bossBlock], moveSpeed);

        StartCoroutine("LinkLastBlock");
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
}
