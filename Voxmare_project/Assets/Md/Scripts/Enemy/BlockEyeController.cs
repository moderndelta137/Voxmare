using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockEyeController : MonoBehaviour
{
    private SpammerController spammer_script;
    private Transform target;
    public GameObject Eye;
    public Vector3 Eye_vector_up;
    public Vector3 Eye_vector_right;
    public float Eye_movement_range;
    private Vector3 target_vector;
    public float Eye_movement_scale;
    // Start is called before the first frame update
    void Start()
    {
        spammer_script = GetComponent<SpammerController>();
        if(spammer_script.Lockon)
        {
            target = spammer_script.Target.transform;
        }
        else{
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        target_vector = target.position-this.transform.position;
        target_vector.y = 0;
        target_vector = Vector3.ClampMagnitude(target_vector*Eye_movement_scale,Eye_movement_range);
        Eye.transform.localPosition = target_vector;
    }
}
