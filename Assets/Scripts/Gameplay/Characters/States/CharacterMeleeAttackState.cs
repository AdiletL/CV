using UnityEngine;

namespace Character
{
    public class CharacterMeleeAttackState : CharacterBaseAttackState
    {
        protected virtual int checkEnemyLayer { get; }

        protected float durationAttack, countDurationAttack;
        protected float timeApplyDamage, countTimeApplyDamage;
        protected float cooldown, countCooldown;

        protected bool isApplyDamage;
        
        public GameObject GameObject { get; set; }
        public CharacterAnimation CharacterAnimation { get; set; }
        public AnimationClip AnimationClip { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            durationAttack = Calculate.Attack.TotalDurationAttack(AmountAttack);
            timeApplyDamage = durationAttack * .8f;
            cooldown = durationAttack / 2;
        }

        public override void Enter()
        {
            base.Enter();
            this.CharacterAnimation?.ChangeAnimation(AnimationClip, duration: durationAttack);
            countDurationAttack = 0;
            countTimeApplyDamage = 0;
            countCooldown = 0;
        }

        public override void Update()
        {
            base.Update();
            
            if(!currentTarget) 
                currentTarget = Calculate.Attack.CheckForwardEnemy(this.GameObject, checkEnemyLayer);
            
            if(!currentTarget)
                this.StateMachine.ExitCategory(Category);
                    
            countDurationAttack += Time.deltaTime;
            if (countDurationAttack < durationAttack)
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
                countCooldown += Time.deltaTime;
                if (countCooldown > cooldown)
                {
                    countTimeApplyDamage = 0;
                    countDurationAttack = 0;
                    countCooldown = 0;
                    isApplyDamage = false;
                }
            }
        }

        public void SetTarget(GameObject target)
        {
            this.currentTarget = target;
        }
        
        public override void ApplyDamage()
        {
            var enemyGameObject = Calculate.Attack.CheckForwardEnemy(this.GameObject, checkEnemyLayer);
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

        public CharacterMeleeAttackBuilder SetGameObject(GameObject gameObject)
        {
            if (state is CharacterMeleeAttackState characterMeleeAttack)
            {
                characterMeleeAttack.GameObject = gameObject;
            }

            return this;
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

        public CharacterMeleeAttackBuilder SetAmountAttack(float amount)
        {
            if (state is CharacterMeleeAttackState characterMeleeAttack)
            {
                characterMeleeAttack.AmountAttack = amount;
            }

            return this;
        }
    }
}