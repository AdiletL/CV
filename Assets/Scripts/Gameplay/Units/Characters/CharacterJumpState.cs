using System.Collections.Generic;
using Machine;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Gameplay.Unit.Character
{
    public class CharacterJumpState : State, IJump
    {
        public override StateCategory Category { get; } = StateCategory.Jump;

        protected SO_CharacterMove so_CharacterMove;
        protected GameObject gameObject;
        protected CharacterAnimation characterAnimation;

        public bool IsCanJump { get; protected set; } = true;
       

        public void SetConfig(SO_CharacterMove config) => so_CharacterMove = config;
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;

        public override void Enter()
        {
            base.Enter();
            if(!IsCanJump)
                stateMachine.ExitCategory(Category, null);
        }

        public override void Update()
        {
        }
        
        public void ActivateJump()
        {
            IsCanJump = true;
        }

        public void DeactivateJump()
        {
            IsCanJump = false;
        }
    }
    
    public class CharacterJumpStateBuilder : StateBuilder<CharacterJumpState>
    {
        public CharacterJumpStateBuilder(CharacterJumpState instance) : base(instance)
        {
        }
        
        public CharacterJumpStateBuilder SetConfig(SO_CharacterMove config)
        {
            state.SetConfig(config);
            return this;
        }
        public CharacterJumpStateBuilder SetGameObject(GameObject gameObject)
        {
            state.SetGameObject(gameObject);
            return this;
        }
        public CharacterJumpStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            state.SetCharacterAnimation(characterAnimation);
            return this;
        }
    }
}