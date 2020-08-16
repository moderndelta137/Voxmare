using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause_Menu_Controller : MonoBehaviour
{
    public GameObject Pause_menu;
    public GameObject Option_menu;
    //public bool Paused;
    public bool Option_menu_opened;

    public AudioSource Click_SE;
    public AudioSource Cancel_SE;
    
    public Eyelid_Controller Eyelid_script;
    private WaitForSecondsRealtime blink_wait;
    // Start is called before the first frame update
    void Start()
    {
        blink_wait = new WaitForSecondsRealtime(Eyelid_script.Blink_duration);
    }

    // Update is called once per frame
    void Update()
    {
    if(Input.GetButtonDown("Escape"))
        {
            PlayCancelSE();
            if(!LevelData.isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    
    if(Input.GetButtonDown("Cancel"))
        {
            if(LevelData.isPaused)
            {
                PlayCancelSE();
                if(Option_menu_opened)
                {
                    CloseOptionMenu();
                }
                else
                {
                    ResumeGame();
                }
            }
        }
    }

    public void PauseGame()
    {
        StartCoroutine(PauseGameDelay());
    }
    private IEnumerator PauseGameDelay()
    {
        LevelData.isPaused = true;
        Time.timeScale = 0;
        Eyelid_script.EyeClose();
        yield return blink_wait;
        Pause_menu.SetActive(true);
        Eyelid_script.EyeOpen();
        yield return blink_wait;
    }

    public void ResumeGame()
    {
        LevelData.isPaused = false;
        Time.timeScale = 1;
        LevelData.isPaused = false;
        Option_menu_opened = false;
        Option_menu.SetActive(false);
        Pause_menu.SetActive(false);
    }

    public void OpenOptionMenu()
    {
        StartCoroutine(OpenOptionMenuDelay());
    }
    private IEnumerator OpenOptionMenuDelay()
    {
        Eyelid_script.EyeClose();
        yield return blink_wait;
        Option_menu_opened = true;
        Option_menu.SetActive(true);
        Pause_menu.SetActive(false);
        Eyelid_script.EyeOpen();
        yield return blink_wait;
    }
    public void CloseOptionMenu()
    {
        StartCoroutine(CloseOptionMenuDelay());
    }
    private IEnumerator CloseOptionMenuDelay()
    {
        Eyelid_script.EyeClose();
        yield return blink_wait;
        Option_menu_opened = false;
        Option_menu.SetActive(false);
        Pause_menu.SetActive(true);
        Eyelid_script.EyeOpen();
        yield return blink_wait;
    }


    public void ReturnToMenu()
    {
        StartCoroutine(ReturnToMenuDelay());        
    }
    private IEnumerator ReturnToMenuDelay()
    {
        Time.timeScale = 1;
        LevelData.isPaused = false;
        Eyelid_script.EyeClose();
        yield return blink_wait;
        SceneManager.LoadScene(0);
    }

    public void PlayClickSE()
    {
        Click_SE.Play();
    }
    public void PlayCancelSE()
    {
        Cancel_SE.Play();
    }
}
