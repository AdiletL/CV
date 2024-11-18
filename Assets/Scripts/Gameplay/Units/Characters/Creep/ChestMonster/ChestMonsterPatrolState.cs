namespace Unit.Character.Creep
{
    public class ChestMonsterPatrolState : CreepPatrolState
    {
        
    }
    
    public class ChestMonsterPatrolStateBuilder : CreepPatrolStateBuilder
    {
        public ChestMonsterPatrolStateBuilder() : base(new ChestMonsterPatrolState())
        {
        }
    }
}