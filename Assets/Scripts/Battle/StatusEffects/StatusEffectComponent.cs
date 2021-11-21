using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class StatusEffectComponent
{
    /// <summary>
    /// The status effect component for the battler.  This is used to hold status effects and hold data about their effects.
    /// </summary>
    /// <param name="battlerToReference">The battler to reference for this component</param>
    public StatusEffectComponent(Battler battlerToReference)
    {
        _battler = battlerToReference;
    }

    /// <summary>
    /// Checks to see if the status exists.
    /// </summary>
    /// <param name="statusToCheckFor"></param>
    /// <returns>True if the status effect exists.</returns>
    public bool HasStatus(StatusEffectList statusToCheckFor) => _statusEffectList.Any(status => status.StatusEffectName == statusToCheckFor);

    /// <summary>
    /// The str modifier from all of the status effects
    /// </summary>
    public int StrModifier => _statusEffectList.Sum(item => item.StatusEffectStatModifiers.Str);
    /// <summary>
    /// The def modifier from all of the status effects.
    /// </summary>
    public int DefModifier => _statusEffectList.Sum(status => status.StatusEffectStatModifiers.Def);

    /// <summary>
    /// The battler to reference
    /// </summary>
    private readonly Battler _battler;
    /// <summary>
    /// The current status effects.
    /// </summary>
    private readonly List<StatusEffect> _statusEffectList = new List<StatusEffect>();
    /// <summary>
    /// The status effects that are due for removal
    /// </summary>
    private readonly List<StatusEffect> _statusEffectListToRemove = new List<StatusEffect>();

    private bool _playerStartStatusTickedThisTurn = false;

    /// <summary>
    /// Checks to see if the status effect should be applied
    /// </summary>
    /// <param name="percentChance">The chance that it should happen</param>
    /// <returns>if the status should be applied</returns>
    public static bool ShouldStatusEffectBeApplied(float percentChance)
    {
        var randomNumber = Random.Range(0, 100);
        return randomNumber <= percentChance;
    }
    /// <summary>
    /// Applies the status effect.
    /// </summary>
    /// <param name="statusEffectToAdd">The status effect that should be added</param>
    public void ApplyStatusEffect(StatusEffectList statusEffectToAdd)
    {
        if (StatusAlreadyApplied(statusEffectToAdd)) return;
        var instantiatedStatusEffect = SpawnStatusEffect(statusEffectToAdd, _battler);
        instantiatedStatusEffect.StatusEffectEndEvent += OnStatusEffectEnd;
        _statusEffectList.Add(instantiatedStatusEffect);
    }

    /// <summary>
    /// Checks the list to see if the status effect is already applied.  If it is, applies that status' stack effect.
    /// </summary>
    /// <param name="statusEffectToBeAdded">The status affect that should be applied</param>
    /// <returns>True if the status is already applied</returns>
    private bool StatusAlreadyApplied(StatusEffectList statusEffectToBeAdded)
    {
        var duplicateStatus =
            _statusEffectList.FirstOrDefault(status => status.StatusEffectName == statusEffectToBeAdded);
        duplicateStatus?.HandleStatusEffectStack();
        return duplicateStatus != null;
    }

    /// <summary>
    /// Gets rid of all of the status effects that are expired
    /// </summary>
    private void RemoveStaleStatusEffects()
    {
        _statusEffectListToRemove.ForEach(statusEffect => _statusEffectList.Remove(statusEffect));
        _statusEffectListToRemove.Clear();
    }

    /// <summary>
    /// Applies all of the player start status conditions.
    /// </summary>
    public void ApplyAllPlayerStartStateStatus()
    {
        if (_playerStartStatusTickedThisTurn)
            return;
        _playerStartStatusTickedThisTurn = true;
        _statusEffectList.ForEach(statusEffect => statusEffect.PlayerStart());
    }

    /// <summary>
    /// Applies all of the defensive status effects.
    /// </summary>
    /// <param name="beginningDamage">The initial damage amount</param>
    /// <returns>The new damage amount.</returns>
    public float ApplyBeforeDamageStatusEffects(int beginningDamage)
    {
        var newDamage = beginningDamage;
        _statusEffectList.ForEach(statusEffect =>
        {
            newDamage = Mathf.RoundToInt(statusEffect.BeforeDamageTaken(newDamage));
        });
        return newDamage;
    }

    /// <summary>
    /// Applies the Player end state status effects and returns the values to false.
    /// </summary>
    public void EndTurn()
    {
        ApplyAllPlayerEndStateStatus();
        _playerStartStatusTickedThisTurn = false;
        RemoveStaleStatusEffects();
    }

    /// <summary>
    /// Applies all of the player end status effects.
    /// </summary>
    private void ApplyAllPlayerEndStateStatus()
    {
        _statusEffectList.ForEach(statusEffect =>
        {
            statusEffect.PlayerEnd();
        });
    }



    /// <summary>
    /// Handles the deletion of status effects when they call their event.
    /// </summary>
    /// <param name="status"></param>
    private void OnStatusEffectEnd(StatusEffect status)
    {
        _statusEffectListToRemove.Add(status);
    }
    public static StatusEffect SpawnStatusEffect(StatusEffectList statusEffectToSpawn, Battler battler)
    {
        return statusEffectToSpawn switch
        {
            StatusEffectList.Default => null,
            StatusEffectList.Poison => new Poison(battler),
            StatusEffectList.Defend => new Defend(battler),
            StatusEffectList.GetBig => new GetBig(battler),
            StatusEffectList.MpSteal => new PlayerAttackMpSteal(battler),
            _ => throw new ArgumentOutOfRangeException(nameof(statusEffectToSpawn), statusEffectToSpawn, null)
        };
    }
}

/// <summary>
/// All of the status effect names.
/// </summary>
public enum StatusEffectList
{
    Default,
    Poison,
    Defend,
    GetBig,
    MpSteal,
}
