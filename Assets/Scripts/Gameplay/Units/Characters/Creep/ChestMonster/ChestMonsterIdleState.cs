namespace Unit.Character.Creep
{
    public class ChestMonsterIdleState : CreepIdleState
    {
        
    }

    public class ChestMonsterIdleStateBuilder : CreepIdleStateBuilder
    {
        public ChestMonsterIdleStateBuilder() : base(new ChestMonsterIdleState())
        {
        }
    }
}