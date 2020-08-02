using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class BossController : MonoBehaviour
{
    GameObject target;
    float randomSeed;
    State state;

    [Header("Moveable Area")]
    [SerializeField] public Vector3 center;
    [SerializeField] public float radius;
    [Header("Random Walk")]
    [SerializeField] float speed;
    [SerializeField] public AnimationCurve centripetalPower;
    [Header("Tackle")]
    [SerializeField] float interval;
    [SerializeField] float intervalDivergence;
    [SerializeField] float tackleDuration;
    [SerializeField] Ease tackleEase;

    Vector3 detination;
    Tween tackleTween;
    enum State
    {
        Random,
        Tackle,
        Leave
    }

    void Start()
    {
        target = GameObject.Find("Player");
        randomSeed = Random.value;
        state = State.Random;

        StartCoroutine("Tackle");
    }

    void Update()
    {

        switch (state)
        {
            case State.Random:
                MoveRandom();
                break;
            case State.Tackle:
                break;
            case State.Leave:
                break;
            default:
                break;
        }
    }

    void MoveRandom()
    {
        float rand = Mathf.PerlinNoise(Time.time * 0.1f, randomSeed * 256);
        float angle = rand * 2 * Mathf.PI;
        Vector3 velocity = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));    // [0, 1]
        Vector3 toCenter = center - transform.position;
        Vector3 centripetalVector = toCenter.normalized * centripetalPower.Evaluate(toCenter.magnitude/radius); // [0, 1]
        transform.position += (velocity + centripetalVector) * speed * Time.deltaTime;
    }

    private IEnumerator Tackle()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval + Random.Range(-intervalDivergence, intervalDivergence));
            state = State.Tackle;
            tackleTween = transform.DOMove(target.transform.position, tackleDuration).SetEase(tackleEase);
            yield return tackleTween.WaitForCompletion();
            state = State.Random;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, radius);
    }
}
