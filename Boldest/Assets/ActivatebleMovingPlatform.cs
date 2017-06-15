using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatebleMovingPlatform : MonoBehaviour
{
    [SerializeField] ToggleTarget _toggleTarget;

    [SerializeField] float _moveSpeed = 10.0f;

    Vector3 _startPosition, _targetPosition;

    bool _hasBeenToggled = false;

    private void Awake()
    {
        _startPosition = transform.position;
        _targetPosition = transform.GetChild(0).transform.position;
    }

    private void Update()
    {
        if (_toggleTarget._isActive && !_hasBeenToggled)
        {
            Toggle();
            _hasBeenToggled = true;
        }
    }

    public void Toggle()
    {
        StartCoroutine(LerpPositions());
    }

    IEnumerator LerpPositions()
    {
        float moveTimer = 0;

        bool isFinished = false;
        while (!isFinished)
        {
            moveTimer += Time.deltaTime * _moveSpeed;

            if (moveTimer > 1)
            {
                moveTimer = 1;
                isFinished = true;
            }

            transform.position = Vector3.Lerp(_startPosition, _targetPosition, moveTimer);
            yield return null;
        }

        yield return null;
    }
}