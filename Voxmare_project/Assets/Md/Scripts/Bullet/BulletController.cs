﻿using System.Collections;
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
    public float Bullet_force;

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
    public float DEBUG_reflect_cooldown;
    private WaitForSeconds reflect_wait;
    private RaycastHit reflect_hit;
    private int reflect_enemy_layMask;
    private int reflect_terrian_layMask;
    public bool Cluster;
    public GameObject Cluster_prefab;
    private ClusterBulletController cluster_controller;
    private bool shuttingdown;

    private Collider cd;
    // Start is called before the first frame update
    void Start()
    {
        Bullet_render=this.GetComponent<MeshRenderer>();
        if(Homing)
        {
            Homing_target = GameObject.Find("Player");
        }
        cd = this.GetComponent<Collider>();
        reflect_wait = new WaitForSeconds(DEBUG_reflect_cooldown);
        reflect_enemy_layMask = (1 << 9);
        reflect_terrian_layMask = (1 << 11);
        Destroy(this.gameObject,30);
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
        
        switch(other.tag)
        {
            case "Player":
                if(Damage_player)
                {
                    other.gameObject.SendMessage("ApplyDamage", Damage* this.transform.forward);
                    Destroy(this.gameObject);
                }
            break;
            case "Pickup":
                if(Damage_player)
                {
                    other.gameObject.SendMessage("ApplyDamage", Damage* this.transform.forward);
                    Destroy(this.gameObject);
                }
            break;         
            case "Enemy":
                if(!Damage_player)
                {      
                    if(Penetrate)//Penetrate effect has higher priority over Reflect effect
                    {
                        other.gameObject.SendMessage("ApplyDamage", Damage* this.transform.forward);
                        Destroy(this.gameObject,Penetrate_lifespan);
                    }     
                    else
                    {
                        if(Reflect)
                        {
                            Physics.Raycast(this.transform.position-this.transform.forward*0.7f,this.transform.forward,out reflect_hit, 5f,reflect_enemy_layMask);
                            this.transform.rotation=Quaternion.LookRotation(Vector3.Reflect(this.transform.forward, reflect_hit.normal));
                            other.gameObject.SendMessage("ApplyDamage", Damage* this.transform.forward);
                            Destroy(this.gameObject,Reflect_lifespan);
                        }
                        else
                        {
                            other.gameObject.SendMessage("ApplyDamage", Damage* this.transform.forward);
                            Destroy(this.gameObject);
                        }  
                    }      
                }
            break;    
            case "Terrian":
                other.attachedRigidbody.AddForce(this.transform.forward*Bullet_force);
                if(!Reflect)
                {   
                    Destroy(this.gameObject);
                }
                else
                {
                    Physics.Raycast(this.transform.position-this.transform.forward*0.7f,this.transform.forward,out reflect_hit, 5f, reflect_terrian_layMask);
                    this.transform.rotation=Quaternion.LookRotation(Vector3.Reflect(this.transform.forward, reflect_hit.normal));
                    StartCoroutine(ReflectCooldown());
                    Destroy(this.gameObject,Reflect_lifespan);
                }
            break;  
            case "Wall":
                Destroy(this.gameObject,5f);
                /*
                other.attachedRigidbody.AddForce(this.transform.forward*10f);
                if(!Reflect)
                {   
                    Destroy(this.gameObject);
                }
                else
                {
                    Physics.Raycast(this.transform.position-this.transform.forward*0.7f,this.transform.forward,out reflect_hit, 5f, reflect_terrian_layMask);
                    this.transform.rotation=Quaternion.LookRotation(Vector3.Reflect(this.transform.forward, reflect_hit.normal));
                    StartCoroutine(ReflectCooldown());
                    Destroy(this.gameObject,Reflect_lifespan);
                }
                */
            break;  
        }
        /*
        if(other.CompareTag("Player"))
        {
            if(Damage_player)
            {
                other.gameObject.SendMessage("ApplyDamage", Damage* this.transform.forward);
                Destroy(this.gameObject);
            }
        }   
        if(other.CompareTag("Pickup"))
        {
            if(Damage_player)
            {
                other.gameObject.SendMessage("ApplyDamage", Damage* this.transform.forward);
                Destroy(this.gameObject);
            }
        }   
        else if(other.CompareTag("Enemy"))
        {            
            if(!Damage_player)
            {      
                if(Penetrate)//Penetrate effect has higher priority over Reflect effect
                {
                    other.gameObject.SendMessage("ApplyDamage", Damage* this.transform.forward);
                    Destroy(this.gameObject,Penetrate_lifespan);
                }     
                else
                {
                    if(Reflect)
                    {
                        Physics.Raycast(this.transform.position-this.transform.forward*0.7f,this.transform.forward,out reflect_hit, 5f);
                        this.transform.rotation=Quaternion.LookRotation(Vector3.Reflect(this.transform.forward, reflect_hit.normal));
                        other.gameObject.SendMessage("ApplyDamage", Damage* this.transform.forward);
                        Destroy(this.gameObject,Reflect_lifespan);
                    }
                    else
                    {
                        other.gameObject.SendMessage("ApplyDamage", Damage* this.transform.forward);
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
                Physics.Raycast(this.transform.position-this.transform.forward*0.7f,this.transform.forward,out reflect_hit, 5f);
                //Debug.DrawRay(this.transform.position-this.transform.forward*0.2f, this.transform.forward*1f, Color.yellow,1);
                this.transform.rotation=Quaternion.LookRotation(Vector3.Reflect(this.transform.forward, reflect_hit.normal));
                this.transform.rotation=Quaternion.LookRotation(Vector3.Reflect(this.transform.forward, reflect_hit.normal));
                StartCoroutine(ReflectCooldown());
                Destroy(this.gameObject,Reflect_lifespan);
            }
        }
        */
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

    private IEnumerator ReflectCooldown()
    {
        cd.enabled =false;
        yield return reflect_wait;
        cd.enabled =true;
    }
}
