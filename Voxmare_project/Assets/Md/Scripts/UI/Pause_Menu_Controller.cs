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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    if(Input.GetButtonDown("Escape"))
        {
            PlayCancelSE();
            if(!LevelData.isPaused)
            {
                LevelData.isPaused = true;
                Time.timeScale = 0;
                LevelData.isPaused = true;
                Pause_menu.SetActive(true);
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
        Option_menu_opened = true;
        Option_menu.SetActive(true);
        Pause_menu.SetActive(false);
    }

    public void CloseOptionMenu()
    {
        Option_menu_opened = false;
        Option_menu.SetActive(false);
        Pause_menu.SetActive(true);
    }

    public void ReturnToMenu()
    {
        //TODO Resume game timescale
        Time.timeScale = 1;
        LevelData.isPaused = false;
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
