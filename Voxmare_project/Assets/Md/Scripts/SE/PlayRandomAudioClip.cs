using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayRandomAudioClip : MonoBehaviour
{
    public AudioClip[] clips;
    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = clips[Random.Range(0,clips.Length)];
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
