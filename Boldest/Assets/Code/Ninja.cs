using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja : EnemyBase
{
    bool _awake = false;
    bool _walking = true;
    float _playerDistance = 0;
    [SerializeField] float _awakeDistance = 10.0f;
    float _findNewPointTimer = 0.0f;
    [SerializeField] float _findNewPointTime = 8.0f;
    Vector3 _randomClosePos;
    Vector3 _targetPos;
    Animator _animator;

    

    protected override void Start()
    {
        base.Start();
        _navMeshAgent.enabled = true;
        _animator = GetComponent<Animator>();
        _findNewPointTimer = _findNewPointTime;
              
       
        
    }

    protected override void Update()
    {
        base.Update();
        GetPlayerDistance();

        if (_awake)
            PlayerIsNear();

        SetAnimations();
        
    }


    void PlayerIsNear()
    {
        if (_navMeshAgent.enabled)
            _navMeshAgent.destination = GameObject.Find("Player").transform.position;

    }


    void GetPlayerDistance()
    {
        _findNewPointTimer += Time.deltaTime;

        Vector3 vec = transform.position - _player.transform.position;
        _playerDistance = vec.magnitude;

        if (_playerDistance < _awakeDistance)
        {
            _awake = true;
        }
        else
        {
            if(_findNewPointTimer > _findNewPointTime)
            {
                _randomClosePos = new Vector3(Random.Range(-10, 10), transform.position.y, Random.Range(-10, 10));
                _navMeshAgent.destination = transform.position + _randomClosePos;
                _targetPos = _navMeshAgent.destination;
                _findNewPointTimer = 0;
            }
            
            _awake = false;
        }
            

       

    }


    void SetAnimations()
    {
        
        if (transform.position == _targetPos )
        {
            _walking = false;
        }
        else
            _walking = true;


        _animator.SetBool("isWalking", _walking);

    }
}