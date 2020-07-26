using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    //[SerializeField] GameObject stdBlock = null;
    //[SerializeField] GameObject atkBlock = null;
    public List<GameObject> Block_prefabs;
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

        if(Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Size : " + GetSizeOfLinkingBlock(blocks[0]));
        }
    }

    void GenerateBlock(int count)
    {
        for (int i = 0; i < count; i++)
        {
            //int randomValue = Random.Range(0, (int)Block.BlockType.ATTACK + 1);
            GameObject block;
            /*
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
            */
            block = GameObject.Instantiate(Block_prefabs[Random.Range(0,Block_prefabs.Count)]);

            block.transform.position = new Vector3((i % 10) * 4, 0, i / 10 * 4); // Line up 10 blocks of each row.

            blocks.Add(block.GetComponent<Block>());                             // The blocks are ordered by id.
        }
    }

    IEnumerator LinkAllBlock()
    {
        if (blocks.Count < 1) yield break;

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
        if (bossBlocks.Count == 0 && aloneBlocks.Count > 0)
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
        // store blocks next to the block
        var nextBlocks = new List<Block>();
        foreach (int id in block.pairs)
        {
            if (id == Block.DUMMYID || id == Block.EMPTYID) continue;
            Block blk = blocks.Find(b => b.id == id);
            nextBlocks.Add(blk);
        }

        // if next block is animating, stop it
        foreach (Block nextBlock in nextBlocks)
        {
            if(nextBlock.isMoving) nextBlock.StopMoving();
        }

        // Cut linking
        CutLink(block);
        // Remove block from list
        blocks.Remove(block);

        // Find biggest block group
        if (nextBlocks.Count > 1)
        {
            int max = 0;
            Block maxBlock = null;
            foreach (Block nextBlock in nextBlocks)
            {
                int cnt = GetSizeOfLinkingBlock(nextBlock);
                if(cnt > max)
                {
                    max = cnt;
                    maxBlock = nextBlock;
                }
            }

            // reset dummy for the biggest group and cut all link for the other group
            foreach (Block nextBlock in nextBlocks)
            {
                if (nextBlock == maxBlock)
                {
                    ResetDummy(nextBlock);
                }
                else
                {
                    CutLinkBlockGroup(nextBlock);
                }
            }
        }

        // Restart Linking
        if(!isLinking) StartCoroutine("LinkAllBlock");
    }

    void CutLink(Block block)
    {
        for (int i = 0; i < block.pairs.Count; i++)
        {
            switch (block.pairs[i])
            {
                case Block.EMPTYID:
                    continue;
                case Block.DUMMYID:
                    block.pairs[i] = Block.EMPTYID;
                    continue;
                default:
                    Block pairBlock = blocks.Find(b => b.id == block.pairs[i]);
                    block.CutLinkBlockTo(pairBlock);
                    continue;
            }
        }
    }

    void CutLinkBlockGroup(Block firstBlock)
    {
        // Initialization
        var visited = new List<int>();
        var searchStack = new Stack<Block>();

        // Start Depth-first search
        searchStack.Push(firstBlock);

        while (searchStack.Count != 0)
        {
            Block current = searchStack.Pop();
            visited.Add(current.id);

            foreach (var nextBlockId in current.pairs)
            {
                if (nextBlockId == Block.DUMMYID || nextBlockId == Block.EMPTYID) continue;
                if (visited.Contains(nextBlockId)) continue;
                Block nextBlock = blocks.Find(b => b.id == nextBlockId);
                searchStack.Push(nextBlock);
            }

            CutLink(current);
            current.isAlone = true;
        }
    }

    /// <summary>
    /// Get the number of blocks in a block group
    /// </summary>
    /// <param name="firstBlock">first block to search all blocks in the group</param>
    /// <returns>the number of blocks</returns>
    int GetSizeOfLinkingBlock(Block firstBlock)
    {
        // Initialization
        int size = 0;
        var visited = new List<int>();
        var searchStack = new Stack<Block>();
        
        // Start Depth-first search
        searchStack.Push(firstBlock);

        while (searchStack.Count != 0)
        {
            Block current = searchStack.Pop();
            visited.Add(current.id);
            size++;

            foreach (var nextBlockId in current.pairs)
            {
                if (nextBlockId == Block.DUMMYID || nextBlockId == Block.EMPTYID) continue;
                if (visited.Contains(nextBlockId)) continue;
                Block nextBlock = blocks.Find(b => b.id == nextBlockId);
                if (nextBlock == null) Debug.Log("[Error] cannot find nextBlock in blocks!");
                searchStack.Push(nextBlock);
            }
        }
        return size;
    }

    void ResetDummy(Block firstBlock)
    {
        // Initialization
        var visited = new List<int>();
        var searchStack = new Stack<Block>();

        // Start Depth-first search
        searchStack.Push(firstBlock);

        while (searchStack.Count != 0)
        {
            Block current = searchStack.Pop();
            visited.Add(current.id);

            for (int i = 0; i < current.pairs.Count; i++)
            {
                int nextBlockId = current.pairs[i];

                if (nextBlockId == Block.EMPTYID) continue;

                // Change Dummy to Empty
                if (nextBlockId == Block.DUMMYID)
                {
                    current.pairs[i] = Block.EMPTYID;
                    continue;
                }

                if (visited.Contains(nextBlockId)) continue;
                Block nextBlock = blocks.Find(b => b.id == nextBlockId);
                if (nextBlock == null) Debug.Log("[Error] cannot find nextBlock in blocks!");
                searchStack.Push(nextBlock);
            }
        }
    }
}
