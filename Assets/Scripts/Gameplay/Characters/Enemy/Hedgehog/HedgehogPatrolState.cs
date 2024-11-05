using Calculate;

namespace Character.Enemy
{
    public class HedgehogPatrolState : EnemyPatrolState
    {
        
    }

    public class HedgehogPatrolStateBuilder : EnemyPatrolStateBuilder
    {
        public HedgehogPatrolStateBuilder() : base(new HedgehogPatrolState())
        {
        }
    }
}