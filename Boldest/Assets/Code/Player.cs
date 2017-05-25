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
    [SerializeField] float _dashingTimer = 0;
    bool _inputEnabled = true;
    bool _rightTriggerReleased = true;


    Vector3 _movementVector = Vector3.zero;
    Vector3 _lastMovementVector = Vector3.zero;  

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (_inputEnabled)
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
        if (!_useController)
        {         
            if (Input.GetButtonDown("RightHandButton"))
            {
                transform.GetChild(1).GetComponent<Weapon>().TryAttack();
            }
        }

        if (_useController)
        {
            if (Input.GetButtonDown("RightHandButton"))
            {
                transform.GetChild(1).GetComponent<Weapon>().TryAttack();
            }
        }           
    }


    void HandleDash()
    {
        _dashingTimer -= Time.deltaTime;
        
        if(Input.GetAxisRaw("RightHandTrigger") > 0 && _dashingTimer + _dashCoolDown < 0 && _rightTriggerReleased )
        {
            _dashingTimer = _dashDuration;
            _rightTriggerReleased = false;
            
        }
        if (Input.GetAxisRaw("RightHandTrigger") == 0)
            _rightTriggerReleased = true;

        if(_dashingTimer > 0)
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * _dashSpeed);
            _inputEnabled = false;
        }       
        else
            _inputEnabled = true;

    }



    public void CancelDash()
    {

        _dashingTimer = 0;
    }







}