public interface ITrap
{
    public void Initialize();

    public void Activate();
    
    public void Deactivate();
}

public interface ITrapInteractable : IInteractable
{
    
}

public interface ITrapAttackable : IAttackable, ITrapInteractable
{
    
}
