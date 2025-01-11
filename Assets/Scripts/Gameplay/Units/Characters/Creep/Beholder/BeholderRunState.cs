namespace Unit.Character.Creep
{
    public class BeholderRunState : CreepRunState
    {
        private BeholderSwitchAttackState beholderSwitchAttackState;
        
        
    }
    
    public class BeholderRunStateBuilder : CreepRunStateBuilder
    {
        public BeholderRunStateBuilder() : base(new BeholderRunState())
        {
        }
    }
}