using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CrumblingPlatform : MonoBehaviour
{

    [SerializeField] float _timeBeforeCrumble = 2.0f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            StartCoroutine(Crumble());
    }

    IEnumerator Crumble()
    {
        yield return new WaitForSeconds(_timeBeforeCrumble);

        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().AddTorque(new Vector3(20.0f, 15.0f, 4.5f));

        yield return null;
    }
}