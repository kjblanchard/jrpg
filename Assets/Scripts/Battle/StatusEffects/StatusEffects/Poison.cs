public class Poison : StatusEffect
{
    private const byte _statusLength = 3;
    private int _stacks;

    public Poison(Battler battler) : base(battler, _statusLength)
    {
        StatusEffectName = StatusEffectList.Poison;
    }

    public override void PlayerStart()
    {
        var damageToGive = CalculatePoisonDamage(_battlerToReference.BattleStats.BattlerMaxHp);
        _battlerToReference.BattlerDamageComponent.TakeDamage(damageToGive);
        base.PlayerStart();
    }

    public override void HandleStatusEffectStack()
    {
        _stacks++;
        base.HandleStatusEffectStack();
    }

    private int CalculatePoisonDamage(int battlerHp)
    {
        return battlerHp / 4 + _stacks;

    }
}
