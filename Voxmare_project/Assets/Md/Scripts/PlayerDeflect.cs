using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeflect : MonoBehaviour
{
    public List<BulletController> Bullet_list;
    public RaycastHit[] hits;
    public float Sphere_radius;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Bullet_list.Clear();
        hits=null;
        hits=Physics.SphereCastAll(this.transform.position,Sphere_radius,Vector3.up,0);
        foreach(RaycastHit hit in hits)
        {
            if(hit.transform.CompareTag("Bullet"))
            {
                Bullet_list.Add(hit.transform.GetComponent<BulletController>());
            }
        }
        if(Input.GetButtonDown("Fire1"))
        {
            foreach(BulletController bullet in Bullet_list)
            {
                bullet.Damage_Player=false;
                bullet.transform.rotation = this.transform.rotation;
                //Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look_input_vector), Rotate_speed);
            }
        }
    }
    /*
    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log("Collide");
        if(other.CompareTag("Bullet"))
        {
            Bullet_list.Add(other.GetComponent<BulletController>());
        }
    }
    private void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Bullet"))
        {
            Bullet_list.Remove(other.GetComponent<BulletController>());
        }
    }
    */
}
