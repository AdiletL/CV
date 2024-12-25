
namespace Unit.Character
{
    public abstract class CharacterStates : UnitStates
    {
        public CharacterStates() : base()
        {
        }
    }
    
    public class CharacterHealthStates : UnitStateHealth
    {
        public CharacterHealthStates(int health, int MaxHealth, float RegenerationRate) : base(health, MaxHealth, RegenerationRate)
        {
        }
    }

    public class CharacterAttackStates : CharacterStates
    {
        public int Damage { get; private set; }
        public int AmountAttack { get; private set; }

        public CharacterAttackStates() : base()
        {
            
        }
    }
}