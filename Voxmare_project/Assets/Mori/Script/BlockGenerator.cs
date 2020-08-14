using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class BlockGenerator : MonoBehaviour
{
    [SerializeField] int currentStage;
    [SerializeField] List<Stage> stages;

    [Header("Position")]
    [SerializeField] Vector3 generateCenter;
    [SerializeField] float lengthX;
    [SerializeField] float lengthZ;

    [Header("Landing Animation")]
    [SerializeField] float landHeight;
    [SerializeField] float landDuration;
    [SerializeField] Ease landEase;

    [Header("Animation")]
    [SerializeField] float generateDuration;
    [SerializeField] Ease generateEase;
    [SerializeField] float generateInterval;

    private BlockManager manager;
    private ClearChecker clearChecker;
    private WaitForSeconds waitGenerate;
    private Transform emptyObject;

    void Start()
    {
        waitGenerate = new WaitForSeconds(generateInterval);
        manager = GameObject.Find("BlockManager").GetComponent<BlockManager>();
        clearChecker = GameObject.Find("ClearChecker").GetComponent<ClearChecker>();
        StartCoroutine(SetupStage());
    }

    IEnumerator SetupStage()
    {
        var coroutine = StartCoroutine(GenerateBlockAndNpc());
        yield return coroutine;
        coroutine = StartCoroutine(LandObjects());
        yield return coroutine;
        EnableObjects();
    }

    IEnumerator GenerateBlockAndNpc()
    {
        Stage stage = stages[currentStage - 1];
        int coreCount = 0;
        emptyObject = new GameObject("EmptyObjectForLand").transform;
        emptyObject.position = generateCenter;

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
                block.transform.parent = emptyObject;
                block.GetComponent<Block>().enabled = false;

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

                npc.transform.parent = emptyObject;
                npc.GetComponent<PickupController>().enabled = false;
                npc.GetComponent<NavMeshAgent>().enabled = false;
                yield return waitGenerate;
            }
        }

        clearChecker.setCount(coreCount);
    }

    IEnumerator LandObjects()
    {
        var tween = emptyObject.DOMoveY(landHeight, landDuration).SetEase(landEase);
        yield return tween.WaitForCompletion();
    }

    void EnableObjects()
    {
        foreach (Transform obj in emptyObject)
        {
            var block = obj.GetComponent<Block>();
            if (block != null) block.enabled = true;

            var npc = obj.GetComponent<PickupController>();
            if (npc != null) npc.enabled = true;

            var navmesh = obj.GetComponent<NavMeshAgent>();
            if (navmesh != null) navmesh.enabled = true;
        }
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
