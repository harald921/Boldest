using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakSpot : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Sword")
        {
            Vector3 attackerToMeDirection = transform.position - other.transform.parent.position;



            GetComponentInParent<Shielder>().ModifyHealth(-15.0f);
        }

        if (other.tag == "Arrow")
        {
            Vector3 attackerToMeDirection = transform.position - other.transform.position;



            GetComponentInParent<Shielder>().ModifyHealth(-15.0f);
            Destroy(other.gameObject);
        }
    }
}