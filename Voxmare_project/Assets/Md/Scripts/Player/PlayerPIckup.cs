﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPIckup : MonoBehaviour
{
    public float Pickup_radius;
    private PlayerDeflect deflect_script;
    private PlayerMovement movement_script;
    private PickupController pickup_script;
    public List<PickupController> pickup_list;
    public int[] Powerup_ranks;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale = Vector3.one * Pickup_radius * 2.0f;
        deflect_script=this.transform.parent.GetComponentInChildren<PlayerDeflect>();
        movement_script=GetComponentInParent<PlayerMovement>();
        Powerup_ranks=new int[7];
        GetPlayerPowerRank();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private void GetPlayerPowerRank()
    {
        Powerup_ranks[0] = deflect_script.Radius_rank;
        Powerup_ranks[1] = deflect_script.Velocity_rank;
        Powerup_ranks[2] = deflect_script.Reflect_rank;
        Powerup_ranks[3] = deflect_script.Penetrate_rank;
        Powerup_ranks[4] = deflect_script.Cluster_rank;
        Powerup_ranks[5] = deflect_script.Homing_rank;
        Powerup_ranks[6] = deflect_script.Spray_rank;
    }
    

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Pickup"))
        {
            pickup_script = other.GetComponent<PickupController>();
            if(!pickup_list.Contains(pickup_script))
            {
                if(!pickup_script.Pickedup)
                {
                    //Pick up pickup ;)
                    pickup_script.Pickedup = true;
                    pickup_script.Pickup_manager = this;
                    pickup_script.Nav_target = this.transform.parent;
                    pickup_list.Add(pickup_script);
                    Powerup_ranks[pickup_script.Type] += 1;
                    deflect_script.UpdatePowerRank(pickup_script.Type,Powerup_ranks[pickup_script.Type]);
                    deflect_script.RankupAnimation();
                }
            }
        }
    }

    public void RemovePickup(PickupController pickup)
    {
        //Remove Pickup
        pickup_list.Remove(pickup);
        Powerup_ranks[pickup.Type] -= 1;
        deflect_script.UpdatePowerRank(pickup.Type,Powerup_ranks[pickup.Type]);
        pickup.Pickedup = false;
        //pickup.Can_move=true;
    }
}
