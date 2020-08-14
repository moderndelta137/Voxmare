using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDEyeAnimation : MonoBehaviour
{
    //private SpammerController spammer_script;
    public Camera cam;
    private Transform target;
    public GameObject Eye;
    public float y_scaler;
    public float y_clamp;
    //public Vector3 
    private Vector3 target_vector;
    private float z_rotate;
    //public float Eye_movement_scale;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        target_vector = Input.mousePosition - cam.WorldToScreenPoint(this.transform.position);
        //target_vector = target.position-this.transform.position;
        target_vector.z = 0;
//        Debug.Log(target_vector);
        Eye.transform.rotation = Quaternion.LookRotation(target_vector,Vector3.forward);
        z_rotate = Mathf.Clamp(target_vector.magnitude*y_scaler,-y_clamp,y_clamp);
        Eye.transform.Rotate(Vector3.right*-z_rotate);
    }
}
