namespace Character.Enemy
{
    public class HedgehogIdleState : EnemyIdleState
    {
        
    }

    public class HedgehogIdleStateBuilder : EnemyIdleStateBuilder
    {
        public HedgehogIdleStateBuilder() : base(new EnemyIdleState())
        {
        }
    }
}