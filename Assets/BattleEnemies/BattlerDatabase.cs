using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

public class BattlerDatabase : MonoBehaviour
{

    /// <summary>
    /// The Dictionary that is used to map a battler name to the prefab that will be spawned, so that you can spawn the characters easily when loading.
    /// </summary>
    [SerializeField]
    private SerializableDictionaryBase<BattlerNames, GameObject> _enemyBattlerLookupDictionary;

    /// <summary>
    /// Instantiates a battler; should be used in the battle scenes in the loading state.
    /// </summary>
    /// <param name="battlerName">The enum of battlers to spawn</param>
    /// <param name="locationToSpawnAt">The transform to load the character at on the field</param>
    /// <param name="battlerBaseStats">The stats that should be attached to this battler, used for player characters. For enemies this is left blank. </param>
    /// <returns></returns>
    public Battler InstantiateBattler(BattlerNames battlerName, Transform locationToSpawnAt, BattlerBaseStats battlerBaseStats = null)
    {
        var charLookupSuccessful = _enemyBattlerLookupDictionary.TryGetValue(battlerName, out var potentialBattler);
        if (!charLookupSuccessful)
        {
            //TODO Put this debug log into a logger
            Debug.Log($"Char lookup failed for {battlerName}");
            return null;
        }
        var instantiatedBattler = Instantiate(potentialBattler, locationToSpawnAt).GetComponent<Battler>();
        if (battlerBaseStats)
            instantiatedBattler.AssignPlayerBaseBattleStats(battlerBaseStats);

        return instantiatedBattler;
    }

}
/// <summary>
/// A enum of battler names used for referencing prior to the battle, and passed into the Battler database to load their prefab for battle.
/// </summary>
[System.Serializable]
public enum BattlerNames
{

    Default,
    Kevin,
    Todd,
    Cory,
    Melissa,
    Circle,
    BigCircle
}
