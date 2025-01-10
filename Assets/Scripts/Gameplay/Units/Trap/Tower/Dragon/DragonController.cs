using System;
using ScriptableObjects.Gameplay.Trap.Tower.Dragon;
using UnityEngine;
using Zenject;

namespace Unit.Trap.Tower
{
    public class DragonController : TowerController
    {
        [Inject] private DiContainer diContainer;
        
        protected SO_Dragon so_Dragon;

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }

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

        protected override void InitializeConfig()
        {
            base.InitializeConfig();
            so_Dragon = (SO_Dragon)so_Tower;
        }
        
        public override void Appear()
        {
            stateMachine.SetStates(typeof(DragonIdleState));
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