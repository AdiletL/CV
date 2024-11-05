
public interface IAttack : IApplyDamage
{
    public void Initialize();
    public void Enter();
    public void Update();
    public void Exit();
}

public interface IApplyDamage
{
    public IDamageble Damageble { get; set; }
    public void ApplyDamage();
}
