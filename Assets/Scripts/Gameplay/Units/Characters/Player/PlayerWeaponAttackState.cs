using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerWeaponAttackState : CharacterWeaponAttackState
    {
        public override void Enter()
        {
            base.Enter();
            if(currentTarget.TryGetComponent(out UnitRenderer unitRenderer))
                unitRenderer.SetColor(Color.green);
        }

        public override void Exit()
        {
            ClearColorAtTarget();
            base.Exit();
        }

        private void ClearColorAtTarget()
        {
            if (currentTarget)
            {
                if(currentTarget.TryGetComponent(out UnitRenderer unitRenderer))
                    unitRenderer.ResetColor();
            }
        }
        public override void SetTarget(GameObject target)
        {
            ClearColorAtTarget();
            
            base.SetTarget(target);
            
            if(currentTarget.TryGetComponent(out UnitRenderer unitRenderer))
                unitRenderer.SetColor(Color.green);
        }
    }

    public class PlayerWeaponAttackStateStateBuilder : CharacterWeaponAttackStateBuilder
    {
        public PlayerWeaponAttackStateStateBuilder() : base(new PlayerWeaponAttackState())
        {
        }
    }
}