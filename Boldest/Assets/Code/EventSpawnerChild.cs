using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSpawnerChild : MonoBehaviour
{
    [SerializeField] EventSpawnerHub _hub;

    [SerializeField] GameObject[] _enemyTypesToSpawn;   // The types of enemies that should be possible to be spawned

    [SerializeField] int    _numEnemiesToSpawn  = 1;    // The total amount of enemies to spawn
    [SerializeField] float  _spawnInterval      = 1;    // How often the enemies are to be spawned

    [SerializeField] bool   _syncedSpawning     = true; // If all spawn positions should spawn enemies synchronised or not

    float _spawnProgress = 0;

    [SerializeField] bool _isFinished = false;
    public bool isFinished
    {
        get { return _isFinished;  }
    }

    private void Update()
    {
        _spawnProgress += Time.deltaTime;

        if (_spawnProgress >= 1)
        {
            TrySpawnEnemy();
            _spawnProgress = 0;
        }
    }

    void TrySpawnEnemy()
    {
        if (_numEnemiesToSpawn > 0)
        {
            _numEnemiesToSpawn--;
            SpawnEnemy();
        }

        else
            _isFinished = false;

    }

    void SpawnEnemy()
    {
        int numEnemyTypesToSpawn = _enemyTypesToSpawn.Length - 1;
        int randomEnemyToSpawn = Random.Range(0, numEnemyTypesToSpawn);

        GameObject spawnedEnemy = Instantiate(_enemyTypesToSpawn[randomEnemyToSpawn], transform.position, Quaternion.identity);
    }
}