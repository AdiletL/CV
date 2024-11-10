
using Character;

public interface IAttack : IApplyDamage
{
    public float AmountAttack { get; }
    public void Initialize();

    public void IncreaseStates(IState state);
}

public interface IApplyDamage
{
    IDamageble Damageble { get; set; }
    public void ApplyDamage();
}
