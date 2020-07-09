using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gizmo : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] float lineLength = 1.0f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(this.transform.position, radius);
        Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.rotation * new Vector3(0, 0, lineLength));
    }
}
