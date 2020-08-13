using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Stage")]
public class Stage : ScriptableObject
{
	public List<BlockSetting> block = new List<BlockSetting>();
	public List<NPCSetting> npc = new List<NPCSetting>();
	public List<ObstacleSetting> obstacle = new List<ObstacleSetting>();

	[System.Serializable]
	public class BlockSetting
	{
		public GameObject blockPrefab;
		public int count;
		public int coreCount;
	}

	[System.Serializable]
	public class NPCSetting
	{
		public GameObject NPCPrefab;
		public int count;
	}

	[System.Serializable]
	public class ObstacleSetting
	{
		public GameObject ObstaclePrefab;
	}
}