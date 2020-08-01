using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    GameObject target;
    NavMeshAgent navmesh;
    float randomSeed;
    State state;

    [SerializeField] float speed;
    [SerializeField] AnimationCurve centripetalPower;
    [Header("Moveable Area")]
    [SerializeField] Vector3 center;
    [SerializeField] float radius;

    //[Header("Random Walk")]
    //[SerializeField] float minDistance;
    //[SerializeField] float maxDistance;

    Vector3 detination;
    enum State
    {
        Random,
        Approach,
        Leave
    }

    void Start()
    {
        navmesh = GetComponent<NavMeshAgent>();
        target = GameObject.Find("Player");
        randomSeed = Random.value;
        state = State.Random;
    }

    void Update()
    {
        switch (state)
        {
            case State.Random:
                MoveRandom();
                break;
            case State.Approach:
                if (target != null)
                {
                    navmesh.destination = target.transform.position;
                }
                break;
            case State.Leave:
                break;
            default:
                break;
        }
    }

    void MoveRandom()
    {
        navmesh.isStopped = true;

        float rand = Mathf.PerlinNoise(Time.time * 0.1f, randomSeed * 256);
        float angle = rand * 2 * Mathf.PI;
        Vector3 velocity = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));    // [0, 1]
        Vector3 toCenter = center - transform.position;
        Vector3 centripetalVector = toCenter.normalized * centripetalPower.Evaluate(toCenter.magnitude/radius); // [0, 1]
        transform.position += (velocity + centripetalVector) * speed * Time.deltaTime;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, radius);
    }
}
