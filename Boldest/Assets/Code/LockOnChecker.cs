using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnChecker : MonoBehaviour
{

    Player _player;

    void Start()
    {
        _player = GetComponentInParent<Player>();
    }









    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            other.GetComponent<Renderer>().material.color = Color.black;
            _player._lockables.Add(other);
        }
           
    }


    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            other.GetComponent<Renderer>().material.color = Color.white;
            _player._lockables.Remove(other);
        }
           
    }

}