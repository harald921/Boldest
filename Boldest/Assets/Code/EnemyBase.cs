﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : MonoBehaviour
{
	[HideInInspector] public bool _isVunurable = true;
    [HideInInspector] public float _currentHealth;
    [HideInInspector] public bool _inWeakState = true;
	protected Color _defaultColor;
	protected NavMeshAgent _navMeshAgent;
	protected Player _player;
	protected float _invulnerableTimer;

	[SerializeField] float _health = 100.0f;
	[SerializeField] protected float _invulnerableTime = 0.1f;
	[SerializeField] protected float _bowDamage = 10.0f;
	[SerializeField] protected float _swordDamage = 10.0f;
	[SerializeField] protected float _fireDamage = 10.0f;
	[SerializeField] protected float _knockBackForce = 3000.0f;
	[SerializeField] protected bool _useKnockback = false;

	public Text _damageText;
	Canvas _canvas;

	protected virtual void Start()
	{
		_player = FindObjectOfType<Player>();
		_navMeshAgent = GetComponent<NavMeshAgent>();

		_canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        //uggly solution for now, samurai has skinnedmeshrenderer in child0
        if(GetComponent<MeshRenderer>())
		    _defaultColor = GetComponent<MeshRenderer>().material.color;
        else
            _defaultColor = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color;

        _currentHealth = _health;
	}

	protected virtual void Update()
	{
		_invulnerableTimer += Time.deltaTime;
		if (_invulnerableTimer > _invulnerableTime)
			_isVunurable = true;
		else
			_isVunurable = false;
	}


	public void TryKnockBack(Vector3 inVelocity)
	{
		StartCoroutine(KnockBack(inVelocity));
	}

	public void ModifyHealth(float inHealthModifier)
	{
		_currentHealth += inHealthModifier;
		SpawnDamageText(inHealthModifier);
		if (_currentHealth < 0)
		{
			_player.RemoveEnemyFromList(GetComponent<Collider>());
            OnDeath();
		}

		StartCoroutine(DamageFlash());
	}

    public virtual void OnDeath()
    {

        Destroy(gameObject);
    }

    public virtual void OnGettingVisceraled()
    {
        int k = 0;
        k += 2;
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

            if(GetComponent<MeshRenderer>())
			    GetComponent<MeshRenderer>().material.color = Color.Lerp(_defaultColor, Color.red, flashLerpValue);
            else
                transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = Color.Lerp(_defaultColor, Color.red, flashLerpValue);

            yield return null;
		}
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Sword")
		{
			if (_invulnerableTimer > _invulnerableTime)
			{
				Vector3 attackerToMeDirection = transform.position - other.transform.parent.position;
				TryKnockBack(attackerToMeDirection.normalized * 500);
				ModifyHealth(-_swordDamage);
				_invulnerableTimer = 0;
			}			
		}
		if (other.tag == "Arrow")
		{
			if (_invulnerableTimer > _invulnerableTime)
			{
				Vector3 attackerToMeDirection = transform.position - other.transform.position;
				TryKnockBack(attackerToMeDirection.normalized * 500);
				ModifyHealth(-_bowDamage);
				Destroy(other.gameObject);
			}			
		}
		if (other.tag == "RevertedFire")
		{
			if (_invulnerableTimer > _invulnerableTime)
			{
				Vector3 attackerToMeDirection = transform.position - other.transform.position;
				TryKnockBack(attackerToMeDirection.normalized * 500);
				ModifyHealth(-_fireDamage);
				Destroy(other.gameObject);
			}			
		}
	}

	void SpawnDamageText(float damage)
	{				
		Text text = Instantiate(_damageText, Vector3.zero, Quaternion.identity);
		text.transform.SetParent(_canvas.transform); // add text to canvas

		//set values in damageText Component
		DamageText dmgText = text.GetComponent<DamageText>();
		dmgText._posInWorld = transform.position;
		dmgText._dmg = -damage;				
	}
   
}


