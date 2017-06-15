using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Shielder : EnemyBase
{
   
    Animator _shieldAnim;
    Animator _swordAnim;

    float _playerDistance;
	Rigidbody _rigidBody;

	[SerializeField] float _awakeDistance = 15.0f;
	[SerializeField] float _attackDistance = 5.0f;
	[SerializeField] float _attackForce = 100000;
	[SerializeField] float _recuperateTime = 1;

	[HideInInspector] public bool _isAttacking = false;
    [HideInInspector] public bool _inSwordSwing = false;
    [HideInInspector] public float _recuperateTimer = 0;
	                        
    public GameObject _death;
    public GameObject _damager;
	



	protected override void Start()
    {
		base.Start();
		
        _player = FindObjectOfType<Player>();
        _rigidBody = GetComponent<Rigidbody>();
        _shieldAnim = transform.GetChild(0).GetComponent<Animator>();
        _swordAnim = transform.GetChild(1).GetComponent<Animator>();           
        _navMeshAgent.enabled = false;      
        _damager.gameObject.SetActive(false);
    }


	protected override void Update()
    {
		base.Update();
        GetPlayerDistance();

		//while not in recuperate state, enemy is invulnerable
		if (_recuperateTimer > _recuperateTime)
		{
			_invulnerableTimer = 0;
			GetComponent<MeshRenderer>().material.color = _defaultColor;
		}			
		else
		{
			GetComponent<MeshRenderer>().material.color = Color.black;
		}

        if (!_inSwordSwing && _recuperateTimer > _recuperateTime)
        {
            Vector3 look = new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z) - transform.position;
            look.Normalize();
            transform.forward = look;
			_invulnerableTimer = 0;
		}
               
        _recuperateTimer += Time.deltaTime;
       
        if(_playerDistance < _awakeDistance && !_isAttacking && _recuperateTimer > _recuperateTime)
        {
            _navMeshAgent.enabled = true;
            _rigidBody.isKinematic = true;
			_navMeshAgent.SetDestination(_player.transform.position);
			
        }
        else
        {
			_navMeshAgent.enabled = false;
            _rigidBody.isKinematic = false;
        }
        
        
    }

    public override void OnGettingVisceraled()
    {
        GettingDashed();

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

        if(_playerDistance < _attackDistance && !_isAttacking && _recuperateTimer > _recuperateTime)
        {
            _shieldAnim.SetBool("Attack", true);
            _swordAnim.SetBool("Attack", true);          
            _isAttacking = true;
        }

    }

    public void AttackMomentum()
    {
        _inSwordSwing = true;
        _rigidBody.AddForce(transform.forward * _attackForce);
        
    }


   

    





}