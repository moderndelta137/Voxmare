using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class BlockGenerator : MonoBehaviour
{
    //private int currentStage;
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
    [SerializeField] float deleteDuration;
    [SerializeField] Ease deleteEase;
    [SerializeField] float deleteInterval;

    private BlockManager manager;
    private ClearChecker clearChecker;
    private WaitForSeconds waitGenerate;
    private Transform emptyObject;

    private bool first_time;
    //private List<GameObject> objects;

    void Start()
    {
        first_time = true;
        //currentStage = LevelData.Selected_level;
        waitGenerate = new WaitForSeconds(generateInterval);
        manager = GameObject.Find("BlockManager").GetComponent<BlockManager>();
        clearChecker = GameObject.Find("ClearChecker").GetComponent<ClearChecker>();
        //objects = new List<GameObject>();
    }

    public void StartGenerate()
    {
        StartCoroutine(Generate());
    }

    /*
    public void Reset()
    {
        StartCoroutine(ResetBlockAndNpc());
    }
    */

    IEnumerator Generate()
    {
        var coroutine = StartCoroutine(GenerateBlockAndNpc());
        yield return coroutine;
        coroutine = StartCoroutine(LandObjects());
        yield return coroutine;
        EnableObjects();
    }

    IEnumerator GenerateBlockAndNpc()
    {
        //objects.Clear();
        manager.blocks = new List<Block>();
        Stage stage = stages[LevelData.Selected_level - 1];
        int coreCount = 0;
        emptyObject = new GameObject("EmptyObjectForLand").transform;
        emptyObject.position = generateCenter;

        // Generate Block
        GameObject block;
        foreach (var blockSetting in stage.block)
        {
            for (int i = 0; i < blockSetting.count; i++)
            {
                block = GameObject.Instantiate(blockSetting.blockPrefab);
                //objects.Add(block);
                // Position
                block.transform.position = generateCenter + new Vector3(Random.Range(-lengthX/2, lengthX/2), 0, Random.Range(-lengthZ/2, lengthZ/2));
                block.transform.Rotate(Vector3.up*Random.Range(0f,360f),Space.World);
                // Generate Animation
                Vector3 scale = block.transform.localScale;
                block.transform.localScale = Vector3.zero;
                block.transform.DOScale(scale, generateDuration).SetEase(generateEase);

                // Core
                if(block.GetComponent<Block>().isCore)
                {
                    coreCount++;
                }

                // Store
                manager.blocks.Insert(Random.Range(0,manager.blocks.Count),block.GetComponent<Block>());
                block.transform.parent = emptyObject;
                block.GetComponent<Block>().enabled = false;

                yield return waitGenerate;
            }
        }

        // Generate NPC
        GameObject npc;
        if(first_time)
        {
            for (int i = 0; i < LevelData.Selected_level; i++)
            foreach (var npcSetting in stages[i].npc)
            {
                for (int j = 0; j < npcSetting.count; j++)
                {
                    npc = GameObject.Instantiate(npcSetting.NPCPrefab);
                    //objects.Add(npc);
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
            first_time=false;
        }
        else
        {            
            foreach (var npcSetting in stage.npc)
            {
                for (int i = 0; i < npcSetting.count; i++)
                {
                    npc = GameObject.Instantiate(npcSetting.NPCPrefab);
                    //objects.Add(npc);
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

    /*
    IEnumerator ResetBlockAndNpc()
    {
        foreach (var obj in objects)
        {
            if (obj == null) continue;
            // Delete Animation
            obj.transform.DOScale(Vector3.zero, deleteDuration).SetEase(deleteEase);
            Destroy(obj, deleteDuration);
        }
        objects.Clear();
        yield return null;
    }
    */

    public void DestroyAllBlocks()
    {
        foreach(Block block_left in manager.blocks)
        {
            block_left.GetComponent<EnemyController>().SendMessage("ApplyDamage",Vector3.up*999);
        }
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
