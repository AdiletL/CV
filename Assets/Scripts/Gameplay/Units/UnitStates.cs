using Unit;

namespace Unit
{
    
    public abstract class UnitStates : IState
    {
        public StateType StateType { get; set; }

        public UnitStates(StateType stateType)
        {
            this.StateType = stateType;
        }
    }
    
    public abstract class UnitLevelStates : UnitStates
    {
        public int Level { get; private set; }
        public int Experience { get; private set; }

        public UnitLevelStates(StateType stateType, int level, int experience) : base(stateType)
        {
            this.Level = level;
            this.Experience = experience;
        }
    }
    
    public abstract class UnitStateHealth : UnitStates
    {
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        
        public float RegenerationRate { get; private set; }

        public UnitStateHealth(StateType stateType, int health, int MaxHealth, float RegenerationRate) : base(stateType)
        {
            this.Health = health;
            this.MaxHealth = MaxHealth;
            this.RegenerationRate = RegenerationRate;
        }
    }
}