using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public abstract class CharacterAnimation : MonoBehaviour, ICharacter
    {
        [SerializeField] protected Animator animator;
        
        protected AnimationClip currentClip;
        protected AnimatorClipInfo[] currentClips;
        
        protected float currentSpeed;

        protected const string TRANSITION_SPEED = "speed";

        public virtual void Initialize()
        {

        }

        public void ChangeAnimation(AnimationClip clip, float division = 1, float duration = 1,
            float transitionDuration = .1f, bool force = false)
        {
            currentClips = animator.GetCurrentAnimatorClipInfo(0);

            if (!clip)
            {
                return;
            } 
            else if (!force && currentClips.Length != 0)
            {
                currentClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
                if (clip == currentClip) return;
            }

            SetSpeedAnimation(ref clip, division, duration);
            animator?.CrossFadeInFixedTime(clip?.name, transitionDuration, 0);
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