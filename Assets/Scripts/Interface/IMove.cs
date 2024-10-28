public interface IMove
{
    public void Initialize();
}

public interface IRun : IMove
{
    public float runSpeed { get; set; }
    public void Run();
}