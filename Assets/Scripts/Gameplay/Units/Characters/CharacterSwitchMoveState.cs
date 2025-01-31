using Machine;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterSwitchMoveState : CharacterSwitchState
    {
        protected GameObject currentTarget;
        protected SO_CharacterMove so_CharacterMove;
        
        protected CharacterSwitchAttackState characterSwitchState;
        protected UnitEndurance unitEndurance;
        
        public float RotationSpeed { get; protected set; }
        
        
        public void SetConfig(SO_CharacterMove config) => so_CharacterMove = config;
        public void SetSwitchAttackState(CharacterSwitchAttackState switchState) => characterSwitchState = switchState;
        public void SetUnitEndurance(UnitEndurance unitEndurance) => this.unitEndurance = unitEndurance;
        public void SetRotationSpeed(float rotationSpeed) => this.RotationSpeed = rotationSpeed;
        

        public override void Initialize()
        {
            
        }
        public override void SetState()
        {
            
        }

        public override void ExitOtherStates()
        {
        }

        public override void ExitCategory(StateCategory category)
        {
        }

        public virtual void SetTarget(GameObject target)
        {
            currentTarget = target;
        }
    }

    public class CharacterSwitchMoveStateBuilder : CharacterSwitchStateBuilder
    {
        public CharacterSwitchMoveStateBuilder(UnitSwitchState instance) : base(instance)
        {
        }

        public CharacterSwitchMoveStateBuilder SetConfig(SO_CharacterMove config)
        {
            if(switchState is CharacterSwitchMoveState characterSwitchMoveState)
                characterSwitchMoveState.SetConfig(config);
            return this;
        }

        public CharacterSwitchMoveStateBuilder SetEndurance(UnitEndurance unitEndurance)
        {
            if(switchState is CharacterSwitchMoveState characterSwitchMoveState)
                characterSwitchMoveState.SetUnitEndurance(unitEndurance);
            return this;
        }
        
        public CharacterSwitchMoveStateBuilder SetRotationSpeed(float rotationSpeed)
        {
            if(switchState is CharacterSwitchMoveState characterSwitchMoveState)
                characterSwitchMoveState.SetRotationSpeed(rotationSpeed);
            return this;
        }
    }
}