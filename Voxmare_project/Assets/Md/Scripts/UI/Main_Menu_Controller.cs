using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    // Start is called before the first frame update
    void Start()
    {
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
                break;
                case Menu_status.Level_select:
                OpenMainMenu();
                break;
            }
        }
    }

    public void OpenMainMenu()
    {
        Title_text.SetActive(true);
        Main_menu_page.SetActive(true);
        Option_menu_page.SetActive(false);
        Level_select_page.SetActive(false);
        Current_menu = Menu_status.Main_menu;
    }
    public void OpenOptionMenu()
    {
        Title_text.SetActive(false);
        Main_menu_page.SetActive(false);
        Option_menu_page.SetActive(true);
        Level_select_page.SetActive(false);
        Current_menu = Menu_status.Option_menu;
    }

    public void OpenSelectMenu()
    {
        Title_text.SetActive(false);
        Main_menu_page.SetActive(false);
        Option_menu_page.SetActive(false);
        Level_select_page.SetActive(true);
        Current_menu = Menu_status.Level_select;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
