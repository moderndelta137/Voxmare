using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ClearChecker : MonoBehaviour
{
    //[SerializeField] private int count; // Show in inspector for debug
    public UnityEvent UpdateCoreCount;
    //public UnityEvent UpdateLevelCount;
    public UnityEvent LevelCleared;
    public UnityEvent LevelStarted;
    public UnityEvent LevelPrepared;
    public UnityEvent GameOvered;
    public UnityEvent NewGameStart;
    public UnityEvent ShowTutorial;

    public float Clear_wait_duration;
    public float Prepare_wait_duration;
    public float Gameover_wait_duration;
    public float Fisrt_time_wait_duration;
    private bool first_time;
    private IEnumerator Fisrt_time_wait;
    private IEnumerator Clear_wait;
    private IEnumerator Prepare_wait;
    private IEnumerator Gameover_wait;

    public UnityEvent LevelPrepareReady;
    public UnityEvent LevelClearReady;
    public UnityEvent GameOverReady;
    public UnityEvent Restart;
    public enum LevelStatus
    {
        Turotial,
        Ready,
        Start,
        Result,
        GameOver
    }
    public LevelStatus Current_status;

    private void Start() 
    {
        Clear_wait = new WaitForSecondsRealtime(Clear_wait_duration);
        Prepare_wait = new WaitForSecondsRealtime(Prepare_wait_duration);
        Gameover_wait = new WaitForSecondsRealtime(Gameover_wait_duration);
        Fisrt_time_wait = new WaitForSecondsRealtime(Fisrt_time_wait_duration);
        if(LevelData.Selected_level<=1)
        {
            first_time = true;
        }
        StartCoroutine(LevelPrepare());
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown && LevelData.isPaused)
        {
            switch(Current_status)
            {
                case LevelStatus.Turotial:
                    //Time.timeScale = 1;
                    LevelData.isPaused=false;
                    StopAllCoroutines();
                    StartCoroutine(LevelPrepare());
                break;
                case LevelStatus.Ready:
                    StopAllCoroutines();
                    LevelStart();
                break;
                case LevelStatus.Start:
                break;
                case LevelStatus.Result:
                    //Time.timeScale = 1;
                    LevelData.isPaused=false;
                    StopAllCoroutines();
                    StartCoroutine(LevelPrepare());
                break;
                case LevelStatus.GameOver:
                    StopAllCoroutines();
                    Restart.Invoke();
                break;
            }
        }

        //!!!!!!!!!!!FOR DEBUG ONLY!!!!!!! To Be Deleted!!!!!!!!!!!!!!
        /*
        if(Input.GetKeyDown(KeyCode.V) && Current_status ==LevelStatus.Start)
        {
            Debug.Log("V");
            StopAllCoroutines();
            StartCoroutine(LevelClear());
        }
        */
    }

    public void CoreDestroyed()
    {
        //Debug.Log("destroy");
        LevelData.Remain_core--;
        if(Current_status == LevelStatus.Start)
        {
            UpdateCoreCount.Invoke();
            if(LevelData.Remain_core <= 0)
            {
                Current_status = LevelStatus.Result;
                StopAllCoroutines();
                StartCoroutine(LevelClear());
                //Debug.Log("check");
            }
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
        //Debug.Log("clear");
        if(LevelData.Selected_level>PlayerPrefs.GetInt("MaxLevel"))
        {
            PlayerPrefs.SetInt("MaxLevel",LevelData.Selected_level);
        }
        LevelData.Selected_level++;
        LevelCleared.Invoke();
        
        yield return Clear_wait;
        LevelClearReady.Invoke();
        Time.timeScale = 0;
        LevelData.isPaused = true;
    }

    // Start is called before the first frame update

    private void LevelStart()
    {
        Current_status = LevelStatus.Start;
        Time.timeScale = 1;
        LevelData.isPaused = false;     
        LevelStarted.Invoke();
    }

    private IEnumerator LevelPrepare()
    {
        Time.timeScale = 1;
        if(LevelData.Selected_level > 20)
        {
            LevelData.isPaused = false;
            SceneManager.LoadScene(3);
        }

        if(first_time)
        {
            Current_status = LevelStatus.Turotial;
            yield return new WaitForSecondsRealtime(0.2f);
            NewGameStart.Invoke();
            yield return Fisrt_time_wait;
            first_time = false;
            ShowTutorial.Invoke();
            Time.timeScale = 0;
            LevelData.isPaused=true;

        }
        else
        {
            Current_status = LevelStatus.Ready;
            LevelPrepared.Invoke();
            yield return Prepare_wait;
            LevelPrepareReady.Invoke();
            Time.timeScale = 0;
            LevelData.isPaused = true;
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        LevelData.isPaused = true;
        GameOvered.Invoke();
        StartCoroutine(GameOverPrepare());
    }
    private IEnumerator GameOverPrepare()
    {
        yield return Gameover_wait;
        GameOverReady.Invoke();
        Current_status = LevelStatus.GameOver;
    }


}
