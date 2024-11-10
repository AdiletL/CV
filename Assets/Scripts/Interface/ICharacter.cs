using Character;

public interface ICharacter
{
    public void Initialize();
}

public interface ICharacterController : ICharacter
{ 
    public void IncreaseStates(params IState[] stats);
    
    public void AddExperience(int experience);
    public void LevelUp();
}