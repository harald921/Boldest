using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRusher : MonoBehaviour
{
    bool _isBeingKnockBacked = false;

    private void Update()
    {
        GetComponent<NavMeshAgent>().destination = GameObject.Find("Player").transform.position;
    }

    void KnockBack(Vector3 inVelocity)
    {

    }
}