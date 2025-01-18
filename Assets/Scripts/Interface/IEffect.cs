using System;
using UnityEngine;

public interface IEffect
{ 
    public event Action<IEffect> OnDestroyEffect;

    public GameObject target { get; }
    
    public void SetTarget(GameObject target);
    public void ClearValues();
    public void Update();
    public void LateUpdate();
    public void ApplyEffect();
    public void DestroyEffect();
}
