using Calculate;

namespace Unit.Character.Creep
{
    public class HedgehogPatrolState : CreepPatrolState
    {
        protected override int checkEnemyLayer { get; } = Layers.PLAYER_LAYER;
    }

    public class HedgehogPatrolStateBuilder : CreepPatrolStateBuilder
    {
        public HedgehogPatrolStateBuilder() : base(new HedgehogPatrolState())
        {
        }
    }
}