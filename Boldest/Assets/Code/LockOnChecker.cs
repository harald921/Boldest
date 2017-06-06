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
        if (!other.isTrigger)
        {
            if (other.gameObject.layer == 8)
            {

                _player._lockables.Add(other);
            }

        }
        
           
    }


    void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
        {
            if (other.gameObject.layer == 8)
            {

                _player._lockables.Remove(other);
            }
        }
        
           
    }

}