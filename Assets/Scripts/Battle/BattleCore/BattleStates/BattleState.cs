using UnityEngine;

public abstract class BattleState : MonoBehaviour
{
    protected static Battle _battleComponent;
    protected static Battler _currentBattler;
    protected static Battler _targetBattler;
    protected static Ability _currentAbility;

    public abstract void StartState(params bool[] startupBools);
    public abstract void StateUpdate();
    public abstract void EndState();
    public abstract void ResetState();

    private void Start()
    {
        if (_battleComponent == null)
            _battleComponent = FindObjectOfType<Battle>();
    }

    /// <summary>
    /// Used to set the ability that is going to be used in the next turn by outside sources.
    /// </summary>
    /// <param name="abilityToSetTo"></param>
    public static void SetAbility(Ability abilityToSetTo)
    {
        _currentAbility = abilityToSetTo;
    }

    public static bool IsCurrentBattlerAttacking = false;
    public static bool IsCurrentBattlerDefending = false;

}
