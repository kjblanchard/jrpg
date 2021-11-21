using System;
using System.Linq;
using Random = UnityEngine.Random;

/// <summary>
/// The component that each battler has that chooses the ability to be used in battle.  Used for enemy AI and for players gambits.
/// </summary>
public class BattlerGambitComponent
{
    /// <summary>
    /// The gambit groups.  It is wrapped in this as unity sucks at displaying 2d arrays.
    /// </summary>
    private BattlerBaseStats.MultiDimensionalGambits[] _battlerGambitGroups;
    public bool IsGambitsEnabled = false;
    public int GambitGroupChosen;
    /// <summary>
    /// Instantiates a gambit controller.  Pass in the gambits and the battler for it to reference. Battler is used to add the battlers default attack.
    /// </summary>
    /// <param name="battlerGambitGroups">The gambit groups that are going to be used for selection during the battle.</param>
    /// <param name="battler">The battler who we should grab his default attack.</param>
    public BattlerGambitComponent(BattlerBaseStats.MultiDimensionalGambits[] battlerGambitGroups, Battler battler)
    {
        _battlerGambitGroups = battlerGambitGroups;
        AddDefaultAttackToGambitGroups(battler);
    }

    /// <summary>
    /// Adds the default attack to each gambit groups so that there is a default attack implied at the end of every group.j   /// </summary>
    /// <param name="battler"></param>
    private void AddDefaultAttackToGambitGroups(Battler battler)
    {
        foreach (var _multiDimensionalGambits in _battlerGambitGroups)
        {
            _multiDimensionalGambits.BattlerGambits.Add(new BattlerGambit(battler.BattleStats.IsPlayer,
                battler.battlerAttackAbility));
        }
    }


    /// <summary>
    /// Chooses the ability to used based on the battlers and the current gambit group.
    /// </summary>
    /// <param name="enemyBattlers"></param>
    /// <param name="playerBattlers"></param>
    /// <param name="currentBattler"></param>
    /// <returns></returns>
    public Tuple<Battler, Ability> ChooseAction(Battler[] enemyBattlers, Battler[] playerBattlers, Battler currentBattler)
    {
        return (
            from battlerGambit in _battlerGambitGroups[GambitGroupChosen].BattlerGambits
            let targetBattlers = GetPotentialTargetsBasedOnGambitTarget(battlerGambit.ConditionTarget, enemyBattlers, playerBattlers, currentBattler)
            let potentialBattler = GetTargetBasedOnGambitCondition(battlerGambit, targetBattlers)
            let abilityToUse = ChooseAbilityFromAbilities(battlerGambit.AbilityToPerform, currentBattler)
            where potentialBattler != null
            where CheckIfMpIsAvailable(currentBattler.BattleStats.BattlerCurrentMp, abilityToUse.MpCost)
            where CheckConstraint(battlerGambit.ConstraintCondition, battlerGambit.ConstraintValue, GetPotentialTargetsBasedOnGambitTarget(battlerGambit.ConstraintTarget, enemyBattlers, playerBattlers, currentBattler))
            select new Tuple<Battler, Ability>(potentialBattler, abilityToUse)).FirstOrDefault();

    }

    /// <summary>
    /// This handles the player and the enemies ability selection from the gambit.  Players will generally only have one ability and will exit early, but if you setup random attacks on the enemy, you can also choose randomly between them.
    /// </summary>
    /// <param name="abilitiesForThisGambit"></param>
    /// <param name="currentBattler"></param>
    /// <returns></returns>
    private static Ability ChooseAbilityFromAbilities(AbilityAndWeight[] abilitiesForThisGambit, Battler currentBattler) => abilitiesForThisGambit.Length == 1 ? HandleSingleAbilityInGambit(abilitiesForThisGambit, currentBattler) : CalculateRandomAbilityFromGambitAbilities(currentBattler, abilitiesForThisGambit);

    /// <summary>
    /// This is used mostly for enemy actions when you want to choose randomly.  This takes the abilities and their weight, and chooses one randomly.
    /// </summary>
    /// <param name="currentBattler"></param>
    /// <param name="usableAbilityList"></param>
    /// <returns></returns>
    private static Ability CalculateRandomAbilityFromGambitAbilities(Battler currentBattler,
      AbilityAndWeight[] usableAbilityList)
    {
        var totalAbilityWeight = usableAbilityList.Sum(ability => ability.Weight);
        var randomNumber = Random.Range(0, totalAbilityWeight);
        var currentCounter = 0;
        foreach (var _abilityAndWeight in usableAbilityList)
        {
            var totalWeight = currentCounter + _abilityAndWeight.Weight;
            if (randomNumber <= totalWeight) return _abilityAndWeight.Ability.AbilityNameEnum == Ability.AbilityName.BaseAttack ? currentBattler.battlerAttackAbility : _abilityAndWeight.Ability;
            currentCounter += _abilityAndWeight.Weight;
        }
        throw new Exception("Something happened here and you didn't match in the foreach");
    }

    /// <summary>
    /// This is used for most player actions.  It returns the ability at position 0, unless that is a base attack, then it will return the battlers base attack ability.
    /// </summary>
    /// <param name="abilitiesForThisGambit">The ability list for this gambit</param>
    /// <param name="currentBattler">The current battler to reference</param>
    /// <returns></returns>
    private static Ability HandleSingleAbilityInGambit(AbilityAndWeight[] abilitiesForThisGambit, Battler currentBattler) => abilitiesForThisGambit[0].Ability.AbilityNameEnum == Ability.AbilityName.BaseAttack ? currentBattler.battlerAttackAbility : abilitiesForThisGambit[0].Ability;

