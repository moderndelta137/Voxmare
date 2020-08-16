using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Banner_Controller : MonoBehaviour
{
    public GameObject Prepare_screen;
    public Text Prepare_level_text;
    public GameObject Prepare_ready;
    public GameObject Clear_screen;
    public GameObject Clear_ready;

    public AudioSource Prepare_SE_source;
    public AudioSource Clear_SE_source;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayPreapreScreen()
    {
        Prepare_level_text.text = "Level " + LevelData.Selected_level;
        Prepare_screen.SetActive(true);
        Clear_screen.SetActive(false);
        Prepare_ready.SetActive(false);
    }
    public void PrepareReady()
    {
        Prepare_ready.SetActive(true);
        Prepare_SE_source.Play();
    }
    public void DisplayClearScreen()
    {
        Prepare_level_text.text = "Level " + LevelData.Selected_level;
        Prepare_screen.SetActive(false);
        Clear_screen.SetActive(true);
        Clear_ready.SetActive(false);
        Clear_SE_source.Play();
    }
    public void ClearReady()
    {
        Clear_ready.SetActive(true);
    }
    public void LevelStart()
    {
        Prepare_screen.SetActive(false);
    }
}
