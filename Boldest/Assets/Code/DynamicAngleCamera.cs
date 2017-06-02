using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicAngleCamera : MonoBehaviour
{

	public float _distance = 10;
	public float _moveSpeed;
	public float _interpolationSpeed = 1.0f;

	Vector3 _targetPosition;
	Vector3 _currentRotation = new Vector3(65, 0, 0);
	Vector3 _targetRotation;

	float _fraction = 0;
	bool _isChangingAngle = false;
	Player _player;


	Vector3 _velocity = Vector3.zero;

	[HideInInspector] public Vector3 _orientationVector;

	void Start ()
	{
		_player = FindObjectOfType<Player>();
		_targetPosition = _player.transform.position;
		transform.position = _targetPosition + Quaternion.Euler(_currentRotation) * new Vector3(0, 0, -_distance);
		transform.LookAt(_player.transform.position);
		_targetRotation = _currentRotation;
		
	}
		
	void Update ()
	{
		if (!_isChangingAngle)
		{						
			_targetPosition = _player.transform.position;
			transform.position = _targetPosition + Quaternion.Euler(_currentRotation) * new Vector3(0, 0, -_distance);
			transform.LookAt(_player.transform.position);
		}

		_orientationVector = new Vector3();
		_orientationVector = new Vector3 (_player.transform.position.x, transform.position.y, _player.transform.position.z) - transform.position;
		_orientationVector.Normalize();

		CheckAngleinput();

		if (_isChangingAngle)
			ChangeAngle();
	}

	void CheckAngleinput()
	{
		if (!_isChangingAngle)
		{
			if (Input.GetButtonDown("LeftBumper"))
			{
				_isChangingAngle = true;
				_targetRotation -= new Vector3(0, 45, 0);
			}

			if (Input.GetButtonDown("RightBumper"))
			{
				_isChangingAngle = true;
				_targetRotation += new Vector3(0, 45, 0);
			}

		}
	}

	void ChangeAngle()
	{
		_fraction += Time.deltaTime * _moveSpeed;
		_fraction = Mathf.Clamp(_fraction, 0.0f, 1.0f);

		Vector3 newRotation = Vector3.Slerp(_currentRotation, _targetRotation, _fraction);
				
		transform.position = _player.transform.position + Quaternion.Euler(newRotation) * new Vector3(0, 0, -_distance);
		transform.LookAt(_player.transform.position);

		if (_fraction == 1.0f)
		{
			_isChangingAngle = false;
			_fraction = 0;
			_currentRotation = _targetRotation;

		}

	}











}
