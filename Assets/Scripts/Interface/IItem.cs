using System;
using System.Collections.Generic;
using Gameplay.Ability;
using Unit.Character.Player;
using UnityEngine;

public interface IItem
{
    public event Action<int?> OnActivated; 
    public event Action<int?> OnStarted;
    public event Action<int?> OnFinished;
    public event Action<int?> OnExit;
    public event Action<int?, float, float> OnCountCooldown;
    
    public int? InventorySlotID { get; }
    public GameObject GameObject { get; }
    public ItemName ItemNameID { get; }
    public ItemCategory ItemCategoryID { get; }
    public ItemBehaviour ItemBehaviourID { get; }
    public InputType BlockInputType { get; }
    public Action FinishedCallBack { get; }
    public int Amount { get; }
    public float Cooldown { get; }
    public bool IsCooldown { get; }
    public List<Ability> Abilities { get; }
    
    public void Initialize();
    public void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null);
    public void StartEffect();
    public void Update();
    public void LateUpdate();
    public void FinishEffect();
    public void Exit();
}