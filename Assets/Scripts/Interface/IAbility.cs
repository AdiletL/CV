using System;
using Unit.Character.Player;
using UnityEngine;

public interface IAbility
{
    public event Action<int?, float, float> OnCountCooldown;
    public event Action<int?> OnActivated; 
    public event Action<int?> OnStartedCast;
    public event Action<int?> OnFinishedCast;
    public event Action<int?> OnExit;
    
    public int? InventorySlotID { get; }
    public GameObject GameObject { get; }
    public AbilityType AbilityType { get; }
    public AbilityBehaviour AbilityBehaviour { get; }
    public InputType BlockedInputType { get; }
    public Action FinishedCallBack { get; }
    public float Cooldown { get; }
    public bool IsCooldown { get; }
    
    public void Initialize();
    public void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null);

    public void Update();
    public void LateUpdate();

    public void Exit();
}
