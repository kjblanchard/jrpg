using UnityEngine;

public class ActionPerformState : BattleState
{
    public override void StartState(params bool[] startupBools)
    {
        HandleBaseAttack();
        HandleDefend();
        var damageToCause = Mathf.RoundToInt(_currentBattler.BattlerDamageComponent.GiveDamage(_targetBattler.BattleStats, _currentAbility));
        if (_currentAbility.SkillType != Ability.AbilityType.Healing)
            damageToCause =
                (int)_targetBattler.StatusEffectComponent.ApplyBeforeDamageStatusEffects(damageToCause);


        if (_currentAbility?.AnimationSteps?.Length > 0)
        {
            BattlerActionPerformComponent.PerformAction(_currentBattler,_targetBattler,_currentAbility, damageToCause,ApplyAllDamageStuff,ChangeStateFunc);
        }
        else
        {
            ApplyAllDamageStuff(damageToCause);
            ChangeStateFunc();
        }


    }

    private void ApplyAllDamageStuff(int damageToCause)
    {
            _targetBattler.BattlerDamageComponent.TakeDamage(damageToCause);
            _currentBattler.BattlerDamageComponent.TakeMpDamage(_currentAbility.MpCost);
            ApplyOnDamageStatusEffect(damageToCause);
            ApplyStatusEffectsToTarget();
            TickCurrentBattlersStatusEffects();

    }

    private void ChangeStateFunc()
    {
        _battleComponent.BattleStateMachine.ChangeBattleState(BattleStateMachine.BattleStates.BetweenTurnState);
    }

    /// <summary>
    /// Checks to see if the current battler is defending, and if so targets himself and changes the ability to his defend ability.
    /// </summary>
    private static void HandleDefend()
    {
        if (!IsCurrentBattlerDefending) return;
        _currentAbility = _currentBattler.battlerDefendAbility;
        _targetBattler = _currentBattler;
    }

    /// <summary>
    /// Checks to see if the current battler is defending, and if so changes the ability to that players attack ability.
    /// </summary>
    private static void HandleBaseAttack()
    {
        if (!IsCurrentBattlerAttacking)
            return;
        _currentAbility = _currentBattler.battlerAttackAbility;
    }

    private static void ApplyOnDamageStatusEffect(int damage)
    {
        foreach (var _currentAbilityStatusEffect in _currentAbility.StatusEffects)
        {
            var shouldStatusEffectBeApplied =
                StatusEffectComponent.ShouldStatusEffectBeApplied(_currentAbilityStatusEffect.StatusEffectChance);
            if (!shouldStatusEffectBeApplied) continue;
            var tempStatus =
                StatusEffectComponent.SpawnStatusEffect(_currentAbilityStatusEffect.StatusEffect, _currentBattler);
            tempStatus.OnDamageGivenAttackerEffect(damage);
            tempStatus =
                StatusEffectComponent.SpawnStatusEffect(_currentAbilityStatusEffect.StatusEffect, _targetBattler);
            tempStatus.OnDamageGivenTargetEffect(damage);
        }
    }

    /// <summary>
    /// For each of the status effects, checks to see if they should be applied, and then applies them to the target.
    /// </summary>
    private static void ApplyStatusEffectsToTarget()
    {
        foreach (var _currentAbilityStatusEffect in _currentAbility.StatusEffects)
        {
            var shouldStatusEffectBeApplied =
                StatusEffectComponent.ShouldStatusEffectBeApplied(_currentAbilityStatusEffect.StatusEffectChance);
            if (shouldStatusEffectBeApplied)
                _targetBattler.StatusEffectComponent.ApplyStatusEffect(_currentAbilityStatusEffect.StatusEffect);
        }
    }

    /// <summary>
    /// Applies the current battlers status effects, and removes expired ones.
    /// </summary>
    private static void TickCurrentBattlersStatusEffects()
    {
        _currentBattler.StatusEffectComponent.EndTurn();
        //_currentBattler.StatusEffectComponent.RemoveStaleStatusEffects();
    }


    public override void StateUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void EndState()
    {
        IsCurrentBattlerAttacking = false;
        IsCurrentBattlerDefending = false;
        _battleComponent.BattleGui.BattleNotifications.EnableSelectATarget(false);
        _battleComponent.BattleGui.BattleNotifications.DisableBattleNotification();
    }

    public override void ResetState()
    {
        throw new System.NotImplementedException();
    }

}
