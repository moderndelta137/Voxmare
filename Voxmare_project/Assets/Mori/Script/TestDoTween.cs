using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestDoTween : MonoBehaviour
{
    [SerializeField] float distance = 0.01f;
    [SerializeField] float duration = 0.5f;
    [SerializeField] Ease ease = Ease.Linear;

    Ease currentEase;

    Tween tween;
    Sequence seq;
    Vector3 toPosition;
    // Start is called before the first frame update
    void Start()
    {

        toPosition = new Vector3(0, 0, 1) * distance;
        tween = transform.DOLocalMoveZ(distance, duration).SetEase(ease).SetLoops(-1, LoopType.Yoyo);

    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}
