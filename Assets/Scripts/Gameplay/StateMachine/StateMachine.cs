﻿using System;
using System.Collections.Generic;
using Machine;
using UnityEngine;

public class StateMachine
{
    public event Action<StateCategory, IState> OnChangedState;
    public event Action<StateCategory, IState> OnExitCategory;
    
    private readonly Dictionary<StateCategory, IState> activeStates = new();
    private readonly Dictionary<Type, IState> states = new();
    private readonly List<IState> updateStates = new();
    private readonly List<StateCategory> cachedCategories = new(); // Static to avoid repeated allocations
    
    // Cache the default idle state
    private IState defaultIdleState;

    public bool IsStateNotNull(Type state)
    {
        return states.ContainsKey(state) && states[state] != null;
    }

    public bool IsActivateType(StateCategory category, Type state)
    {
        return activeStates.ContainsKey(category) &&  activeStates[category].GetType().IsAssignableFrom(state);
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
                if (activeStates.TryGetValue(category, out var activeState) && activeState != state)
                {
                    activeState.Exit();
                }

                // Активируем новое состояние, если оно еще не активно
                if (!activeStates.ContainsKey(category) || activeStates[category] != state)
                {
                    activeStates[category] = state;
                    state.Enter();
                    OnChangedState?.Invoke(category, state);
                }
            }
        }
    }
    

// Метод поиска самого глубокого состояния в иерархии для данного базового типа
    private IState FindMostDerivedState(Type baseType)
    {
        IState mostDerivedState = null;

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


    public void ExitOtherCategories(StateCategory excludedCategory)
    {
        cachedCategories.Clear();
        cachedCategories.AddRange(activeStates.Keys);

        foreach (var category in cachedCategories)
        {
            if (category != excludedCategory)
            {
                activeStates[category].Exit();
                activeStates.Remove(category);
            }
        }
        
        if (activeStates.Count == 0)
            SetDefaultState();
    }

    public void ExitCategory(StateCategory excludedCategory)
    {
        if (activeStates.TryGetValue(excludedCategory, out var state))
        {
            state.Exit();
            activeStates.Remove(excludedCategory);
            OnExitCategory?.Invoke(excludedCategory, state);
            
            if (activeStates.Count == 0)
                SetDefaultState();
        }
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
