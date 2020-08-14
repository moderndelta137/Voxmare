using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreEyeController : MonoBehaviour
{
    private SpammerController spammer_script;
    private Transform target;
    public GameObject Eye;
    public float y_scaler;
    public float y_clamp;
    private Vector3 target_vector;
    private float x_rotate;
    //public float Eye_movement_scale;
    // Start is called before the first frame update
    void Start()
    {
        spammer_script = GetComponent<SpammerController>();
        if(spammer_script.Lockon)
        {
            target = GameObject.Find("Player").transform;
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
       
        Eye.transform.rotation = Quaternion.LookRotation(target_vector,-Vector3.up);
        x_rotate = Mathf.Clamp(target_vector.magnitude*y_scaler,-y_clamp,y_clamp);
        Eye.transform.Rotate(Vector3.right*-x_rotate);
    }
}
