using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public int HP;
    public float Move_speed;
    public float Rotate_speed;
    public CharacterController Player_CC;
    private Vector3 move_input_vector;
    private Vector3 look_input_vector;
    // Start is called before the first frame update
    void Start()
    {
        Player_CC=GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        move_input_vector.x = Input.GetAxis("Horizontal");
        move_input_vector.z = Input.GetAxis("Vertical");
        Vector3.ClampMagnitude(move_input_vector,1.0f);
        if(move_input_vector.magnitude>0)
            Player_CC.Move(move_input_vector*Move_speed);
        look_input_vector.x = Input.GetAxis("Look_Horizontal");
        look_input_vector.z = Input.GetAxis("Look_Vertical");
        if(look_input_vector.magnitude>0.2f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look_input_vector), Rotate_speed);
    }
}
