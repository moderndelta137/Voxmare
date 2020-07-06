using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/BlockParameter", fileName = "BlockParameter")]
public class BlockParameter : ScriptableObject
{
    public int maxPairs = 4;
    public int blockPower = 1;
    public float blockCoolTime = 1.0f;
    public List<Block.BlockType> linkableBlockType;
}
