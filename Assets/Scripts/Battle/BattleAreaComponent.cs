using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class BattleAreaComponent : MonoBehaviour
{
    public BattleEncounterZone.BattleEncounter NextEncounter { get; private set; }

    [SerializeField] private int _maxStepsForEncounter;
    [SerializeField] private BattleEncounterZone _battleEncounterZone;

    private int _remainingStepsUntilEncounter;

    /// <summary>
    /// Calculates the steps until battle and the next encounter when the scene loads
    /// </summary>
    private void Start()
    {
        _remainingStepsUntilEncounter = CalculateNextBattleSteps();
        NextEncounter = CalculateNextEncounter();
    }

    /// <summary>
    /// Updates the steps in this zone until next encounter.  This is called by the player component
    /// </summary>
    /// <returns>If battle should start</returns>
    public bool UpdateBattleZoneOnStep()
    {
        _remainingStepsUntilEncounter--;
        if (_remainingStepsUntilEncounter > 0) return false;
        _remainingStepsUntilEncounter = CalculateNextBattleSteps();
        return true;

    }

    /// <summary>
    /// Calculate the next battles amount of steps.
    /// </summary>
    /// <returns></returns>
    private int CalculateNextBattleSteps()
    {
        var stepCount = Random.Range(_maxStepsForEncounter / 2, _maxStepsForEncounter);
        return stepCount;
    }

    /// <summary>
    /// Calculates the next encounter that this zone will engage with
    /// </summary>
    /// <returns>The next battle encounter</returns>
    private BattleEncounterZone.BattleEncounter CalculateNextEncounter()
    {
        var totalEncounterWeight = _battleEncounterZone.BattleEncounters.Select(x => x.EncounterWeight).Sum();
        var randomNumber = Random.Range(0, totalEncounterWeight);
        var currentCounter = 0;
        foreach (var battleEncounter in _battleEncounterZone.BattleEncounters)
        {
            var totalWeight = currentCounter + battleEncounter.EncounterWeight;
            if (randomNumber <= totalWeight)
                return battleEncounter;
            currentCounter += battleEncounter.EncounterWeight;
        }
        return null;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag != "PlayerBattleCollider")
            return;
        var _playerBattleComponent = col.gameObject.GetComponent<PlayerBattleComponent>();
        if (!_playerBattleComponent)
            return;
        _playerBattleComponent.UpdateBattleArea(this);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag != "PlayerBattleCollider")
            return;
        var _playerBattleComponent = col.gameObject.GetComponent<PlayerBattleComponent>();
        if (!_playerBattleComponent)
            return;
        _playerBattleComponent.UpdateBattleArea(null);
    }

}

