using System;
using UnityEngine;

public enum EffectType
{
    Nothing,
    SlowMovement,
    Vampirism,
    Disable,
    Evasion,
    Transformation,
}

[Flags]
public enum EffectCategory
{
    Nothing = 0,
    Buff = 1 << 0,
    Debuff = 1 << 1,
    Passive = 1 << 2,
    Interim = 1 << 3,
    Active = 1 << 4,
}

public interface IEffect
{ 
    public event Action<IEffect> OnDestroyEffect;
    public EffectType EffectTypeID { get; }
    public EffectCategory EffectCategoryID { get; }
    public GameObject Target { get; }
    
    public void SetTarget(GameObject target);
    public void ClearValues();
    public void Update();
    public void FixedUpdate();
    public void UpdateEffect();
    public void ApplyEffect();
    public void RemoveEffect();
    public void DestroyEffect();
}
