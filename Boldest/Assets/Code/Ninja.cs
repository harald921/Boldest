﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja : EnemyBase
{
    bool _awake = false;
    bool _walking = true;
    bool _inAttack = false;
    float _playerDistance = 0;
    float _findNewPointTimer = 0.0f;
    [SerializeField] float _awakeDistance = 10.0f;
    [SerializeField] float _attackDistance = 2.0f;
    [SerializeField] float _findNewPointTime = 8.0f;
    [SerializeField] float _walkSpeed = 4.0f;
    [SerializeField] float _runSpeed = 8.0f;

    Vector3 _randomClosePos;
    Vector3 _targetPos;
    Animator _animator;

    public GameObject _katana;
    GameObject _damager;
    Transform _joint;

    
    

    protected override void Start()
    {
        base.Start();
        _navMeshAgent.enabled = true;
        _navMeshAgent.speed = _walkSpeed;
        _animator = GetComponent<Animator>();
        _findNewPointTimer = _findNewPointTime;

        _joint = transform.Find("Skeleton_Group/Root/Spine_1/Spine_2/Spine_3/Spine_4/R_Clavicle/R_Shoulder/R_Elbow/R_Wrist/Sword_Joint").transform;
        _damager = transform.Find("DamageArea").gameObject;
        _damager.SetActive(false);
        _katana.transform.parent = _joint.transform;
        
    }

    protected override void Update()
    {
        base.Update();
        GetPlayerDistance();

        if (_awake)
        {
            PlayerIsNear();
            CheckAttack();
        }
           
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
            _navMeshAgent.speed = _runSpeed;
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
            _navMeshAgent.speed = _walkSpeed;
        }
            

       

    }

    void CheckAttack()
    {
        if (_playerDistance < _attackDistance && !_inAttack)
        {
            _inAttack = true;
            _damager.SetActive(true);
            _navMeshAgent.enabled = false;

        }
        else
        {
            _inAttack = false;
            _damager.SetActive(false);
            _navMeshAgent.enabled = true;
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
        _animator.SetBool("playerInZone", _awake);
        _animator.SetBool("inAttack", _inAttack);

    }
}