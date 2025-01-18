using System;
using UnityEngine;

public interface ISkill
{
    public event Action<ISkill> OnExit;
    
    public GameObject GameObject { get; }
    public Action ExitCallBack { get; }
    public AnimationClip CastClip { get; }
    
    public void Initialize(AnimationClip castClip = null);
    public void Execute(Action callback = null);

    public void Update();
    public void LateUpdate();

    public void Exit();
}
