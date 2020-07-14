using System.Collections;
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
    }

    // Update is called once per frame
    void Update()
    {
        
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
                    pickup_script.Pickedup = true;
                    pickup_list.Add(pickup_script);
                    deflect_script.PowerUp(pickup_script.Type);
                    pickup_script.transform.SetParent(this.transform.parent);
                }
            }
        }
    }

}
