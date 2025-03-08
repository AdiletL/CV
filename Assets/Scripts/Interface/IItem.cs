using System;
using System.Collections.Generic;
using Gameplay;
using Gameplay.Ability;
using Unit;
using UnityEngine;

public interface IItem
{
    public event Action<int?> OnStartedCast;
    public event Action<int?> OnFinishedCast;
    public event Action<int?, float, float> OnCountCooldown;
    
    public int? InventorySlotID { get; }
    public GameObject OwnerGameObject { get; }
    public ItemName ItemNameID { get; }
    public ItemCategory ItemCategoryID { get; }
    public ItemBehaviour ItemBehaviourID { get; }
    public InputType BlockInputTypeID { get; }
    public Action FinishedCallBack { get; }
    public int Amount { get; }
    public float Cooldown { get; }
    public bool IsCooldown { get; }
    public StatConfig[] Stats { get; }
    public List<Ability> Abilities { get; }
    
    public void Initialize();
    public void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null);
    public void StartEffect();
    public void Update();
    public void LateUpdate();
    public void Exit();
}