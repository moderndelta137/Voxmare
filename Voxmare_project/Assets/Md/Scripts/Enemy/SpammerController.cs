using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpammerController : MonoBehaviour
{
    public GameObject Bullet;
    public float Start_delay;
    public float Start_delay_divergence;
    public float Interval;
    public float Angular_divergence;
    public float Temporal_divergence;
    private GameObject bullet_instance;

    [Header("Rapidfire")]
    public bool Rapidfire;
    public float Rapidfire_count;
    private float rapidfire_counter;
    public float Rapidfire_Interval;
    private WaitForSeconds rapid_wait;
    private IEnumerator rapidfire_coroutine;
    [Header("Lockon")]
    public Transform Target;
    public bool Lockon;
    public float Rotate_speed;
    private Vector3 lockon_vector;
    [Header("Animation")]
    public float Shrink_scale;
    public float Shrink_duration;
    public float Expand_scale;
    public float Expand_duration;
    private Transform parent;
    private Tween shootTween;

    void Awake()
    {
        Target = GameObject.Find("Player").transform;
    }

    // Start is called before the first frame update
    void  Start()
    {

        StartCoroutine(StartDelay());
        if(Rapidfire)
        {           
            rapidfire_coroutine = RapidFire();
        }

        parent = this.transform.parent;
        rapid_wait = new WaitForSeconds(Rapidfire_Interval);
    }

    // Update is called once per frame
    void Update()
    {
        //if(Lockon)
        //{
        //    lockon_vector = Target.position-this.transform.position;
        //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lockon_vector), Rotate_speed*Time.deltaTime);
        //}
        
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(Start_delay+Random.Range(-Start_delay_divergence,Start_delay_divergence));
        StartCoroutine(WaitAndSpam());
    }

    private IEnumerator WaitAndSpam()
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
            //Shoot
            StartCoroutine(ShootAnimation());
            //ShootBullet();
        }
        yield return null;
        }
    }

    IEnumerator RapidFire()
    {
        while(rapidfire_counter<Rapidfire_count)
        {

            yield return rapid_wait;
            //Shoot
            StartCoroutine(ShootAnimation());
            //ShootBullet();
            rapidfire_counter += 1;
        }
        yield return null;
    }

    public IEnumerator ShootAnimation()
    {
        shootTween = parent.DOScale(Vector3.one*Shrink_scale,Shrink_duration);
        yield return shootTween.WaitForCompletion();
        ShootBullet();
        shootTween = parent.DOScale(Vector3.one*Expand_scale,Expand_duration);
        yield return shootTween.WaitForCompletion();
        shootTween = parent.DOScale(Vector3.one,Expand_duration);

    }

    public void ShootBullet()
    {
        bullet_instance=Instantiate(Bullet,this.transform.position,this.transform.rotation);
        bullet_instance.transform.Rotate(Random.Range(-Angular_divergence,Angular_divergence)*Vector3.up,Space.Self);
    }
}
