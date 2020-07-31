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

    public bool Rapidfire;
    public float Rapidfire_count;
    private float rapidfire_counter;
    public float Rapidfire_Interval;
    private IEnumerator rapidfire_coroutine;

    public Transform Target;
    public bool Lockon;
    public float Rotate_speed;
    private Vector3 lockon_vector;
    // Start is called before the first frame update
    void  Start()
    {

        StartCoroutine(WaitAndSpam());
        Target=GameObject.Find("Player").transform;
        if(Rapidfire)
        {           
            rapidfire_coroutine = RapidFire();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Lockon)
        {
            lockon_vector = Target.position-this.transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lockon_vector), Rotate_speed*Time.deltaTime);
        }
        
    }
    IEnumerator WaitAndSpam()
    {
        while (true)
        {
        yield return new WaitForSeconds(Interval+Random.Range(-Temporal_divergence,Temporal_divergence));
        if(Rapidfire)
        {
            rapidfire_counter = 0;
            StopCoroutine(rapidfire_coroutine);
            rapidfire_coroutine = RapidFire();
            StartCoroutine(rapidfire_coroutine);

        }
        else
        {
            bullet_instance=Instantiate(Bullet,this.transform.position,this.transform.rotation);
            bullet_instance.transform.Rotate(Random.Range(-Angular_divergence,Angular_divergence)*Vector3.up,Space.Self);
        }
        yield return null;
        }
    }

    IEnumerator RapidFire()
    {
        while(rapidfire_counter<Rapidfire_count)
        {

            yield return new WaitForSeconds(Rapidfire_Interval);
            bullet_instance=Instantiate(Bullet,this.transform.position,this.transform.rotation);
            bullet_instance.transform.Rotate(Random.Range(-Angular_divergence,Angular_divergence)*Vector3.up,Space.Self);
            rapidfire_counter += 1;
        }
        yield return null;
    }
}
