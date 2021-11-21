using UnityEngine;
using System.Linq;

public class BattlerClock
{

    /// <summary>
    /// Gets the next20turns that has been confirmed
    /// </summary>
    public float[] Next20Turns { get; private set; } = new float[_turnsToCalculate];
    /// <summary>
    /// Gets the next20Turns that had been generated and not confirmed
    /// </summary>
    public float[] PotentialNext20Turns { get; } = new float[_turnsToCalculate];

    public static Battler[] Next20Battlers { get; private set; } = new Battler[20];
    public static Battler[] PotentialNext20Battlers { get; private set; } = new Battler[20];

    /// <summary>
    /// The max clock value, this is used for the speed calculation and is a variable that determines the speed stat per turn
    /// </summary>
    private const ushort _maxClockValue = 1024;
    /// <summary>
    /// How many turns that will be calculated for each character and stored here
    /// </summary>
    private const byte _turnsToCalculate = 20;

    /// <summary>
    /// This is a variance for the initial turn, causes a more random range the higher it gets
    /// </summary>
    private const byte _initialTurnVariance = 2;

    /// <summary>
    /// This generates the next 20 turns based on it's battler input.  Debugs out the order if you want to as well
    /// </summary>
    /// <param name="battlersToCombine">The battlers that it's going to combine with their confirmed 20 turns</param>
    /// <param name="debugOutput">If you should see the output in the debug log</param>
    /// <returns></returns>
    public static Battler[] GenerateTurnList(Battler[] battlersToCombine)
    {
        var query = from battler in battlersToCombine
                    where !battler.BattleStats.IsDead
                    from next20Turn in battler.BattlerTimeManager.CurrentTurns
                    orderby next20Turn
                    select battler;

        PotentialNext20Battlers = query.Take(20).ToArray();

        return PotentialNext20Battlers;
    }

    /// <summary>
    /// Confirms the potential battlers list that was generated last into the next 20 turns
    /// </summary>
    public static void ConfirmNext20Battlers()
    {
        PotentialNext20Battlers.CopyTo(Next20Battlers, 0);

    }

    /// <summary>
    /// Confirms the potential turns, and copies them into the next 20 turns
    /// </summary>
    public void ConfirmTurn()
    {
        PotentialNext20Turns.CopyTo(Next20Turns, 0);
    }

    /// <summary>
    /// Calculates the potential next 20 turns based on it's input.  This needs to be confirmed before it is applied
    /// </summary>
    /// <param name="battleStats">The stats that it is going to look at</param>
    /// <param name="skillSpeedModifier">The skill speed that will modify the value</param>
    /// <param name="initialTurn">If it is the initial turn of the battle</param>
    /// <returns>The potential values</returns>
    public float[] CalculatePotentialTurns(BattleStats battleStats, float skillSpeedModifier = 1.0f, bool initialTurn = false)
    {
        //var initialTurnModifier = 0.0f;
        var eachTurnClock = CalculateClock(battleStats.BattlerLvl, battleStats.BattlerSpd, skillSpeedModifier);
        var initialTurnModifier = initialTurn ? Random.Range(0, eachTurnClock * _initialTurnVariance) : Next20Turns[1];
        PotentialNext20Turns[0] = initialTurnModifier;
        for (var i = 1; i < _turnsToCalculate; i++)
        {
            PotentialNext20Turns[i] = PotentialNext20Turns[0] + eachTurnClock * i;
        }

        return PotentialNext20Turns;
    }

    /// <summary>
    /// This is the calculation That is ran to generate the amount of wait time for each players turn.
    /// </summary>
    private static float CalculateClock(int battlerLevel, int battlerSpeed, float skillSpeedModifier)
    {
        return (_maxClockValue) / (float)(battlerLevel +
                                     battlerSpeed) * skillSpeedModifier;
    }
    public void Testing(float[] newTurns)
    {
        Next20Turns = newTurns;


    }

}
