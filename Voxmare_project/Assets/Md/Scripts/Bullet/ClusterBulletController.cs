using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ClusterBulletController : MonoBehaviour
{
    public int Damage;
    public float Lifespan;
    public float Radius;
    public bool Damage_player;
    public ParticleSystem.MinMaxGradient Enemy_color;
    public ParticleSystem.MinMaxGradient Player_color;
    private int layerMask;
    private RaycastHit[] hits;
    private ParticleSystem particle;
    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponentInChildren<ParticleSystem>();
        var colorOverLifetime = particle.colorOverLifetime;
        Destroy(this.gameObject, Lifespan);
        if(Damage_player)
        {
            colorOverLifetime.color = Enemy_color;
            layerMask = 1 << 8;//Check Player Layer
        }
        else
        {
            colorOverLifetime.color = Player_color;
            layerMask = 1 << 9;//Check Enemy Layer
        }
        hits=Physics.SphereCastAll(this.transform.position, Radius, Vector3.up, 0, layerMask);
        foreach(RaycastHit target in hits)
        {
            target.transform.gameObject.SendMessage("ApplyDamage", (this.transform.position-target.transform.position).normalized * Damage);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
