using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float _health      = 100.0f;
    [SerializeField] float _moveSpeed   = 16.0f;
    [SerializeField] float _turnSpeed   = 1.0f;

    Vector3 _movementVector = Vector3.zero;
    Vector3 _lastMovementVector = Vector3.zero;

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
        if (Input.GetButton("Aiming"))
        {
            Vector3 rightStick = new Vector3(Input.GetAxisRaw("RightStickHorizontal"), 0, Input.GetAxisRaw("RightStickVertical"));
          
            if (Mathf.Abs(rightStick.x) == 0 && Mathf.Abs(rightStick.z) == 0)
            {
                Vector3 aimTarget;
                aimTarget = Input.mousePosition;
                aimTarget.z = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
                aimTarget = Camera.main.ScreenToWorldPoint(aimTarget);
                aimTarget = new Vector3(aimTarget.x, transform.position.y, aimTarget.z);

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(aimTarget - transform.position), _turnSpeed * Time.deltaTime);
            }
            else
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rightStick), _turnSpeed * Time.deltaTime);
        }
        else        
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(_lastMovementVector),_turnSpeed * Time.deltaTime);
        
       

            
        if (Input.GetButtonDown("RightHand"))
        {
            transform.GetChild(1).GetComponent<Weapon>().TryAttack();
        }
    }
}