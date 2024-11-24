namespace Unit.Character.Creep
{
    public class ChestMonsterPatrolState : CreepPatrolState
    {
        protected override int checkEnemyLayer { get; } = Layers.PLAYER_LAYER;
    }
    
    public class ChestMonsterPatrolStateBuilder : CreepPatrolStateBuilder
    {
        public ChestMonsterPatrolStateBuilder() : base(new ChestMonsterPatrolState())
        {
        }
    }
}