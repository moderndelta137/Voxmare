using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlockGenerator : MonoBehaviour
{
    private int currentStage;
    [SerializeField] List<Stage> stages;

    [Header("Position")]
    [SerializeField] Vector3 generateCenter;
    [SerializeField] float lengthX;
    [SerializeField] float lengthZ;

    [Header("Animation")]
    [SerializeField] float generateDuration;
    [SerializeField] Ease generateEase;
    [SerializeField] float generateInterval;

    private BlockManager manager;
    private ClearChecker clearChecker;
    private WaitForSeconds waitGenerate;

    void Start()
    {
        currentStage = LevelData.Selected_level;
        waitGenerate = new WaitForSeconds(generateInterval);
        manager = GameObject.Find("BlockManager").GetComponent<BlockManager>();
        clearChecker = GameObject.Find("ClearChecker").GetComponent<ClearChecker>();
        StartCoroutine(GenerateBlockAndNpc());
    }

    IEnumerator GenerateBlockAndNpc()
    {
        Stage stage = stages[currentStage - 1];
        int coreCount = 0;

        // Generate Block
        GameObject block;
        foreach (var blockSetting in stage.block)
        {
            coreCount += blockSetting.coreCount;

            int coreRemain = blockSetting.coreCount;
            for (int i = 0; i < blockSetting.count; i++)
            {
                block = GameObject.Instantiate(blockSetting.blockPrefab);
                // Position
                block.transform.position = generateCenter + new Vector3(Random.Range(-lengthX/2, lengthX/2), 0, Random.Range(-lengthZ/2, lengthZ/2));

                // Generate Animation
                Vector3 scale = block.transform.localScale;
                block.transform.localScale = Vector3.zero;
                block.transform.DOScale(scale, generateDuration).SetEase(generateEase);

                // Core
                if(coreRemain > 0)
                {
                    block.GetComponent<Block>().isCore = true;
                    coreRemain--;
                }

                // Store
                manager.blocks.Add(block.GetComponent<Block>());

                yield return waitGenerate;
            }
        }

        // Generate NPC
        GameObject npc;
        foreach (var npcSetting in stage.npc)
        {
            for (int i = 0; i < npcSetting.count; i++)
            {
                npc = GameObject.Instantiate(npcSetting.NPCPrefab);
                // Position
                npc.transform.position = generateCenter + new Vector3(Random.Range(-lengthX / 2, lengthX / 2), 0, Random.Range(-lengthZ / 2, lengthZ / 2));
                
                // Generate Animation
                Vector3 scale = npc.transform.localScale;
                npc.transform.localScale = Vector3.zero;
                npc.transform.DOScale(scale, generateDuration).SetEase(generateEase);

                yield return waitGenerate;
            }
        }

        clearChecker.setCount(coreCount);
        manager.StartLink();
    }

    void Update()
    {
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(generateCenter, new Vector3(lengthX, 0.0f, lengthZ));
    }
}
