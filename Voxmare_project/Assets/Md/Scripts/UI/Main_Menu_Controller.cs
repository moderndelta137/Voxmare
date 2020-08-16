using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Main_Menu_Controller : MonoBehaviour
{
    public GameObject Title_text;
    public GameObject Main_menu_page;
    public GameObject Option_menu_page;
    public GameObject Level_select_page;
    public enum Menu_status
    {
        Main_menu,
        Option_menu,
        Level_select
    }
    public Menu_status Current_menu;

    public AudioSource Click_SE;
    public AudioSource Cancel_SE;

    public Eyelid_Controller Eyelid_script;

    public float NewGame_wait_duration;
    private WaitForSeconds newgame_wait;
    public GameObject Day_Model;
    public GameObject Night_Model;
    public AudioSource BGM_source;
    public AudioClip StartGame_clip;

    private WaitForSeconds blink_wait;
    // Start is called before the first frame update
    void Start()
    {
        newgame_wait = new WaitForSeconds(NewGame_wait_duration);
        blink_wait = new WaitForSeconds(Eyelid_script.Blink_duration);
        OpenMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        //When receiving"Back" input
        if(Input.GetButtonDown("Cancel"))
        {
            switch(Current_menu)
            {
                case Menu_status.Main_menu:
                break;
                case Menu_status.Option_menu:
                OpenMainMenu();
                PlayCancelSE();
                break;
                case Menu_status.Level_select:
                OpenMainMenu();
                PlayCancelSE();
                break;
            }
        }
    }

    public void OpenMainMenu()
    {
        
        StartCoroutine(OpenMainMenuDelay());
    }
    private IEnumerator OpenMainMenuDelay()
    {
        Eyelid_script.EyeClose();
        yield return blink_wait;
        Title_text.SetActive(true);
        Main_menu_page.SetActive(true);
        Option_menu_page.SetActive(false);
        Level_select_page.SetActive(false);
        Eyelid_script.EyeOpen();
        yield return blink_wait;
        Current_menu = Menu_status.Main_menu;

    }

    public void OpenOptionMenu()
    {
        //Eyelid_script.Blink();
        StartCoroutine(OpenOptionMenuDelay());
    }
    private IEnumerator OpenOptionMenuDelay()
    {
        Eyelid_script.EyeClose();
        yield return blink_wait;
        Title_text.SetActive(false);
        Main_menu_page.SetActive(false);
        Option_menu_page.SetActive(true);
        Level_select_page.SetActive(false);
        Eyelid_script.EyeOpen();
        yield return blink_wait;
        Current_menu = Menu_status.Option_menu;
    }

    public void OpenSelectMenu()
    {
        
        StartCoroutine(OpenSelectMenuDelay());
    }
    private IEnumerator OpenSelectMenuDelay()
    {
        Eyelid_script.EyeClose();

        yield return blink_wait;
        Title_text.SetActive(false);
        Main_menu_page.SetActive(false);
        Option_menu_page.SetActive(false);
        Level_select_page.SetActive(true);
        Eyelid_script.EyeOpen();
        yield return blink_wait;
        Current_menu = Menu_status.Level_select;

    }


    public void StartNewGame()
    {
        LevelData.Selected_level = 1;
        StartCoroutine(StartGameDelay());
        
    }

    private IEnumerator StartGameDelay()
    {
        Eyelid_script.EyeClose();
        BGM_source.DOFade(0,Eyelid_script.Blink_duration);
        yield return blink_wait;
        Title_text.SetActive(true);
        Day_Model.SetActive(false);
        Night_Model.SetActive(true);
        Main_menu_page.SetActive(false);
        Level_select_page.SetActive(false);
        Eyelid_script.EyeOpen();
        BGM_source.Stop();
        BGM_source.clip = StartGame_clip;
        BGM_source.volume = 0.6f;
        BGM_source.Play();
        yield return newgame_wait;
        SceneManager.LoadScene(1);
    }

    public void StartSelectedLevel()
    {
        //LevelData.Selected_level = 1;
        StartCoroutine(StartGameDelay());
        
    }

    public void QuitGame()
    {
        Application.Quit();
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
