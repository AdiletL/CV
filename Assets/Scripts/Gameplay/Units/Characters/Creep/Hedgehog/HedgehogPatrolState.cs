using Calculate;

namespace Unit.Character.Creep
{
    public class HedgehogPatrolState : CreepPatrolState
    {
        
    }

    public class HedgehogPatrolStateBuilder : CreepPatrolStateBuilder
    {
        public HedgehogPatrolStateBuilder() : base(new HedgehogPatrolState())
        {
        }
    }
}