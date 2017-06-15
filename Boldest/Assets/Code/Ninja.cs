using System.Collections;
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
    [SerializeField] float _attackMomentum = 500.0f;
    [SerializeField] float _weakOnHealth = 50.0f;
    [SerializeField] float _flashSpeed = 0.1f;
    [SerializeField] Color _flashColor = Color.white;

    bool _gettingVisceral = false;


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
        _inWeakState = false;

        _joint = transform.Find("Skeleton_Group/Root/Spine_1/Spine_2/Spine_3/Spine_4/R_Clavicle/R_Shoulder/R_Elbow/R_Wrist/Sword_Joint").transform;
        _damager = transform.Find("DamageArea").gameObject;
        _damager.SetActive(false);
        _katana.transform.parent = _joint.transform;
        
    }

    protected override void Update()
    {
        base.Update();
        if (!_gettingVisceral)
        {
            GetPlayerDistance();

            if (_awake)
                PlayerIsNear();

            CheckAttack();
            SetAnimations();

            if (_currentHealth <= _weakOnHealth)
            {
                if (!_inWeakState)
                {
                    _inWeakState = true;
                    StartCoroutine(WeakFlashing());
                }

            }
        }
        else
        {

            _navMeshAgent.enabled = false;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        
        
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
            if(_findNewPointTimer > _findNewPointTime && _navMeshAgent.enabled)
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
       

        if (_playerDistance < _attackDistance)
        {           
            _inAttack = true;           
            _navMeshAgent.enabled = false;

            Player player = FindObjectOfType<Player>();
            Vector3 dir = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z) - transform.position;
            dir.Normalize();
            transform.forward = dir;
            GetComponent<Rigidbody>().isKinematic = false;
           
        }
        else
        {
            _inAttack = false;
            _damager.SetActive(false);
            _navMeshAgent.enabled = true;
            GetComponent<Rigidbody>().isKinematic = true;
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

    void Onstartattack()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * _attackMomentum);

    }

    void OnAttack()
    {
        _damager.SetActive(true);

    }

    void OnAttackDone()
    {


    }

    public override void OnGettingVisceraled()
    {
        _gettingVisceral = true;
        
        _animator.SetBool("Dead", true);
    }

    IEnumerator WeakFlashing()
    {

        while (true)
        {
            transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = _flashColor;
            yield return new WaitForSeconds(_flashSpeed);
            transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = _defaultColor;
            yield return new WaitForSeconds(_flashSpeed);
        }
        
    }
}

