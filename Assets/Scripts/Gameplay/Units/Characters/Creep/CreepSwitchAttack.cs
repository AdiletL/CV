namespace Unit.Character.Creep
{
    public class CreepSwitchAttack : CharacterSwitchAttack
    {
        public override bool IsFindUnitInRange()
        {
            return Calculate.Attack.IsFindUnitInRange<ICreepAttackable>(Center.position, RangeAttack, EnemyLayer, ref findUnitColliders);
        }
    }

    public class CreepSwitchSwitchAttackBuilder : CharacterSwitchAttackBuilder
    {
        public CreepSwitchSwitchAttackBuilder(CharacterSwitchAttack instance) : base(instance)
        {
        }
    }
}