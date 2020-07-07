using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class CollisionEvent : UnityEvent<Transform> { }
public class BulletController : MonoBehaviour
{
    public CollisionEvent onHit;
    public int Damage;
    public float Bullet_speed;
    public bool Reflectable;
    public bool Harmful;
    public float Life_span;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject,Life_span);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.forward*Bullet_speed);
    }

     private void OnTriggerEnter(Collider other)
     {
        if(other.CompareTag("Player"))
         {
            onHit.Invoke(other.transform);
            Destroy(this.gameObject);
         }
        else if(other.CompareTag("Enemy"))
         {
            onHit.Invoke(other.transform);
            Destroy(this.gameObject);
         }
     }
}
