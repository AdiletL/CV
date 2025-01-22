
public interface IUnit
{
    public void Initialize();

    public bool TryGetComponentInUnit<T>(out T component) where T : class;
    public T GetComponentInUnit<T>() where T : class;
    
    public void Show();
    public void Hide();
}
