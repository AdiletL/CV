using UnityEngine;

public interface IEffect
{ 
    public GameObject target { get; }
    
    public void SetTarget(GameObject target);
    public void ResetEffect();
    public void UpdateEffect();
    public void ApplyEffect();
    public void DestroyEffect();
}
