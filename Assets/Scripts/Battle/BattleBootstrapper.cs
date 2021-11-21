using UnityEngine;

/// <summary>
/// This is used for debug purposes to quickly iterate on battles
/// </summary>
public class BattleBootstrapper : MonoBehaviour
{
    public BattlerBaseStats[] PlayerBattlers;
    public BattleEncounterZone.BattleEncounter EnemyBattleEncounter;
    public PersistantData ThePersistantData;

    public void Start()
    {
        var battleData = new PersistantData.BattleData(EnemyBattleEncounter,PlayerBattlers);
        ThePersistantData.UpdateBattleData(battleData);
    }
}
