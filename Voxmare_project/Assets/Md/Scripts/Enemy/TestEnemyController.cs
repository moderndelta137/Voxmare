using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestEnemyController : MonoBehaviour
{
    public int HP;
    public int Damage;
    public bool Damage_player;
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
        HP -= (int)Incoming.magnitude;
        rend.material = Hit_reaction_mat;
        if(hit_reacting == 0)
        {
            hit_reaction_original_position = this.transform.position;
        }
        hit_reacting += 1;
        myTween = this.transform.DOMove(hit_reaction_original_position + Incoming.normalized * DEBUG_hit_reaction_flinch, DEBUG_hit_reaction_duration);
        yield return myTween.WaitForCompletion();
        myTween = this.transform.DOMove(hit_reaction_original_position, DEBUG_hit_reaction_duration);
        yield return myTween.WaitForCompletion();
        hit_reacting -= 1;
        rend.material = original_mat;
        ResetY();
        if(HP <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    /*
    private void OnCollisionEnter(Collision other) 
    {
                    Debug.Log("Hits");
        if(other.gameObject.CompareTag("Player"))
        {
                    Debug.Log("Hit");
            other.gameObject.SendMessage("Knockback",(other.transform.position-this.transform.position).normalized);
        }
    }
    */
    
    private void OnTriggerEnter(Collider other) 
    {
        if(Damage_player)
        {
            if(other.gameObject.CompareTag("Player"))
            {
                other.gameObject.SendMessage("Knockback",(other.transform.position-this.transform.position).normalized*Damage);
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
