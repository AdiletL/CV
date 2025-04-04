﻿using System;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable : IInteractable
{
    public void TakeDamage(DamageData damageData);
}

[Flags]
public enum DamageType
{
    Nothing = 0,
    Physical = 1 << 0,
    Magical = 1 << 1,
    Pure = 1 << 2,
}

public class DamageData
{
    public GameObject Owner { get; }
    public DamageType DamageTypeID { get; }
    public float Amount;
    public bool IsCanEvade { get; }
    
    public DamageData(GameObject owner, DamageType damageTypeID, float amount, bool isCanEvade)
    {
        Owner = owner;
        DamageTypeID = damageTypeID;
        Amount = amount;
        IsCanEvade = isCanEvade;
    }
    
    public DamageData Clone()
    {
        return new DamageData(Owner, DamageTypeID, Amount, IsCanEvade);
    }
}

public interface IDamageDataModifier
{
    public DamageData DamageResistanceModifiers(DamageData damageData);
}
