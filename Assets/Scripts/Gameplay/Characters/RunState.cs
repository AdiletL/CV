using UnityEngine;

namespace Character
{
    public class RunState : IState
    {
        public CharacterMove CharacterMove;
        public CharacterAnimation CharacterAnimation;
        public AnimationClip RunClip;
        public Transform CharacterTransform;
        public float RunSpeed;
        public float RotateSpeed;

        private Transform targetTransform;
        private Quaternion targetForRotate;
        private bool isRuning;

        public void SetTarget(Transform target)
        {
            targetTransform = target;
            var diractionRotate = targetTransform.position - CharacterTransform.position;
            targetForRotate = Quaternion.LookRotation(diractionRotate, Vector3.up);
        }

        public void Enter()
        {
            CharacterAnimation.ChangeAnimation(RunClip);
            isRuning = true;
        }

        public void Execute()
        {
            if (!isRuning) return;
            
            CharacterTransform.rotation = Quaternion.RotateTowards(CharacterTransform.rotation, targetForRotate, RotateSpeed * Time.deltaTime);
            if (CharacterTransform.rotation == targetForRotate)
            {
                CharacterTransform.position = Vector3.MoveTowards(CharacterTransform.position, targetTransform.position, RunSpeed * Time.deltaTime);
            }
        }

        public void Exit()
        {
            isRuning = false;
        }
    }

    public class RunStateBuild
    {
        private RunState state = new();

        public RunStateBuild SetCharacters(CharacterMove move, CharacterAnimation animation)
        {
            state.CharacterMove = move;
            state.CharacterAnimation = animation;
            return this;
        }
        public RunStateBuild SetRunClip(AnimationClip clip)
        {
            state.RunClip = clip;
            return this;
        }
        public RunStateBuild SetRunSpeed(float runSpeed)
        {
            state.RunSpeed = runSpeed;
            return this;
        }

        public RunStateBuild SetRotateSpeed(float rotateSpeed)
        {
            state.RotateSpeed = rotateSpeed;
            return this;
        }
        public RunStateBuild SetCharacterTransform(Transform current)
        {
            state.CharacterTransform = current;
            return this;
        }
        public RunState Build()
        {
            return state;
        }
    }
}