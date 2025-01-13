using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderDefaultAttackState : CreepDefaultAttackState
    {
        
        

        public override void LateUpdate()
        {
            if (!Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, currentTarget.transform.position,
                    rangeSqr))
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