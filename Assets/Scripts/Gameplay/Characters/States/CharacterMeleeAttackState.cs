using UnityEngine;

namespace Character
{
    public class CharacterMeleeAttackState : CharacterBaseAttackState
    {
        public CharacterAnimation CharacterAnimation { get; set; }
        public AnimationClip AnimationClip { get; set; }

        private CharacterAttackState characterAttackState;

        protected float countTimeApplyDamage, countTimeAnimation;

        protected bool isApplyDamage;

        public override void Initialize()
        {
            base.Initialize();
            characterAttackState = this.StateMachine.GetState<CharacterAttackState>();
        }

        public override void Enter()
        {
            base.Enter();
            this.CharacterAnimation?.ChangeAnimation(AnimationClip);
        }

        public override void Update()
        {
            base.Update();
            
            if(!currentTarget) 
                currentTarget = characterAttackState.CheckForwardEnemy();
            
            if(!currentTarget)
                this.StateMachine.ExitCategory(Category);
                    
            var timeAnimation = AnimationClip.length;
            var timeApplyDamage = timeAnimation * .8f;

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
            this.currentTarget = target;
        }
        
        public override void ApplyDamage()
        {
            var enemyGameObject = characterAttackState.CheckForwardEnemy();
            if (enemyGameObject)
            {
                currentTarget = enemyGameObject;
                currentTarget.GetComponent<IHealth>()?.TakeDamage(Damageble);
            }
            else
            {
                currentTarget = null;
            }
        }

        public override void Exit()
        {
            base.Exit();
            currentTarget = null;
        }
    }

    public class CharacterMeleeAttackBuilder : CharacterBaseAttackStateBuilder
    {
        public CharacterMeleeAttackBuilder(CharacterMeleeAttackState instance) : base(instance)
        {
        }

        public CharacterMeleeAttackBuilder SetAnimationClip(AnimationClip animationClip)
        {
            if (state is CharacterMeleeAttackState characterMeleeAttack)
            {
                characterMeleeAttack.AnimationClip = animationClip;
            }

            return this;
        }
        
        
        public CharacterMeleeAttackBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if (state is CharacterMeleeAttackState characterMeleeAttack)
            {
                characterMeleeAttack.CharacterAnimation = characterAnimation;
            }

            return this;
        }
    }
}