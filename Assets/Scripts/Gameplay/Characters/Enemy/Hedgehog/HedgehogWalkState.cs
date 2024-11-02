using UnityEngine;

namespace Character.Enemy
{
    public class HedgehogWalkState : EnemyWalkState
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

    public class HedgehogWalkStateBuilder : EnemyWalkStateBuilder
    {
        public HedgehogWalkStateBuilder(EnemyWalkState instance) : base(instance)
        {
        }
    }
}