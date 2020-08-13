using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Banner_Controller : MonoBehaviour
{
    public GameObject Ready_screen;
    public Text Ready_level_text;
    public GameObject Clear_screen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayReadyScreen()
    {
        Ready_level_text.text = "Level " + LevelData.Selected_level;
        Ready_screen.SetActive(true);
        Clear_screen.SetActive(false);
    }

    public void DisplayClearScreen()
    {
        Ready_level_text.text = "Level " + LevelData.Selected_level;
        Ready_screen.SetActive(false);
        Clear_screen.SetActive(true);
    }
    public void LevelStart()
    {
        Ready_screen.SetActive(false);
    }
}
