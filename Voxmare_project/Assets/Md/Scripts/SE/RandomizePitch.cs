using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizePitch : MonoBehaviour
{
    private AudioSource source;
    public Vector2 Pitch_range;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.pitch = Random.Range(Pitch_range.x,Pitch_range.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
