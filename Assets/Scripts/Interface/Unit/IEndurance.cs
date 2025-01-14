using Unit;

public interface IEndurance
{
    public float MaxEndurance { get;}
    public float CurrentEndurance { get; }

    public void Initialize();
    public void AddEndurance(float value);
    public void RemoveEndurance(float value);
    public void IncreaseMaxEndurance(float value);
}
