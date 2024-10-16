using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDummy : MonoBehaviour
{
    NavMeshAgent agent;

    private float minInterval = 0f;
    private float maxInterval = 12f;

    private float minX = -9.29f;
    private float minY = -7.98f;

    private float maxX = 9.29f;
    private float maxY = 7.98f;

    private Vector2 targetPos;


    private IEnumerator moveToRandomPos()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;

                yield return new WaitForSeconds(1f);

                //yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

                agent.isStopped = false;

                targetPos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

                agent.SetDestination(targetPos);
            }
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        StartCoroutine(moveToRandomPos());
        targetPos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        agent.SetDestination(targetPos);
    }
} 
