using Unit;

namespace Unit
{
    
    public abstract class UnitStates : IState
    {
        public UnitStates()
        {
        }
    }
    
    public abstract class UnitLevelStates : UnitStates
    {
        public int Level { get; private set; }
        public int Experience { get; private set; }

        public UnitLevelStates(int level, int experience) : base()
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

        public UnitStateHealth(int health, int MaxHealth, float RegenerationRate) : base()
        {
            this.Health = health;
            this.MaxHealth = MaxHealth;
            this.RegenerationRate = RegenerationRate;
        }
    }
}