using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firebolt : MonoBehaviour
{
    [SerializeField] float _knockbackForce = 400.0f;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.GetComponent<Player>().ModifyHealth(-34.0f);

            Vector3 dirToPlayer = collision.transform.position - transform.position;
            dirToPlayer.Normalize();

            collision.gameObject.GetComponent<Player>().StartCoroutine(collision.gameObject.GetComponent<Player>().KnockBack(new Vector3(dirToPlayer.x, 0, dirToPlayer.z) * _knockbackForce));

            Destroy(gameObject);
        }

        else if (collision.tag == "Sword")
        {
            GetComponent<Rigidbody>().velocity = collision.transform.parent.forward * GetComponent<Rigidbody>().velocity.magnitude;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}