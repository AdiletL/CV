namespace Unit.Character.Creep
{
    public class HedgehogSwitchAttackState : CreepSwitchAttackState
    {
        
    }

    public class HedgehogAttackStateBuilder : CreepAttackStateBuilder
    {
        public HedgehogAttackStateBuilder() : base(new HedgehogSwitchAttackState())
        {
        }
    }
}