using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class CreepSwitchMove : CharacterSwitchMove
    {
        private SO_CreepMove so_CreepMove;
        
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
    }

    public class CreepSwitchSwitchMoveBuilder : CharacterSwitchMoveBuilder<CreepSwitchMove>
    {
        public CreepSwitchSwitchMoveBuilder(CharacterSwitchMove instance) : base(instance)
        {
        }

        public CreepSwitchSwitchMoveBuilder SetCenter(Transform center)
        {
            if(state is CreepSwitchMove creepSwitchMoveState)
                creepSwitchMoveState.Center = center;
            return this;
        }
    }
}