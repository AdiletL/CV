namespace Character.Enemy
{
    public class HedgehogAttackState : EnemyAttackState
    {
        
    }

    public class HedgehogAttackStateBuilder : EnemyAttackStateBuilder
    {
        public HedgehogAttackStateBuilder() : base(new HedgehogAttackState())
        {
        }
    }
}