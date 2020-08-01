using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    public GameObject target;
    NavMeshAgent navmesh;

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
    }

    void Update()
    {
        if (target != null)
        {
            navmesh.destination = target.transform.position;
        }
    }
}
