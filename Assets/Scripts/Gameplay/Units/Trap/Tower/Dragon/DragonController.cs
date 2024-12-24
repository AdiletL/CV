using System;
using Zenject;

namespace Unit.Trap.Tower.Dragon
{
    public class DragonController : TowerController
    {
        [Inject] private DiContainer diContainer;

        public override T GetComponentInUnit<T>()
        {
            return GetComponent<T>();
        }

        public override void Initialize()
        {
            base.Initialize();
            
            stateMachine.SetStates(typeof(DragonIdleState));
        }

        protected override void CreateState()
        {
            var idle = (DragonIdleState)new DragonIdleStateBuilder()
                .SetPointSpawnProjectile(pointSpawnProjectile)
                .SetConfig(so_TowerAttack)
                .SetCenter(GetComponent<UnitCenter>().Center)
                .SetGameObject(gameObject)
                .SetStateMachine(this.stateMachine)
                .Build();
            
            diContainer.Inject(idle);
            
            this.stateMachine.AddStates(idle);
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