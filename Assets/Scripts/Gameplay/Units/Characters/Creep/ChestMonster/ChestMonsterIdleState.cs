namespace Unit.Character.Creep
{
    public class ChestMonsterIdleState : CreepIdleState
    {
        protected override int checkEnemyLayer { get; } = Layers.PLAYER_LAYER;
    }

    public class ChestMonsterIdleStateBuilder : CreepIdleStateBuilder
    {
        public ChestMonsterIdleStateBuilder() : base(new ChestMonsterIdleState())
        {
        }
    }
}