using Machine;
using UnityEngine;

namespace Gameplay.Unit.Character
{
    public class CharacterTakeDamageState : State
    {
        public override StateCategory Category { get; } = StateCategory.Action;

        protected GameObject gameObject;
        
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        
        public override void Update()
        {
        }


    }

    public class CharacterTakeDamageStateBuilder : StateBuilder<CharacterTakeDamageState>
    {
        public CharacterTakeDamageStateBuilder(CharacterTakeDamageState instance) : base(instance)
        {
        }

        public CharacterTakeDamageStateBuilder SetGameObject(GameObject gameObject)
        {
            state.SetGameObject(gameObject);
            return this;
        }
    }
}