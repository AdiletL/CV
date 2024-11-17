using UnityEngine;

namespace Unit.Character.Creep
{
    public class CreepIdleIdleState : CharacterIdleIdleState
    {
        protected virtual int checkEnemyLayer { get; }

        
        public override void Update()
        {
            base.Update();
            var enemyGameObject = Calculate.Attack.CheckForwardEnemy(GameObject, checkEnemyLayer);
            if (!enemyGameObject)
            {
                this.StateMachine.ExitCategory(Category);
                this.StateMachine.SetStates(typeof(CreepSwitchMoveState));
            }
        }
    }

    public class CreepIdleStateBuilder : CharacterIdleStateBuilder
    {
        public CreepIdleStateBuilder(CharacterIdleIdleState instance) : base(instance)
        {
        }

    }
}