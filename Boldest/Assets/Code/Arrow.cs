﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] float _rotateSpeed = 1.0f;

    [SerializeField] float _decayTime = 5.0f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<EnemyRusher>())
        {
            EnemyRusher enemyRusher = collision.gameObject.GetComponent<EnemyRusher>();

            enemyRusher.ModifyHealth(-20.0f);

            Vector3 knockbackDirection = collision.transform.position - transform.position;

            enemyRusher.TryKnockBack(knockbackDirection.normalized * 300);
        }

        StartCoroutine(DecayCooldown());
    }

    IEnumerator DecayCooldown()
    {
        Destroy(GetComponent<BoxCollider>());
        GetComponent<Rigidbody>().isKinematic = true;

        yield return new WaitForSeconds(_decayTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shield")
            Destroy(gameObject);
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward, _rotateSpeed * Time.deltaTime);
    }
}
