using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    NavMeshAgent agent;

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update() {
        if (target != null) {
            agent.SetDestination(target.position);
        }
    }

    public void Kill() {
        Destroy(gameObject);
    }
}
