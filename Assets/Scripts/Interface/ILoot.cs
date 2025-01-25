using UnityEngine;

public interface ILoot
{
    public void Initialize();
    public void TakeLoot(GameObject gameObject);
    public void Enable(KeyCode takeKey);
    public void Disable();
    
    public void JumpToPoint(Vector3 point);
}
