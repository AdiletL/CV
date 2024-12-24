namespace Unit.Trap.Tower.Dragon
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