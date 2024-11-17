
namespace Unit.Character
{
    public abstract class CharacterStates : UnitStates
    {
        public CharacterStates(StateType stateType) : base(stateType)
        {
            this.StateType = stateType;
        }
    }
    
    public class CharacterHealthStates : UnitStateHealth
    {
        public CharacterHealthStates(StateType stateType, int health, int MaxHealth, float RegenerationRate) : base(stateType, health, MaxHealth, RegenerationRate)
        {
        }
    }

    public class CharacterAttackStates : CharacterStates
    {
        public int Damage { get; private set; }
        public int AmountAttack { get; private set; }

        public CharacterAttackStates(StateType stateType) : base(stateType)
        {
            
        }
    }
}