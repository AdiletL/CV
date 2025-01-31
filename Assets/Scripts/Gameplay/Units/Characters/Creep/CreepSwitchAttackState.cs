namespace Unit.Character.Creep
{
    public class CreepSwitchAttackState : CharacterSwitchAttackState
    {
        public override bool IsFindUnitInRange()
        {
            return Calculate.Attack.IsFindUnitInRange<ICreepAttackable>(center.position, RangeAttack, enemyLayer, ref findUnitColliders);
        }
    }

    public class CreepSwitchAttackStateSwitchAttackStateBuilder : CharacterSwitchAttackStateBuilder
    {
        public CreepSwitchAttackStateSwitchAttackStateBuilder(CharacterSwitchAttackState instance) : base(instance)
        {
        }
    }
}