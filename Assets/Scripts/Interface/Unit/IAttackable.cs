using System;
using UnityEngine;

public interface IAttackable : IInteractable
{
    public void TakeDamage(DamageData damageData);
}

[Flags]
public enum DamageType
{
    Nothing,
    Physical,
    Magical,
    Pure,
}

public class DamageData
{
    public GameObject Owner { get; }
    public DamageType DamageTypeID;
    public float Amount;

    public DamageData(GameObject owner, DamageType damageTypeID, float amount)
    {
        Owner = owner;
        DamageTypeID = damageTypeID;
        Amount = amount;
    }
}
