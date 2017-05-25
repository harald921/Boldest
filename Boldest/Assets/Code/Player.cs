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

    //settings for dash
    [SerializeField] float _dashDuration;
    [SerializeField] float _dashSpeed;
    [SerializeField] float _dashCoolDown;
    float _dashingTimer = 0;
    public bool _isDashing = false;
    bool _rightTriggerReleased = true;

    //settings for visceral attack
    [SerializeField] float _timeWindowToAttack;
    [SerializeField] float _timePreformingAttack; // will later be controlled by animation I guess
	[SerializeField] GameObject _visceralAttackParticle;
	bool _inVisceralAttack = false;
    Coroutine _visceralCo;
	



    Vector3 _movementVector = Vector3.zero;
    Vector3 _lastMovementVector = Vector3.zero;  

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //can only controll player if not in dash or in the middle of chain attacks
        if (!_isDashing && !_inVisceralAttack)
        {
            HandleMovement();
            HandleAttackInput();
        }
       
        HandleDash();
      
       
    }

    private void FixedUpdate()
    {
        
            GetComponent<Rigidbody>().AddForce(_movementVector.normalized * _moveSpeed);
            _movementVector = Vector3.zero;
        
       
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
        if (_useController)
        {         
            if (Input.GetButtonDown("RightHandButton"))
            {
                transform.GetChild(1).GetComponent<Weapon>().TryAttack();
            }
            if (Input.GetButtonDown("BowButton"))
            {
                transform.GetChild(2).GetComponent<Bow>().DrawBow();
            }
			else if (Input.GetButtonUp("BowButton"))
				transform.GetChild(2).GetComponent<Bow>().ReleaseString();


		}

        if (!_useController)
        {
            if (Input.GetButtonDown("RightHandButton"))
            {
                transform.GetChild(1).GetComponent<Weapon>().TryAttack();              
            }
			if (Input.GetButtonDown("BowButton"))
			{
				transform.GetChild(2).GetComponent<Bow>().DrawBow();
			}
			else if (Input.GetButtonUp("BowButton"))
				transform.GetChild(2).GetComponent<Bow>().ReleaseString();
		}           
    }


    void HandleDash()
    {
        _dashingTimer -= Time.deltaTime;
        
        //
        if(Input.GetAxisRaw("RightHandTrigger") > 0 && _dashingTimer + _dashCoolDown < 0 && _rightTriggerReleased )
        {
            _dashingTimer = _dashDuration;
            _rightTriggerReleased = false;
            _isDashing = true;

        }
        if (Input.GetAxisRaw("RightHandTrigger") == 0)
            _rightTriggerReleased = true;

        if(_dashingTimer > 0 && !_inVisceralAttack)
        {
            Vector3 dashDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            dashDir.Normalize();

            if(Mathf.Abs(dashDir.x) ==0 && Mathf.Abs(dashDir.z) == 0)
            {
				_dashingTimer = 0;
				return;
            }
            transform.forward = dashDir;
            GetComponent<Rigidbody>().AddForce(dashDir * _dashSpeed);           
        }       
        else
            _isDashing = false;

    }

    public void visceralAttackWindow(Collider enemyCollider)
    {
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        _inVisceralAttack = true;
        _dashingTimer = 0;
        _visceralCo = StartCoroutine(HandleVisceralAttackWindow(enemyCollider));
    }
  
    void FailedVisceralAttack()
    {
        // here maybe we want knockback on player and deal damage to player osv     
        _inVisceralAttack = false;
    }

    IEnumerator HandleVisceralAttackWindow(Collider enemyCollider)
    {
        float timeToAttack = _timeWindowToAttack;
        while (timeToAttack > 0)
        {
            timeToAttack -= Time.deltaTime;
            if (Input.GetButtonDown("RightHandButton"))
            {
                StartCoroutine(PreformVisceralAttack(enemyCollider));
				
                StopCoroutine(_visceralCo);              
            }
            yield return null;
        }
        FailedVisceralAttack();  // will be called if failed to press attack during timewindow  
    }

    IEnumerator PreformVisceralAttack(Collider enemyCollider)
    {
       //preform attack
        transform.GetChild(1).GetComponent<Weapon>().TryAttack();// use standard sword for debuging

        yield return new WaitForSeconds(_timePreformingAttack); // wait to finish attack, will be timed on attack animation	later on maybe	
		
		Destroy(enemyCollider.gameObject); //destroy enemy you attacked (maybe can do different things depending on the type of enemy and tag later on)			
		Instantiate(_visceralAttackParticle, enemyCollider.transform.position, _visceralAttackParticle.transform.rotation);
		//automaticly set up new dash after finishing attack									   									   
		_dashingTimer = _dashDuration;
        _isDashing = true;
        _inVisceralAttack = false;

    }


    public void CancelDash()
    {

        _dashingTimer = 0;
    }







}