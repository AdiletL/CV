using System;
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
    public float Cooldown { get; }
    public bool IsCooldown { get; }
    
    public void Initialize();
    public void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null);

    public void Update();

    public void Exit();
}
