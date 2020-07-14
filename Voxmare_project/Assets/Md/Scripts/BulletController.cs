using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//[System.Serializable] public class CollisionEvent : UnityEvent<Transform> { }
public class BulletController : MonoBehaviour
{
    [Header("Basic")]
    public int Damage;
    public float Bullet_speed;
    public bool Deflectable;
    public bool Damage_player;
    public MeshRenderer Bullet_render;
    public Material[] Bullet_materials;//0:Enemy Bullet, 1:Entered Boundry, 2:Deflected Bullet
    //public CollisionEvent onHit;

    [Header("Bonus Effect")]
    public bool Penetrate;
    public float Penetrate_lifespan;
    public bool Homing;
    public GameObject Homing_target;
    public float Homing_rotation_speed;
    public float Homing_distance;
    private Vector3 homing_vector;
    public bool Reflect;
    public float Reflect_lifespan;
    private RaycastHit reflect_hit;
    public bool Cluster;
    public GameObject Cluster_prefab;
    private ClusterBulletController cluster_controller;
    private bool shuttingdown;
    // Start is called before the first frame update
    void Start()
    {
        Bullet_render=this.GetComponent<MeshRenderer>();
        if(Homing)
        {
            Homing_target = GameObject.Find("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.forward*Bullet_speed* Time.deltaTime);

        if(Homing)
        {
            if(Homing_target != null)
            {
                homing_vector=Homing_target.transform.position-this.transform.position;
                if(homing_vector.magnitude>Homing_distance)
                {
                    this.transform.rotation=Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(homing_vector),Homing_rotation_speed);
                }
                else
                {
                    Homing = false;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(Damage_player)
            {
                other.gameObject.SendMessage("ApplyDamage", Damage);
                Destroy(this.gameObject);
            }
        }   
        else if(other.CompareTag("Enemy"))
        {            
            if(!Damage_player)
            {      
                if(Penetrate)//Penetrate effect has higher priority over Reflect effect
                {
                    other.gameObject.SendMessage("ApplyDamage", Damage);
                    Destroy(this.gameObject,Penetrate_lifespan);
                }     
                else
                {
                    if(Reflect)
                    {
                        Physics.Raycast(this.transform.position-this.transform.forward*0.5f,this.transform.forward,out reflect_hit, 2f);
                        this.transform.rotation=Quaternion.LookRotation(Vector3.Reflect(this.transform.forward,reflect_hit.normal));
                        other.gameObject.SendMessage("ApplyDamage", Damage);
                        Destroy(this.gameObject,Reflect_lifespan);
                    }
                    else
                    {
                        other.gameObject.SendMessage("ApplyDamage", Damage);
                        Destroy(this.gameObject);
                    }  
                }      
            }
        }
        else if(other.CompareTag("Terrian"))
        {
            if(!Reflect)
            {
                Destroy(this.gameObject);
            }
            else
            {
                bool result = Physics.Raycast(this.transform.position-this.transform.forward*0.5f,this.transform.forward,out reflect_hit, 2f);
                //Debug.DrawRay(this.transform.position-this.transform.forward*0.2f, this.transform.forward*1f, Color.yellow,1);
                this.transform.rotation=Quaternion.LookRotation(Vector3.Reflect(this.transform.forward,reflect_hit.normal));
                //Debug.Log(result);
                Destroy(this.gameObject,Reflect_lifespan);
            }
        }
    }

    void OnApplicationQuit()
    {
        this.shuttingdown = true;
    }

    private void OnDestroy() 
    {
        if(!shuttingdown)
        {
            if(Cluster)
            {
                cluster_controller = Instantiate(Cluster_prefab, this.transform.position, Quaternion.identity).GetComponent<ClusterBulletController>();
                cluster_controller.Damage_player = Damage_player;
            }
        }
    }

    public void ChangeMaterial(int index)
    {
        Bullet_render.material = Bullet_materials[index];
    }
}
