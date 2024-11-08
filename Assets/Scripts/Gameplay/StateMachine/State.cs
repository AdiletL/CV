namespace Machine
{
    public abstract class State : IState
    {
        public virtual StateCategory Category { get; }
        public StateMachine StateMachine;

        public abstract void Initialize();
        public abstract void Enter();

        public abstract void Update();

        public abstract void Exit();
    }

    public abstract class StateBuilder<T> where T : State, new()
    {
        protected T state;

        public StateBuilder(T instance)
        {
            state = instance;
        }

        public StateBuilder<T> SetStateMachine(StateMachine stateMachine)
        {
            state.StateMachine = stateMachine;
            return this;
        }

        public T Build()
        {
            return state;
        }
    }
}
