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
    [HideInInspector] public bool _isAttacking = false;
    [HideInInspector] public bool _inSwordSwing = false;

    [HideInInspector] public float _recuperateTimer = 0;
    public float _recuperateTime = 1;
    
       
    float _stateChange = 0;
    NavMeshAgent _agent;
    Rigidbody _rigidBody;

    [SerializeField] float _health = 100.0f;
    [SerializeField] float _attackForce = 100000;

    Color _defaultColor;

    public GameObject _death;

    public GameObject _damager;



    void Start()
    {
        _player = FindObjectOfType<Player>();
        _rigidBody = GetComponent<Rigidbody>();
        _shieldAnim = transform.GetChild(0).GetComponent<Animator>();
        _swordAnim = transform.GetChild(1).GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();      
        _agent.enabled = false;
        _defaultColor = GetComponent<MeshRenderer>().material.color;
        _damager.gameObject.SetActive(false);
    }


    void Update()
    {
        GetPlayerDistance();

        if (!_inSwordSwing && _recuperateTimer > _recuperateTime)
        {
            Vector3 look = new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z) - transform.position;
            look.Normalize();
            transform.forward = look;
        }
        
        _stateChange += Time.deltaTime;
        _recuperateTimer += Time.deltaTime;
       
        if(_playerDistance < _awakeDistance && !_isAttacking && _recuperateTimer > _recuperateTime)
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

        if(_playerDistance < _attackDistance && !_isAttacking && _stateChange > 5.5f + _recuperateTime)
        {
            _shieldAnim.SetBool("Attack", true);
            _swordAnim.SetBool("Attack", true);
            _stateChange = 0;
            _isAttacking = true;
        }

    }

    public void AttackMomentum()
    {
        _inSwordSwing = true;
        _rigidBody.AddForce(transform.forward * _attackForce);
        
    }


    public void ModifyHealth(float inHealthModifier)
    {
        _health += inHealthModifier;

        if (_health < 0)
        {
            _player.RemoveEnemyFromList(GetComponent<Collider>());
            GameObject blood = Instantiate(_death, transform.position, Quaternion.identity);
            Destroy(blood, 10);
            Destroy(gameObject);
        }


        StartCoroutine(DamageFlash());
    }

    IEnumerator DamageFlash()
    {
        float flashDuration = 0.13f;
        float timer = flashDuration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            float flashLerpValue = Mathf.InverseLerp(0, flashDuration, timer);

            GetComponent<MeshRenderer>().material.color = Color.Lerp(_defaultColor, Color.red, flashLerpValue);

            yield return null;
        }
    }

    





}