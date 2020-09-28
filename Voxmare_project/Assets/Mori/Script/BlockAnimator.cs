using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlockAnimator : MonoBehaviour
{
    [Header("Hit Reaction")]
    public Material Hit_reaction_mat;
    private Material original_mat;
    [SerializeField] float hit_reaction_duration = 0.05f;
    [SerializeField] float hit_reaction_flinch = 1;
    private int hit_reacting;
    private MeshRenderer rend;
    private Tween myTween;
    private Vector3 hit_reaction_original_position;

    [Header("Scale Animation")]
    [SerializeField] bool s_animation;
    [SerializeField] float s_duration;
    [SerializeField] Vector3 s_scale;
    [SerializeField] Ease s_ease;
    [SerializeField] bool s_syncrosynchronized;

    [Header("Move Animation")]
    [SerializeField] bool m_animation;
    [SerializeField] float m_duration;
    [SerializeField] Vector3 m_distance;
    [SerializeField] Ease m_ease;
    [SerializeField] bool m_syncrosynchronized;

    float delay;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponentInChildren<MeshRenderer>();
        original_mat = rend.sharedMaterial;

        if (s_animation)
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
        }
        
        if(m_animation)
        {
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
    }

    public void DamageAnimation(Vector3 Incoming)
    {
        StartCoroutine(DamageAnimationCoroutine(Incoming));
    }

    private IEnumerator DamageAnimationCoroutine(Vector3 Incoming)
    {
        rend.material = Hit_reaction_mat;
        if (hit_reacting == 0)
        {
            hit_reaction_original_position = this.transform.localPosition;
        }
        hit_reacting += 1;
        myTween = this.transform.DOLocalMove(hit_reaction_original_position + Incoming.normalized * hit_reaction_flinch, hit_reaction_duration);
        yield return myTween.WaitForCompletion();
        myTween = this.transform.DOLocalMove(hit_reaction_original_position, hit_reaction_duration);
        yield return myTween.WaitForCompletion();
        hit_reacting -= 1;
        rend.material = original_mat;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
