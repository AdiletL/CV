
public interface IHealth
{
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public bool IsLive {get;set;}
    public void TakeDamage(IDamageble damageble);
}

public interface IDamageble
{
    
}
