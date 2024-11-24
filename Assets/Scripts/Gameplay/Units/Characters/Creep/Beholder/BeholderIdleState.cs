namespace Unit.Character.Creep
{
    public class BeholderIdleState : CreepIdleState
    { 
        protected override int checkEnemyLayer { get; } = Layers.PLAYER_LAYER;
    }

    public class BeholderIdleStateBuilder : CreepIdleStateBuilder
    {
        public BeholderIdleStateBuilder() : base(new BeholderIdleState())
        {
        }
    }
}