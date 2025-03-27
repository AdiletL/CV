using System;
using System.Collections.Generic;
using System.Linq;
using Machine;
using UnityEngine;

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
        return default(T);
    }

    public T GetInterfaceImplementingClass<T>() where T : class
    {
        foreach (var kvp in states)
        {
            if (kvp.Key.GetInterfaces().Contains(typeof(T)))
            {
                return kvp.Value as T;
            }
        }

        return null;
    }
    
    public bool TryGetInterfaceImplementingClass<T>(out T result) where T : class
    {
        foreach (var kvp in states)
        {
            if (kvp.Key.GetInterfaces().Contains(typeof(T)))
            {
                result = kvp.Value as T;
                return true;
            }
        }

        result = null;
        return false;
    }

    public void Initialize()
    {
        foreach (var state in states.Values)
        {
            if(!state.IsInitialized) state.Initialize();
            if (state.Category == StateCategory.Idle && defaultIdleState == null)
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
            var iState = FindMostDerivedState(baseType);
            if (iState == null) continue;
           /* if (!states.TryGetValue(baseType, out var state))
                continue;*/

            var category = iState.Category;
            var type = iState.GetType();
            if (activeStates.TryGetValue(category, out var activeState))
            {
                if ((activeState.GetType() == type ||
                    !activeState.IsCanExit /*|| iState == activeState*/))
                    continue;
                
                activeState.Exit();
            }
            
            if (!activeStates.ContainsKey(category) || (isForceSetState && activeStates[category] != iState) || 
                activeStates[category] != iState /*|| iState != activeState*/)
            {
                activeStates[category] = iState;
                iState.Enter();
                OnChangedState?.Invoke(iState);
            }
        }
    }

    private IState FindMostDerivedState(Type baseType)
    {
        if (baseType == null) return null;
        
        IState mostDerivedState = null;

        foreach (var state in states.Values)
        {
            Type stateType = state.GetType();

            if (baseType.IsAssignableFrom(stateType) &&
                (mostDerivedState == null || stateType.IsSubclassOf(mostDerivedState.GetType())))
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
            if (category == targetState?.Category ||
                !activeStates[category].IsCanExit) continue;

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
            if (state.IsCanExit && !isForceSetState ||
                isForceSetState)
            {
                state.Exit();
                activeStates.Remove(excludedCategory);
                OnExitCategory?.Invoke(state);
            }
        }

        if (installationState != null)
        {
            SetStates(isForceSetState, installationState);
        }
        
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
