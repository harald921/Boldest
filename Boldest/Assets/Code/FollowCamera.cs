using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    GameObject _playerGO;
    Vector3 _posOffset;

    void Start()
    {
        _playerGO = GameObject.Find("Player");
        _posOffset = transform.position;
    }

    void Update()
    {
        transform.position =  new Vector3((_playerGO.transform.position + _posOffset).x, transform.position.y, (_playerGO.transform.position + _posOffset).z);
    }
}
