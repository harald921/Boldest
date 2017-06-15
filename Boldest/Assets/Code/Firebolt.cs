using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firebolt : MonoBehaviour
{
    [SerializeField] float _knockbackForce = 400.0f;
    public GameObject _hitParticle;


    private void OnTriggerEnter(Collider collision)
    {      
        if (collision.tag == "Sword")
        {
            GetComponent<Rigidbody>().velocity = collision.transform.parent.forward * GetComponent<Rigidbody>().velocity.magnitude;
            transform.GetComponent<Collider>().tag = "RevertedFire";
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject ob = Instantiate(_hitParticle, transform.position, Quaternion.identity);
        Destroy(ob, 3);

        Destroy(gameObject);
    }
}