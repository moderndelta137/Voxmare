using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRotationInitializer : MonoBehaviour
{
    void Start()
    {
        switch (Random.Range(0, 6))
        {
            case 0:
                transform.rotation = Quaternion.LookRotation(transform.forward, transform.up);
                break;
            case 1:
                transform.rotation = Quaternion.LookRotation(transform.forward, transform.right);
                break;
            case 2:
                transform.rotation = Quaternion.LookRotation(transform.forward, -transform.up);
                break;
            case 3:
                transform.rotation = Quaternion.LookRotation(transform.forward, -transform.right);
                break;
            case 4:
                transform.rotation = Quaternion.LookRotation(transform.up, -transform.forward);
                break;
            case 5:
                transform.rotation = Quaternion.LookRotation(-transform.up, transform.forward);
                break;
            default:
                break;
        }
    }

    void Update()
    {
        
    }
}
