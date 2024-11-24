using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class CreepSwitchMoveState : CharacterSwitchMoveState
    {
        private SO_CreepMove soCreepMove;
        private GameObject currentTarget;
        
        public Transform Center { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            soCreepMove = (SO_CreepMove)this.SO_CharacterMove;
        }
        
        
        protected override void DestermineState()
        {
            
        }
    }

    public class CreepSwitchMoveStateBuilder : CharacterMoveStateBuilder
    {
        public CreepSwitchMoveStateBuilder(CharacterSwitchMoveState instance) : base(instance)
        {
        }

        public CreepSwitchMoveStateBuilder SetCenter(Transform center)
        {
            if(state is CreepSwitchMoveState creepSwitchMoveState)
                creepSwitchMoveState.Center = center;
            return this;
        }
    }
}