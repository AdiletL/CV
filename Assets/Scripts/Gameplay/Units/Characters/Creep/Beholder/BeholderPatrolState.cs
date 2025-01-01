namespace Unit.Character.Creep
{
    public class BeholderPatrolState : CreepPatrolState
    {
    }
    
    public class BeholderPatrolStateBuilder : CreepPatrolStateBuilder
    {
        public BeholderPatrolStateBuilder() : base(new BeholderPatrolState())
        {
        }
    }
}