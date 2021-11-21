/// <summary>
/// This class handles loading in the players and enemies
/// </summary>
public class BattlerLoader
{
    /// <summary>
    /// Loads the enemy battlers by calling into the battler database.
    /// </summary>
    /// <param name="enemyBattlerNames">An array of Enemy names to location number in reference to the scene locations of enemies to spawn</param>
    /// <param name="theSpawnLocations">A reference to the spawn locations on the scene</param>
    /// <param name="theBattlerDatabase">The battler database that holds the dictionary of prefabs to names</param>
    /// <returns></returns>
    public static Battler[] LoadEnemyBattlers(EnemyBattleGroup.EnemyNameToLocation[] enemyBattlerNames, SpawnLocations theSpawnLocations, BattlerDatabase theBattlerDatabase)
    {
        var count = enemyBattlerNames.Length;
        var battlerArray = new Battler[count];

        for (var i = 0; i < enemyBattlerNames.Length; i++)
        {
            var enemyBattlerInfo = enemyBattlerNames[i];
            var enemySpawnLocation = theSpawnLocations.EnemySpawnLocation[enemyBattlerInfo.LocationToSpawnInBattle];
            var enemyBattler = theBattlerDatabase.InstantiateBattler(enemyBattlerInfo.EnemyBattler, enemySpawnLocation);
            battlerArray[i] = enemyBattler;
            enemyBattler.BattleStats.AddBattlerNumber(i);
        }
        return battlerArray;
    }
    /// <summary>
    /// Loads the player battlers by calling into the battler database.
    /// </summary>
    /// <param name="playerStatsAndNames">An array of Player names to stats number in reference to the scene locations of players to spawn</param>
    /// <param name="theSpawnLocations">A reference to the spawn locations on the scene</param>
    /// <param name="theBattlerDatabase">The battler database that holds the dictionary of prefabs to names</param>
    /// <returns></returns>
    public static Battler[] LoadPlayerBattlers(BattlerBaseStats[] playerBattlers, SpawnLocations theSpawnLocations, BattlerDatabase theBattlerDatabase)
    {
        var playerBattlerArray = new Battler[3];

        for (var i = 0; i < playerBattlers.Length; i++)
        {
            var currentPlayerBattlerStats = playerBattlers[i];
            if(!currentPlayerBattlerStats)
                continue;
            var playerSpawnLocation = theSpawnLocations.PlayerSpawnLocations[i];
            var battler = theBattlerDatabase.InstantiateBattler(currentPlayerBattlerStats.BattlerNameEnum,
                playerSpawnLocation, currentPlayerBattlerStats);
            battler.BattleStats.AddBattlerNumber(i);
            playerBattlerArray[i] = battler;
        }

        return playerBattlerArray;
    }
}
