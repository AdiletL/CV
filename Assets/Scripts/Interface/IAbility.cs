using System;
using Gameplay;
using UnityEngine;

public interface IAbility
{
    public event Action<int?, float, float> OnCountCooldown;
    public event Action<int?> OnStartedCast;
    public event Action<int?> OnFinishedCast;
    
    public int? InventorySlotID { get; }
    public GameObject GameObject { get; }
    public AbilityType AbilityTypeID { get; }
    public AbilityBehaviour AbilityBehaviourID { get; }
    public Action FinishedCallBack { get; }
    public bool IsCooldown { get; }
    
    public void Initialize();
    public void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null);

    public void Update();

    public void Exit();
}

public enum AbilityType
{
    Nothing,
    Dash,
    DamageResistance,
    BarrierDamage,
}

[Flags]
public enum AbilityBehaviour
{
    Nothing = 0,
    NoTarget = 1 << 0,   // Моментальное применение (Blink)
    UnitTarget = 1 << 1, // Требует выбора цели (Hex)
    PointTarget = 1 << 2,// Требует точки на земле (Sun Strike)
    Toggle = 1 << 3,     // Включается/выключается (Armlet)
    Passive = 1 << 5,    // Пассивная способность (Critical Strike)
    Hidden = 1 << 6,  // Скрытая способность (Invoke)
}

public enum AbilityStatType
{
    Nothing,
    Damage,
    Cooldown,
    Range,
}
