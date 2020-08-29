using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class UIAppearAnimation : MonoBehaviour
{
    public float Appear_duration;
    public float Appear_delay;
    private Vector3 original_scale;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnEnable() 
    {
        StartCoroutine(AppearAnimation());
    }

    private IEnumerator AppearAnimation()
    {
        original_scale = transform.localScale;
        transform.localScale=Vector3.zero;
        yield return new WaitForSeconds(Appear_delay);;
        transform.DOScale(original_scale,Appear_duration).SetUpdate(true);;
    }
    private void OnDisable() 
    {
        StopAllCoroutines();
    }
}
