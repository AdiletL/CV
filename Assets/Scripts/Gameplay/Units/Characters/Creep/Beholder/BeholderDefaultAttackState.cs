using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderDefaultAttackState : CreepDefaultAttackState
    {
        public override void Enter()
        {
            FindUnit();
            base.Enter();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
            
            if(!currentTarget) return;
            
            if (!isAttack && !Calculate.Distance.IsNearUsingSqr(gameObject.transform.position, currentTarget.transform.position,
                    rangeSqr))
            {
                characterSwitchMoveState.SetTarget(currentTarget);
                characterSwitchMoveState.ExitCategory(Category);
            }
        }
    }
    
    public class BeholderDefaultAttackStateBuilder : CreepDefaultAttackStateBuilder
    {
        public BeholderDefaultAttackStateBuilder() : base(new BeholderDefaultAttackState())
        {
        }
    }
}