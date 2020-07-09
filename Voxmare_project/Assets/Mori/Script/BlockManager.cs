using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [SerializeField] Block block01 = null;
    [SerializeField] Block block02 = null;

    int blockCount = 2;
    List<Block> blocks;
    List<bool> visited;
    Stack<int> searchStack;

    // Start is called before the first frame update
    void Start()
    {
        blocks = new List<Block>();

        // Generate Blocks (implement later)
        blocks.Add(block01);
        blocks.Add(block02);


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
            Debug.Log("Link block(id:" + block01.id + ") and block(id:" + block02.id + ")");
            // initialize blocks
            block01.LinkBlockTo(block02);
        }

        // Move
        if(Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Move");
            blocks[1].MoveTo(blocks[0]);
            //MoveAllBlock();
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
