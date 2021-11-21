
/// <summary>
/// A class that holds and returns information about the battlers time, and instanties the battle clock
/// </summary>
public class BattlerTimeManager
{
    /// <summary>
    /// Gets the Confirmed turns from the battlers clock
    /// </summary>
    public float[] CurrentTurns => _battlerClock.Next20Turns;
    /// <summary>
    /// Gets the last generated turns from the battlers clock
    /// </summary>
    public float[] PotentialTurns => _battlerClock.PotentialNext20Turns;

    /// <summary>
    /// Instantiates a battler time manager
    /// </summary>
    /// <param name="battleStats">The battle stats that this should reference for timekeeping decisions</param>
    public BattlerTimeManager(BattleStats battleStats)
    {
        _battleStats = battleStats;
        _battlerClock = new BattlerClock();
    }

    private readonly BattleStats _battleStats;
    private readonly BattlerClock _battlerClock;

    public void ConfirmTurn()
    {
        _battlerClock.ConfirmTurn();
    }
    public float[] CalculatePotentialNext20Turns(float skillModifier = 1.0f, bool firstTurn = false)
    {
        return _battlerClock.CalculatePotentialTurns(_battleStats, skillModifier, firstTurn);
    }

    public void SubtractBattleTime(float timeThatHasPassed)
    {

        for (var i = 0; i < _battlerClock.Next20Turns.Length; i++)
        {
            _battlerClock.Next20Turns[i] -= timeThatHasPassed;
        }

    }

    /// <summary>
    /// Calculates the next 20 turns for the battler when they finish their turn.  Needed as their speed is 0 to progress their float array.
    /// </summary>
    public void EndTurn()
    {
        CalculatePotentialNext20Turns();
        ConfirmTurn();
    }


}

