using System;
using System.Collections.Generic;

public class StateMachine
{
    public event Action<StateCategory, IState> OnChangedState; 
    
    private readonly Dictionary<StateCategory, IState> activeStates = new();
    private readonly Dictionary<Type, IState> states = new();
    private readonly List<IState> updateStates = new();
    private readonly List<StateCategory> cachedCategories = new(); // Static to avoid repeated allocations
    
    // Cache the default idle state
    private IState defaultIdleState;

    public void Initialize()
    {
        foreach (var state in states.Values)
        {
            state.Initialize();
            if (state.Category == StateCategory.idle && defaultIdleState == null)
            {
                defaultIdleState = state; // Cache idle state on initialization
            }
        }
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
    }

    public void ExitCategory(StateCategory excludedCategory)
    {
        if (activeStates.TryGetValue(excludedCategory, out var state))
        {
            state.Exit();
            activeStates.Remove(excludedCategory);
        }
    }

    private void SetDefaultState()
    {
        if (defaultIdleState != null)
        {
            activeStates[StateCategory.idle] = defaultIdleState;
            defaultIdleState.Enter();
        }
    }

    public void Update()
    {
        if (activeStates.Count == 0)
            SetDefaultState();

        updateStates.Clear();
        updateStates.AddRange(activeStates.Values);

        for (int i = updateStates.Count - 1; i >= 0; i--)
        {
            updateStates[i]?.Update();
        }
    }
}
