using System.Collections;
using Calculate;
using UnityEngine;

namespace Character
{
    public class IdleState : IState
    {
        public CharacterMove CharacterMove;
        public CharacterAnimation CharacterAnimation;
        public AnimationClip IdleClip;


        public void Enter()
        {
            //Animation
            CharacterAnimation.ChangeAnimation(IdleClip);
            FindPlatform.GetPlatform(CharacterMove.transform.position + (Vector3.up * .5f), Vector3.down)?.AddGameObject(CharacterMove.gameObject);
        }

        public void Execute()
        {
            //Update

        }

        public void Exit()
        {
            FindPlatform.GetPlatform(CharacterMove.transform.position + (Vector3.up * .5f), Vector3.down)?.RemoveGameObject(CharacterMove.gameObject);
        }
    }
    public class IdleStateBuilder
    {
        private readonly IdleState state = new();

        public IdleStateBuilder SetCharacters(CharacterMove move, CharacterAnimation animation)
        {
            state.CharacterMove = move;
            state.CharacterAnimation = animation;
            return this;
        }
        public IdleStateBuilder SetIdleClip(AnimationClip clip)
        {
            state.IdleClip = clip;
            return this;
        }

        public IdleState Build()
        {
            return state;
        }
    }
}