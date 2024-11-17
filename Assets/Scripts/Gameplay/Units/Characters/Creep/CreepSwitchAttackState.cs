namespace Unit.Character.Creep
{
    public class CreepSwitchAttackState : CharacterSwitchAttackState
    {
        
    }

    public class CreepAttackStateBuilder : CharacterAttackStateBuilder
    {
        public CreepAttackStateBuilder(CharacterSwitchAttackState instance) : base(instance)
        {
        }
    }
}