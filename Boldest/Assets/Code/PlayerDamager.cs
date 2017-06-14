using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerDamager : MonoBehaviour
{
    [SerializeField] private float _damageToGive   = 15;
    [SerializeField] private bool _destroyOnImpact = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Player>().ModifyHealth(-_damageToGive);
            Vector3 dir = other.transform.position - transform.position;
            dir.Normalize();

            other.GetComponent<Player>().StartCoroutine(other.GetComponent<Player>().KnockBack(dir));

            if (_destroyOnImpact)
                Destroy(gameObject);
        }
    }
}