using UnityEngine;

namespace Unit.Character
{
    public class CharacterMeleeAttackState : CharacterBaseAttackState
    {
        private CharacterSwitchMoveState characterSwitchMoveState;
        

        protected float durationAttack, countDurationAttack;
        protected float applyDamage, countApplyDamage;
        protected float cooldown, countCooldown;

        protected bool isApplyDamage;
        protected bool isAttack;
        
        public GameObject GameObject { get; set; }
        public Transform Center { get; set; }
        public CharacterAnimation CharacterAnimation { get; set; }
        public AnimationClip[] AttackClips { get; set; }
        public AnimationClip CooldownClip { get; set; }
        public float RangeAttack { get; set; }
        public int EnemyLayer { get; set; }
        

        protected AnimationClip getRandomAnimationClip()
        {
            return AttackClips[Random.Range(0, AttackClips.Length)];
        }
        
        protected bool isCheckDistanceToTarget()
        {
            float distanceToTarget = Vector3.Distance(this.GameObject.transform.position, this.currentTarget.transform.position);
            if (distanceToTarget > RangeAttack + .15f)
            {
                return false;
            }

            return true;
        }


        public override void Initialize()
        {
            base.Initialize();
            characterSwitchMoveState = this.StateMachine.GetState<CharacterSwitchMoveState>();
            durationAttack = Calculate.Attack.TotalDurationAttack(AmountAttack);
            applyDamage = durationAttack * .6f;
            cooldown = durationAttack;
        }

        public override void Enter()
        {
            base.Enter();

            FindTarget();
            if (!currentTarget)
            {
                this.StateMachine.ExitCategory(Category);
                return;
            }
            
            CharacterAnimation?.ChangeAnimation(null, isDefault: true);
            isAttack = false;
            isApplyDamage = false;
            countDurationAttack = 0;
            countApplyDamage = 0;
            countCooldown = 0;
        }

        public override void Update()
        {
            base.Update();

            if (!currentTarget)
            {
                FindTarget();
                if (!currentTarget)
                {
                    this.StateMachine.ExitCategory(Category);
                    return;
                }
            }

            if(!isCheckDistanceToTarget())
            {
                this.StateMachine.ExitCategory(Category);
                return;
            }

            if (!Calculate.Move.IsFacingTargetUsingAngle(this.GameObject.transform, currentTarget.transform))
            {
                Calculate.Move.Rotate(this.GameObject.transform, currentTarget.transform, characterSwitchMoveState.RotationSpeed, ignoreX: true, ignoreY: false, ignoreZ: true);
            }
            else
            {
                isAttack = true;
            }

            if (!isAttack)
            {
                CharacterAnimation?.ChangeAnimation(null, isDefault: true);
                return;
            }

            Cooldown();
            Attack();
        }
        
        public override void Exit()
        {
            base.Exit();
            currentTarget = null;
        }

        protected void FindTarget()
        {
            Collider[] hits = Physics.OverlapSphere(Center.position, RangeAttack, EnemyLayer);
            float closestDistanceSqr = RangeAttack;

            foreach (var hit in hits)
            {
                if(!hit.TryGetComponent(out IHealth health) || !health.IsLive) continue;
                
                if (hit.TryGetComponent(out UnitCenter unitCenter))
                {
                    float distanceToTarget = Vector3.Distance(this.GameObject.transform.position, hit.transform.position);
                    var direcitonToTarget = (unitCenter.Center.position - Center.position).normalized;
                    
                    if (Physics.Raycast(Center.position, direcitonToTarget, out var hit2, RangeAttack) 
                        && hit2.transform.gameObject == hit.gameObject
                        && distanceToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = distanceToTarget;
                        currentTarget = hit.transform.gameObject;
                    }
                }
            }
        }

        protected virtual void Attack()
        {
            if (!isApplyDamage) return;
            
            countApplyDamage += Time.deltaTime;
            if (countApplyDamage > applyDamage)
            {
                ApplyDamage();
                isApplyDamage = false;
                countApplyDamage = 0;
            }
            
        }
        
        public override void ApplyDamage()
        {
            if (currentTarget&& currentTarget.TryGetComponent(out IHealth health))
            {
                if (health.IsLive)
                    health.TakeDamage(Damageble, this.GameObject);
                else
                    currentTarget = null;
            }
            this.CharacterAnimation?.ChangeAnimation(CooldownClip);
            isAttack = false;
        }

        protected virtual void Cooldown()
        {
            if(isApplyDamage) return;
            countCooldown += Time.deltaTime;
            
            if (countCooldown > cooldown)
            {
                if (currentTarget &&
                    Calculate.Move.IsFacingTargetUsingAngle(this.GameObject.transform, currentTarget.transform))
                {
                    this.CharacterAnimation?.ChangeAnimation(getRandomAnimationClip(), duration: durationAttack);
                    isApplyDamage = true;
                }
                else
                {
                    isAttack = false;
                }

                countCooldown = 0;
            }
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

        public CharacterMeleeAttackBuilder SetEnemyLayer(int index)
        {
            if (state is CharacterMeleeAttackState characterMeleeAttack)
            {
                characterMeleeAttack.EnemyLayer = index;
            }

            return this;
        }
        
        public CharacterMeleeAttackBuilder SetCenter(Transform center)
        {
            if (state is CharacterMeleeAttackState characterMeleeAttack)
            {
                characterMeleeAttack.Center = center;
            }

            return this;
        }
    }
}