using UnityEngine;

namespace Character.Enemy
{
    public class EnemyWalkState : CharacterBaseMovementState
    {
        private GameObject currentTarget;

        public override void Update()
        {
            Move();
        }

        public void SetTarget(GameObject target)
        {
            currentTarget = target;
        }

        public override void Move()
        {
            GameObject.transform.position = Vector3.MoveTowards(GameObject.transform.position, currentTarget.transform.position, MovementSpeed * Time.deltaTime);
        }
    }
    
    public class EnemyWalkStateBuilder : CharacterBaseMovementStateBuilder
    {
        public EnemyWalkStateBuilder(EnemyWalkState instance) : base(instance)
        {
        }
    }
}