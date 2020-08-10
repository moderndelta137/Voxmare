using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleEyeAnimation : MonoBehaviour
{
    public float movement_clamp;
    public float movement_scale;
    private Vector3 origin;
    private Vector3 mouse_position;
    public Camera UI_camera;
    private Vector3 movement_direction;
    // Start is called before the first frame update
    void Start()
    {
        origin = this .transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        mouse_position.x = Input.mousePosition.x;
        mouse_position.y = Input.mousePosition.y;
        mouse_position.z = 0;
//        Debug.Log(UI_camera.WorldToScreenPoint(this.transform.position));
        movement_direction = UI_camera.WorldToScreenPoint(this.transform.position);
        movement_direction.z = 0;
        movement_direction = mouse_position - movement_direction;
        movement_direction = movement_direction * movement_scale;
        Vector3.ClampMagnitude(movement_direction,movement_clamp);

        this.transform.localPosition = origin + movement_direction;
    }
}
