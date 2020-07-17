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
    public Color Enemy_color;
    public Color Player_color;
    private int layerMask;
    private RaycastHit[] hits;
    private VisualEffect vfx; 
    // Start is called before the first frame update
    void Start()
    {
        vfx = this.GetComponent<VisualEffect>();

        Destroy(this.gameObject, Lifespan);
        if(Damage_player)
        {
            vfx.SetVector4("Color",Enemy_color);
            layerMask = 1 << 8;//Check Player Layer
        }
        else
        {
            vfx.SetVector4("Color",Player_color);
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
