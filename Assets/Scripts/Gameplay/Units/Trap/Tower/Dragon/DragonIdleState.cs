namespace Unit.Trap.Tower
{
    public class DragonIdleState : TowerIdleState
    {
        
    }
    
    public class DragonIdleStateBuilder : TowerIdleStateBuilder
    {
        public DragonIdleStateBuilder() : base(new DragonIdleState())
        {
        }
    }
}