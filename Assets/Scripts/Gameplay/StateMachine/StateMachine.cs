using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private List<IState> activeStates = new List<IState>();
    private Dictionary<Type, IState> states = new Dictionary<Type, IState>();

    public bool CheckState<T>() where T : IState
    {
        foreach (var state in states.Values)
            if (state is T desiredState)
                return true;
        
        return false;
    }

    public T GetState<T>() where T : IState
    {
        foreach (var state in states.Values)
            if (state is T desiredState)
                return desiredState;
        
        throw new InvalidOperationException($"State of type {typeof(T)} not found.");
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
            bool keepActive = Array.Exists(desiredStates, desiredType => 
                desiredType.IsAssignableFrom(activeStateType));

            if (!keepActive)
            {
                activeStates[i].Exit();
                activeStates.RemoveAt(i);
            }
        }

        foreach (var stateType in desiredStates)
        {
            if (!activeStates.Exists(state => stateType.IsAssignableFrom(state.GetType())) 
                && states.TryGetValue(stateType, out var newState))
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