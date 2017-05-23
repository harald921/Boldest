using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float _health      = 100.0f;
    [SerializeField] float _moveSpeed   = 16.0f;
    [SerializeField] float _turnSpeed   = 1.0f;

    Vector3 _movementVector = Vector3.zero;

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
        _movementVector = Vector3.zero;

        if      (Input.GetKey(KeyCode.W)) _movementVector += Vector3.forward;
        else if (Input.GetKey(KeyCode.S)) _movementVector += Vector3.back;
        if      (Input.GetKey(KeyCode.A)) _movementVector += Vector3.left;
        else if (Input.GetKey(KeyCode.D)) _movementVector += Vector3.right;
    }

    void HandleAiming()
    {
        Vector3 aimTarget;

        aimTarget = Input.mousePosition;
        aimTarget.z = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
        aimTarget = Camera.main.ScreenToWorldPoint(aimTarget);
        aimTarget = new Vector3(aimTarget.x, transform.position.y, aimTarget.z);


        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(aimTarget - transform.position), _turnSpeed * Time.deltaTime);
    }
}