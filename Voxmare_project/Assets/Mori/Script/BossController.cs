using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class BossController : MonoBehaviour
{
    GameObject target;
    float randomSeed;
    Vector3 blocksCenter;
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
    [Header("CalculateCenter")]
    [SerializeField] float calculateCenterInterval;

    Vector3 detination;
    Tween tackleTween;
    WaitForSeconds waitTackleInterval;
    WaitForSeconds waitCalculateCenterInterval;

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

        waitTackleInterval = new WaitForSeconds(interval + Random.Range(-intervalDivergence, intervalDivergence));
        waitCalculateCenterInterval = new WaitForSeconds(calculateCenterInterval);

        StartCoroutine("Tackle");
        StartCoroutine("CalculateCenter");
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
            yield return waitTackleInterval;
            state = State.Tackle;
            tackleTween = transform.DOMove(target.transform.position, tackleDuration).SetEase(tackleEase);
            yield return tackleTween.WaitForCompletion();
            state = State.Random;
        }
    }

    private IEnumerator CalculateCenter()
    {
        while(true)
        {
            yield return waitCalculateCenterInterval;
            Vector3 sum = Vector3.zero;
            foreach (Transform block in transform)
            {
                sum += block.position;
            }

            if (transform.childCount > 0)
            {
                blocksCenter = sum / transform.childCount;
            }
            else
            {
                blocksCenter = transform.position;
            }

            Vector3 posToCenter = blocksCenter - transform.position;
            transform.position = blocksCenter;
            foreach (Transform block in transform)
            {
                block.localPosition += -posToCenter;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, radius);
    }
}
