using UnityEngine;

public interface IFall
{
    public float Speed { get; }

    public void Initialize();
    public void Fall();
}

public interface IFallGravity
{
    public float Mass { get; }
    
    public void Initialize();
    public void ExecuteFallGravity();
}

public interface IFallatable
{
    public bool IsFalling { get; }
    public void ActivateFall(float mass);
    public void DeactivateFall();
}
