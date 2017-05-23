using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRusher : MonoBehaviour
{
    private void Update()
    {
        GetComponent<NavMeshAgent>().destination = GameObject.Find("Player").transform.position;
    }
}