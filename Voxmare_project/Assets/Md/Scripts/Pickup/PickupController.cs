using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class PickupController : MonoBehaviour
{
    public bool Pickedup;
    public int Type;
    /*
    0 = deflect radius increase
    1 = Velocity_bonus, velocity increase
    2 = Reflect_bonus, lifespan increase
    3 = Penetrate_bonus, lifespan increase
    4 = Cluster_bonus, Cluster radius increase
    5 = Homing_bonus, Homing rotaiton speed increase
    6 = Spray_bonus, bullet count increase
    movespeed?
    Max HP?
    */
    public PlayerPIckup Pickup_manager;

    [Header("Hit Reaction")]
    public Material Hit_reaction_mat;
    private Material original_mat;
    private MeshRenderer rend;
    public float DEBUG_hit_reaction_duration;
    public float DEBUG_hit_reaction_flinch;
    private int hit_reacting;
    private Tween myTween;
    private Vector3 hit_reaction_original_position;
    private Vector3 temp_position;

    [Header("Crowd Control")]
    //public bool Can_move;
    public float Lookat_distance;
    public float Lookat_speed;
    public Transform Nav_target;
    private NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponentInChildren<MeshRenderer>();
        agent = GetComponent<NavMeshAgent>();
       
        original_mat = rend.material;
    }

    // Update is called once per frame
    void Update()
    {
        if(Pickedup)
        {
         agent.SetDestination(Nav_target.position);
         if(agent.remainingDistance<Lookat_distance)
         {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Nav_target.forward), Lookat_speed*Time.deltaTime);           
         }
        }

    }

    public IEnumerator ApplyDamage(Vector3 Incoming)
    {
        rend.material = Hit_reaction_mat;
        if(hit_reacting == 0)
        {
            hit_reaction_original_position = this.transform.position;
        }
        hit_reacting += 1;
        myTween = this.transform.DOMove(this.transform.position + Incoming.normalized * DEBUG_hit_reaction_flinch, DEBUG_hit_reaction_duration);
        yield return myTween.WaitForCompletion();
        myTween = this.transform.DOMove(hit_reaction_original_position, DEBUG_hit_reaction_duration);
        yield return myTween.WaitForCompletion();
        hit_reacting -= 1;
        rend.material = original_mat;
        ResetY();
        if(Pickedup)
        {
            if(Pickup_manager!=null)
            {
                Pickup_manager.RemovePickup(this);
                this.transform.parent=null;
                //TODO Cool Dwon
                Pickedup = false;
            }
        }
    }

    public void ResetY()
    {
        temp_position = this.transform.position;
        temp_position.y = 0;
        this.transform.position = temp_position;
    }
}
