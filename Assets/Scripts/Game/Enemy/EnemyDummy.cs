using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDummy : MonoBehaviour
{
    /*
    *
    *  IntroScene 등에 사용되는 더미 적에 적용되는 스크립트
    *
    */

    NavMeshAgent agent;

    private float minInterval = 0f;
    private float maxInterval = 12f;

    private float minX = -9.29f;
    private float minY = -7.98f;

    private float maxX = 9.29f;
    private float maxY = 7.98f;

    private Vector2 targetPos;

    //랜덤 위치로 이동 코루틴 구현부. 무한 반복
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
    
    //변수 초기화 및 설정.
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
