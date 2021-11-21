using System.Collections;
using UnityEngine;

public class EnemyTurnState : BattleState
{
    public override void StartState(params bool[] startupBools)
    {
        _currentBattler.StatusEffectComponent.ApplyAllPlayerStartStateStatus();
        if (_currentBattler.BattleStats.IsDead)
        {
            _battleComponent.BattleStateMachine.ChangeBattleState(BattleStateMachine.BattleStates.BetweenTurnState);
            return;
        }

        var AttackAndTarget =
            _currentBattler.BattlerGambitComponent.ChooseAction(_battleComponent.BattleData.EnemyBattlers,
                _battleComponent.BattleData.PlayerBattlers, _currentBattler);
        _targetBattler = AttackAndTarget.Item1;
        _currentAbility = AttackAndTarget.Item2;
        StartCoroutine(DisplayBattleMessageCo());
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

    private static IEnumerator DisplayBattleMessageCo()
    {
        _battleComponent.BattleGui.BattleNotifications.DisplayBattleNotification($"The enemy {_currentBattler.BattleStats.BattlerDisplayName} attacks {_targetBattler.BattleStats.BattlerDisplayName} with {_currentAbility.Name}");
        yield return new WaitForSeconds(1);
        _battleComponent.BattleGui.BattleNotifications.DisableBattleNotification();
        _battleComponent.BattleStateMachine.ChangeBattleState(BattleStateMachine.BattleStates.ActionPerformState);
    }

}
