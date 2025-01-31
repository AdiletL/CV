using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderDefaultAttackState : CreepDefaultAttackState
    {
        private BeholderSwitchMoveState beholderSwitchMoveState;

        public override void Initialize()
        {
            base.Initialize();
            beholderSwitchMoveState = (BeholderSwitchMoveState)characterSwitchMoveState;
        }

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
                beholderSwitchMoveState.SetTarget(currentTarget);
                beholderSwitchMoveState.ExitCategory(Category);
            }
        }
    }
    
    public class BeholderDefaultAttackStateBuilder : CharacterDefaultAttackStateBuilder
    {
        public BeholderDefaultAttackStateBuilder() : base(new BeholderDefaultAttackState())
        {
        }
    }
}