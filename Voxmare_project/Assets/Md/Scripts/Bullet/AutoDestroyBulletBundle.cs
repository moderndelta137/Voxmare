using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyBulletBundle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.DetachChildren();
        Destroy(this.transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
