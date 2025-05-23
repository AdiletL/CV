﻿public enum StateCategory
{
    nothing,
    Idle,
    Attack,
    Move,
    Action,
    Jump,
    Disable,
}

public interface IState
{
    public StateCategory Category { get; }
    public bool IsActive { get; }
    public bool IsCanExit { get; }
    public bool IsInitialized { get; }

    public void Initialize();
    public void Enter();
    public void Subscribe();
    public void Update();
    public void Unsubscribe();
    public void Exit();
}
