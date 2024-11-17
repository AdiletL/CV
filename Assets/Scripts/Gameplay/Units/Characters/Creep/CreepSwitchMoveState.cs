using ScriptableObjects.Unit.Character.Enemy;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class CreepSwitchMoveState : CharacterSwitchMoveState
    {
        private SO_CreepMove soCreepMove;
        private GameObject currentTarget;

        public override void Initialize()
        {
            base.Initialize();
            soCreepMove = (SO_CreepMove)this.SO_CharacterMove;
        }

        public void SetTarget(GameObject target)
        {
            currentTarget = target;
        }
        
        protected override void DestermineState()
        {
            
        }
    }

    public class CreepMoveStateBuilder : CharacterMoveStateBuilder
    {
        public CreepMoveStateBuilder(CharacterSwitchMoveState instance) : base(instance)
        {
        }
    }
}