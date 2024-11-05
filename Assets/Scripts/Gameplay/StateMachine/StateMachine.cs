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

    public void AddState(IState state)
    {
        states[state.GetType()] = state;
    }

    public void SetStates(params Type[] desiredStates)
    {
        foreach (var stateType in desiredStates)
        {
            if (states.TryGetValue(stateType, out var state))
            {
                var category = state.Category;

                // If a state of this category is active, replace it
                if (activeStates.TryGetValue(category, out var activeState) && activeState != state)
                {
                    activeState.Exit();
                }

                // Activate new state if not already active
                if (!activeStates.ContainsKey(category) || activeStates[category] != state)
                {
                    activeStates[category] = state;
                    state.Enter();
                    OnChangedState?.Invoke(category, state);
                }
            }
        }
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
