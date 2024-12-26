using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class CreepSwitchMoveState : CharacterSwitchMoveState
    {
        private SO_CreepMove so_CreepMove;
        private GameObject currentTarget;
        
        public Transform Center { get; set; }

        public virtual bool IsCanMovement()
        {
            throw new System.NotImplementedException();
        }


        public override void Initialize()
        {
            base.Initialize();
            so_CreepMove = (SO_CreepMove)this.SO_CharacterMove;
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