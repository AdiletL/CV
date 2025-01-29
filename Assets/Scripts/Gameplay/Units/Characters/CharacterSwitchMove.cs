using Machine;
using Photon.Pun;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterSwitchMove : ISwitchState
    {
        protected GameObject currentTarget;
        
        public PhotonView PhotonView { get; set; }
        public StateMachine StateMachine { get; set; }
        public SO_CharacterMove SO_CharacterMove { get; set; }
        public CharacterAnimation CharacterAnimation { get; set; }
        public GameObject GameObject  { get; set; }
        public float RotationSpeed { get; set; }
        public ISwitchState CharacterSwitchAttack { get; protected set; }

        
        public virtual void Initialize()
        {
            
        }
        public virtual void SetState()
        {
            
        }

        public virtual void ExitOtherStates()
        {
        }

        public virtual void ExitCategory(StateCategory category)
        {
        }
        
        public virtual void SetTarget(GameObject target)
        {
            currentTarget = target;
        }

        public void SetSwitchAttack(ISwitchState attackISwitchState)
        {
            CharacterSwitchAttack = (CharacterSwitchAttack)attackISwitchState;
        }
    }

    public class CharacterSwitchMoveBuilder<T> where T : CharacterSwitchMove
    {
        protected CharacterSwitchMove state;

        public CharacterSwitchMoveBuilder(CharacterSwitchMove instance)
        {
            state = instance;
        }

        public CharacterSwitchMoveBuilder<T> SetGameObject(GameObject gameObject)
        {
            state.GameObject = gameObject;
            return this;
        }

        public CharacterSwitchMoveBuilder<T> SetConfig(SO_CharacterMove config)
        {
            state.SO_CharacterMove = config;
            return this;
        }

        public CharacterSwitchMoveBuilder<T> SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            state.CharacterAnimation = characterAnimation;
            return this;
        }
        public CharacterSwitchMoveBuilder<T> SetRotationSpeed(float speed)
        {
            state.RotationSpeed = speed;
            return this;
        }
        public CharacterSwitchMoveBuilder<T> SetPhotonView(PhotonView view)
        {
            state.PhotonView = view;
            return this;
        }
        public CharacterSwitchMoveBuilder<T> SetStateMachine(StateMachine stateMachine)
        {
            state.StateMachine = stateMachine;
            return this;
        }
        public CharacterSwitchMove Build()
        {
            return state;
        }
    }
}