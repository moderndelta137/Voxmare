using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticle : MonoBehaviour
{
    private ParticleSystem this_particle;
    // Start is called before the first frame update
    private void OnEnable() {
        this_particle = GetComponent<ParticleSystem>();
        Destroy(this.gameObject,this_particle.main.duration);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
