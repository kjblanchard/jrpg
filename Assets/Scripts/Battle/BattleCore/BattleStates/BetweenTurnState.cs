using System.Collections.Generic;
using System.Linq;

public class BetweenTurnState : BattleState
{
    public override void StartState(params bool[] startupBools)
    {
        if (CheckForWinCondition()) return;
        if (CheckForLoseCondition()) return;

        var isFirstTurn = _battleComponent.BattleData.IsItFirstTurn;
        var next20Turns = BattlerClock.Next20Battlers;
        var nextBattler = NextBattlerSelection(isFirstTurn, next20Turns);
        var timeToSubtract = CalculateTimeToSubtractForAllBattlers(isFirstTurn, _currentBattler, nextBattler);
        _currentBattler?.BattlerTimeManager.EndTurn();
        var allLiveBattlers = _battleComponent.BattleData.AllLiveBattlers;
        _currentBattler = nextBattler;
        var newBattlerTurnOrder = UpdateBattlerClocks(allLiveBattlers, timeToSubtract);
        _battleComponent.BattleGui.TurnOrder.UpdateBattlerPicturesInTurnOrderGui(newBattlerTurnOrder);
        _battleComponent.BattleStateMachine.ChangeBattleState(_currentBattler.BattleStats.IsPlayer
            ? BattleStateMachine.BattleStates.PlayerTurnState
            : BattleStateMachine.BattleStates.EnemyTurnState);
        _battleComponent.BattleData.IncreaseTurnNumber();
    }

    /// <summary>
    /// Checks to see if any of the enemies are alive.  If not, there is a win condition
    /// </summary>
    /// <returns>If the battle is won</returns>
    private static bool CheckForWinCondition()
    {
        var allEnemies = _battleComponent.BattleData.EnemyBattlers;
        var areAnyEnemiesAlive = allEnemies.Any(_allEnemy => !_allEnemy.BattleStats.IsDead);
        if (areAnyEnemiesAlive) return false;
        _battleComponent.BattleStateMachine.ChangeBattleState(BattleStateMachine.BattleStates.BattleEndState);
        return true;
    }

    /// <summary>
    /// Checks to see if any of the players are alive.  If not, there is a lose condition.
    /// </summary>
    /// <returns>If the battle is lost</returns>
    private static bool CheckForLoseCondition()
    {
        var allPlayers = _battleComponent.BattleData.PlayerBattlers;
        var areAnyPlayersAlive = allPlayers.Any(_allPlayer => _allPlayer != null && !_allPlayer.BattleStats.IsDead);
        if (areAnyPlayersAlive) return false;
        _battleComponent.BattleStateMachine.ChangeBattleState(BattleStateMachine.BattleStates.BattleEndState);
        return true;
    }
    /// <summary>
    /// Looks at the startup bools if they are available.  If they are there, read the first one and That is if this is the first turn.
    /// </summary>
    /// <param name="startupBools"></param>
    /// <returns></returns>
    private static bool CheckForFirstTurn(IReadOnlyList<bool> startupBools) => startupBools != null && startupBools[0];

    /// <summary>
    /// Chooses the next battler.  If it's the first turn, set the next battler to the first in the list.  Otherwise, look for the next battler that is alive and choose him.
    /// </summary>
    /// <param name="isFirstTurn"></param>
    /// <param name="nextBattlers"></param>
    /// <returns></returns>
    private static Battler NextBattlerSelection(bool isFirstTurn, Battler[] nextBattlers)
    {
        if (isFirstTurn)
            return nextBattlers[0];

        for (var i = 1; i < nextBattlers.Length; i++)
        {
            if (nextBattlers[i].BattleStats.IsDead)
                continue;
            return nextBattlers[i];
        }
        return null;
    }

    /// <summary>
    /// Looks at the current battler and the next battler.  Determines the amount of time that should pass based on them.
    /// </summary>
    /// <param name="isFirstTurn">If it's the first turn of the battle</param>
    /// <param name="currentBattler">The current battler</param>
    /// <param name="nextBattler">The next battler</param>
    /// <returns></returns>
    private static float CalculateTimeToSubtractForAllBattlers(bool isFirstTurn, Battler currentBattler, Battler nextBattler)
    {
        if (isFirstTurn)
            return nextBattler.BattlerTimeManager.CurrentTurns[0];
        var isNextBattlerTheCurrentBattler =
            currentBattler?.BattleStats.BattlerGuid == nextBattler.BattleStats.BattlerGuid;
        return isNextBattlerTheCurrentBattler ? nextBattler.BattlerTimeManager.CurrentTurns[1] : nextBattler.BattlerTimeManager.CurrentTurns[0];
    }

    /// <summary>
    /// Subtracts the time from all battlers clocks, and then generates a new turn list from it.
    /// </summary>
    /// <param name="allBattlers"></param>
    /// <param name="timeToSubtract"></param>
    private static Battler[] UpdateBattlerClocks(Battler[] allBattlers, float timeToSubtract)
    {
        SubtractTimeFromAllBattlers(allBattlers, timeToSubtract);
        var newBattlerList = BattlerClock.GenerateTurnList(_battleComponent.BattleData.AllBattlers);
        BattlerClock.ConfirmNext20Battlers();
        return newBattlerList;
    }

    /// <summary>
    /// Goes through all of the battlers, and subtracts the time that has passed from their clocks
    /// </summary>
    /// <param name="allBattlers">All of the battlers in an array</param>
    /// <param name="timeToSubtract">The amount of time that has passed</param>
    private static void SubtractTimeFromAllBattlers(Battler[] allBattlers, float timeToSubtract)
    {
        foreach (var _battler in allBattlers)
        {
            _battler.BattlerTimeManager.SubtractBattleTime(timeToSubtract);

            //if (!_debugMode) continue;
            //var next5Turns = _battler.BattlerTimeManager.CurrentTurns.Take(5).ToList();
            //next5Turns.ForEach(i =>
            //{
            //    Debug.Log($"The battler {_battler.BattleStats.BattlerDisplayName} and his speed time is {i}");
            //});
        }
    }




    public override void StateUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void EndState()
    {
    }

    public override void ResetState()
    {
        throw new System.NotImplementedException();
    }
}
