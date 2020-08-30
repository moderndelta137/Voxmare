using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rotation : MonoBehaviour
{
    [SerializeField] float duration;
    // Start is called before the first frame update
    void Start()
    {
        transform.DOLocalRotateQuaternion(Quaternion.LookRotation(-transform.forward), duration).SetLoops(-1, LoopType.Incremental);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
