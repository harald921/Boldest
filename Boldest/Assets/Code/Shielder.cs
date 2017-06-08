using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Shielder : MonoBehaviour
{
    Player _player;
    Animator _shieldAnim;
    Animator _swordAnim;
    float _playerDistance;
    public float _awakeDistance;
    public float _attackDistance;
    public bool _isAttacking = false;
       
    float _stateChange = 0;
    NavMeshAgent _agent;
    Rigidbody _rigidBody;
    
    

    void Start()
    {
        _player = FindObjectOfType<Player>();
        _rigidBody = GetComponent<Rigidbody>();
        _shieldAnim = transform.GetChild(0).GetComponent<Animator>();
        _swordAnim = transform.GetChild(1).GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();      
        _agent.enabled = false;
    }


    void Update()
    {
        GetPlayerDistance();
        Vector3 look = new Vector3( _player.transform.position.x, transform.position.y, _player.transform.position.z) - transform.position;
        look.Normalize();
        transform.forward = look;

        _stateChange += Time.deltaTime;
       
        if(_playerDistance < _awakeDistance && !_isAttacking)
        {
            _agent.enabled = true;
            _rigidBody.isKinematic = true;
            _agent.SetDestination(_player.transform.position);

        }
        else
        {
            _agent.enabled = false;
            _rigidBody.isKinematic = false;

        }
        
        
    }


   public void GettingDashed()
    {
        _shieldAnim.SetBool("Dashed", true);
        _swordAnim.SetBool("Dashed", true);

    }

    void GetPlayerDistance()
    {
        Vector3 vec = transform.position - _player.transform.position;
        _playerDistance = vec.magnitude;

        if(_playerDistance < _attackDistance && !_isAttacking && _stateChange > 5.5f)
        {
            _shieldAnim.SetBool("Attack", true);
            _swordAnim.SetBool("Attack", true);
            _stateChange = 0;
            _isAttacking = true;
        }

    }

    public void AttackMomentum()
    {


    }



}