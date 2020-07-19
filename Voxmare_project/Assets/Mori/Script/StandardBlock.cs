using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardBlock : Block
{
    override protected void Start()
    {
        base.Start();
        this.blockType = BlockType.STANDARD;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
