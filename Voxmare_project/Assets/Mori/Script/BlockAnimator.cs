using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlockAnimator : MonoBehaviour
{
    [Header("Scale Animation")]
    [SerializeField] float s_duration;
    [SerializeField] Vector3 s_scale;
    [SerializeField] Ease s_ease;
    [SerializeField] bool s_syncrosynchronized;

    [Header("Move Animation")]
    [SerializeField] float m_duration;
    [SerializeField] Vector3 m_distance;
    [SerializeField] Ease m_ease;
    [SerializeField] bool m_syncrosynchronized;

    float delay;

    // Start is called before the first frame update
    void Start()
    {
        if (s_syncrosynchronized)
        {
            delay = 0;
        }
        else
        {
            delay = Random.value * s_duration;
        }
        transform.DOScale(s_scale, s_duration).SetEase(s_ease).SetDelay(delay).SetLoops(-1, LoopType.Yoyo);

        if (m_syncrosynchronized)
        {
            delay = 0;
        }
        else
        {
            delay = Random.value * m_duration;
        }
        transform.DOLocalMove(m_distance, m_duration).SetEase(m_ease).SetDelay(delay).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
