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
    public void FallGravity();
}
