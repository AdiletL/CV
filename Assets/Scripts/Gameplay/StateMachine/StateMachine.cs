using System;
using System.Collections.Generic;
using UnityEngine;


public class StateMachine
{
    private IState currentState;
    private Dictionary<Type, IState> states = new Dictionary<Type, IState>();

    public bool CheckState<T>() where T : IState
    {
        return states.ContainsKey(typeof(T));
    }
    public T GetState<T>() where T : IState
    {
        return (T)states[typeof(T)];
    }
    public void AddState(IState state)
    {
        states.Add(state.GetType(), state);
    }
    public void SetState<T>() where T : IState
    {
        var type = typeof(T);
        
        if(currentState != null && currentState.GetType() == type)
            return;

        if (states.TryGetValue(type, out var newState))
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }
    }

    public void Update()
    {
        currentState?.Update();
    }
}
