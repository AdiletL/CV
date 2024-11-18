using UnityEngine;

namespace Unit.Character.Creep
{
    public class HedgehogIdleState : CreepIdleState
    {
        protected override int checkEnemyLayer { get; } = Layers.PLAYER_LAYER;
    }

    public class HedgehogIdleStateBuilder : CreepIdleStateBuilder
    {
        public HedgehogIdleStateBuilder() : base(new HedgehogIdleState())
        {
        }
    }
}