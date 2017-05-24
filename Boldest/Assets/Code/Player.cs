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
        HandleAttackInput();
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
}