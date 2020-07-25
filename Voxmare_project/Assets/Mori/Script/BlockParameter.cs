using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/BlockParameter", fileName = "BlockParameter")]
public class BlockParameter : ScriptableObject
{
    public int maxPairs = 4;
    public List<Block.BlockType> linkableBlockType;
}
