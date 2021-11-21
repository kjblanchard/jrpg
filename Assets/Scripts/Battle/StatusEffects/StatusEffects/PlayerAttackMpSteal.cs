
public class PlayerAttackMpSteal : StatusEffect
{
    private const int _length = 0;
    /// <summary>
    /// Steals Mp equal to the damage given.
    /// </summary>
    /// <param name="battler"></param>
    public PlayerAttackMpSteal(Battler battler) : base(battler, _length)
    {
        StatusEffectName = StatusEffectList.MpSteal;
    }

    public override void OnDamageGivenAttackerEffect(int damageGiven)
    {
        _battlerToReference.BattlerDamageComponent.TakeMpDamage(-damageGiven);
    }

}
