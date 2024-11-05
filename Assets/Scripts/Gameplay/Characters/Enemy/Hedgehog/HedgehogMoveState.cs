namespace Character.Enemy
{
    public class HedgehogMoveState : EnemyMoveState
    {
        
    }

    public class HedgehogMoveStateBuilder : EnemyMoveStateBuilder
    {
        public HedgehogMoveStateBuilder() : base(new HedgehogMoveState())
        {
        }
    }
}