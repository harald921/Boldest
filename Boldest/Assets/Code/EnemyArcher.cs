using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyArcher : EnemyBase
{
   
    [SerializeField] float _drawTime = 5;
    [SerializeField] float _shootLength = 10.0f;
    [SerializeField] float _detectionLength = 15.0f;
    [SerializeField] GameObject _fireBolt;
	

	bool _isFiringAtPlayer = false;
  
    protected override void Start()
    {
		base.Start();
		
	}

	protected override void Update()
    {
		base.Update();
        Vector3 dirToPlayer = transform.position - _player.transform.position;

        if (dirToPlayer.magnitude <= _shootLength)
            MagicAIStuff();

        else if (dirToPlayer.magnitude <= _detectionLength)
            _navMeshAgent.destination = _player.gameObject.transform.position;

        

    }


    private void MagicAIStuff()
    {
        if (!_isFiringAtPlayer)
            StartCoroutine(FireAtPlayer());

        transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));
    }
   
    IEnumerator FireAtPlayer()
    {
        _isFiringAtPlayer = true;
        _navMeshAgent.destination = transform.position;
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);


        yield return new WaitForSeconds(_drawTime);

        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

        _navMeshAgent.destination = _player.transform.position;

        GameObject fireBolt = Instantiate(_fireBolt, transform.GetChild(0).position, Quaternion.identity);
        fireBolt.GetComponent<Rigidbody>().AddForce(transform.forward * 1200);

        _isFiringAtPlayer = false;

        yield return null;
    }

    public override void OnDeath()
    {
        print("custom particle");
        Destroy(gameObject);
    }

    public override void OnGettingVisceraled()
    {
        base.OnGettingVisceraled();
    }
}