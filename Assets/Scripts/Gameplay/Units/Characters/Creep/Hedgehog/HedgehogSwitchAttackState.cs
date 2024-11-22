namespace Unit.Character.Creep
{
    public class HedgehogSwitchAttackState : CreepSwitchAttackState
    {
        
    }

    public class HedgehogSwitchSwitchAttackStateBuilder : CreepSwitchSwitchAttackStateBuilder
    {
        public HedgehogSwitchSwitchAttackStateBuilder() : base(new HedgehogSwitchAttackState())
        {
        }
    }
}