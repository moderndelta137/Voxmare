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
        UpdateMaxLevel();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        //ONLY FOR DEBUG
        //PlayerPrefs.SetInt("MaxLevel",1);
    }

    public void UpdateMaxLevel()
    {
        if(!PlayerPrefs.HasKey("MaxLevel"))
        {
            PlayerPrefs.SetInt("MaxLevel",1);
        }
        if(PlayerPrefs.GetInt("MaxLevel")>20)
        {
            PlayerPrefs.SetInt("MaxLevel",20);
        }
        Level_slider.maxValue = PlayerPrefs.GetInt("MaxLevel") + 1;
    }

    public void UpdateSelectedLevel()
    {
        Selected_level=(int)Level_slider.value;
        Level_text.text = Level_slider.value.ToString();
        LevelData.Selected_level = Selected_level;
    }

}
