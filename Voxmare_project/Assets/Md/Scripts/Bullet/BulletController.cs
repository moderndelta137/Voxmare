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
    public float Cluster_radius_scale;
    public GameObject Cluster_prefab;
    private ClusterBulletController cluster_controller;
    private bool shuttingdown;

    private Collider cd;

    public GameObject Hit_fx_prefab;//Only a placehold FX, feel free to change it.
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
                {   if(!Cluster)
                        other.gameObject.SendMessage("ApplyDamage", Damage* this.transform.forward);
                    ShowHitFX();//TODO change it to a hit enemy effect
                    Destroy(this.gameObject);
                }
            break;
            case "Pickup":
                if(Damage_player)
                {
                    if(!Cluster)
                        other.gameObject.SendMessage("ApplyDamage", Damage* this.transform.forward);
                    ShowHitFX();//TODO change it to a hit enemy effect
                    Destroy(this.gameObject);
                }
            break;         
            case "Enemy":
                if(!Damage_player)
                {      
                    if(Reflect)
                    {
                        Physics.Raycast(this.transform.position-this.transform.forward*0.7f,this.transform.forward,out reflect_hit, 5f,reflect_enemy_layMask);
                        this.transform.rotation=Quaternion.LookRotation(Vector3.Reflect(this.transform.forward, reflect_hit.normal));
                        if(Cluster)
                        {
                            cluster_controller = Instantiate(Cluster_prefab, this.transform.position, Quaternion.identity).GetComponent<ClusterBulletController>();
                            cluster_controller.Radius *= Cluster_radius_scale;
                            cluster_controller.transform.localScale*=Cluster_radius_scale;
                            cluster_controller.Damage_player = Damage_player;
                        }
                        else
                            other.gameObject.SendMessage("ApplyDamage", Damage* this.transform.forward);
                        ShowHitFX();//TODO change it to a hit enemy effect
                        Destroy(this.gameObject,Reflect_lifespan);
                    }
                    else if(Penetrate)//Penetrate effect has higher priority over Reflect effect
                    {
                        ShowHitFX();//TODO change it to a hit enemy effect
                        if(!Cluster)
                            other.gameObject.SendMessage("ApplyDamage", Damage* this.transform.forward);
                        Destroy(this.gameObject,Penetrate_lifespan);
                    }     
                    else
                    {
                        ShowHitFX();//TODO change it to a hit enemy effect
                        if(!Cluster)
                            other.gameObject.SendMessage("ApplyDamage", Damage* this.transform.forward);
                        Destroy(this.gameObject);
                    }     
                }
            break;    
            case "Terrian":
                other.attachedRigidbody.AddForce(this.transform.forward*Bullet_force,ForceMode.VelocityChange);
                if(Reflect)
                {   
                    Physics.Raycast(this.transform.position-this.transform.forward*0.7f,this.transform.forward,out reflect_hit, 5f, reflect_terrian_layMask);
                    this.transform.rotation=Quaternion.LookRotation(Vector3.Reflect(this.transform.forward, reflect_hit.normal));
                    StartCoroutine(ReflectCooldown());
                    if(Cluster)
                    {
                        cluster_controller = Instantiate(Cluster_prefab, this.transform.position, Quaternion.identity).GetComponent<ClusterBulletController>();
                        cluster_controller.Radius *= Cluster_radius_scale;
                        cluster_controller.transform.localScale*=Cluster_radius_scale;
                        cluster_controller.Damage_player = Damage_player;
                    }
                    ShowHitFX();
                    Destroy(this.gameObject,Reflect_lifespan);
                    
                }
                else if (Penetrate)
                {
                    Destroy(this.gameObject,Penetrate_lifespan);
                }
                else
                {
                    ShowHitFX();
                    Destroy(this.gameObject);
                }
            break;  
            case "Wall":
                Destroy(this.gameObject,5f);
            break;  
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
                cluster_controller.Radius *= Cluster_radius_scale;
                cluster_controller.transform.localScale*=Cluster_radius_scale;
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

    private void ShowHitFX()
    {
    if(Hit_fx_prefab!=null)
        Instantiate(Hit_fx_prefab, this.transform.position, Quaternion.identity);
    }
}
