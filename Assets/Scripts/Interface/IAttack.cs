
public interface IAttack : IApplyDamage
{
    
}

public interface IApplyDamage
{
    public IDamageble Damageble { get; set; }
    public void ApplyDamage();
}
