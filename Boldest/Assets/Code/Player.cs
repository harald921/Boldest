using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float _health      = 100.0f;
    [SerializeField] float _moveSpeed   = 16.0f;
    [SerializeField] float _turnSpeed   = 1.0f;
    [SerializeField] bool _useController = false;

    Vector3 _movementVector = Vector3.zero;
    Vector3 _lastMovementVector = Vector3.zero;
    bool _rightTriggerRelased = true;

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        HandleMovement();
        HandleAiming();
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
        
    }

    void HandleAiming()
    {
        if (!_useController)
        {
            if (Input.GetButton("Aiming"))
            {
                Vector3 aimTarget;
                aimTarget = Input.mousePosition;
                aimTarget.z = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
                aimTarget = Camera.main.ScreenToWorldPoint(aimTarget);
                aimTarget = new Vector3(aimTarget.x, transform.position.y, aimTarget.z);

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(aimTarget - transform.position), _turnSpeed * Time.deltaTime);              
            }
            else
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_lastMovementVector), _turnSpeed * Time.deltaTime);

            if (Input.GetButtonDown("RightHandButton"))
            {
                transform.GetChild(1).GetComponent<Weapon>().TryAttack();
            }
        }

        if (_useController)
        {
            Vector3 rightStick = new Vector3(Input.GetAxisRaw("RightStickHorizontal"), 0, Input.GetAxisRaw("RightStickVertical"));

            if (Input.GetButton("Aiming"))
            {
                if(Mathf.Abs(rightStick.x) >0 || Mathf.Abs(rightStick.z) > 0)               
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rightStick.normalized), _turnSpeed * Time.deltaTime);
                else
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_lastMovementVector), _turnSpeed * Time.deltaTime);

            }
            else
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_lastMovementVector), _turnSpeed * Time.deltaTime);

            if (Input.GetAxisRaw("RightHandTrigger") == 0)
                _rightTriggerRelased = true;

            if (Input.GetAxisRaw("RightHandTrigger") > 0 && _rightTriggerRelased)
            {
                transform.GetChild(1).GetComponent<Weapon>().TryAttack();
                _rightTriggerRelased = false;
            }
                
        }

       

        
    }
}