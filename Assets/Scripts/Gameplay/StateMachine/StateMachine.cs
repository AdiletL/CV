using System;
using System.Collections.Generic;
using Machine;
using UnityEngine;

public class StateMachine
{
    public event Action<IState> OnChangedState;
    public event Action<IState> OnExitCategory;
    
    private readonly Dictionary<StateCategory, IState> activeStates = new();
    private readonly Dictionary<Type, IState> states = new();
    private readonly List<IState> updateStates = new();
    private readonly List<StateCategory> cachedCategories = new(); // Static to avoid repeated allocations
    
    // Cache the default idle state
    private IState defaultIdleState;
    private IState mostDerivedState;
    private IState targetState;
    
    public Dictionary<StateCategory, IState> ActiveStates => activeStates;

    public bool IsStateNotNull(Type state)
    {
        return FindMostDerivedState(state) != null;
    }

    public bool IsActivateType(Type state)
    {
        foreach (var item in activeStates.Values)
        {
            // Проверяем, что состояние является наследником baseType
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
                defaultIdleState = FindMostDerivedState(state.GetType());// Cache idle state on initialization
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

    public void SetStates(params Type[] desiredStates)
    {
        foreach (var baseType in desiredStates)
        {
            // Сначала проверяем, есть ли базовый тип в словаре состояний
            if (!states.ContainsKey(baseType))
                continue;

            // Находим самое глубокое состояние, которое наследуется от указанного базового типа
            var state = FindMostDerivedState(baseType);

            if (state != null)
            {
                var category = state.Category;

                // Если состояние этой категории уже активно, заменяем его новым
                if (activeStates.TryGetValue(category, out var activeState) && activeState.GetType() != state.GetType())
                {
                    activeState.Exit();
                }

                // Активируем новое состояние, если оно еще не активно
                if (!activeStates.ContainsKey(category) || activeStates[category] != state)
                {
                    activeStates[category] = state;
                    state.Enter();
                    OnChangedState?.Invoke(state);
                }
            }
        }
    }
    

// Метод поиска самого глубокого состояния в иерархии для данного базового типа
    private IState FindMostDerivedState(Type baseType)
    {
        mostDerivedState = null;

        foreach (var state in states.Values)
        {
            // Проверяем, что состояние является наследником baseType
            if (baseType.IsAssignableFrom(state.GetType()) &&
                (mostDerivedState == null || state.GetType().IsSubclassOf(mostDerivedState.GetType())))
            {
                mostDerivedState = state;
            }
        }

        return mostDerivedState;
    }


    public void ExitOtherStates(Type installationState)
    {
        cachedCategories.Clear();
        cachedCategories.AddRange(activeStates.Keys);
        targetState = FindMostDerivedState(installationState);

        foreach (var category in cachedCategories)
        {
            if(category == targetState.Category) continue;
            
            activeStates[category].Exit();
            activeStates.Remove(category);
        }
        
        SetStates(installationState);
        
        if (activeStates.Count == 0)
            SetDefaultState();
    }

    public void ExitCategory(StateCategory excludedCategory, Type installationState)
    {
        if (activeStates.TryGetValue(excludedCategory, out var state))
        {
            state.Exit();
            activeStates.Remove(excludedCategory);
            OnExitCategory?.Invoke(state);
        }

        if (installationState != null)
        {
            cachedCategories.Clear();
            cachedCategories.AddRange(activeStates.Keys);

            SetStates(installationState);
        }

        if (activeStates.Count == 0)
            SetDefaultState();
    }

    private void SetDefaultState()
    {
        if (defaultIdleState != null)
        {
            SetStates(defaultIdleState.GetType());
        }
    }

    public void Update()
    {
        updateStates.Clear();
        updateStates.AddRange(activeStates.Values);
        
        for (int i = updateStates.Count - 1; i >= 0; i--)
        {
            updateStates[i]?.Update();
        }
    }
    
    public void LateUpdate()
    {
        updateStates.Clear();
        updateStates.AddRange(activeStates.Values);

        for (int i = updateStates.Count - 1; i >= 0; i--)
        {
            updateStates[i]?.LateUpdate();
        }
    }
}
