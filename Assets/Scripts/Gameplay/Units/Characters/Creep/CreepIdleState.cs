using UnityEngine;

namespace Unit.Character.Creep
{
    public class CreepIdleState : CharacterIdleState
    {
        protected virtual int checkEnemyLayer { get; }
        
        
        public override void Update()
        {
            base.Update();
            var enemyGameObject = Calculate.Attack.CheckForwardEnemy(GameObject, Center.position, checkEnemyLayer);
            if (!enemyGameObject)
            {
                this.StateMachine.ExitCategory(Category);
                this.StateMachine.SetStates(typeof(CreepSwitchMoveState));
            }
        }
    }

    public class CreepIdleStateBuilder : CharacterIdleStateBuilder
    {
        public CreepIdleStateBuilder(CharacterIdleState instance) : base(instance)
        {
        }
        
    }
}