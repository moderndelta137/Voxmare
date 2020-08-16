using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClearChecker : MonoBehaviour
{
    //[SerializeField] private int count; // Show in inspector for debug
    public UnityEvent UpdateCoreCount;
    //public UnityEvent UpdateLevelCount;
    public UnityEvent LevelCleared;
    public UnityEvent LevelStarted;
    public UnityEvent LevelPrepared;
    public float Clear_wait_duration;
    public float Prepare_wait_duration;
    public float Fisrt_time_wait_duration;
    private IEnumerator Clear_wait;
    private IEnumerator Prepare_wait;
    private IEnumerator Fisrt_time_wait;
    private bool first_time;
    public UnityEvent LevelPrepareReady;
    public UnityEvent LevelClearReady;
    public enum LevelStatus
    {
        Ready,
        Start,
        Result
    }
    public LevelStatus Current_status;

    private void Start() 
    {
        Clear_wait = new WaitForSecondsRealtime(Clear_wait_duration);
        Prepare_wait = new WaitForSecondsRealtime(Prepare_wait_duration);
        Fisrt_time_wait = new WaitForSecondsRealtime(Fisrt_time_wait_duration);
        first_time = true;
        StartCoroutine(LevelPrepare());
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown && LevelData.isPaused)
        {
            switch(Current_status)
            {
                case LevelStatus.Ready:
                    StopAllCoroutines();
                    LevelStart();
                break;
                case LevelStatus.Start:
                break;
                case LevelStatus.Result:
                    StopAllCoroutines();
                    StartCoroutine(LevelPrepare());
                break;
            }
        }

        //!!!!!!!!!!!FOR DEBUG ONLY!!!!!!! To Be Deleted!!!!!!!!!!!!!!
        if(Input.GetKeyDown(KeyCode.V) && Current_status ==LevelStatus.Start)
        {
            Debug.Log("V");
            StopAllCoroutines();
            StartCoroutine(LevelClear());
        }
    }

    public void CoreDestroyed()
    {
        Debug.Log(LevelData.Remain_core);
        LevelData.Remain_core--;
        UpdateCoreCount.Invoke();
        if(LevelData.Remain_core <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(LevelClear());
        }
    }

    public void setCount(int num)
    {
        LevelData.Remain_core = num;
        UpdateCoreCount.Invoke();
        //count = num;
    }

    private IEnumerator LevelClear()
    {
        if(LevelData.Selected_level>PlayerPrefs.GetInt("MaxLevel"))
        {
            PlayerPrefs.SetInt("MaxLevel",LevelData.Selected_level);
        }
        LevelData.Selected_level++;
        LevelCleared.Invoke();
        Current_status = LevelStatus.Result;
        yield return Clear_wait;
        LevelClearReady.Invoke();
        Time.timeScale = 0;
        LevelData.isPaused = true;
        /*
        Call:
            UI to hide HUD   -Done
            UI to display level clear banner;   -Done
            Player to disable control;   -Done
            BlockManager to Destroy all blocks;   -Done
        */
    }

    // Start is called before the first frame update

    private void LevelStart()
    {
        Current_status = LevelStatus.Start;
        Time.timeScale = 1;
        LevelData.isPaused = false;     
        //
        LevelStarted.Invoke();
        /*
        Call:
            UI to display HUD;   -Done
            UI to update current level and core count;   -Done
            Player to enable control;   -Done
            BlockGenerator to GenerateBlockAndNpc;   -Done
        */
    }

    private IEnumerator LevelPrepare()
    {
        Time.timeScale = 1;
        Current_status = LevelStatus.Ready;
        if(first_time)
        {
            yield return Fisrt_time_wait;
            first_time = false;
        }
        LevelPrepared.Invoke();
        yield return Prepare_wait;
        LevelPrepareReady.Invoke();
        Time.timeScale = 0;
        LevelData.isPaused = true;
    }

   
}
