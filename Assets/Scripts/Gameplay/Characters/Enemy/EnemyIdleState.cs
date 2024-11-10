using UnityEngine;

namespace Character.Enemy
{
    public class EnemyIdleState : CharacterIdleState
    {
        protected virtual int checkEnemyLayer { get; }

        public GameObject GameObject { get; set; }
        

        public override void Enter()
        {
            base.Enter();
            this.CharacterAnimation?.ChangeAnimation(IdleClip);
        }

        public override void Update()
        {
            base.Update();
            var enemyGameObject = Calculate.Attack.CheckForwardEnemy(GameObject, checkEnemyLayer);
            if (!enemyGameObject)
            {
                this.StateMachine.ExitCategory(Category);
                this.StateMachine.SetStates(typeof(EnemyMoveState));
            }
        }
    }

    public class EnemyIdleStateBuilder : CharacterIdleStateBuilder
    {
        public EnemyIdleStateBuilder(CharacterIdleState instance) : base(instance)
        {
        }

        public EnemyIdleStateBuilder SetGameObject(GameObject gameObject)
        {
            if (state is EnemyIdleState enemyIdleState)
            {
                enemyIdleState.GameObject = gameObject;
            }

            return this;
        }
    }
}