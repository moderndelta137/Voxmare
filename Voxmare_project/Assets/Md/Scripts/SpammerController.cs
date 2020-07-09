using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpammerController : MonoBehaviour
{
    public GameObject Bullet;
    public float Interval;
    public float Angular_divergence;
    public float Temporal_divergence;
    private GameObject bullet_instance;

    public GameObject Target;
    public bool Lockon;
    public float Rotate_speed;
    private Vector3 lockon_vector;
    // Start is called before the first frame update
    void  Start()
    {
         //yield return StartCoroutine("WaitAndSpam");
        StartCoroutine("WaitAndSpam");
        //yield return new WaitForSeconds(Interval);
        //StopCoroutine("WaitAndSpam");
        Target=GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(Lockon)
        {
            lockon_vector = Target.transform.position-this.transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lockon_vector), Rotate_speed*Time.deltaTime);

        }
    }
    IEnumerator WaitAndSpam()
    {
        while (true)
        {
        // suspend execution for 5 seconds
        //WaitForSeconds(Interval);
        yield return new WaitForSeconds(Interval+Random.Range(-Temporal_divergence,Temporal_divergence));

        bullet_instance=Instantiate(Bullet,this.transform.position,this.transform.rotation);
        bullet_instance.transform.Rotate(Random.Range(-Angular_divergence,Angular_divergence)*Vector3.up,Space.Self);
        yield return null;
        }
    }
}
