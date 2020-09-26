using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
//using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    [Header("General Control")]
    public bool Mouse_control;
    public float Joystick_cursor_offset;
    public CharacterController Player_CharacterController;
    public Camera Main_camera;
    public float Move_speed;
    public float Rotate_speed;
    public bool can_move;
    private Vector3 move_input_vector;
    private Vector3 look_input_vector;

    [Header("Mouse Control")]
    public GameObject Mouse_cursor;
    public GameObject Mouse_cursor_prefab;
    private Plane mouse_plane;//plane for checking mouse world position;
    private Ray mouse_ray;
    private float mouse_plane_distance;
    private Vector3 mouse_world_position;
    private Vector3 mouse_direction_vector;

    [Header("Player Parameter")]
    public int MaxHP;
    public int HP;
    public float PushForce;

    [Header("Hit Reaction")]
    public float Invencible_duration;
    private bool invencible;
    private WaitForSeconds invencible_wait;
    public Material Hit_reaction_mat;
    private Material original_mat;
    public SkinnedMeshRenderer rend;
    public float DEBUG_hit_reaction_duration;
    public float DEBUG_hit_reaction_flinch;
    private int hit_reacting;
    private Tween myTween;
    private Vector3 hit_reaction_original_position;
    public float Knockback_cast_distance;
    public float DEBUG_knockback_duration;
    private float knockback_distance;
    private RaycastHit knockback_hit;
    private int knockback_layerMask;
    private Vector3 damage_incoming;

    public CinemachineImpulseSource Hit_impulse;
    public CinemachineImpulseSource Knockback_impulse;

    [Header("Animation")]
    private Animator player_animator;

    [Header("UI")]
    public GameObject Health_bar_prefab;
    private Health_Bar health_bar_script;
    public Vector3 Health_bar_offset;


    //private Rigidbody rb;

    [Header("SE")]
    public AudioClip[] Knockback_clips;
    public AudioClip Hit_clip;
    public AudioClip Gameover_clip;
    private AudioSource SE_source;

    public UnityEvent Gameover;
    // Start is called before the first frame update
    void Start()
    {
        Player_CharacterController = GetComponent<CharacterController>();
        Main_camera=Camera.main;
        UpdateControlType();
        InitiateMouseControl();
        
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
        original_mat = rend.material;
        knockback_layerMask = (1 << 9) + (1 << 11)+ (1 << 13);//Check Enemy Layer and Terrian Layer for shooting
        player_animator = GetComponentInChildren<Animator>();
        invencible_wait = new WaitForSeconds(Invencible_duration);
        health_bar_script = Instantiate(Health_bar_prefab,Vector3.zero,Quaternion.identity,this.transform).GetComponent<Health_Bar>();
        health_bar_script.transform.localPosition = Health_bar_offset;
        health_bar_script.SetMaxHealth(MaxHP);
        
        SE_source = GetComponent<AudioSource>();
        BecomeInvencible();
        can_move = false;

    }
    public void UpdateControlType()
    {
        if(PlayerPrefs.GetInt("ControlType")==0)
        {
            Mouse_control = true;
        }
        else
        {
            Mouse_control = false;
        }
    }
    public void InitiateMouseControl()
    {
        Mouse_cursor=Instantiate(Mouse_cursor_prefab,Vector3.zero,Quaternion.identity);
        mouse_plane = new Plane(this.transform.up, this.transform.position);
        Mouse_cursor.SetActive(false);
    }

    public void LevelStart()
    {
        HP = MaxHP;
        health_bar_script.gameObject.SetActive(true);
        health_bar_script.SetHealth(HP);
        can_move = true;
        
        Mouse_cursor.SetActive(true);
        StartCoroutine(BecomeInvencible());
    }

    public void LevelClear()
    {
        can_move = false;
        move_input_vector = Vector3.zero;
        look_input_vector = Vector3.zero;
        player_animator.SetFloat("Move_Y", 0);
        player_animator.SetFloat("Move_X", 0);
        player_animator.SetFloat("Move_input",0);
        player_animator.SetFloat("Look_input",0);
        
        Mouse_cursor.SetActive(false);
        invencible = true;
    }



    // Update is called once per frame
    void Update()
    {
        if(!LevelData.isPaused && can_move)
        {
            //Movement
            move_input_vector.x = Input.GetAxis("Horizontal");
            move_input_vector.z = Input.GetAxis("Vertical");
            Vector3.ClampMagnitude(move_input_vector,1.0f);
            //if(can_move)
            {
                if(move_input_vector.magnitude>0)
                {
                    Player_CharacterController.Move(move_input_vector*Move_speed*Time.deltaTime);
                    player_animator.SetFloat("Move_Y", Vector3.Dot(move_input_vector,this.transform.forward));
                    player_animator.SetFloat("Move_X", Vector3.Dot(move_input_vector,this.transform.right));
                    ResetY();
                }
            }
            //Rotation
            if(Mouse_control)
            {
                //Create a ray from the Mouse position
                mouse_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (mouse_plane.Raycast(mouse_ray, out mouse_plane_distance))
                {
                    mouse_world_position = mouse_ray.GetPoint(mouse_plane_distance);
                    Mouse_cursor.transform.position = mouse_world_position;
                    mouse_direction_vector = mouse_world_position - this.transform.position;
                    look_input_vector = mouse_direction_vector.normalized-this.transform.forward;
                    if(look_input_vector.magnitude>0.1f)
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(mouse_direction_vector), Rotate_speed*Time.deltaTime);
                    ResetRotation();
                }

            }
            else
            {
                look_input_vector.x = Input.GetAxis("Look_Horizontal");
                look_input_vector.z = Input.GetAxis("Look_Vertical");
                if(look_input_vector.magnitude>0.1f)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look_input_vector), Rotate_speed*Time.deltaTime);
                Mouse_cursor.transform.position = this.transform.position+this.transform.forward*Joystick_cursor_offset;
            }

            //Update Movement animation
            player_animator.SetFloat("Move_input",move_input_vector.magnitude);
            player_animator.SetFloat("Look_input",look_input_vector.magnitude);
        }

        if(invencible)
        {
            rend.enabled = !rend.enabled;
        }
    }



    public IEnumerator ApplyDamage(Vector3 Incoming)
    {
        if(!invencible)
        {
            StartCoroutine(BecomeInvencible());
            can_move = false;
            LoseHP((int)Incoming.magnitude);
            rend.material = Hit_reaction_mat;
            damage_incoming = Incoming.normalized;
            damage_incoming.y = 0;
            if(hit_reacting == 0)
            {
                hit_reaction_original_position = this.transform.position;
            }
            hit_reacting += 1;
            Hit_impulse.GenerateImpulse();
            //Update animation
            player_animator.SetTrigger("Hit");
            SE_source.clip =  Hit_clip;
            SE_source.Play();
            myTween = this.transform.DOMove(this.transform.position + damage_incoming * DEBUG_hit_reaction_flinch, DEBUG_hit_reaction_duration);
            yield return myTween.WaitForCompletion();
            myTween = this.transform.DOMove(hit_reaction_original_position, DEBUG_hit_reaction_duration);
            yield return myTween.WaitForCompletion();
            hit_reacting -= 1;
            rend.material = original_mat;


            if(HP <= 0)
            {
                GameOver();
            }
            can_move = true;
            ResetY();
        }
    }

    public IEnumerator Knockback(Vector3 Incoming)
    {
        if(!invencible)
        {
            StartCoroutine(BecomeInvencible());
            can_move = false;
            LoseHP((int)Incoming.magnitude);
            rend.material = Hit_reaction_mat;
            damage_incoming = Incoming.normalized;
            damage_incoming.y = 0;
            Knockback_impulse.GenerateImpulse();
            player_animator.SetTrigger("Hit");
            SE_source.clip = Knockback_clips[Random.Range(0,Knockback_clips.Length)];
            SE_source.Play();
            if(Physics.CapsuleCast(this.transform.position,this.transform.position,0.5f,damage_incoming, out knockback_hit, Knockback_cast_distance,knockback_layerMask))
            {
                myTween = this.transform.DOMove(this.transform.position+damage_incoming*knockback_hit.distance,DEBUG_knockback_duration);
            }
            else
            {
                myTween = this.transform.DOMove(this.transform.position+damage_incoming*Knockback_cast_distance,DEBUG_knockback_duration);
            }
            yield return myTween.WaitForCompletion();
            rend.material = original_mat;
            
            if(HP <= 0)
            {
                GameOver();
            }
            can_move = true;
            ResetY();
        }
    }

    private IEnumerator BecomeInvencible()
    {
        invencible=true;
        yield return invencible_wait;
        invencible=false;
        rend.enabled = true;
        ResetY();
    }
    private void ResetY()
    {
        Vector3 temp_position;
        temp_position = this.transform.position;
        temp_position.y = 0;
        this.transform.position = temp_position;
    }
    private void ResetRotation()
    {
        Vector3 temp_eular;
        temp_eular = this.transform.rotation.eulerAngles;
        this.transform.rotation=Quaternion.Euler(0,temp_eular.y,0);
    }

    private void LoseHP(int delta)
    {
        HP -= delta;
        health_bar_script.SetHealth(HP);
    }

    public void GameOver()
    {
        can_move = false;
        invencible=false;
        rend.enabled = true;
        StopAllCoroutines();
        //Destroy(this.gameObject);
        player_animator.SetTrigger("Death");
        player_animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        SE_source.clip = Gameover_clip;
        SE_source.Play();
        //SceneManager.LoadScene(1);
        Gameover.Invoke();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if(hit.gameObject.tag == "Terrian")
        {
            hit.rigidbody.velocity = (hit.transform.position-this.transform.position).normalized*PushForce;
        }
    }
}
