using UnityEngine;

namespace Character.Enemy
{
    public class HedgehogController : EnemyController
    {

        
        protected override void CreateStates()
        {
            base.CreateStates();

            var animation = components.GetComponentInGameObjects<HedgehogAnimation>();
            
            var moveState = (HedgehogMoveState)new HedgehogMoveStateBuilder()
                .SetStartPlatform(startPlatform)
                .SetEndPlatform(endPlatform)
                .SetCharacterAnimation(animation)
                .SetConfig(so_EnemyMove)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();
            
            this.StateMachine.AddStates(moveState);
            
        }

    }
}