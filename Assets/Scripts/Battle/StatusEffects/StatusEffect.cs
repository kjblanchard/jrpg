[System.Serializable]
public abstract class StatusEffect
{
    /// <summary>
    /// The event that should be fired when the status is expired
    /// </summary>
    public event StatusEffectEndEventArgs StatusEffectEndEvent;
    public delegate void StatusEffectEndEventArgs(StatusEffect obj);

    protected StatusEffect(Battler battler, int initialLength)
    {
        _battlerToReference = battler;
        _statusInitialLength = initialLength;
        _statusTurnsRemaining = _statusInitialLength;
    }

    /// <summary>
    /// The Status effects stat modifiers.
    /// </summary>
    public StatModifiers StatusEffectStatModifiers { get; private set; } = new StatModifiers();
    /// <summary>
    /// The name of the specific status effect, used to check for stacking.
    /// </summary>
    public StatusEffectList StatusEffectName;


    protected Battler _battlerToReference;

    /// <summary>
    /// The initial length of the status effect.
    /// </summary>
    protected readonly int _statusInitialLength;
    /// <summary>
    /// The remaining turns for the status effect.
    /// </summary>
    protected int _statusTurnsRemaining;

    public virtual void PlayerStart() { }

    /// <summary>
    /// Player end base will decrement the turns remaining, and call the event to delete this from the status effect component
    /// </summary>
    public virtual void PlayerEnd()
    {
        _statusTurnsRemaining--;
        if (_statusTurnsRemaining <= 0)
            OnStatusEffectEnd(this);
    }

    /// <summary>
    /// Handles the defensive before damage taken for the status effect.  By default, just returns the potential damage back.
    /// </summary>
    /// <param name="potentialDamage">The damage that will be applied</param>
    /// <returns>The actual damage after the status effect is applied</returns>
    public virtual float BeforeDamageTaken(int potentialDamage)
    {
        return potentialDamage;
    }


    /// <summary>
    /// Handles what happens when you apply the status effect again.  By default, it will renew it's time.
    /// </summary>
    public virtual void HandleStatusEffectStack()
    {
        _statusTurnsRemaining = _statusInitialLength;

    }

    /// <summary>
    /// This happens when damage is given and affects the target.
    /// </summary>
    /// <param name="damageGiven"></param>
    public virtual void OnDamageGivenTargetEffect(int damageGiven)
    {

    }
    /// <summary>
    /// This happens when damage is given and affects the attacker.
    /// </summary>
    /// <param name="damageGiven"></param>
    public virtual void OnDamageGivenAttackerEffect(int damageGiven)
    {

    }

    /// <summary>
    /// Fire the event to get rid of the status effect.
    /// </summary>
    /// <param name="obj"></param>
    public void OnStatusEffectEnd(StatusEffect obj)
    {
        StatusEffectEndEvent?.Invoke(this);
    }

    public virtual void OnFirstApplied()
    {

    }



    /// <summary>
    /// Stat modifiers that is used by the status effects.
    /// </summary>
    public class StatModifiers
    {
        public int Hp;
        public int Mp;
        public int Str;
        public int Def;
        public int Spd;
    }
}
