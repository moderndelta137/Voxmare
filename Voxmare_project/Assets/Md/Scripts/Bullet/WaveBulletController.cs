using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveBulletController : MonoBehaviour
{
    public Vector3 Scale_vector;
    public float Scale_speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localScale+=Scale_vector*Scale_speed*Time.deltaTime;
    }
}
