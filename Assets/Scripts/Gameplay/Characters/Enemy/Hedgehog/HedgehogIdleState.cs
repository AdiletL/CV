using UnityEngine;

namespace Character.Enemy
{
    public class HedgehogIdleState : EnemyIdleState
    {
        protected override int checkEnemyLayer { get; } = Layers.PLAYER_LAYER;
    }

    public class HedgehogIdleStateBuilder : EnemyIdleStateBuilder
    {
        public HedgehogIdleStateBuilder() : base(new HedgehogIdleState())
        {
        }
    }
}