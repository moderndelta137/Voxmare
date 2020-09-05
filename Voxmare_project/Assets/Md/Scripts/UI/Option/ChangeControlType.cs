using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeControlType : MonoBehaviour
{
    //public int Control_type;
    public Slider Controltype_slider;
    // Start is called before the first frame update
    void Start()
    {
        if(!PlayerPrefs.HasKey("ControlType"))
        {
            PlayerPrefs.SetInt("ControlType",0);
        }
        //Control_type = PlayerPrefs.GetInt("ColorPalette");
        Controltype_slider.value = PlayerPrefs.GetInt("ControlType");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateControltype(float value)
    {
        PlayerPrefs.SetInt("ControlType",(int)value);
    }
}
