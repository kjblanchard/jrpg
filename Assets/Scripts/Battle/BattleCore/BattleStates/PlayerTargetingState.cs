using UnityEngine;

public class PlayerTargetingState : BattleState
{


    public override void StartState(params bool[] startupBools)
    {
        _targetBattler = null;
        _battleComponent.BattleGui.BattleNotifications.EnableSelectATarget(true);
    }


    public override void StateUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void EndState()
    {

        _battleComponent.BattleGui.BattleNotifications.EnableSelectATarget(false);
        foreach (var _battleDataAllBattler in _battleComponent.BattleData.AllBattlers)
        {
            if ((_battleDataAllBattler.BattleStats.IsDead || _battleDataAllBattler.spriteComp.color == Color.white))
                continue;
            _battleDataAllBattler.spriteComp.color = Color.white;
            _battleDataAllBattler.battlerNameDisplay.enabled = false;
        }
    }

    public override void ResetState()
    {
        throw new System.NotImplementedException();
    }

}

