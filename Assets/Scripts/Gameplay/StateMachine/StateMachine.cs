using System;
using System.Collections.Generic;
using Machine;
using Unit;
using Unit.Character.Player;
using UnityEngine;
using IState = Machine.IState;

public class StateMachine
{
    public event Action<IState> OnChangedState;
    public event Action<IState> OnExitCategory;
    public event Action OnUpdate;
    public event Action OnLateUpdate;
    
    private readonly Dictionary<StateCategory, IState> activeStates = new();
    private readonly Dictionary<Type, IState> states = new();
    private readonly List<StateCategory> cachedCategories = new();

    private IState defaultIdleState;
    private bool isBlockChangeState;

    public bool IsStateNotNull(Type state) => FindMostDerivedState(state) != null;

    public bool IsActivateType(Type state)
    {
        foreach (var item in activeStates.Values)
        {
            if (state.IsAssignableFrom(item.GetType()))
            {
                return true;
            }
        }
        return false;
    }

    public T GetState<T>() where T : IState
    {
        foreach (var state in states.Values)
        {
            if (state is T desiredState)
            {
                return desiredState;
            }
        }
        throw new InvalidOperationException($"State of type {typeof(T)} not found.");
    }

    public List<T> GetStates<T>() where T : IState
    {
        var result = new List<T>();
        foreach (var state in states.Values)
        {
            if (state is T desiredState)
            {
                result.Add(desiredState);
            }
        }
        return result;
    }

    public void Initialize()
    {
        foreach (var state in states.Values)
        {
            state.Initialize();
            if (state.Category == StateCategory.idle && defaultIdleState == null)
            {
                defaultIdleState = FindMostDerivedState(state.GetType());
            }
        }
    }

    public void AddStates(params IState[] states)
    {
        foreach (var state in states)
        {
            this.states[state.GetType()] = state;
        }
    }

    public void ActiveBlockChangeState()
    { 
        isBlockChangeState = true;
    }
    public void InActiveBlockChangeState()
    { 
        isBlockChangeState = false;
    }
    
    public void SetStates(bool isForceSetState = false, params Type[] desiredStates)
    {
        if(isBlockChangeState) return;
        
        foreach (var baseType in desiredStates)
        {
            if (!states.TryGetValue(baseType, out var state))
                continue;

            var category = state.Category;

            if (activeStates.TryGetValue(category, out var activeState))
            {
                if (activeState.GetType() == state.GetType() && !isForceSetState)
                    continue;

                activeState.Exit();
            }

            if (isForceSetState || !activeStates.ContainsKey(category) || activeStates[category] != state)
            {
                activeStates[category] = state;
                state.Enter();
                OnChangedState?.Invoke(state);
            }
        }
    }

    private IState FindMostDerivedState(Type baseType)
    {
        IState mostDerivedState = null;

        foreach (var state in states.Values)
        {
            if (baseType.IsAssignableFrom(state.GetType()) &&
                (mostDerivedState == null || state.GetType().IsSubclassOf(mostDerivedState.GetType())))
            {
                mostDerivedState = state;
            }
        }

        return mostDerivedState;
    }

    public void ExitOtherStates(Type installationState, bool isForceSetState = false)
    {
        if(this.isBlockChangeState) return;
        
        cachedCategories.Clear();
        cachedCategories.AddRange(activeStates.Keys);
        var targetState = FindMostDerivedState(installationState);
        
        foreach (var category in cachedCategories)
        {
            if (category == targetState.Category ||
                !activeStates[category].isCanExit) continue;

            activeStates[category].Exit();
            OnExitCategory?.Invoke(activeStates[category]);
            activeStates.Remove(category);
        }

        SetStates(isForceSetState, installationState);

        if (activeStates.Count == 0)
            SetDefaultState();
    }

    public void ExitCategory(StateCategory excludedCategory, Type installationState, bool isForceSetState = false)
    {
        if (this.isBlockChangeState) return;
        
        if (activeStates.TryGetValue(excludedCategory, out var state))
        {
            if (state.isCanExit)
            {
                state.Exit();
                activeStates.Remove(excludedCategory);
                OnExitCategory?.Invoke(state);
            }
        }

        if (installationState != null)
            SetStates(isForceSetState, installationState);
        
        if (activeStates.Count == 0)
            SetDefaultState();
    }

    private void SetDefaultState()
    {
        if (defaultIdleState != null)
        {
            SetStates(desiredStates: defaultIdleState.GetType());
        }
    }

    public void Update() => OnUpdate?.Invoke();

    public void LateUpdate() => OnLateUpdate?.Invoke();
}
