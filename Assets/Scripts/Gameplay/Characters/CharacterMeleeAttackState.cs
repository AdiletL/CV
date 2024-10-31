using UnityEngine;

namespace Character
{
    public class CharacterMeleeAttackState : CharacterBaseAttackState
    {
        public CharacterAnimation CharacterAnimation { get; set; }
        public AnimationClip AnimationClip { get; set; }

        protected float countTimeApplyDamage, countTimeAnimation;

        protected bool isApplyDamage;
        
        public override void Enter()
        {
            base.Enter();
            this.CharacterAnimation?.ChangeAnimation(AnimationClip);
        }

        public override void Update()
        {
            base.Update();

            var timeAnimation = AnimationClip.length;
            var timeApplyDamage = AnimationClip.length / 2;
            
            countTimeAnimation += Time.deltaTime;
            if (countTimeAnimation < timeAnimation)
            {
                countTimeApplyDamage += Time.deltaTime;
                if (countTimeApplyDamage >= timeApplyDamage && !isApplyDamage)
                {
                    ApplyDamage();
                    isApplyDamage = true;
                }
            }
            else
            {
                countTimeApplyDamage = 0;
                countTimeAnimation = 0;
                isApplyDamage = false;
            }
        }
        
        public void SetTarget(GameObject target)
        {
            this.target = target;
            Debug.Log(target);
        }
        
        public override void ApplyDamage()
        {
            var health = target.GetComponent<IHealth>();
            health?.TakeDamage(Damageble);
            Debug.Log(health);
        }
    }

    public class CharacterMeleeAttackStateBuilder : CharacterBaseAttackStateBuilder
    {
        public CharacterMeleeAttackStateBuilder(CharacterBaseAttackState instance) : base(instance)
        {
        }

        public CharacterMeleeAttackStateBuilder SetAnimationClip(AnimationClip animationClip)
        {
            if (state is CharacterMeleeAttackState characterMeleeAttack)
            {
                characterMeleeAttack.AnimationClip = animationClip;
            }

            return this;
        }
        
        
        public CharacterMeleeAttackStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if (state is CharacterMeleeAttackState characterMeleeAttack)
            {
                characterMeleeAttack.CharacterAnimation = characterAnimation;
            }

            return this;
        }
    }
}