    private static Battler[] GetPotentialTargetsBasedOnGambitTarget(GambitTarget targetToGet, Battler[] enemyBattlers, Battler[] playerBattlers, Battler currentBattler)
    {
        return targetToGet switch
        {
            GambitTarget.Default => null,
            GambitTarget.Players => playerBattlers,
            GambitTarget.Enemies => enemyBattlers,
            GambitTarget.Self => new[] { currentBattler },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// Finds a battler target based on the gambit condition.
    /// </summary>
    /// <param name="battlerGambit">The gambit we are checking</param>
    /// <param name="conditionTarget">The battler group that we are checking against.</param>
    /// <returns>A battler if the condition is met, else null</returns>
    private static Battler GetTargetBasedOnGambitCondition(BattlerGambit battlerGambit, Battler[] conditionTarget)
    {
        return battlerGambit.Condition switch
        {
            GambitCondition.Default => null,
            GambitCondition.None => null,
            GambitCondition.HpNot100 => null,
            GambitCondition.LeastHpPercent => LeastHp(conditionTarget),
            GambitCondition.HpGreater => CheckHpGreater(conditionTarget, battlerGambit.ConditionValue),
            GambitCondition.HpLess => CheckHpLess(conditionTarget, battlerGambit.ConditionValue),
            GambitCondition.MpGreater => null,
            GambitCondition.MpLess => null,
            GambitCondition.Random => CheckRandom(conditionTarget, battlerGambit.ConditionValue),
            GambitCondition.IsDead => null,
            GambitCondition.StatusEffectNotExist => CheckStatusEffectNotExist(conditionTarget, battlerGambit.StatusEffectToCheckForConstraint),
            GambitCondition.SingleTarget => CheckSingleTarget(conditionTarget),
            GambitCondition.TargetNumGreaterThan => null,
            _ => throw new ArgumentOutOfRangeException("Ugh")
        };
    }

    private static Battler CheckHpLess(Battler[] battlersToCheck, int value)
    {
        return battlersToCheck
            .FirstOrDefault(battler => !battler.BattleStats.IsDead && battler.BattleStats.BattlerCurrentHp <= value);
    }

    private static Battler CheckSingleTarget(Battler[] battlersToCheck)
    {
        var liveBattlers = battlersToCheck.Where(battler => !battler.BattleStats.IsDead);
        return liveBattlers.Count() == 1 ? liveBattlers.FirstOrDefault() : null;
    }



    /// <summary>
    /// Checks the array of battlers based on the value thrown in.  Returns enemies HP percent that is greater than the value, and orders them by the lowest and returns that one.
    /// </summary>
    /// <param name="battlersToCheck"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static Battler CheckHpGreater(Battler[] battlersToCheck, int value)
    {
        return battlersToCheck
            .Where(battler => !battler.BattleStats.IsDead && battler.BattleStats.BattlerCurrentHp >= value)
            .OrderByDescending(battler => battler.BattleStats.BattlerCurrentHpPercent())
            .FirstOrDefault();
    }

    private static Battler CheckRandom(Battler[] battlersToCheck, int value)
    {

        var livingBattlers = battlersToCheck.Where(battler => battler != null && battler.BattleStats.IsDead == false);
        var randomNumber = Random.Range(0, livingBattlers.Count() - 1);
        return livingBattlers.ElementAt(randomNumber);
    }

    private static Battler LeastHp(Battler[] battlersToCheck)
    {
        return battlersToCheck
            .Where(battler => !battler.BattleStats.IsDead)
            .OrderByDescending(battler => battler.BattleStats.BattlerCurrentHpPercent())
            .FirstOrDefault();
    }

    private static Battler CheckStatusEffectNotExist(Battler[] battlersToCheck, StatusEffectList statusToCheckFor)
    {
        return battlersToCheck
            .FirstOrDefault(battler => !battler.StatusEffectComponent.HasStatus(statusToCheckFor));
    }

    private static bool CheckConstraint(GambitCondition constraintToCheckFor, int value, Battler[] constraintTarget)
    {
        return constraintToCheckFor switch
        {
            GambitCondition.Default => true,
            GambitCondition.None => true,
            GambitCondition.HpNot100 => throw new Exception("Not in"),
            GambitCondition.LeastHpPercent => throw new Exception("Not in"),
            GambitCondition.HpGreater => throw new Exception("Not in"),
            GambitCondition.HpLess => throw new Exception("Not in"),
            GambitCondition.MpGreater => constraintTarget.Any(battler => battler.BattleStats.BattlerCurrentMp >= value),
            GambitCondition.MpLess => throw new Exception("Not in"),
            GambitCondition.Random => throw new Exception("Not in"),
            GambitCondition.IsDead => throw new Exception("Not in"),
            GambitCondition.StatusEffectNotExist => throw new Exception("Not in"),
            GambitCondition.SingleTarget => constraintTarget.Length == 1,
            GambitCondition.TargetNumGreaterThan => constraintTarget.Count(target => !target.BattleStats.IsDead) >= value,
            _ => throw new ArgumentOutOfRangeException(nameof(constraintToCheckFor), constraintToCheckFor, null)
        };
    }

    /// <summary>
    /// This is used to see if you have enough MP to use the ability.
    /// </summary>
    /// <param name="currentMp">Your current mp</param>
    /// <param name="abilityMpCost">The abilities MP cost</param>
    /// <returns></returns>
    private static bool CheckIfMpIsAvailable(int currentMp, int abilityMpCost) => currentMp >= abilityMpCost;


}

