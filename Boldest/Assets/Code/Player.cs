using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //settings for player
    [SerializeField] float _health      = 100.0f;
    [SerializeField] float _moveSpeed   = 16.0f;
    [SerializeField] float _turnSpeed   = 1.0f;
    [SerializeField] bool _useController = false;
	[SerializeField] float _gravityPower = 0;
	Vector3 _movementVector = Vector3.zero;
	[HideInInspector] public Vector3 _lastMovementVector = Vector3.zero;

	//settings for dash
	[SerializeField] float _dashDuration;
    [SerializeField] float _dashSpeed;
    [SerializeField] float _dashCoolDown;   
    [HideInInspector] public bool _isDashing = false;
	float _dashingTimer = 0;
	bool _rightTriggerReleased = true;
	bool _inKnockBack = false;
	Animator _dashAnimator;

	//settings for visceral attack
	[SerializeField] float _timeWindowToAttack;
    [SerializeField] float _timePreformingAttack; // will later be controlled by animation I guess
	[SerializeField] GameObject _visceralAttackParticle;
	[SerializeField] ParticleSystem _dashParticle;
	[SerializeField] float _knockBackForce;
	bool _inVisceralAttack = false;
	Coroutine _visceralCo;

    //different dashes for testing
    public bool _dash1;
    public bool _dash2;

    //bow stuff
    [HideInInspector] public bool _isBowing = false;
				

	void Start()
	{
		_dashAnimator = transform.GetChild(3).GetComponent<Animator>();		
	}

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //can only controll player if not in dash or in the middle of chain attacks
        if (!_isDashing && !_inVisceralAttack && !_inKnockBack && !_isBowing)
        {
            HandleMovement();
            HandleAttackInput();
        }
       
     

		

       
    }

    private void FixedUpdate()
    {
		if (!_isDashing && !_inVisceralAttack)
		{
			GetComponent<Rigidbody>().AddForce(new Vector3(_movementVector.normalized.x * _moveSpeed, -_gravityPower, _movementVector.normalized.z * _moveSpeed));
			_movementVector = Vector3.zero;

		}
        if(_dash1)
        HandleDash();

        if(_dash2)
        HandleDash2();
    }

    void HandleMovement()
    {              
            _movementVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            if (Mathf.Abs(_movementVector.x) > 0 || Mathf.Abs(_movementVector.z) > 0)
                _lastMovementVector = _movementVector;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_lastMovementVector), _turnSpeed * Time.deltaTime);             
    }

    void HandleAttackInput()
    {
             
            if (Input.GetButtonDown("RightHandButton"))
            {
                transform.GetChild(1).GetComponent<Weapon>().TryAttack();
            }
            if (Input.GetButtonDown("BowButton"))
            {
                transform.GetChild(2).GetComponent<Bow>().DrawBow();
            }				

    }


    void HandleDash()
    {
        _dashingTimer -= Time.deltaTime;
		_dashAnimator.SetBool("dashing", _isDashing);

		if (Input.GetAxisRaw("RightHandTrigger") > 0 && _dashingTimer + _dashCoolDown < 0 && _rightTriggerReleased && !_inVisceralAttack )
        {
            _dashingTimer = _dashDuration;
            _rightTriggerReleased = false;

			// start dashparticle
			_dashParticle.gameObject.SetActive(true);			
			_dashParticle.Play();
        }
		else if (Input.GetAxisRaw("RightHandTrigger") == 0)
            _rightTriggerReleased = true;

		if (_dashingTimer > 0 && !_inVisceralAttack)
		{			
			Vector3 dashDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
			dashDir.Normalize();

			if (Mathf.Abs(dashDir.x) == 0 && Mathf.Abs(dashDir.z) == 0)
			{
				_dashingTimer = 0;
				return;
			}
			_isDashing = true;
			transform.forward = dashDir;
			GetComponent<Rigidbody>().AddForce(dashDir * _dashSpeed);
			GetComponent<MeshRenderer>().enabled = false; // don't render player model while dashing
		}
		else
		{
			_isDashing = false;
			GetComponent<MeshRenderer>().enabled = true;
		}            

    }

    void HandleDash2()
    {
        _dashingTimer -= Time.deltaTime;
        _dashAnimator.SetBool("dashing", _isDashing);
        Vector3 dashDir = transform.forward;

        if (Input.GetAxisRaw("RightHandTrigger") > 0 && _dashingTimer + _dashCoolDown < 0 && _rightTriggerReleased && !_inVisceralAttack)
        {                              
            _dashingTimer = _dashDuration;
            _rightTriggerReleased = false;
          
            // start dashparticle
            _dashParticle.gameObject.SetActive(true);
            _dashParticle.Play();
        }
        else if (Input.GetAxisRaw("RightHandTrigger") == 0)
            _rightTriggerReleased = true;

        if (_dashingTimer > 0 && !_inVisceralAttack)
        {                   
            _isDashing = true;
            GetComponent<Rigidbody>().AddForce(dashDir * _dashSpeed);
            GetComponent<MeshRenderer>().enabled = false; // don't render player model while dashing
        }
        else
        {
            _isDashing = false;
            GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void visceralAttackWindow(Collider enemyCollider)
    {
		
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0); //stop player for attack window

		//make player face enemy														   
		Vector3 playerToEnemy = enemyCollider.transform.position - transform.position;
		playerToEnemy.Normalize();
		transform.forward = playerToEnemy;

		//start coRoutine that handle the attck window
        _inVisceralAttack = true;
        _dashingTimer = 0;
        _visceralCo = StartCoroutine(HandleVisceralAttackWindow(enemyCollider));
    }
  
    void FailedVisceralAttack(Collider enemyCollider)
    {
		//if visceral attack failed, re-enable collision between enemy and player
		Physics.IgnoreCollision(enemyCollider, GetComponent<Collider>(), false);
		StartCoroutine(KnockBack());
		_inVisceralAttack = false;
    }

    IEnumerator HandleVisceralAttackWindow(Collider enemyCollider)
    {
        float timeToAttack = _timeWindowToAttack;
		bool failedAttack = true;
        while (timeToAttack > 0)
        {
            timeToAttack -= Time.deltaTime;
            if (Input.GetButtonDown("RightHandButton"))
            {
				failedAttack = false;
                StartCoroutine(PreformVisceralAttack(enemyCollider));					
                StopCoroutine(_visceralCo);            
            }
            yield return null;
        }
		if(failedAttack) // for some reason StopCoroutine seems to not be 100% reliable, added bool if this line gets read even on succsesful attack
			FailedVisceralAttack(enemyCollider);  // will be called if failed to press attack during timewindow  
    }

    IEnumerator PreformVisceralAttack(Collider enemyCollider)
    {
		// play attack animation
		transform.GetChild(5).GetComponent<VisceralAttack>().SetAttackActive();

        yield return new WaitForSeconds(_timePreformingAttack); // wait to finish attack, will be timed on attack animation	
		
		Destroy(enemyCollider.gameObject); //destroy enemy you attacked (maybe can do different things depending on the type of enemy and tag later on)			
		GameObject deathParticle = Instantiate(_visceralAttackParticle, enemyCollider.transform.position, _visceralAttackParticle.transform.rotation);
		Destroy(deathParticle.gameObject, 3);

        //automaticly set up new dash after finishing attack	
        if (_dash2)
        {
            bool haveDashDirection = true;
            Vector3 newDashDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            newDashDir.Normalize();

            if (newDashDir.x == 0 && newDashDir.z == 0)
                haveDashDirection = false;

            if (haveDashDirection)
            {
                transform.forward = newDashDir;
                _dashingTimer = _dashDuration;
                _isDashing = true;                             	
                _dashParticle.Play();
            }
            _inVisceralAttack = false;

        }

        if (_dash1)
        {
            _dashingTimer = _dashDuration;
            _isDashing = true;
            _inVisceralAttack = false;         
            _dashParticle.Play();           
        }
        

	}

	IEnumerator KnockBack()
	{
		_inKnockBack = true;
		GetComponent<Rigidbody>().AddForce(-transform.forward * _knockBackForce);
		yield return new WaitForSeconds(0.5f);
		_inKnockBack = false;
	}

	public void CancelDash()
    {
        _dashingTimer = 0;
    }


	

	


}