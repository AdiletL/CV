using UnityEngine;

namespace Unit.Character
{
    public class CharacterMeleeAttackState : CharacterBaseAttackState
    {
        private CharacterSwitchMoveState characterSwitchMoveState;
        
        protected virtual int checkEnemyLayer { get; }

        protected float durationAttack, countDurationAttack;
        protected float applyDamage, countApplyDamage;
        protected float cooldown, countCooldown;

        protected bool isApplyDamage;
        
        public GameObject GameObject { get; set; }
        public CharacterAnimation CharacterAnimation { get; set; }
        public AnimationClip[] AttackClips { get; set; }
        public AnimationClip CooldownClip { get; set; }
        public float RangeAttack { get; set; }
        

        protected AnimationClip getRandomAnimationClip()
        {
            return AttackClips[Random.Range(0, AttackClips.Length)];
        }

        protected bool isCheckTarget()
        {
            currentTarget = Calculate.Attack.CheckForwardEnemy(this.GameObject, checkEnemyLayer, RangeAttack);

            if (!currentTarget 
                || !currentTarget.TryGetComponent(out IHealth health)
                || !health.IsLive)
            {
                this.StateMachine.ExitCategory(Category);
                return false;
            }

            return true;
        }

        public override void Initialize()
        {
            base.Initialize();
            characterSwitchMoveState = this.StateMachine.GetState<CharacterSwitchMoveState>();
            durationAttack = Calculate.Attack.TotalDurationAttack(AmountAttack);
            applyDamage = durationAttack * .65f;
            cooldown = durationAttack;
        }

        public override void Enter()
        {
            base.Enter();

            if (!isCheckTarget())
            {
                this.StateMachine.ExitCategory(Category);
                return;
            }
            
            this.CharacterAnimation?.ChangeAnimation(getRandomAnimationClip(), duration: durationAttack);
            isApplyDamage = false;
            countDurationAttack = 0;
            countCooldown = 0;
        }

        public override void Update()
        {
            base.Update();

            var distanceToTarget = Vector3.Distance(this.GameObject.transform.position, this.currentTarget.transform.position);

            if (!currentTarget || distanceToTarget > RangeAttack)
            {
                this.StateMachine.ExitCategory(Category);
                return;
            }
            
            if (!Calculate.Move.IsFacingTargetUsingDot(this.GameObject.transform, currentTarget.transform))
            {
                Calculate.Move.Rotate(this.GameObject.transform, currentTarget.transform, characterSwitchMoveState.RotationSpeed);
                return;
            }

            if (!isApplyDamage)
            {
                countApplyDamage += Time.deltaTime;
                if (countApplyDamage > applyDamage)
                {
                    ApplyDamage();
                    countApplyDamage = 0;
                }
            }
            else
            {
                countCooldown += Time.deltaTime;
                if (countCooldown > cooldown)
                {
                    if (isCheckTarget())
                        this.CharacterAnimation?.ChangeAnimation(getRandomAnimationClip(), duration: durationAttack);
                    
                    isApplyDamage = false;
                    countCooldown = 0;
                }
            }
        }
        
        public override void ApplyDamage()
        {
            if (isCheckTarget())
            { 
                currentTarget.GetComponent<IHealth>()?.TakeDamage(Damageble, this.GameObject);
                this.CharacterAnimation?.ChangeAnimation(CooldownClip);
                isApplyDamage = true;
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

        public CharacterMeleeAttackBuilder SetAttackClip(AnimationClip[] animationClips)
        {
            if (state is CharacterMeleeAttackState characterMeleeAttack)
            {
                characterMeleeAttack.AttackClips = animationClips;
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

        public CharacterMeleeAttackBuilder SetRangeAttack(float range)
        {
            if (state is CharacterMeleeAttackState characterMeleeAttack)
            {
                characterMeleeAttack.RangeAttack = range;
            }

            return this;
        }
    }
}