using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpammerController : MonoBehaviour
{
    public GameObject Bullet;
    public float Interval;
    public Vector3 Spam_position_offset;

    // Start is called before the first frame update
    void  Start()
    {
         //yield return StartCoroutine("WaitAndSpam");
        StartCoroutine("WaitAndSpam");
        //yield return new WaitForSeconds(Interval);
        //StopCoroutine("WaitAndSpam");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator WaitAndSpam()
    {
        while (true)
        {
        // suspend execution for 5 seconds
        //WaitForSeconds(Interval);
        yield return new WaitForSeconds(Interval);

        Instantiate(Bullet,this.transform.position,this.transform.rotation);
        yield return null;
        }
    }
}
