using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRusher : EnemyBase
{
   
        
    protected override void Start()
    {
		base.Start();
		_navMeshAgent.enabled = true;
    }

	protected override void Update()
    {
		base.Update();
        if (_navMeshAgent.enabled)
            _navMeshAgent.destination = GameObject.Find("Player").transform.position;
    }  
     
}