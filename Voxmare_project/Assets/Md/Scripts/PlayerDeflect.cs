using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeflect : MonoBehaviour
{
    [Header("Boundry")]
    public float Boundary_radius;
    public float Boundary_mesh_scaler;
    public float Boundry_slowdown_scale;
    public List<BulletController> Bullet_list;
    private BulletController bullet_instance;
    private RaycastHit[] hits;
    [Space]
    [Header("Shoot")]
    private RaycastHit hit;
    private int shoot_layerMask;
    private Vector3 shoot_vector;
    [Space]
    [Header("Deflect Effect")]
    public bool Velocity_bonus;//Increase bullet velocity after deflection
    public float Deflect_velocity_scale;
    public bool Spray_bonus;//Apply penetrate effect on bullet after deflection
    public int Spray_count;
    public float Spray_range;
    private float spray_rotate;
    private GameObject spray_instance;
    public bool Penetrate_bonus;//Apply penetrate effect on bullet after deflection TODO
    public bool Homing_bonus;//Apply homing effect on bullet after deflection TODO
    public bool Reflect_bonus;//Apply reflect effect on bullet after deflection TODO
    public bool Cluster_bonus;//Apply cluster effect on bullet after deflection TODO
    // Start is called before the first frame update
    void Start()
    {
        shoot_layerMask = (1 << 9) + (1 << 11);//Check Enemy Layer and Terrian Layer for shooting
        this.transform.localScale = Vector3.one * Boundary_radius * Boundary_mesh_scaler;
    }

    // Update is called once per frame
    void Update()
    {


        if(Input.GetButtonDown("Fire1"))
        {        
            Bullet_list.Clear();
            hits=null;
            hits=Physics.SphereCastAll(this.transform.position,Boundary_radius,Vector3.up,0);
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, shoot_layerMask);

            foreach(RaycastHit bullet in hits)
            {
                if(bullet.transform.CompareTag("Bullet"))
                {
                    bullet_instance=bullet.transform.GetComponent<BulletController>();
                    Bullet_list.Add(bullet_instance);
                    bullet_instance.Damage_Player=false;
                    shoot_vector = hit.point-bullet.transform.position;
                    bullet_instance.transform.rotation = Quaternion.LookRotation(shoot_vector);
                    bullet_instance.ChangeMaterial(2);

                    //Apply Deflect Bonus
                    if(Velocity_bonus)
                        bullet_instance.Bullet_speed *= Deflect_velocity_scale;
                    if(Spray_bonus)
                    {
                        if(Spray_count>1)
                        {
                            //Rotate the first bullet
                            bullet_instance.transform.Rotate(Vector3.up * - Spray_range / 2.0f, Space.Self);
                            spray_instance = bullet_instance.gameObject;
                            for(int i = 1; i < Spray_count; i++)
                            {
                                spray_instance = Instantiate(spray_instance, spray_instance.transform.position, spray_instance.gameObject.transform.rotation);
                                //spray_rotate
                                spray_instance.transform.Rotate(Vector3.up * Spray_range / (Spray_count - 1), Space.Self);

                            }
                        }

                    }

                }
            }


        }
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Bullet"))
        {   
            bullet_instance=other.GetComponent<BulletController>();
            if(bullet_instance.Damage_Player)
            {
            bullet_instance.Bullet_speed=bullet_instance.Bullet_speed*Boundry_slowdown_scale;
            bullet_instance.ChangeMaterial(1);
            }
        }
    }
    private void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Bullet"))
        {
            bullet_instance=other.GetComponent<BulletController>();
            if(bullet_instance.Damage_Player)
            {
                bullet_instance.Bullet_speed=bullet_instance.Bullet_speed/Boundry_slowdown_scale;
                bullet_instance.ChangeMaterial(0);
            }
        }
    }
    
}
