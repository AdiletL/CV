using UnityEngine;

namespace Character.Enemy
{
    public class HedgehogWalkState : EnemyWalkState
    {
        
    }

    public class HedgehogWalkStateBuilder : EnemyWalkStateBuilder
    {
        public HedgehogWalkStateBuilder() : base(new HedgehogWalkState())
        {
        }
    }
}