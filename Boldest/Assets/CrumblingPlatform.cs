using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CrumblingPlatform : MonoBehaviour
{
    [SerializeField] float _timeBeforeCrumble = 2.0f;

    [SerializeField] float _blinkInterval = 0.1f;

    Color _startColor;

    private void Awake()
    {
        _startColor = GetComponent<MeshRenderer>().material.color;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(Blink(_timeBeforeCrumble));
            StartCoroutine(Crumble());
        }
    }

    IEnumerator Blink(float inTimeToBlink)
    {
        bool colorToggle = true;
        float blinkTimer = 0;

        while (blinkTimer <= inTimeToBlink)
        {
            blinkTimer += _blinkInterval;

            Debug.Log(blinkTimer);

            if (colorToggle)
                GetComponent<MeshRenderer>().material.color = Color.gray;
            else
                GetComponent<MeshRenderer>().material.color = _startColor;

            colorToggle = !colorToggle;

            yield return new WaitForSeconds(_blinkInterval);
        }

        GetComponent<MeshRenderer>().material.color = Color.gray;
    }

    IEnumerator Crumble()
    {
        yield return new WaitForSeconds(_timeBeforeCrumble);

        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().AddTorque(new Vector3(20.0f, 15.0f, 4.5f));

        yield return null;
    }
}