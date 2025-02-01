using System;
using Gameplay.Skill;
using Unit.Character.Player;
using UnityEngine;

public interface ISkill
{
    public event Action<ISkill> OnFinished;
    
    public GameObject GameObject { get; }
    public AnimationClip CastClip { get; }
    public InputType BlockedInputType { get; }
    public SkillType BlockedSkillType { get; }
    public Action ExitCallBack { get; }
    
    public void Initialize();
    public void Execute(Action callback = null);

    public void Update();
    public void LateUpdate();

    public void Exit();
}
