using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public abstract class CharacterAnimation : MonoBehaviour, ICharacter
    {
        [SerializeField] protected Animator animator;
        protected AnimationClip currentClip;

        protected float currentSpeed;

        protected const string TRANSITION_SPEED = "speed";

        public virtual void Initialize()
        {

        }

        public void ChangeAnimation(AnimationClip clip, float division = 1, float duration = 1,
            float transitionDuration = .1f)
        {
            if (!clip || clip == currentClip) return;

            SetSpeedAnimation(ref clip, division, duration);
            animator.CrossFadeInFixedTime(clip.name, transitionDuration, 0);
            currentClip = clip;
        }

        protected void SetSpeedAnimation(ref AnimationClip clip, float division = 1, float duration = 1)
        {
            float speed = (clip.length / division) * duration;
            if (currentSpeed == speed) return;

            animator.SetFloat(TRANSITION_SPEED, speed);
            currentSpeed = speed;
        }
    }
}