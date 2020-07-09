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
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, shoot_layerMask))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow,5);
            }
            foreach(RaycastHit bullet in hits)
            {
                if(bullet.transform.CompareTag("Bullet"))
                {
                    bullet_instance=bullet.transform.GetComponent<BulletController>();
                    Bullet_list.Add(bullet_instance);
                    bullet_instance.Damage_Player=false;
                    shoot_vector = hit.point-bullet.transform.position;
                    bullet_instance.transform.rotation = Quaternion.LookRotation(shoot_vector);
                    //Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look_input_vector), Rotate_speed);
                    bullet_instance.ChangeMaterial(2);
                }
            }


        }
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Bullet"))
        {
            bullet_instance=other.GetComponent<BulletController>();
            bullet_instance.Bullet_speed=bullet_instance.Bullet_speed*Boundry_slowdown_scale;
            bullet_instance.ChangeMaterial(1);
        }
    }
    private void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Bullet"))
        {
            bullet_instance=other.GetComponent<BulletController>();
            bullet_instance.Bullet_speed=bullet_instance.Bullet_speed/Boundry_slowdown_scale;
            if(bullet_instance.Damage_Player)
                bullet_instance.ChangeMaterial(0);
        }
    }
    
}
