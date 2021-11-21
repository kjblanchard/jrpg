using System;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Stats/Ability", order = 0)]
public class Ability : ScriptableObject
{

    public Guid Id = new Guid();
    public string Name;
    public int MpCost;
    public float AtkPowModifier;
    public int AtkBonusDamage;
    public StatusEffectAndChance[] StatusEffects;
    public string Description;

    public AbilityName AbilityNameEnum;
    public AbilitySortType SortType;
    public AbilityDamageType DamageType;
    public AbilityType SkillType;
    public TargetingType TargetType;
    public SoundController.Sfx[] AbilitySfxToLoad;
    public GameObject ObjToSpawn;


    public enum AbilityName
    {
        Default,
        BaseAttack,
        BaseDefend,
    }

    /// <summary>
    /// This is how the ability will be sorted into the player window.
    /// </summary>
    public enum AbilitySortType
    {
        Default,
        Magic,
        Skill
    }

    /// <summary>
    /// This is how the calculation for the ability is going to be done
    /// </summary>
    public enum AbilityDamageType
    {
        Default,
        Physical,
        Magic,
        Neutral
    }

    /// <summary>
    /// This is how the ability's damage is calculated.
    /// </summary>
    public enum AbilityType
    {
        Default,
        Attacking,
        Buff,
        Healing,
    }

    /// <summary>
    /// This is how the ability will target the enemies
    /// </summary>
    public enum TargetingType
    {
        Default,
        Any,
        All,
        AllPlayer,
        AllEnemy,
        Self,
        Random
    }

    public AbilityAnimStep[] AnimationSteps;

    [System.Serializable]
    public struct StatusEffectAndChance
    {
        public StatusEffectList StatusEffect;
        public float StatusEffectChance;
    }

}
