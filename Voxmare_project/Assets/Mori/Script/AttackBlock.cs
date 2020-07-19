using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBlock : Block
{
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        blockType = BlockType.ATTACK;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
