using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    NavMeshAgent agent;

    [SerializeField]
    private int health;

    [SerializeField]
    private int damage;

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update() {
        if (target != null && agent.isOnNavMesh) {
            agent.SetDestination(target.position);
        }
    }

    public void Kill() {
        Destroy(gameObject);
    }

    public void gainDamage(int amount) {
        health -= amount;
        if (health <= 0) {
            Kill();
        }
    }
}
