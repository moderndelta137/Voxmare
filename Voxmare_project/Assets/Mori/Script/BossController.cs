﻿using System.Collections;
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
    [SerializeField] public float lengthX;
    [SerializeField] public float lengthZ;
    [SerializeField] public float wallStrength;
    [SerializeField] float speed;
    [Header("Tackle")]
    [SerializeField] float tackleInterval;
    [SerializeField] float tackleIntervalDivergence;
    [SerializeField] float tackleDuration;
    [SerializeField] Ease tackleEase;
    [Header("Spin")]
    [SerializeField] float radius;
    [SerializeField] float spinInterval;
    [SerializeField] float spinIntervalDivergence;
    [SerializeField] float spinDuration;
    [SerializeField] int spinCount;
    [SerializeField] Ease spinEase;
    [Header("CalculateCenter")]
    [SerializeField] float calculateCenterInterval;

    Vector3 detination;
    List<Vector3> paths;
    Tween tackleTween;
    Sequence spinSequence;
    WaitForSeconds waitTackleInterval;
    WaitForSeconds waitSpinInterval;
    WaitForSeconds waitCalculateCenterInterval;

    enum State
    {
        Random,
        Tackle,
        Spin
    }

    void Start()
    {
        target = GameObject.Find("Player");
        randomSeed = Random.value;
        state = State.Random;

        paths = new List<Vector3>();
        waitTackleInterval = new WaitForSeconds(tackleInterval + Random.Range(-tackleIntervalDivergence, tackleIntervalDivergence));
        waitSpinInterval = new WaitForSeconds(spinInterval + Random.Range(-spinIntervalDivergence, spinIntervalDivergence));
        waitCalculateCenterInterval = new WaitForSeconds(calculateCenterInterval);
        spinSequence = DOTween.Sequence();
        StartCoroutine("Tackle");
        StartCoroutine("Spin");
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
            case State.Spin:
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
        velocity += GetRepulsiveVector();
        transform.position += velocity * speed * Time.deltaTime;
    }

    Vector3 GetRepulsiveVector()
    {
        Vector3 velocity = Vector3.zero;
        Vector3 pos = transform.position;
        float posX = pos.x - center.x;
        float posZ = pos.z - center.z;
        float halfLengthX = lengthX / 2;
        float halfLengthZ = lengthZ / 2;

        if(posX > 0) velocity.x -= wallStrength / ((halfLengthX - posX) / halfLengthX); 
        if(posX < 0) velocity.x -= wallStrength / ((-halfLengthX - posX) / halfLengthX);
        if(posZ > 0) velocity.z -= wallStrength / ((halfLengthZ - posZ) / halfLengthZ);
        if(posZ < 0) velocity.z -= wallStrength / ((-halfLengthZ - posZ) / halfLengthZ);

        return velocity;
    }

    private IEnumerator Tackle()
    {
        while (true)
        {
            yield return waitTackleInterval;
            if (state != State.Random) continue;

            state = State.Tackle;
            tackleTween = transform.DOMove(target.transform.position, tackleDuration).SetEase(tackleEase);
            yield return tackleTween.WaitForCompletion();
            state = State.Random;
        }
    }

    private IEnumerator Spin()
    {
        int divideCount = 8;
        int rand = Random.Range(0, divideCount);
        for (int i = 0; i < divideCount; i++)
        {
            float angle = ((i + rand) % divideCount) * (2 * Mathf.PI / divideCount);
            paths.Add(new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle)));
        }

        while (true)
        {
            yield return waitSpinInterval;
            if (state != State.Random) continue;
            state = State.Spin;
            spinSequence.Join(transform.DOLocalPath(paths.ToArray(), spinDuration, PathType.CatmullRom).SetEase(spinEase));
            spinSequence.Join(transform.DOBlendableLocalRotateBy(new Vector3(0, 360*spinCount, 0), spinDuration, RotateMode.FastBeyond360).SetEase(spinEase));
            yield return spinSequence.WaitForCompletion();
            state = State.Random;
        }
    }

    private IEnumerator CalculateCenter()
    {
        while(true)
        {
            yield return waitCalculateCenterInterval;
            if (state != State.Random) continue;

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
                block.localPosition += transform.rotation * -posToCenter;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, new Vector3(lengthX, 0.0f, lengthZ));
    }
}
