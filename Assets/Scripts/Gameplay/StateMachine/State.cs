namespace Machine
{
    public abstract class State : IState
    {
        public abstract StateCategory Category { get; }
        public StateMachine StateMachine { get; set; }
        public bool isActive { get; private set; }
        
        public abstract void Initialize();
        public virtual void Enter()
        {
            isActive = true;
            Subscribe();
        }

        public virtual void Subscribe()
        {
            StateMachine.OnUpdate += Update;
            StateMachine.OnLateUpdate += LateUpdate;
        }

        public abstract void Update();
        public abstract void LateUpdate();
        
        public virtual void Unsubscribe()
        {
            StateMachine.OnUpdate -= Update;
            StateMachine.OnLateUpdate -= LateUpdate;
        }

        public virtual void Exit()
        {
            isActive = false;
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
            state.StateMachine = stateMachine;
            return this;
        }

        public T Build()
        {
            return state;
        }
    }
}
