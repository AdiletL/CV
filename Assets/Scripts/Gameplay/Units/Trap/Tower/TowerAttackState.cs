using Gameplay.Weapon.Projectile;
using UnityEngine;
using Zenject;

namespace Unit.Trap.Tower
{
    public class TowerAttackState : UnitBaseAttackState
    {
        private IPoolable pool;
        
        protected float durationAttack, countDurationAttack;
        protected float timerFire, countTimerFire;
        protected float cooldown, countCooldown;

        protected bool isAttack;
        
        public TowerAnimation TowerAnimation { get; set; }
        public AnimationClip AttackClip { get; set; }
        public GameObject GameObject { get; set; }
        public Transform Center { get; set; }
        public Transform PointSpawnProjectile { get; set; }
        

        [Inject]
        public void Construct(IPoolable pool)
        {
            this.pool = pool;
        }

        public override void Initialize()
        {
            var duration = Calculate.Attack.TotalDurationAttack(AmountAttack);
            timerFire = duration * .55f;
            cooldown = duration;
        }
        public override void Enter()
        {
            this.TowerAnimation.ChangeAnimation(AttackClip, cooldown);
        }
        public override void Exit()
        {
            
        }

        public override void Update()
        {
            Cooldown();
            Attack();
        }

        public override void LateUpdate()
        {
            
        }


        public override void Attack()
        {
            if (!isAttack) return;
            
            countTimerFire += Time.deltaTime;
            if (countTimerFire > timerFire)
            {
                Fire();
                isAttack = false;
                countTimerFire = 0;
            }
            
        }

        protected virtual void Fire()
        {
            var newGameObject = this.pool.GetObject<SphereController>();
            newGameObject.transform.position = PointSpawnProjectile.position;
            newGameObject.transform.rotation = PointSpawnProjectile.rotation;
            var projectile = newGameObject.GetComponent<SphereController>();
            projectile.Initialize();
            projectile.SetDamageable(Damageable);
        }
        
        public override void ApplyDamage()
        {
            
        }

        protected virtual void Cooldown()
        {
            if(isAttack) return;
            countCooldown += Time.deltaTime;
            
            if (countCooldown > cooldown)
            {
                isAttack = true;
                countCooldown = 0;
            }
        }

        public override void IncreaseStates(IState state)
        {
            
        }
        
    }

    public class TowerAttackStateBuilder : UnitBaseAttackStateBuilder
    {
        public TowerAttackStateBuilder() : base(new TowerAttackState())
        {
        }

        public TowerAttackStateBuilder SetPointSpawnProjectile(Transform pointSpawnProjectile)
        {
            if(state is TowerAttackState towerAttackState)
                towerAttackState.PointSpawnProjectile = pointSpawnProjectile;
            
            return this;
        }
        
        public TowerAttackStateBuilder SetTowerAnimation(TowerAnimation towerAnimation)
        {
            if(state is TowerAttackState towerAttackState)
                towerAttackState.TowerAnimation = towerAnimation;
            
            return this;
        }
        
        public TowerAttackStateBuilder SetAttackClip(AnimationClip animationClip)
        {
            if(state is TowerAttackState towerAttackState)
                towerAttackState.AttackClip = animationClip;
            
            return this;
        }
    }
}