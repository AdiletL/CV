using System;
using System.Collections.Generic;
using Gameplay.Weapon;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerWeaponAttackState : CharacterWeaponAttackState
    {
        public AnimationClip[] SwordAttackClip { get; set; }
        public AnimationClip SwordCooldownClip { get; set; }
        public AnimationClip[] BowAttackClip { get; set; }
        public AnimationClip BowCooldownClip { get; set; }
        
        public override void Enter()
        {
            base.Enter();
            if (currentTarget &&
                !Calculate.Distance.IsNearUsingSqr(gameObject.transform.position, currentTarget.transform.position,
                    rangeSqr))
            {
                stateMachine.ExitCategory(Category, null);
                return;
            }
                
            if(currentTarget.TryGetComponent(out UnitRenderer unitRenderer))
                unitRenderer.SetColor(Color.yellow);
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
                unitRenderer.SetColor(Color.yellow);
        }

        public override void SetWeapon(Weapon weapon)
        {
            base.SetWeapon(weapon);
            
            switch (weapon)
            {
                case Sword sword:
                    SetAnimationClip(SwordAttackClip, SwordCooldownClip);
                    break;
                case Bow bow:
                    SetAnimationClip(BowAttackClip, BowCooldownClip);
                    break;
            }
        }
    }

    public class PlayerWeaponAttackStateStateBuilder : CharacterWeaponAttackStateBuilder
    {
        public PlayerWeaponAttackStateStateBuilder() : base(new PlayerWeaponAttackState())
        {
        }

        public PlayerWeaponAttackStateStateBuilder SetSwordAttackClip(AnimationClip[] clips)
        {
            if(state is PlayerWeaponAttackState playerWeaponAttackState)
                playerWeaponAttackState.SwordAttackClip = clips;

            return this;
        }
        public PlayerWeaponAttackStateStateBuilder SetSwordCooldownClip(AnimationClip clip)
        {
            if(state is PlayerWeaponAttackState playerWeaponAttackState)
                playerWeaponAttackState.SwordCooldownClip = clip;

            return this;
        }
        public PlayerWeaponAttackStateStateBuilder SetBowAttackClip(AnimationClip[] clips)
        {
            if(state is PlayerWeaponAttackState playerWeaponAttackState)
                playerWeaponAttackState.BowAttackClip = clips;

            return this;
        }
        public PlayerWeaponAttackStateStateBuilder SetBowCooldownClip(AnimationClip clip)
        {
            if(state is PlayerWeaponAttackState playerWeaponAttackState)
                playerWeaponAttackState.BowCooldownClip = clip;

            return this;
        }
    }
}