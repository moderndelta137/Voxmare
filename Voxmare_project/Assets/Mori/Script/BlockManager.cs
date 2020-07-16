using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [SerializeField] GameObject stdBlock = null;
    [SerializeField] GameObject atkBlock = null;

    [Header("Generate")]
    [SerializeField] int generateCount = 2;

    int blockCount = 2;
    List<Block> blocks;
    List<bool> visited;
    Stack<int> searchStack;

    // Start is called before the first frame update
    void Start()
    {
        blocks = new List<Block>();

        // Generate Blocks
        GenerateBlock(generateCount);


        visited = new List<bool>(blockCount);
        for (int i = 0; i < visited.Count; i++)
        {
            visited[i] = false;
        }

        searchStack = new Stack<int>();

        
    }

    // Update is called once per frame
    void Update()
    {
        // Link
        if(Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Link block(id:" + blocks[0].id + ") and block(id:" + blocks[1].id + ")");
            // initialize blocks
            blocks[0].LinkBlockTo(blocks[1]);
        }

        // Move
        if(Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Move");
            blocks[1].MoveTo(blocks[0]);
            //MoveAllBlock();
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
                    break;
            }

            block.transform.position = new Vector3((i % 10) * 4, 0, i / 10 * 4); // Line up 10 blocks of each row.

            blocks.Add(block.GetComponent<Block>());
        }
    }

    //void MoveAllBlock()
    //{
    //    searchStack.Push(0);

    //    while(searchStack.Count != 0)
    //    {
    //        int current = searchStack.Pop();
    //        visited[current] = true;

    //        foreach (var nextBlock in blocks[current].pairs)
    //        {
    //            if (visited[nextBlock]) return;

    //            // MoveBlock

    //            searchStack.Push(nextBlock);
    //        }
    //    }
    //}
}
