using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderDefaultAttackState : CreepDefaultAttackState
    {
        private BeholderSwitchMove beholderSwitchMove;

        public override void Initialize()
        {
            base.Initialize();
            beholderSwitchMove = (BeholderSwitchMove)CharacterSwitchMove;
        }

        public override void Enter()
        {
            if(!currentTarget)
                FindUnit();
            
            base.Enter();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
            
            if(!currentTarget) return;
            
            if (!isAttack && !Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, currentTarget.transform.position,
                    rangeSqr))
            {
                beholderSwitchMove.SetTarget(currentTarget);
                beholderSwitchMove.ExitCategory(Category);
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