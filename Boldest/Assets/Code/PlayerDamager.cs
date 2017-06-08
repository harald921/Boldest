﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerDamager : MonoBehaviour
{
    public float _damageToGive = 15;




    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<Player>().ModifyHealth(-_damageToGive);
            Vector3 dir = other.transform.position - transform.position;
            dir.Normalize();

            other.GetComponent<Player>().StartCoroutine(other.GetComponent<Player>().KnockBack(dir));

        }
    }



}