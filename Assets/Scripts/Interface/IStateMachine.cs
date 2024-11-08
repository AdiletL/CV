namespace Machine
{
    public enum StateCategory
    {
        nothing,
        idle,
        attack,
        move,
        action,
    }

    public interface IState
    {
        public StateCategory Category { get; }
        public void Initialize();
        public void Enter();
        public void Update();
        public void Exit();
    }
}