﻿using System.Collections;
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
        
    }

    public void UpdateMaxLevel()
    {
        if(!PlayerPrefs.HasKey("MaxLevel"))
        {
            PlayerPrefs.SetInt("MaxLevel",1);
        }
        Level_slider.maxValue = PlayerPrefs.GetInt("MaxLevel");
    }

    public void UpdateSelectedLevel()
    {
        Selected_level=(int)Level_slider.value;
        Level_text.text = Level_slider.value.ToString();
        LevelData.Selected_level = Selected_level;
    }

    public void StartSelectedLevel()
    {

        SceneManager.LoadScene(1);
    }
}
