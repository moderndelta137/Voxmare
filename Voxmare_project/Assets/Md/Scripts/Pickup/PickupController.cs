using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class PickupController : MonoBehaviour
{
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
    public PlayerPickup Pickup_manager;

    [Header("Hit Reaction")]
    private bool invencible;
    public Material Hit_reaction_mat;
    private Material original_mat;
    private SkinnedMeshRenderer rend;
    public float DEBUG_hit_reaction_duration;
    public float DEBUG_knockback_duration;
    public float DEBUG_knockback_distance;
    private int hit_reacting;
    private Tween myTween;
    private Vector3 hit_reaction_original_position;
    private Vector3 temp_position;
    private Collider pickup_collider;

    [Header("Crowd Control")]
    public float Lookat_distance;
    public float Lookat_speed;
    public Transform Nav_target;
    private NavMeshAgent agent;
    private Vector3 flee_direction;
    private RaycastHit flee_hit;
    private int flee_layMask;
    public float Flee_speed;
    public enum Crowd_status
    {
        Follow,
        Flee,
        Scared,
        Knockback
    };
    public Crowd_status AI_status;
    private float think_duration;
    public Vector2 Think_duration_range;
    private IEnumerator AI_coroutine;

    [Header("Animation")]
    private Animator pickup_animator;

    [Header("SE")]
    public AudioClip[] Pickup_clips;
    public AudioClip[] Damage_clips;
    public AudioClip[] Deflect_clips;
    private AudioSource SE_source;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
        original_mat = rend.material;
        agent = GetComponent<NavMeshAgent>();
        pickup_animator = this.GetComponentInChildren<Animator>();
        SetNewFleeDirection();
        AI_status = Crowd_status.Flee;
        think_duration = Random.Range(Think_duration_range.x,Think_duration_range.y);
        AI_coroutine = FleeOver(think_duration);
        StartCoroutine(AI_coroutine);
        pickup_collider = this.GetComponent<Collider>();
        flee_layMask = (1 << 9) + (1 << 11)+ (1 << 13);//Check Enemy Layer and Terrian Layer for shooting
        SE_source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(AI_status)
        {
            case Crowd_status.Follow:
                agent.SetDestination(Nav_target.position);
                if(agent.remainingDistance<Lookat_distance)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Nav_target.forward), Lookat_speed*Time.deltaTime);
                }
                //Update Animation
                pickup_animator.SetFloat("Move_speed", agent.velocity.magnitude);
                break;
            case Crowd_status.Flee:
                this.transform.rotation=Quaternion.LookRotation(flee_direction);
                if(Physics.Raycast(this.transform.position+Vector3.up*1f,this.transform.forward,out flee_hit, 2f,flee_layMask))
                {
                    //Debug.Log(flee_hit.transform.name);
                    flee_direction = Vector3.Reflect(this.transform.forward,flee_hit.normal);
                    flee_direction.y=0;
                    flee_direction.Normalize();
                }
                agent.Move(flee_direction*Flee_speed*Time.deltaTime);
                pickup_animator.SetFloat("Move_speed", Flee_speed);
                break;
            case Crowd_status.Scared:

                break;
        }
    }

    public IEnumerator ApplyDamage(Vector3 Incoming)
    {
        if(!invencible)
        {
            //Set invencibility
            invencible = true;
            pickup_collider.enabled=false;
            //Remove Player Powerup
            if(Pickup_manager!=null)
            {
                Pickup_manager.RemovePickup(this);

                Pickup_manager = null;
            }
            //Deal hit reaction
            rend.material = Hit_reaction_mat;
            if(hit_reacting == 0)
            {
                hit_reaction_original_position = this.transform.position;
            }
            hit_reacting += 1;
            SetNewFleeDirection();
            AI_status = Crowd_status.Knockback;
            agent.isStopped=true;
            pickup_animator.SetTrigger("Knockback");
            pickup_animator.SetBool("Pickedup", false);
            SE_source.clip = Damage_clips[Random.Range(0,Damage_clips.Length)];
            SE_source.Play();
            myTween = this.transform.DOMove(this.transform.position + Incoming.normalized * DEBUG_knockback_distance, DEBUG_knockback_duration);
            yield return new WaitForSeconds(DEBUG_hit_reaction_duration);
            rend.material = original_mat;   
            yield return new WaitForSeconds(DEBUG_knockback_duration * 3);
            hit_reacting -= 1;
            AI_status = Crowd_status.Flee;
            ResetY();
            StopCoroutine(AI_coroutine);
            think_duration = Random.Range(Think_duration_range.x,Think_duration_range.y);
            AI_coroutine = FleeOver(think_duration);
            StartCoroutine(AI_coroutine);
            //Remove invencibility
            invencible = false;
            pickup_collider.enabled=true;
        }
    }

    public void PickedByPlayer(PlayerPickup player)
    {                    
        Pickup_manager = player;
        Nav_target = player.transform.parent;
        transform.rotation = Quaternion.LookRotation(Nav_target.forward);
        AI_status = Crowd_status.Follow;
        pickup_animator.SetBool("Pickedup", true);
        pickup_animator.SetTrigger("Picking up");
        SE_source.clip = Pickup_clips[Random.Range(0,Pickup_clips.Length)];
        SE_source.Play();
        agent.isStopped=false;
        StopCoroutine(AI_coroutine);
    }

    public IEnumerator FleeOver(float duration)
    {
        yield return new WaitForSeconds(duration);
        pickup_animator.SetFloat("Move_speed", 0);
        AI_status = Crowd_status.Scared;
        
        StopCoroutine(AI_coroutine);
        think_duration = Random.Range(Think_duration_range.x,Think_duration_range.y);
        AI_coroutine = StartFlee(think_duration);
        StartCoroutine(AI_coroutine);
    }
    
    public IEnumerator StartFlee(float duration)
    {
        yield return new WaitForSeconds(duration);
        AI_status = Crowd_status.Flee;
        StopCoroutine(AI_coroutine);
        think_duration = Random.Range(Think_duration_range.x,Think_duration_range.y);
        AI_coroutine = FleeOver(think_duration);
        StartCoroutine(AI_coroutine);
    }

    private void SetNewFleeDirection()
    {
        flee_direction.x = Random.Range(0,1f);
        flee_direction.z = Random.Range(0,1f);
        flee_direction.Normalize();
    }

    public void DeflectAnimation(Vector3 target)
    {
        this.transform.rotation = Quaternion.LookRotation(target-this.transform.position);
        pickup_animator.SetTrigger("Deflect");
        SE_source.clip = Deflect_clips[Random.Range(0,Deflect_clips.Length)];
        SE_source.Play();
    }
    public void ResetY()
    {
        temp_position = this.transform.position;
        temp_position.y = 0;
        this.transform.position = temp_position;
    }
}
