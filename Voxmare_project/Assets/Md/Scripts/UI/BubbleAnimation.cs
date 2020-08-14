using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BubbleAnimation : MonoBehaviour
{
    public float move_strength;
    private float move_duration;
    public Vector2 move_duration_range;
    private Vector3 move_direction;
    private Tween myTween;
    private Vector3 origin;
    private IEnumerator myCoroutine;
    //public RectTransform
    // Start is called before the first frame update
    private void Awake() 
    {
        origin = this.transform.localPosition;

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator StartMoving()
    {
        
        //this.transform.DOShakePosition(move_duration,move_strength,vibrato).SetLoops(-1);
        move_direction.x = Random.Range(-1f,1f);
        move_direction.y = Random.Range(-1f,1f);
        move_direction.Normalize();
        move_duration = Random.Range(move_duration_range.x,move_duration_range.y);
        myTween = this.transform.DOLocalMove(origin+move_direction*move_strength,move_duration).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine);
        yield return myTween.WaitForCompletion();
         StartCoroutine(StartMoving());
    }

    private void OnDisable() 
    {
        StopAllCoroutines();
        DOTween.KillAll();
        this.transform.localPosition = origin;     
    }

    private void OnEnable() 
    {
        //origin = this.transform.position;
        myCoroutine = StartMoving();
        StartCoroutine(myCoroutine);
    }
}
