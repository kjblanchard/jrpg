using System;
using System.Linq;
using UnityEngine;

public class BattleData : MonoBehaviour
{

    public Battler[] EnemyBattlers { get; private set; }
    public Battler[] PlayerBattlers { get; private set; }

    public Battler GetPlayerByGuid(Guid guid) => (PlayerBattlers.FirstOrDefault(x => x.BattleStats.BattlerGuid == guid));

    public Battler[] AllBattlers => EnemyBattlers.Concat(PlayerBattlers).Where(x => x != null).ToArray();

    public Battler[] AllLiveBattlers => EnemyBattlers.Where(x => !x.BattleStats.IsDead).Concat(PlayerBattlers)
        .Where(x => x != null && !x.BattleStats.IsDead).ToArray();

    /// <summary>
    /// If it is the first turn of the battle or not.
    /// </summary>
    public bool IsItFirstTurn => _turnNumber == 0;
    /// <summary>
    /// The turn number of the battle.
    /// </summary>
    public void IncreaseTurnNumber() => _turnNumber++;

    private int _turnNumber;



    /// <summary>
    /// The database that is used for spawning enemies.
    /// </summary>
    [SerializeField] private BattlerDatabase _battlerDatabase;

    [SerializeField] private AbilityDatabase _abilityDatabase;

    /// <summary>
    /// The locations that the players and enemies will be spawned at for each scene
    /// </summary>
    [SerializeField] private SpawnLocations _spawnLocations;

    /// <summary>
    /// The battledata that is going to be grabbed in from the persistant data so that we can reference information from the overworld.
    /// </summary>
    private PersistantData.BattleData _battleData;


    /// <summary>
    /// Sets the battle data from the persistent data from the overworld.
    /// </summary>
    /// <param name="newData"></param>
    public void SetBattleData(PersistantData.BattleData newData)
    {
        _battleData = newData;
    }

    /// <summary>
    /// Instantiates the player battlers and the enemy battlers based on battle data, and returns all of the battlers.
    /// </summary>
    public Battler[] ConfigureAllBattlers()
    {
        EnemyBattlers = BattlerLoader.LoadEnemyBattlers(_battleData.BattleEncounter.BattleGroups.EnemyBattlers,_spawnLocations,_battlerDatabase);
        PlayerBattlers = BattlerLoader.LoadPlayerBattlers(_battleData.PlayerBattlers,_spawnLocations,_battlerDatabase);
        return AllBattlers;

    }



}
