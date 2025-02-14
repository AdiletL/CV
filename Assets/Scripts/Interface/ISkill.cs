using System;
using Unit.Character.Player;
using UnityEngine;

public interface ISkill
{
    public event Action<ISkill> OnFinished;
    
    public int ID { get; }
    public GameObject GameObject { get; }
    public AnimationClip CastClip { get; }
    public SkillType SkillType { get; }
    public SkillType BlockedSkillType { get; }
    public InputType BlockedInputType { get; }
    public Action ExitCallBack { get; }
    public bool IsCanSelect { get; }
    public bool IsCanUseSkill();
    
    
    public void Initialize();
    public void Execute(Action callback = null);

    public void Update();
    public void LateUpdate();

    public void Exit();
}
