﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    public static int Selected_level = 1;
    public static int Remain_core = 0;
    public static bool isPaused;
    // Start is called before the first frame update
     private void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
