using UnityEngine;

namespace Character
{
    public class CharacterMeleeAttackState : CharacterBaseAttackState
    {
        private CharacterMoveState characterMoveState;
        
        protected virtual int checkEnemyLayer { get; }

        protected float durationAttack, countDurationAttack;
        protected float cooldown, countCooldown;

        protected bool isApplyDamage;
        
        public GameObject GameObject { get; set; }
        public CharacterAnimation CharacterAnimation { get; set; }
        public AnimationClip AttackClip { get; set; }
        public AnimationClip CooldownClip { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            characterMoveState = this.StateMachine.GetState<CharacterMoveState>();
            durationAttack = Calculate.Attack.TotalDurationAttack(AmountAttack);
            cooldown = durationAttack;
        }

        public override void Enter()
        {
            base.Enter();
            this.CharacterAnimation?.ChangeAnimation(AttackClip, duration: durationAttack);
            isApplyDamage = false;
            countDurationAttack = 0;
            countCooldown = 0;
        }

        public override void Update()
        {
            base.Update();

            if (!currentTarget)
            {
                currentTarget = Calculate.Attack.CheckForwardEnemy(this.GameObject, checkEnemyLayer);

                if (!currentTarget)
                {
                    this.StateMachine.ExitCategory(Category);
                    return;
                }
            }
            
            Calculate.Move.Rotate(this.GameObject.transform, currentTarget.transform, characterMoveState.RotationSpeed);
            if(!Calculate.Move.IsFacingTargetUsingDot(this.GameObject.transform, currentTarget.transform))
                return;

            if (!isApplyDamage)
            {
                countDurationAttack += Time.deltaTime;
                if (countDurationAttack > durationAttack)
                {
                    ApplyDamage();
                    countDurationAttack = 0;
                }
            }
            else
            {
                countCooldown += Time.deltaTime;
                if (countCooldown > cooldown)
                {
                    this.CharacterAnimation?.ChangeAnimation(AttackClip, duration: durationAttack);
                    isApplyDamage = false;
                    countCooldown = 0;
                }
            }
        }

        public void SetTarget(GameObject target)
        {
            this.currentTarget = target;
        }
        
        public override void ApplyDamage()
        {
            var enemyGameObject = Calculate.Attack.CheckForwardEnemy(this.GameObject, checkEnemyLayer, .7f);
            if (enemyGameObject)
            {
                currentTarget = enemyGameObject;
                currentTarget.GetComponent<IHealth>()?.TakeDamage(Damageble);
                this.CharacterAnimation?.ChangeAnimation(CooldownClip);
                isApplyDamage = true;
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

        public CharacterMeleeAttackBuilder SetAttackClip(AnimationClip animationClip)
        {
            if (state is CharacterMeleeAttackState characterMeleeAttack)
            {
                characterMeleeAttack.AttackClip = animationClip;
            }

            return this;
        }

        public CharacterMeleeAttackBuilder SetCooldowClip(AnimationClip animationClip)
        {
            if (state is CharacterMeleeAttackState characterMeleeAttack)
            {
                characterMeleeAttack.CooldownClip = animationClip;
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