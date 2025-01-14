using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderDefaultAttackState : CreepDefaultAttackState
    {
        private float distanceOffset = 1f;
        

        public override void LateUpdate()
        {
            base.LateUpdate();
            
            if(!currentTarget) return;
            
            if (!Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, currentTarget.transform.position,
                    rangeSqr + distanceOffset))
            {
                this.StateMachine.GetState<BeholderSwitchMoveState>().SetTarget(currentTarget);
                this.StateMachine.ExitCategory(Category, typeof(BeholderSwitchMoveState));
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