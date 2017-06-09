using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyArcher : MonoBehaviour
{
    [SerializeField] float _maxHealth = 100.0f;
    float _currentHealth;

    [SerializeField] float _drawTime = 5;
    [SerializeField] float _shootLength = 10.0f;

    NavMeshAgent _navMeshAgent;
    Player _player;

    [SerializeField] GameObject _fireBolt;

    Color _defaultColor;


    bool _isFiringAtPlayer = false;

    private void Awake()
    {
        _defaultColor = GetComponent<MeshRenderer>().material.color;
        _currentHealth = _maxHealth;
    }

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        Vector3 dirToPlayer = transform.position - _player.transform.position;
        if (_navMeshAgent.enabled)
            _navMeshAgent.destination = _player.gameObject.transform.position;

        if (dirToPlayer.magnitude <= _shootLength)
            MagicAIStuff();
    }

    private void MagicAIStuff()
    {
        if (!_isFiringAtPlayer)
            StartCoroutine(FireAtPlayer());

        transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));
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

            ModifyHealth(-15.0f);

            Destroy(other.gameObject);
        }
        if (other.tag == "RevertedFire")
        {
            Vector3 attackerToMeDirection = transform.position - other.transform.position;

            TryKnockBack(attackerToMeDirection.normalized * 500);

            ModifyHealth(-15.0f);

            Destroy(other.gameObject);
        }
    }

    public void TryKnockBack(Vector3 inVelocity)
    {
        StartCoroutine(KnockBack(inVelocity));
    }

    public void ModifyHealth(float inHealthModifier)
    {
        _currentHealth += inHealthModifier;

        if (_currentHealth < 0)
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

    IEnumerator FireAtPlayer()
    {
        _isFiringAtPlayer = true;
        _navMeshAgent.enabled = false;

        yield return new WaitForSeconds(_drawTime);

        _navMeshAgent.enabled = true;
        _isFiringAtPlayer = false;

        GameObject fireBolt = Instantiate(_fireBolt, transform.GetChild(0).position, Quaternion.identity);
        fireBolt.GetComponent<Rigidbody>().AddForce(transform.forward * 1200);

        yield return null;
    }
}