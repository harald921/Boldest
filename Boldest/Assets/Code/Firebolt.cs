using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firebolt : MonoBehaviour
{
    [SerializeField] float _knockbackForce = 400.0f;

    private void OnTriggerEnter(Collider collision)
    {      
        if (collision.tag == "Sword")
        {
            GetComponent<Rigidbody>().velocity = collision.transform.parent.forward * GetComponent<Rigidbody>().velocity.magnitude;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}