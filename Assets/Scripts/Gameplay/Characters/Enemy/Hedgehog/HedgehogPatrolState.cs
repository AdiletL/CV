using Calculate;

namespace Character.Enemy
{
    public class HedgehogPatrolState : EnemyPatrolState
    {
        
        private PathToPoint pathToPoint;

        public override void Initialize()
        {
            base.Initialize();
            pathToPoint = new PathToPointBuilder()
                .SetPosition(this.GameObject.transform, EndPoint)
                .Build();
        }
        
        protected override void DestermineState()
        {
            if (EndPoint)
            {
                
            }
        }
    }

    public class HedgohogPatrolStateBuilder : EnemyPatrolStateBuilder
    {
        public HedgohogPatrolStateBuilder(CharacterPatrolState instance) : base(instance)
        {
        }
    }
}