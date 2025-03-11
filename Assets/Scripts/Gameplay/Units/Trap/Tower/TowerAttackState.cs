using Cysharp.Threading.Tasks;
using Gameplay.Manager;
using Gameplay.Weapon.Projectile;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit.Trap.Tower
{
    public class TowerAttackState : UnitAttackState
    {
        [Inject] private DiContainer diContainer;
        [Inject] private PoolManager pool;
        
        public Stat DamageStat { get; private set; } = new Stat();
        
        protected float durationAttack, countDurationAttack;
        protected float timerFire, countTimerFire;
        protected float cooldown, countCooldown;

        protected bool isAttack;
        
        public TowerAnimation TowerAnimation { get; set; }
        public AnimationClip AttackClip { get; set; }
        public Transform PointSpawnProjectile { get; set; }
        

        public override void Initialize()
        {
            base.Initialize();
            
            var duration = Calculate.Attack.TotalDurationInSecond(AttackSpeedStat.CurrentValue);
            timerFire = duration * .55f;
            cooldown = duration;
        }
        public override void Enter()
        {
            base.Enter();
            this.TowerAnimation.ChangeAnimationWithDuration(AttackClip, cooldown);
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
        protected virtual async UniTask Fire()
        {
            var newGameObject = await this.pool.GetObjectAsync<SphereController>();
            newGameObject.transform.position = PointSpawnProjectile.position;
            newGameObject.transform.rotation = PointSpawnProjectile.rotation;
            var projectile = newGameObject.GetComponent<SphereController>();
            projectile.SetDamageable(DamageData);
            projectile.Initialize();
        }
        
        public override void ApplyDamage()
        {
            
        }

        protected virtual void Cooldown()
        {
            if (isAttack) return;
            countCooldown += Time.deltaTime;

            if (countCooldown > cooldown)
            {
                isAttack = true;
                countCooldown = 0;
            }
        }
    }

    public class TowerAttackStateBuilder : UnitAttackStateBuilder
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