using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    float _acceleration = 0;
    public float _accelerationSpeed = 1;

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

    //sword attack stuff
    public float _attackMomentum;
    public float _attackCoolDown;
    float _attackTimer = 0;
    public GameObject _swordSwipe;
    public float _swipeLenght = 0.2f;
       
    //bow stuff
    [HideInInspector] public bool _isBowing = false;

    //lock On stuff
    public List<Collider> _lockables;
    [HideInInspector] public int _currentLockOnID = 0;
    [HideInInspector] public bool _isLockedOn = false;
    float _changeTargetTimer = 0;
    [SerializeField] float _changeTargetDelay = 0.3f;  
    public Image _bull;
    
    
    
				

	void Start()
	{
		_dashAnimator = transform.GetChild(3).GetComponent<Animator>();
        _bull.gameObject.SetActive(false);
	}

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                     
        LockOnEnemy();

        //can only controll player if not in dash or in the middle of chain attacks
        if (!_isDashing && !_inVisceralAttack && !_inKnockBack && !_isBowing && _attackTimer > _attackCoolDown)
        {
            HandleMovement();
            HandleAttackInput();
        }

       

        _attackTimer += Time.deltaTime;
        _acceleration += Time.deltaTime * _accelerationSpeed;
        _acceleration = Mathf.Clamp(_acceleration, 0, 1);


       
    }

    private void FixedUpdate()
    {
		if (!_isDashing && !_inVisceralAttack)
		{
			GetComponent<Rigidbody>().AddForce(new Vector3((_movementVector.normalized.x * _moveSpeed) * _acceleration, -_gravityPower, (_movementVector.normalized.z * _moveSpeed) * _acceleration));        
            _movementVector = Vector3.zero;

		}
            
        HandleDash2();
    }

    void HandleMovement()
    {
		
			_movementVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
			if (Mathf.Abs(_movementVector.x) > 0 || Mathf.Abs(_movementVector.z) > 0)
				_lastMovementVector = _movementVector;
            else
                _acceleration = 0;

            if (_isLockedOn && _lockables.Count >0)
            {
                Vector3 lookVector = _lockables[_currentLockOnID].transform.position - transform.position;
                lookVector.Normalize();
                _lastMovementVector = lookVector;
            }
            
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_lastMovementVector), _turnSpeed * Time.deltaTime);				                    
    }

    void HandleAttackInput()
    {
             
            if (Input.GetButtonDown("RightHandButton"))
            {
                if(_attackTimer >= _attackCoolDown)
                {
                    _attackTimer = 0;              
                GameObject swipe = Instantiate(_swordSwipe, transform.position + (transform.forward * 1.5f), transform.rotation * _swordSwipe.transform.rotation);
                swipe.transform.parent = transform;
                
                Destroy(swipe.gameObject, _swipeLenght);
                    AttackForce();
                }              
            }
            if (Input.GetButtonDown("BowButton"))
            {
                transform.GetChild(2).GetComponent<Bow>().DrawBow();
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
                if(enemyCollider.tag == "Shielder")
                {
                    enemyCollider.GetComponent<Shielder>().GettingDashed();

                }
                if (enemyCollider.tag == "Bird")
                {
                    enemyCollider.GetComponentInParent<Bird>()._move = false;

                }
                              
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

        RemoveEnemyFromList(enemyCollider);
                             
        Destroy(enemyCollider.gameObject); //destroy enemy you attacked (maybe can do different things depending on the type of enemy and tag later on)			
		GameObject deathParticle = Instantiate(_visceralAttackParticle, enemyCollider.transform.position, _visceralAttackParticle.transform.rotation);
		Destroy(deathParticle.gameObject, 3);

        //automaticly set up new dash after finishing attack	       		
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


	void AttackForce()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * _attackMomentum);
    }

	void LockOnEnemy()
    {

        if (_lockables.Count == 0)
            _isLockedOn = false;


        if (Input.GetButtonDown("R3") && !_inVisceralAttack)
        {
             if (_lockables.Count > 0)
                _isLockedOn = !_isLockedOn;
             else
                _isLockedOn = false;       
             
             if(_isLockedOn)
                _currentLockOnID = 0;
        }

        _changeTargetTimer += Time.deltaTime;


        if (_isLockedOn)
        {
            _bull.gameObject.SetActive(true);

            if (Input.GetAxisRaw("RightStickHorizontal") > 0 && _currentLockOnID < _lockables.Count - 1 && _changeTargetTimer > _changeTargetDelay)
            {
                _currentLockOnID++;
                _changeTargetTimer = 0;

            }			
			if (Input.GetAxisRaw("RightStickHorizontal") < 0 && _currentLockOnID > 0 && _changeTargetTimer > _changeTargetDelay)
            {
				float kk = Input.GetAxisRaw("RightStickHorizontal");
				_currentLockOnID--;
                _changeTargetTimer = 0;
            }
              
            if (_lockables.Count > 0)
            {
                for (int i = 0; i < _lockables.Count; i++)
                {
                    if(i == _currentLockOnID)
                    {
                        _bull.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, _lockables[i].transform.position);
                    }
                   
                }              
            }
        }
        else
            _bull.gameObject.SetActive(false);
   }

    public void RemoveEnemyFromList(Collider other)
    {
        //if exiting collider is the target then exit target mode and set target to index 0 to avoid out of range indexing
        if (other == _lockables[_currentLockOnID])
        {
            _isLockedOn = false;
            _currentLockOnID = 0;
            _lockables.Remove(other);
            return;
        }

        // find if the exiting collider is before or after the target in the list(if before, decrement the current locked on id by 1 )
        for (int i = 0; i < _lockables.Count; i++)
        {
            if (other == _lockables[i])
            {
                if (i < _currentLockOnID)
                    _currentLockOnID--;

            }
        }
        _lockables.Remove(other);

    }


}