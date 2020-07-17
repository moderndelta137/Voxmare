using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestEnemyController : MonoBehaviour
{
    public int HP;
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
        HP -= (int)Incoming.magnitude;
        rend.material = Hit_reaction_mat;
        hit_reaction_original_position = this.transform.position;
        this.transform.DOMove(this.transform.position + Incoming.normalized * DEBUG_hit_reaction_knockback, DEBUG_hit_reaction_duration);
        yield return new WaitForSeconds(DEBUG_hit_reaction_duration);
        this.transform.DOMove(hit_reaction_original_position, DEBUG_hit_reaction_duration);
        rend.material = original_mat;
        if(HP <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
