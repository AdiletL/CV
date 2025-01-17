namespace Machine
{
    public abstract class State : IState
    {
        public abstract StateCategory Category { get; }
        public StateMachine StateMachine { get; set; }
        public bool isActive { get; protected set; }
        
        public abstract void Initialize();
        public virtual void Enter()
        {
            isActive = true;
            StateMachine.OnUpdate += Update;
            StateMachine.OnLateUpdate += LateUpdate;
        }

        public abstract void Update();
        public abstract void LateUpdate();

        public virtual void Exit()
        {
            isActive = false;
            StateMachine.OnUpdate -= Update;
            StateMachine.OnLateUpdate -= LateUpdate;
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
