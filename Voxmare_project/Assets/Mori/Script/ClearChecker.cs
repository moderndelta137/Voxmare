using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClearChecker : MonoBehaviour
{
    //[SerializeField] private int count; // Show in inspector for debug
    public UnityEvent UpdateCoreCount;
    public UnityEvent UpdateLevelCount;

    public void LoadLevelData()
    {
        //TODO Get core account from level data;
        UpdateLevelCount.Invoke();
        UpdateCoreCount.Invoke();
    }

    public void decrementCount()
    {
        LevelData.Remain_core--;
        UpdateCoreCount.Invoke();
        if(LevelData.Remain_core == 0)
        {
            Clear();
        }
        /*
        count--;
        if(count == 0)
        {
            Clear();
        }
        */
    }

    public void setCount(int num)
    {
        LevelData.Remain_core = num;
        UpdateCoreCount.Invoke();
        //count = num;
    }

    void Clear()
    {
        Debug.Log("Clear");
        LevelData.Selected_level++;
        UpdateLevelCount.Invoke();
        // do something
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
