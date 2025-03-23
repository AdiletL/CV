using Gameplay.Ability;
using Machine;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Gameplay.Unit.Character
{
    public class CharacterSpecialActionState : State, ISpecialAction
    {
        public override StateCategory Category { get; } = StateCategory.Action;

        protected SO_CharacterSpecialAction so_CharacterSpecialAction;
        protected GameObject gameObject;
        
        public void SetConfig(SO_CharacterSpecialAction so_CharacterSpecialAction) => this.so_CharacterSpecialAction = so_CharacterSpecialAction;
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        
        public override void Update()
        {
            
        }

        public virtual void Execute()
        {
            
        }
    }
    
    public class CharacterSpecialActionStateBuilder : StateBuilder<CharacterSpecialActionState>
    {
        public CharacterSpecialActionStateBuilder(CharacterSpecialActionState instance) : base(instance)
        {
        }
        public CharacterSpecialActionStateBuilder SetConfig(SO_CharacterSpecialAction so_CharacterSpecialAction)
        {
            state.SetConfig(so_CharacterSpecialAction);
            return this;
        }
        public CharacterSpecialActionStateBuilder SetGameObject(GameObject gameObject)
        {
            state.SetGameObject(gameObject);
            return this;
        }
    }
}