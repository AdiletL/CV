namespace Unit.Character.Creep
{
    public class BeholderPatrolState : CreepPatrolState
    {
        protected override int checkEnemyLayer { get; } = Layers.PLAYER_LAYER;
    }
    
    public class BeholderPatrolStateBuilder : CreepPatrolStateBuilder
    {
        public BeholderPatrolStateBuilder() : base(new BeholderPatrolState())
        {
        }
    }
}