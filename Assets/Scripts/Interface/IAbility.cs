using System;
using Unit.Character.Player;
using UnityEngine;

public interface IAbility
{
    public event Action<int?, float, float> OnCountCooldown;
    
    public int? SlotID { get; }
    public GameObject GameObject { get; }
    public AnimationClip CastClip { get; }
    public AbilityType AbilityType { get; }
    public AbilityBehaviour AbilityBehaviour { get; }
    public AbilityType BlockedAbilityType { get; }
    public InputType BlockedInputType { get; }
    public Action FinishedCallBack { get; }
    public float Cooldown { get; }
    public bool IsCooldown { get; }
    
    public void Initialize();
    public void Activate(Action finishedCallBack = null, GameObject target = null, Vector3? point = null);

    public void Update();
    public void LateUpdate();

    public void Exit();
}
