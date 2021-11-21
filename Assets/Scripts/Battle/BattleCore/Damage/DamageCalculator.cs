using System;

public class DamageCalculator
{

    /// <summary>
    /// Calculates the damage that should be done to the defending battler from the attacking battlers ability
    /// </summary>
    /// <param name="attackingBattler">The attacking battler</param>
    /// <param name="defendingBattler">The defending battler</param>
    /// <param name="ability">The ability being used</param>
    /// <returns>The damage that is going to be done</returns>
    public static float CalculateDamge(BattleStats attackingBattler, BattleStats defendingBattler, Ability ability)
    {
        return ability.SkillType switch
        {
            Ability.AbilityType.Default => throw new Exception("The ability needs to have a type other than default."),
            Ability.AbilityType.Attacking => CalculateAttackingDamage(attackingBattler, defendingBattler, ability),
            Ability.AbilityType.Buff => 0,
            Ability.AbilityType.Healing => CalculateHealingDamage(attackingBattler, defendingBattler, ability),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static float CalculateAttackingDamage(BattleStats attacking, BattleStats defending, Ability ability)
    {
        var atkPow = (attacking.BattlerStr + ability.AtkBonusDamage) * ability.AtkPowModifier;
        var atkDef = 0.0f;
        return atkPow - atkDef;

    }

    private static float CalculateHealingDamage(BattleStats attacking, BattleStats defending, Ability ability)
    {
        var atkPow = (attacking.BattlerStr + ability.AtkBonusDamage) * ability.AtkPowModifier;
        atkPow *= -1;
        return atkPow;

    }
}
