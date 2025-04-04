public interface IAttackModifier
{
    public AttackModifierType AttackModifierTypeID { get; }
}

public enum AttackModifierType
{
    Nothing,
    CriticalDamage,
}
