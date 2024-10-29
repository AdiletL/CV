
public interface IAttack : IApplyDamage
{
    public void PrepareAttack();
    public void FinishAttack();
}

public interface IApplyDamage
{
    public IDamageble applyDamage { get; set; }
    public void ApplyDamage();
}
