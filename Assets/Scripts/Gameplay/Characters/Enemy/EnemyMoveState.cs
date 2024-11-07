using ScriptableObjects.Character.Enemy;
using UnityEngine;

namespace Character.Enemy
{
    public class EnemyMoveState : CharacterMoveState
    {
        private SO_EnemyMove so_EnemyMove;
        private GameObject currentTarget;

        public override void Initialize()
        {
            base.Initialize();
            so_EnemyMove = (SO_EnemyMove)this.SO_CharacterMove;
        }

        public void SetTarget(GameObject target)
        {
            currentTarget = target;
        }
        
        protected override void DestermineState()
        {
            
        }
    }

    public class EnemyMoveStateBuilder : CharacterMoveStateBuilder
    {
        public EnemyMoveStateBuilder(CharacterMoveState instance) : base(instance)
        {
        }
    }
}