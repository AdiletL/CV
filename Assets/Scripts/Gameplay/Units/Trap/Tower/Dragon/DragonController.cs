using System;
using ScriptableObjects.Gameplay.Trap.Tower.Dragon;
using Zenject;

namespace Unit.Trap.Tower
{
    public class DragonController : TowerController
    {
        [Inject] private DiContainer diContainer;
        
        protected SO_Dragon so_Dragon;

        protected override void CreateState()
        {
            var idle = (DragonIdleState)new DragonIdleStateBuilder()
                .SetTowerAnimation(components.GetComponentFromArray<DragonAnimation>())
                .SetIdleClip(so_Dragon.IdleClip)
                .SetPointSpawnProjectile(pointSpawnProjectile)
                .SetConfig(so_Tower)
                .SetCenter(GetComponent<UnitCenter>().Center)
                .SetGameObject(gameObject)
                .SetStateMachine(this.stateMachine)
                .Build();
            
            diContainer.Inject(idle);
            
            this.stateMachine.AddStates(idle);
        }
        
        public override void Initialize()
        {
            base.Initialize();
            
            stateMachine.SetStates(typeof(DragonIdleState));
        }

        protected override void InitializeConfig()
        {
            base.InitializeConfig();
            so_Dragon = (SO_Dragon)so_Tower;
        }
        
        public override void Appear()
        {
            
        }
        
        public override void Activate()
        {
            this.stateMachine.ExitOtherStates(typeof(DragonIdleState));
        }

        public override void Deactivate()
        {
            this.stateMachine.ExitOtherStates(typeof(DragonDeactivateState));
        }
    }
}