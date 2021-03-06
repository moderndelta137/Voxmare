﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health_Bar : MonoBehaviour
{
    public Slider slider;
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.transform.forward);
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue=health;
        slider.value=health;
    }

    public void SetHealth(int health)
    {
        slider.value=health;
    }
}
