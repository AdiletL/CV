using Unit;

namespace Character
{
    public abstract class CharacterStates : IState
    {
        public StateType StateType { get; set; }
    }
    
    public class CharacterHealthStates : CharacterStates
    {
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        
        public float RegenerationRate { get; private set; }

        public CharacterHealthStates(StateType stateType, int health, int MaxHealth, float RegenerationRate)
        {
            this.StateType = stateType;
            this.Health = health;
            this.MaxHealth = MaxHealth;
            this.RegenerationRate = RegenerationRate;
        }
    }

    public class CharacterAttackStates : CharacterStates
    {
        public int Damage { get; private set; }
        public int AmountAttack { get; private set; }
    }
}