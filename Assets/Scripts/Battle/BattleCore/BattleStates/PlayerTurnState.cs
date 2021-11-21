using System.Collections;
using UnityEngine;

public class PlayerTurnState : BattleState
{
    private const float _displayMessageWaitTime = 1.5f;

    public override void StartState(params bool[] startupBools)
    {
        IsCurrentBattlerAttacking = false;
        IsCurrentBattlerDefending = false;

        _currentBattler.StatusEffectComponent.ApplyAllPlayerStartStateStatus();

        if (_currentBattler.BattlerGambitComponent.IsGambitsEnabled)
            GambitEnabled();
        else
            GambitNotEnabled();
    }

    /// <summary>
    /// If gambits are not enabled, open the menu for the battler.
    /// </summary>
    private static void GambitNotEnabled()
    {
        var battleWindow = _battleComponent.BattleGui.GetPlayerWindow(_currentBattler);
        var magicMenu = _battleComponent.BattleGui.GetMagicWindow(_currentBattler);
        magicMenu.CheckForSufficientMana(_currentBattler.BattleStats.BattlerCurrentMp);
        battleWindow.OpenPlayerWindow();
    }

    /// <summary>
    /// If gambits are enabled, choose an action with your gambits and display a message before switching states.
    /// </summary>
    private void GambitEnabled()
    {
        var targetAndAbility = _currentBattler.BattlerGambitComponent.ChooseAction(
            _battleComponent.BattleData.EnemyBattlers, _battleComponent.BattleData.PlayerBattlers, _currentBattler);
        _targetBattler = targetAndAbility.Item1;
        _currentAbility = targetAndAbility.Item2;
        StartCoroutine(DisplayBattleMessageCo());
    }
    /// <summary>
    /// Displays a battle message for a specific amount of time, used when gambits are enabled so you can see what is happening.
    /// </summary>
    /// <returns></returns>
    private static IEnumerator DisplayBattleMessageCo()
    {
        _battleComponent.BattleGui.BattleNotifications.DisplayBattleNotification($"{_currentBattler.BattleStats.BattlerDisplayName} attacks {_targetBattler.BattleStats.BattlerDisplayName} with {_currentAbility.Name}");
        yield return new WaitForSeconds(_displayMessageWaitTime);
        _battleComponent.BattleGui.BattleNotifications.DisableBattleNotification();
        _battleComponent.BattleStateMachine.ChangeBattleState(BattleStateMachine.BattleStates.ActionPerformState);
    }

    public override void StateUpdate()
    {
    }

    public override void EndState()
    {
    }

    public override void ResetState()
    {
    }

}
