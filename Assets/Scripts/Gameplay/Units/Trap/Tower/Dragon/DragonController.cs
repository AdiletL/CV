using System;
using ScriptableObjects.Gameplay.Trap.Tower.Dragon;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit.Trap.Tower
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

        protected override void InitializeConfig()
        {
            base.InitializeConfig();
            so_Dragon = (SO_Dragon)so_Tower;
        }

        public override void Appear()
        {
            //stateMachine.SetStates(desiredStates: typeof(DragonIdleState));
        }

        public override void Disappear()
        {
            throw new NotImplementedException();
        }

        public override void Trigger()
        {
            this.stateMachine.ExitOtherStates(typeof(DragonIdleState));
        }

        public override void Reset()
        {
            this.stateMachine.ExitOtherStates(typeof(DragonDeactivateState));
        }
    }
}