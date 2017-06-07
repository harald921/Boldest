using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameEvent
{
    bool isFinished { get; }
    bool isActive { get; }
}

public class EventSpawnerHub : MonoBehaviour, IGameEvent
{
    [SerializeField] bool _isFinished;
    public bool isFinished { get { return _isFinished; } }

    [SerializeField] bool _isActive = false;
    public bool isActive
    {
        get { return _isActive;  }
        set { _isActive = value; }
    }

    [SerializeField] List<EventSpawnerChild> _spawnPositions;
    public List<EventSpawnerChild> spawnPositions
    {
        get { return _spawnPositions; }
    }

    private void Start()
    {
        _isActive = true;
    }

    private void Update()
    {
        CheckIfFinished();
    }

    void CheckIfFinished()
    {
        foreach (var spawner in _spawnPositions)
            if (spawner)
                if (!spawner.isFinished)
                    return;


        Debug.Log("Blörk");
        _isFinished = true;
        _isActive = false;
    }
}