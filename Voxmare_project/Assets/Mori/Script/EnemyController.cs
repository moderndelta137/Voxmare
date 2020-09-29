using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyController : MonoBehaviour
{
    public int HP;
    public int Damage;
    public float Push_force;
    //private bool dead;
    //public bool Damage_player;
    
    private BlockAnimator blockAnimator;
    private Vector3 temp_position;

    [Header("UI")]
    public GameObject Health_bar_prefab;
    //public GameObject health_bar;
    private Health_Bar health_bar_script;
    public Vector3 Health_bar_offset;

    private Block block;

    // Start is called before the first frame update
    void Start()
    {
        blockAnimator = GetComponentInChildren<BlockAnimator>();
        health_bar_script = Instantiate(Health_bar_prefab, Vector3.zero, Quaternion.identity,this.transform).GetComponent<Health_Bar>();
        health_bar_script.transform.localPosition=Health_bar_offset;
        health_bar_script.SetMaxHealth(HP);
        block = GetComponent<Block>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ApplyDamage(Vector3 Incoming)
    {
        if(HP>0)
        {
            HP -= (int)Incoming.magnitude;
            health_bar_script.gameObject.SetActive(true);
            health_bar_script.SetHealth(HP);
            blockAnimator.DamageAnimation(Incoming);
            ResetY();
            if (HP <= 0)
            {
                Death();
            }
        }
    }

    void Death()
    {
        block.Death();
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (Damage_player)
        switch(other.tag)
        {
            case "Player":
                other.gameObject.SendMessage("Knockback", (other.transform.position - this.transform.position).normalized * Damage);
            break;
            case "Terrian":
                other.attachedRigidbody.AddForce(other.transform.position-this.transform.position*Push_force, ForceMode.VelocityChange);
            break;
        }
        /*
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.SendMessage("Knockback", (other.transform.position - this.transform.position).normalized * Damage);
            }
        }
        */
    }

    public void ResetY()
    {
        temp_position = this.transform.position;
        temp_position.y = 0;
        this.transform.position = temp_position;
    }
}
