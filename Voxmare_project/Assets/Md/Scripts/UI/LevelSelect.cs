using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public int Selected_level;
    public Slider Level_slider;
    public Text Level_text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSelectedLevel()
    {
        Selected_level=(int)Level_slider.value;
        Level_text.text = Level_slider.value.ToString();
    }

    public void StartSelectedLevel()
    {

        SceneManager.LoadScene(1);
    }
}
