namespace Machine
{
    public abstract class State : IState
    {
        public abstract StateCategory Category { get; }
        public StateMachine stateMachine { get; protected set; }
        public bool IsActive { get; private set; }
        public bool IsCanExit { get; protected set; } = true;
        public bool IsInitialized { get; protected set; }
        
        public void SetStateMachine(StateMachine stateMachine) => this.stateMachine = stateMachine;

        public virtual void Initialize()
        {
            IsInitialized = true;
        }
        public virtual void Enter()
        {
            IsActive = true;
            Subscribe();
        }

        public virtual void Subscribe()
        {
            stateMachine.OnUpdate += Update;
        }

        public abstract void Update();
        
        public virtual void Unsubscribe()
        {
            stateMachine.OnUpdate -= Update;
        }

        public virtual void Exit()
        {
            IsActive = false;
            Unsubscribe();
        }
    }

    public abstract class StateBuilder<T> where T : State
    {
        protected T state;

        public StateBuilder(T instance)
        {
            state = instance;
        }

        public StateBuilder<T> SetStateMachine(StateMachine stateMachine)
        {
            state.SetStateMachine(stateMachine);
            return this;
        }

        public T Build()
        {
            return state;
        }
    }
}
