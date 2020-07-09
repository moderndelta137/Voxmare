using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGraze : MonoBehaviour
{
    public float Graze_slowdown_scale;
    public float Graze_slowdown_duration;
    public bool Grazing;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Bullet"))
        {
            //Time.timeScale=Graze_slowdown_scale;
            StartCoroutine("Slowdown");
        }
    }

    private IEnumerator Slowdown()
    {
        Time.timeScale=Graze_slowdown_scale;
        Grazing=true;
        yield return new WaitForSeconds(Graze_slowdown_duration);
        Time.timeScale=1.0f;
        Grazing=false;
        yield return null;
    }
}
