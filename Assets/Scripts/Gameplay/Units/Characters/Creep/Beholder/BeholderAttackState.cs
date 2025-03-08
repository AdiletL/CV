using UnityEngine;

namespace Gameplay.Unit.Character.Creep
{
    public class BeholderAttackState : CreepAttackState
    {
        public override void Update()
        {
            if(!currentTarget)
                FindUnit();
            
            if (currentTarget && 
                !Calculate.Distance.IsNearUsingSqr(gameObject.transform.position, currentTarget.transform.position,
                    RangeStat.CurrentValue * RangeStat.CurrentValue))
            {
                stateMachine.GetState<CreepMoveState>().SetTarget(currentTarget);
                stateMachine.ExitCategory(Category, typeof(CreepMoveState));
            }
            
            base.Update();
        }
    }
    
    public class BeholderAttackStateBuilder : CreepAttackStateBuilder
    {
        public BeholderAttackStateBuilder() : base(new BeholderAttackState())
        {
        }
    }
}