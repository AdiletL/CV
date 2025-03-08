using Gameplay.Ability;
using Machine;
using UnityEngine;

namespace Gameplay.Unit.Character
{
    public class CharacterSpecialActionState : State, ISpecialAction
    {
        public override StateCategory Category { get; } = StateCategory.Action;

        protected GameObject gameObject;
        
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        
        public override void Update()
        {
            
        }

        public override void LateUpdate()
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

        public CharacterSpecialActionStateBuilder SetGameObject(GameObject gameObject)
        {
            state.SetGameObject(gameObject);
            return this;
        }
    }
}