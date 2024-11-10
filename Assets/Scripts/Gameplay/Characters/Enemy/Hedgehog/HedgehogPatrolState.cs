using Calculate;

namespace Character.Enemy
{
    public class HedgehogPatrolState : EnemyPatrolState
    {
        protected override int checkEnemyLayer { get; } = Layers.PLAYER_LAYER;
    }

    public class HedgehogPatrolStateBuilder : EnemyPatrolStateBuilder
    {
        public HedgehogPatrolStateBuilder() : base(new HedgehogPatrolState())
        {
        }
    }
}