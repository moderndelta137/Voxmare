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

    public List<Block> blocks;
    private bool isLinking;

    // Start is called before the first frame update
    void Start()
    {
        blocks = new List<Block>();
        isLinking = false;
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
        isLinking = true;

        // Initialization
        int bossCapacity = 0;                   // how many boss can be linked left
        int linkingCount = 0;                   // how many times block is linking in this time
        bool allBlocksStop = true;
        var aloneBlocks = new List<Block>();   // blocks which is not linked to boss yet
        var bossBlocks = new List<Block>();   // blocks which blong to boss

        foreach (var block in blocks)
        {
            // ignore a moving block
            if (block.isMoving)
            {
                allBlocksStop = false;
                continue;
            }

            // check if the block is alone
            if (block.isAlone)
            {
                aloneBlocks.Add(block);
            }
            else
            {
                bossBlocks.Add(block);
                bossCapacity += block.GetCapacity();
            }
        }

        // if this is first time, block whose id is 0 changes to initial boss block
        if (bossBlocks.Count == 0)
        {
            TransferItem(aloneBlocks[0], aloneBlocks, bossBlocks);
            bossBlocks[0].isAlone = false;
            bossCapacity += bossBlocks[0].GetCapacity();
        }

        int bossCapacityBg = bossCapacity;
        while (bossCapacity > 0)
        {
            Block aloneBlock = null;     // target block in alone blocks
            Block bossBlock = null;     // target block in boss blocks

            // find good block in alone blocks
            foreach(Block aloneB in aloneBlocks)
            {
                if (bossCapacityBg + aloneB.GetCapacity() - 2 <= 0) continue;

                aloneBlock = aloneB;

                // find good block from boss blocks
                foreach (Block bossB in bossBlocks)
                {
                    if (!bossB.CheckLinkable(aloneBlock) || !aloneBlock.CheckLinkable(bossB)) continue;
                    
                    bossBlock = bossB;
                    break;
                }

                if (bossBlock != null) break;
            }

            // Cannot find good block
            if(bossBlock == null) break;

            // link
            bossBlock.LinkBlockTo(aloneBlock);

            // move
            aloneBlock.MoveTo(bossBlock, moveSpeed);

            linkingCount++;
            bossCapacity--;
            bossCapacityBg = bossCapacityBg + aloneBlock.GetCapacity() - 2;
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
        var aloneBlocks = new List<Block>();   // blocks which is not linked to boss yet
        var bossBlocks = new List<Block>();   // blocks which blong to boss

        foreach (var block in blocks)
        {
            if (block.isAlone)
            {
                aloneBlocks.Add(block);
            }
            else
            {
                bossBlocks.Add(block);
                bossCapacity += block.GetCapacity();
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
        if (bossCapacity != 1)
        {
            isLinking = false;
            yield break;
        }

        // Link last block
        Block aloneBlock = null;     // target block in alone blocks
        Block bossBlock = null;     // target block in boss blocks
        foreach (Block aloneB in aloneBlocks)
        {
            if (aloneB.GetCapacity() != 1) continue;

            aloneBlock = aloneB;

            // Search good block in boss blocks
            foreach (Block bossB in bossBlocks)
            {
                if (bossB.GetCapacity() <= 0) continue;
                if (!bossB.CheckLinkable(aloneB) || !aloneB.CheckLinkable(bossB)) continue;

                bossBlock = bossB;
                break;
            }

            if (bossBlock != null) break;
        }

        // Finish condition 2 (There are no good block)
        if (bossBlock == null)
        {
            isLinking = false;
            yield break;
        }

        // link
        bossBlock.LinkBlockTo(aloneBlock);

        // move
        aloneBlock.MoveTo(bossBlock, moveSpeed);

        StartCoroutine("LinkLastBlock");
    }

    /// <summary>
    /// Transfer item from fromList to toList
    /// </summary>
    /// <param name="item"></param>
    /// <param name="fromList"></param>
    /// <param name="toList"></param>
    void TransferItem(Block item, List<Block> fromList, List<Block> toList)
    {
        if(!fromList.Contains(item))
        {
            Debug.Log("[Error] cannnot find item in fromList.");
            return;
        }

        fromList.Remove(item);
        toList.Add(item);
    }

    public void DeathBlock(Block block)
    {
        // Cut linking
        while(block.GetPairsCount() > 0)
        {
            int i = 0;
            while(block.pairs[i] == Block.EMPTYID)
            {
                i++;
            }

            if(block.pairs[i] == Block.DUMMYID)
            {
                block.pairs[i] = Block.EMPTYID;
            }
            else
            {
                Block pairBlock = blocks.Find(b => b.id == block.pairs[i]);
                block.CutLinkBlockTo(pairBlock);
            }
        }

        // Remove block from list
        blocks.Remove(block);

        // Restart Linking
        if(!isLinking) StartCoroutine("LinkAllBlock");
    }
}
