﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRusher : MonoBehaviour
{
    [SerializeField] float _health = 100.0f;

    Color _defaultColor;

    bool _isInvurnerable = false;

    NavMeshAgent _navMeshAgent;
    Player _player;

    private void Start()
    {
        _defaultColor = GetComponent<MeshRenderer>().material.color;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _player = FindObjectOfType<Player>();
    }


    private void Update()
    {
        if (_navMeshAgent.enabled)
            _navMeshAgent.destination = GameObject.Find("Player").transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Sword")
        {
            Vector3 attackerToMeDirection = transform.position - other.transform.parent.position;

            TryKnockBack(attackerToMeDirection.normalized * 500);

            ModifyHealth(-15.0f);
        }

        if (other.tag == "Arrow")
        {
            Vector3 attackerToMeDirection = transform.position - other.transform.position;

            TryKnockBack(attackerToMeDirection.normalized * 500);

            ModifyHealth(-34.0f);
            Destroy(other.gameObject);
        }
        if (other.tag == "RevertedFire")
        {
            Vector3 attackerToMeDirection = transform.position - other.transform.position;

            TryKnockBack(attackerToMeDirection.normalized * 500);

            ModifyHealth(-34.0f);
            Destroy(other.gameObject);
        }
    }

    public void TryKnockBack(Vector3 inVelocity)
    {
        if (!_isInvurnerable)
        {
            StartCoroutine(KnockBack(inVelocity));
        }
    }

    public void ModifyHealth(float inHealthModifier)
    {
        _health += inHealthModifier;

        if (_health < 0)
        {           
            _player.RemoveEnemyFromList(GetComponent<Collider>());
            Destroy(gameObject);
        }
           

        StartCoroutine(DamageFlash());
    }

    IEnumerator KnockBack(Vector3 inVelocity)
    {
        GetComponent<NavMeshAgent>().enabled = false;

        yield return new WaitForSeconds(0.6f);

        GetComponent<NavMeshAgent>().enabled = true;
    }

    IEnumerator DamageFlash()
    {
        float flashDuration = 0.13f;
        float timer = flashDuration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            float flashLerpValue = Mathf.InverseLerp(0, flashDuration, timer);

            GetComponent<MeshRenderer>().material.color = Color.Lerp(_defaultColor, Color.red, flashLerpValue);

            yield return null;
        }
    }
}