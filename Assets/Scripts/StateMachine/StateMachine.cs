using UnityEngine;


public class StateMachine
{
    public IState currentState { get; private set; }
    public void SetState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Execute();
    }
}
