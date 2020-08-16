using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffleBGM : MonoBehaviour
{
    public AudioClip[] clips;
    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        ChangeClip();
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(!source.isPlaying)
        {
            ChangeClip();
            source.Play();
        }
    }

    private void ChangeClip()
    {
        source.clip = clips[Random.Range(0,clips.Length)];
    }

    public void PauseVolume()
    {
        source.volume *= 0.5f;
    }

    public void ResumeVolume()
    {
        source.volume *= 2f;
    }
}
