using UnityEngine;

namespace Unit.Character.Creep
{
    public class CreepWalkState : CharacterBaseMovementState
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
    
    public class CreepWalkStateBuilder : CharacterBaseMovementStateBuilder
    {
        public CreepWalkStateBuilder(CreepWalkState instance) : base(instance)
        {
        }
    }
}