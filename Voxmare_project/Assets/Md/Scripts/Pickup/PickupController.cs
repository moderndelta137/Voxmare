using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    public float DEBUG_hit_reaction_knockback;
    private Vector3 hit_reaction_original_position;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        original_mat = rend.material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ApplyDamage(Vector3 Incoming)
    {
        rend.material = Hit_reaction_mat;
        hit_reaction_original_position = this.transform.position;
        this.transform.DOMove(this.transform.position + Incoming.normalized * DEBUG_hit_reaction_knockback, DEBUG_hit_reaction_duration);
        yield return new WaitForSeconds(DEBUG_hit_reaction_duration);
        this.transform.DOMove(hit_reaction_original_position, DEBUG_hit_reaction_duration);
        rend.material = original_mat;
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
}
