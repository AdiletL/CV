namespace Character
{
    public enum StateType
    {
        nothing,
        health,
        movement,
        attack,
        level
    }

    public interface IState
    {
        public StateType StateType { get; }
    }
}
