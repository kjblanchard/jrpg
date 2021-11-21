public class Defend : StatusEffect
{
    private const byte _statusLength = 2;
    public Defend(Battler battler) : base(battler, _statusLength)
    {
        StatusEffectName = StatusEffectList.Defend;
    }


    public override float BeforeDamageTaken(int potentialDamage)
    {
        var newDamage = potentialDamage / 2;
        return newDamage;
    }

}
