using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Eyelid_Controller : MonoBehaviour
{
    public GameObject Up_lid;
    public GameObject Down_lid;
    public Vector2 Move_range;
    public float Blink_duration;
    public Sequence mySequence;
    // Start is called before the first frame update
    void Start()
    {
        mySequence = DOTween.Sequence();
        Up_lid.transform.localPosition = Vector3.up*Move_range.y;
        Down_lid.transform.localPosition = Vector3.up*-Move_range.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EyeClose()
    {
        Up_lid.transform.localPosition = Vector3.up*Move_range.x;
        Down_lid.transform.localPosition = Vector3.up*-Move_range.x;
        Up_lid.transform.DOLocalMoveY(Move_range.y,Blink_duration).SetEase(Ease.InOutSine).SetUpdate(true);
        Down_lid.transform.DOLocalMoveY(-Move_range.y,Blink_duration).SetEase(Ease.InOutSine).SetUpdate(true);
    }
    public void EyeOpen()
    {
        Up_lid.transform.localPosition = Vector3.up*Move_range.y;
        Down_lid.transform.localPosition = Vector3.up*-Move_range.y;
        Up_lid.transform.DOLocalMoveY(Move_range.x,Blink_duration).SetEase(Ease.InOutSine).SetUpdate(true);
        Down_lid.transform.DOLocalMoveY(-Move_range.x,Blink_duration).SetEase(Ease.InOutSine).SetUpdate(true);
    }
}
