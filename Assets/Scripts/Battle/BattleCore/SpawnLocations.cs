using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocations : MonoBehaviour
{
    public Transform[] EnemySpawnLocation => _enemySpawnLocations;
    public Transform[] PlayerSpawnLocations => _playerSpawnLocations;

    [SerializeField] private Transform[] _enemySpawnLocations;
    [SerializeField] private Transform[] _playerSpawnLocations;
}
