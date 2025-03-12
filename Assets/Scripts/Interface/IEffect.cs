using System;
using UnityEngine;

public interface IEffect
{ 
    public EffectType EffectTypeID { get; }
    
    public string ID { get; }
    public GameObject Target { get; }
    public bool IsInterim { get; }
    
    public void SetTarget(GameObject target);
    public void ClearValues();
    public void Update();
    public void FixedUpdate();
    public void UpdateEffect();
    public void ApplyEffect();
    public void DestroyEffect();
}

public enum EffectType
{
    Nothing,
    SlowMovement,
    Vampirism,
}
