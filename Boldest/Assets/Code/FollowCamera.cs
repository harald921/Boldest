using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] float _interpolationSpeed = 1.0f;

    GameObject _playerGO;
    Vector3 _posOffset;

    Vector3 _velocity = Vector3.zero;

    void Start()
    {
        _playerGO = GameObject.Find("Player");
        _posOffset = transform.position;
    }

    void Update()
    {
        Vector3 targetPosition = new Vector3((_playerGO.transform.position + _posOffset).x, transform.position.y, (_playerGO.transform.position + _posOffset).z);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _interpolationSpeed);
    }
}
