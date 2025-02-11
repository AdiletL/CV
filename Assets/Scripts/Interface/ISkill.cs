using System;
using Gameplay.Skill;
using Unit.Character.Player;
using UnityEngine;

public interface ISkill
{
    public event Action<ISkill> OnFinished;
    
    public GameObject GameObject { get; }
    public AnimationClip CastClip { get; }
    public SkillType SkillType { get; }
    public SkillType BlockedSkillType { get; }
    public InputType BlockedInputType { get; }
    public Action ExitCallBack { get; }
    public bool IsCanUseSkill { get; }
    
    
    public void Initialize();
    public void Execute(Action callback = null);

    public void Update();
    public void LateUpdate();

    public void CheckTarget();

    public void Exit();
}
