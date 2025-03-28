public interface IJump
{
    public bool IsCanJump { get; }
    
    public void Initialize();

    public void ActivateJump();
    public void DeactivateJump();
}