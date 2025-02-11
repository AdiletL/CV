using UnityEngine;

public interface IItem
{
    public bool IsCanTake { get; }
    public void Initialize();
    public void TakeItem(GameObject gameObject);
    public void Enable(KeyCode takeKey);
    public void Disable();
    
    public void JumpToPoint(Vector3 point);
}
