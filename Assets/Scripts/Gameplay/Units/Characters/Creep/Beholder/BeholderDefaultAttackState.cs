using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderDefaultAttackState : CreepDefaultAttackState
    {
        public override void Update()
        {
            if(!currentTarget)
                FindUnit();
            
            if (currentTarget && 
                !Calculate.Distance.IsNearUsingSqr(gameObject.transform.position, currentTarget.transform.position,
                    rangeSqr))
            {
                characterSwitchMoveState.SetTarget(currentTarget);
                characterSwitchMoveState.ExitCategory(Category);
            }
            
            base.Update();
        }
    }
    
    public class BeholderDefaultAttackStateBuilder : CreepDefaultAttackStateBuilder
    {
        public BeholderDefaultAttackStateBuilder() : base(new BeholderDefaultAttackState())
        {
        }
    }
}