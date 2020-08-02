﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerDeflect : MonoBehaviour
{
    [Header("Boundry")]
    public float[] Boundary_radius;
    public float Boundary_mesh_scaler;
    public float Boundry_slowdown_scale;
    public List<BulletController> Bullet_list;
    private int bullet_layerMask;
    private BulletController bullet_instance;
    private RaycastHit[] hits;

    [Header("Shoot")]
    public bool Can_deflect;
    public float Deflect_cooldown;
    private WaitForSeconds deflect_wait;
    public float DEBUG_cooldown_scale_duration;
    private RaycastHit hit;
    private int shoot_layerMask;
    private Vector3 shoot_vector;

    private PlayerMovement movement_script;

    [Header("Deflect Effect")]
    public bool Velocity_bonus;//Increase bullet velocity after deflection
    public float[] Velocity_bonus_scale;
    public bool Spray_bonus;//Apply penetrate effect on bullet after deflection
    public int[] Spray_count;
    public float[] Spray_range;
    private float spray_rotate;
    private GameObject spray_instance;
    public bool Penetrate_bonus;//Apply penetrate effect on bullet after deflection
    public float[] Penetrate_lifespan;
    public bool Homing_bonus;//Apply homing effect on bullet after deflection
    private int homing_layerMask;
    public float Homing_cast_radius;
    public float[] Homing_rotation_speed;
    private RaycastHit homing_hit;
    public bool Reflect_bonus;//Apply reflect effect on bullet after deflection
    public float[] Reflect_lifespan; 
    public bool Cluster_bonus;//Apply cluster effect on bullet after deflection
    public float[] Cluster_radius; 

    [Header("Effect Rank")]
    public int Radius_rank;
    public int Velocity_rank;
    public int Reflect_rank;
    public int Penetrate_rank;
    public int Cluster_rank;
    public int Homing_rank;
    public int Spray_rank;

    [Header("Animation")]
    private Animator player_animator;
    private PlayerPickup pickup_zone_script;
    // Start is called before the first frame update
    void Start()
    {
        player_animator = this.transform.parent.GetComponentInChildren<Animator>();
        bullet_layerMask = 1 << 10;//Check Bullet Layer
        homing_layerMask = 1 << 9;
        shoot_layerMask = (1 << 9) + (1 << 11);//Check Enemy Layer and Terrian Layer for shooting
        UpdateBoundryRadius();
        Can_deflect = true;
        deflect_wait = new WaitForSeconds(Deflect_cooldown);
        pickup_zone_script = this.transform.parent.GetComponentInChildren<PlayerPickup>();
        movement_script = this.transform.parent.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {        
            if(Can_deflect)
            {

                StartCoroutine(DeflectCooldown());
                //Update Animation
                player_animator.SetTrigger("Deflect");
                foreach(PickupController pickup in pickup_zone_script.pickup_list)
                {
                    pickup.DeflectAnimation();
                }

                //Sphere cast to find all bullets inside the boundary
                //Bullet_list.Clear();
                hits=null;
                hits=Physics.SphereCastAll(this.transform.position, Boundary_radius[Radius_rank], Vector3.up, 0, bullet_layerMask);
                //Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, shoot_layerMask);
                //Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, shoot_layerMask);
                Vector3 temp_position;
                foreach(RaycastHit bullet in hits)
                {
                    if(bullet.transform.CompareTag("Bullet"))
                    {            
                        bullet_instance=bullet.transform.GetComponent<BulletController>();
                        if(bullet_instance.Deflectable)
                        {
                            //Deflect Bullet
                            //Bullet_list.Add(bullet_instance);
                            bullet_instance.Damage_player=false;
                            bullet_instance.Deflectable=false;
                            temp_position = bullet_instance.transform.position;
                            temp_position.y = 0;
                            bullet_instance.transform.position = temp_position;
                            //Debug.Log(bullet_instance.transform.position);
                            //shoot_vector = hit.point-bullet.transform.position;
                            shoot_vector = movement_script.Mouse_cursor.transform.position-bullet.transform.position;
                            shoot_vector.y = 0;
                            bullet_instance.transform.rotation = Quaternion.LookRotation(shoot_vector);
                            bullet_instance.ChangeMaterial(2);

                            //Apply Deflect Bonus
                            ApplyPowerBonus();
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
            if(bullet_instance.Damage_player)
            {
                bullet_instance.Bullet_speed=bullet_instance.Bullet_speed*Boundry_slowdown_scale;
                if(bullet_instance.Deflectable)
                    bullet_instance.ChangeMaterial(1);
            }
        }
    }
    private void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Bullet"))
        {
            bullet_instance=other.GetComponent<BulletController>();
            bullet_instance.Bullet_speed=bullet_instance.Bullet_speed/Boundry_slowdown_scale;
            if(bullet_instance.Damage_player)
            {
                bullet_instance.ChangeMaterial(0);
            }
        }
    }

    private void UpdateBoundryRadius()
    {
        this.transform.localScale = Vector3.one * Boundary_radius[Radius_rank] * Boundary_mesh_scaler;
    }

    public void RankupAnimation()
    {
        player_animator.SetTrigger("Rankup");
    }

    public IEnumerator DeflectCooldown()
    {
        Can_deflect=false;
        this.transform.DOScale(Vector3.zero,DEBUG_cooldown_scale_duration);
        movement_script.can_move = false;
        yield return deflect_wait;
        Can_deflect = true;
        this.transform.DOScale(Vector3.one * Boundary_radius[Radius_rank] * Boundary_mesh_scaler, DEBUG_cooldown_scale_duration);
        movement_script.can_move = true;
        
    }

    private void ApplyPowerBonus()
    {
        if(Velocity_bonus)
        {
            bullet_instance.Bullet_speed *= Velocity_bonus_scale[Velocity_rank];
        }
        if(Reflect_bonus)
        {
            bullet_instance.Reflect = true;
        }
        if(Penetrate_bonus)
        {
            bullet_instance.Penetrate = true;
        }
        if(Cluster_bonus)
        {
            bullet_instance.Cluster = true;
        }
        if(Homing_bonus)
        {
            if(Physics.SphereCast(this.transform.position, Homing_cast_radius, this.transform.forward, out homing_hit, 100f, homing_layerMask))
            {
                Debug.Log(homing_hit.transform.gameObject);
                bullet_instance.Homing_target = homing_hit.transform.gameObject;
                bullet_instance.Homing = true;
            }
        }
        if(Spray_bonus)
        {
            if(Spray_count[Spray_rank]>1)
            {
                //Rotate the first bullet
                bullet_instance.transform.Rotate(Vector3.up * - Spray_range[Spray_rank] / 2.0f, Space.Self);
                spray_instance = bullet_instance.gameObject;
                for(int i = 1; i < Spray_count[Spray_rank]; i++)
                {
                    spray_instance = Instantiate(spray_instance, spray_instance.transform.position, spray_instance.gameObject.transform.rotation);
                    //spray_rotate
                    spray_instance.transform.Rotate(Vector3.up * Spray_range[Spray_rank] / (Spray_count[Spray_rank] - 1), Space.Self);
                }
            }
        }
    }
    public void UpdatePowerRank(int type, int rank)
    {
        switch(type)
        {
            case 0:
                Radius_rank = Mathf.Clamp(Radius_rank = rank, 0, Boundary_radius.Length-1);
                UpdateBoundryRadius();
            break;
            
            case 1:
                Velocity_rank = Mathf.Clamp(Velocity_rank = rank, 0, Velocity_bonus_scale.Length-1);
                Velocity_bonus = true;
            break;

            case 2:
                Reflect_rank = Mathf.Clamp(Reflect_rank = rank, 0, Reflect_lifespan.Length-1);
                Reflect_bonus = true;
            break;

            case 3:
                Penetrate_rank = Mathf.Clamp(Penetrate_rank = rank, 0, Penetrate_lifespan.Length-1);
                Penetrate_bonus = true;
            break;

            case 4:
                Cluster_rank = Mathf.Clamp(Cluster_rank = rank, 0, Cluster_radius.Length-1);
                Cluster_bonus = true;
            break;

            case 5:
                Homing_rank = Mathf.Clamp(Homing_rank = rank, 0, Homing_rotation_speed.Length-1);
                Homing_bonus = true;
            break;

            case 6:
                Spray_rank = Mathf.Clamp(Spray_rank = rank, 0, Spray_count.Length-1);
                Spray_bonus = true;
            break;
        }
    }
}
