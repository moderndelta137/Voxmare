using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ChangeAudioVolume : MonoBehaviour
{
    public AudioMixer SE_mixer;
    public string PlyaerPrefs_Key;
    public Slider SE_slider;
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey(PlyaerPrefs_Key))
        {
        SE_mixer.SetFloat("Volume",PlayerPrefs.GetFloat(PlyaerPrefs_Key));
        SE_slider.value = PlayerPrefs.GetFloat(PlyaerPrefs_Key);
        }
        else{
        SE_mixer.SetFloat("Volume",0);
        SE_slider.value = 0;
        PlayerPrefs.SetFloat(PlyaerPrefs_Key,0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetVolume(float value)
    {
        SE_mixer.SetFloat("Volume",value);
        PlayerPrefs.SetFloat(PlyaerPrefs_Key,value);
    }
}
