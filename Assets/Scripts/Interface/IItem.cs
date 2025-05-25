using System;
using Gameplay;
using Gameplay.Unit;
using UnityEngine;

public interface IItem
{
    public event Action<int?> OnStartedCast;
    public event Action<int?> OnFinishedCast;
    public event Action<int?, float, float> OnCountCooldown;
    
    public int? InventorySlotID { get; }
    public GameObject Owner { get; }
    public string ItemName { get; }
    public ItemCategory ItemCategoryID { get; }
    public ItemBehaviour ItemBehaviourID { get; }
    public ItemUsageType ItemUsageTypeID { get; }
    public Action FinishedCallBack { get; }
    public int Amount { get; }
    public float Cooldown { get; }
    public float Range { get; }
    public bool IsCooldown { get; }
    public UnitStatConfig[] Stats { get; }
    
    public void Initialize();
    public void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null);
    public void StartEffect();
    public void Update();
    public void Exit();
}