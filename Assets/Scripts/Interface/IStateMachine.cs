namespace Machine
{
    public enum StateCategory
    {
        nothing,
        idle,
        attack,
        move,
        action,
        jump,
        defense,
    }

    public interface IState
    {
        public StateCategory Category { get; }
        public bool isActive { get; }
        public bool isCanExit { get; }

        public void Initialize();
        public void Enter();
        public void Subscribe();
        public void Update();
        public void LateUpdate();
        public void Unsubscribe();
        public void Exit();
    }
}