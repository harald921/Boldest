using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyArcher : EnemyBase
{
   
    [SerializeField] float _drawTime = 5;
    [SerializeField] float _shootLength = 10.0f;  
    [SerializeField] GameObject _fireBolt;
	

	bool _isFiringAtPlayer = false;
  
    protected override void Start()
    {
		base.Start();
		_navMeshAgent.enabled = false;
		
	}

	protected override void Update()
    {
		base.Update();
        Vector3 dirToPlayer = transform.position - _player.transform.position;

        if (_navMeshAgent.enabled)
            _navMeshAgent.destination = _player.gameObject.transform.position;

        if (dirToPlayer.magnitude <= _shootLength)
            MagicAIStuff();
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
        _navMeshAgent.enabled = false;

        yield return new WaitForSeconds(_drawTime);

        _navMeshAgent.enabled = true;
        _isFiringAtPlayer = false;

        GameObject fireBolt = Instantiate(_fireBolt, transform.GetChild(0).position, Quaternion.identity);
        fireBolt.GetComponent<Rigidbody>().AddForce(transform.forward * 1200);

        yield return null;
    }

    public override void OnDeath()
    {
        print("custom particle");
        Destroy(gameObject);
    }
}