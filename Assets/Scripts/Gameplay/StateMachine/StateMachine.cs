using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private List<IState> activeStates = new List<IState>();
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
        states[state.GetType()] = state;
    }

    public void SetStates(params Type[] desiredStates)
    {
        for (int i = activeStates.Count - 1; i >= 0; i--)
        {
            var activeStateType = activeStates[i].GetType();
            if (Array.IndexOf(desiredStates, activeStateType) == -1)
            {
                activeStates[i].Exit();
                activeStates.RemoveAt(i);
            }
        }

        foreach (var stateType in desiredStates)
        {
            if (!activeStates.Exists(state => state.GetType() == stateType) && states.TryGetValue(stateType, out var newState))
            {
                activeStates.Add(newState);
                newState.Enter();
            }
        }
    }

    public void Update()
    {
        for (int i = 0; i < activeStates.Count; i++)
        {
            activeStates[i].Update();
        }
    }
}