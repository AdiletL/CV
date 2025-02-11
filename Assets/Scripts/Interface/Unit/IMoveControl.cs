using UnityEngine;

public interface IMoveControl
{
    public void SetVelocity(Vector3 velocity);
    public void AddVelocity(Vector3 velocity);
    public void ClearVelocity();
}
