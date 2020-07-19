using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    [Header("General Control")]
    public bool Mouse_control;
    public CharacterController Player_CC;
    public Camera Main_camera;
    public float Move_speed;
    public float Rotate_speed;
    private Vector3 move_input_vector;
    private Vector3 look_input_vector;
    [Header("Mouse Control")]
    public GameObject Mouse_cursor;
    public GameObject Mouse_cursor_prefab;
    private Plane mouse_plane;//plane for checking mouse world position;
    private Ray mouse_ray;
    private float mouse_plane_distance;
    private Vector3 mouse_world_position;
    [Header("Player Parameter")]
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
        Player_CC=GetComponent<CharacterController>();
        Main_camera=Camera.main;
        if(Mouse_control)
            InitiateMouseControl();
        rend = GetComponentInChildren<MeshRenderer>();
        original_mat = rend.material;
    }

    // Update is called once per frame
    void Update()
    {
        //Movement
        move_input_vector.x = Input.GetAxis("Horizontal");
        move_input_vector.z = Input.GetAxis("Vertical");
        Vector3.ClampMagnitude(move_input_vector,1.0f);
        if(move_input_vector.magnitude>0)
            Player_CC.Move(move_input_vector*Move_speed*Time.deltaTime);

        //Rotation
        if(Mouse_control)
        {
            //Create a ray from the Mouse position
            mouse_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (mouse_plane.Raycast(mouse_ray, out mouse_plane_distance))
            {
                mouse_world_position = mouse_ray.GetPoint(mouse_plane_distance);
                Mouse_cursor.transform.position = mouse_world_position;
                look_input_vector = mouse_world_position - this.transform.position;
            }

        }
        else
        {
        look_input_vector.x = Input.GetAxis("Look_Horizontal");
        look_input_vector.z = Input.GetAxis("Look_Vertical");
        }
        if(look_input_vector.magnitude>0.2f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look_input_vector), Rotate_speed*Time.deltaTime);
    }

    public void InitiateMouseControl()
    {
        Mouse_cursor=Instantiate(Mouse_cursor_prefab,Vector3.zero,Quaternion.identity);
        mouse_plane = new Plane(this.transform.up, this.transform.position);
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
        if(HP == 0)
        {
            Destroy(this.gameObject);
        }
    }
}
