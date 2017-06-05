using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public GameObject[] _positions;
    public float speed =1.0f;
    public int _startPos;
    int _currentPosition = 0;
    [HideInInspector] public bool _move = true;
    


    void Start()
    {
        _currentPosition = _startPos - 1;
    }

    void Update()
    {
        if (_move)
        {
            transform.GetChild(0).position = Vector3.MoveTowards(transform.GetChild(0).position, _positions[_currentPosition].transform.position, speed * Time.deltaTime);

            Vector3 forward = _positions[_currentPosition].transform.position - transform.GetChild(0).position;
            forward.Normalize();

            transform.GetChild(0).up = forward;

        }

        if (transform.GetChild(0).position == _positions[_currentPosition].transform.position)
        {
            if (_currentPosition < _positions.Length -1)
                _currentPosition++;
            else
                _currentPosition = 0;
            

        }



    }

}