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
    public bool Damage_Player;
    public MeshRenderer Bullet_render;
    public Material[] Bullet_materials;
    // Start is called before the first frame update
    void Start()
    {
        Bullet_render=this.GetComponent<MeshRenderer>();
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
            if(Damage_Player)
            {
                onHit.Invoke(other.transform);
                Destroy(this.gameObject);
            }
        }   
        else if(other.CompareTag("Enemy"))
        {            
            if(!Damage_Player)
            {
                onHit.Invoke(other.transform);
                Destroy(this.gameObject);
            }
        }
        else if(other.CompareTag("Terrian"))
        {
            Destroy(this.gameObject);
        }
    }

    public void ChangeMaterial(int index)
    {
        Bullet_render.material=Bullet_materials[index];
    }
}
