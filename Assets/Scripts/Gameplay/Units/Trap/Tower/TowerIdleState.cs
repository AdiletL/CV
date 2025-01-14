using Gameplay.Damage;
using ScriptableObjects.Gameplay.Trap.Tower;
using UnityEngine;
using Zenject;

namespace Unit.Trap.Tower
{
    public class TowerIdleState : UnitIdleState
    {
        [Inject] private DiContainer diContainer; 
        
        private TowerAttackState attackState;
        
        public SO_Tower SO_Tower { get; set; }
        public Transform PointSpawnProjectile { get; set; }
        public TowerAnimation TowerAnimation { get; set; }
        public AnimationClip IdleClip { get; set; }
        

        protected virtual TowerAttackState CreateAttackState()
        {
            return (TowerAttackState)new TowerAttackStateBuilder()
                .SetTowerAnimation(TowerAnimation)
                .SetAttackClip(SO_Tower.AttackClip)
                .SetPointSpawnProjectile(PointSpawnProjectile)
                .SetAmountAttackInSecond(SO_Tower.AmountAttack)
                .SetDamageable(new NormalDamage(SO_Tower.Damage, this.GameObject))
                .SetStateMachine(this.StateMachine)
                .Build();
        }
        
        public override void Enter()
        {
            base.Enter();
            
            this.TowerAnimation.ChangeAnimationWithDuration(IdleClip);
            
            if (attackState == null)
            {
                attackState = CreateAttackState();
                diContainer.Inject(attackState);
                attackState.Initialize();
                
                this.StateMachine.AddStates(attackState);
            }
            
            this.StateMachine.ExitCategory(Category, typeof(TowerAttackState));
        }
        
    }
    
    public class TowerIdleStateBuilder : UnitIdleStateBuilder
    {
        public TowerIdleStateBuilder(UnitIdleState instance) : base(instance)
        {
        }

        public TowerIdleStateBuilder SetConfig(SO_Tower config)
        {
            if(state is TowerIdleState towerIdleState)
                towerIdleState.SO_Tower = config;

            return this;
        }
        
        public TowerIdleStateBuilder SetPointSpawnProjectile(Transform pointSpawnProjectile)
        {
            if(state is TowerIdleState towerIdleState)
                towerIdleState.PointSpawnProjectile = pointSpawnProjectile;

            return this;
        }
        
        public TowerIdleStateBuilder SetTowerAnimation(TowerAnimation towerAnimation)
        {
            if(state is TowerIdleState towerIdleState)
                towerIdleState.TowerAnimation = towerAnimation;

            return this;
        }
        
        public TowerIdleStateBuilder SetIdleClip(AnimationClip idleClip)
        {
            if(state is TowerIdleState towerIdleState)
                towerIdleState.IdleClip = idleClip;

            return this;
        }
    }
}