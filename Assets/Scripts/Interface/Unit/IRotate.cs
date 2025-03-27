using Gameplay;

public interface IRotate
{
    public Stat RotationSpeedStat { get; }
    public bool IsCanRotate { get; }
    
    public void ExecuteRotate();
    public void ActivateRotate();
    public void DeactivateRotate();
}