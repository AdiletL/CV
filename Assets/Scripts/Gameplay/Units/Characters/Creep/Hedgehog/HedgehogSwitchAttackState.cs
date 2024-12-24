using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class HedgehogSwitchAttackState : CreepSwitchAttackState
    {
        private SO_HedgehogAttack so_HedgehogAttack;

        private Collider[] findUnitColliders;
        private float rangeAttack;
        
        public override bool IsFindUnitInRange()
        {
            return Calculate.Attack.IsFindUnitInRange(Center.position, rangeAttack, EnemyLayer, findUnitColliders);
        }


        public override void Initialize()
        {
            base.Initialize();
            so_HedgehogAttack = (SO_HedgehogAttack)so_CharacterAttack;
        }
    }

    public class HedgehogSwitchSwitchAttackStateBuilder : CreepSwitchSwitchAttackStateBuilder
    {
        public HedgehogSwitchSwitchAttackStateBuilder() : base(new HedgehogSwitchAttackState())
        {
        }
    }
}