using Unit;

public interface ICharacter
{
    public void Initialize();
}

public interface ICharacterController : ICharacter
{
    public void IncreaseStates(params IState[] stats);
}