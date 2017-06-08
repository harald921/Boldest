using System.Collections;
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
        }
    }



}