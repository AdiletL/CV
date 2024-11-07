using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public abstract class CharacterAnimation : MonoBehaviour, ICharacter
    {
        [SerializeField] protected Animator animator;
        
        protected AnimationClip currentClip;
        protected float currentSpeed;

        private Dictionary<AnimationClip, float> clipLengths = new(); // Кэш длины клипов
        private const string TRANSITION_SPEED = "speed";

        public virtual void Initialize()
        {
            
        }

        public void ChangeAnimation(AnimationClip clip, float duration = 0,
            float transitionDuration = .1f, bool force = false)
        {
            if (clip == null || (!force && currentClip == clip)) return;
            Debug.Log(clip.name + " / " + force);
            SetSpeedAnimation(clip, duration);
            
            animator.CrossFadeInFixedTime(clip.name, transitionDuration, 0);
            currentClip = clip;
        }

        protected void SetSpeedAnimation(AnimationClip clip, float duration)
        {
            if (!clipLengths.TryGetValue(clip, out float clipLength))
            {
                clipLength = clip.length;
                clipLengths[clip] = clipLength; // Кэшируем длину клипа
            }
            
            if(duration == 0) duration = clipLength;
            
            float speed = Calculate.Animation.TotalAnimationSpeed(clipLength, duration);
            if (Mathf.Approximately(currentSpeed, speed)) return;

            animator.SetFloat(TRANSITION_SPEED, speed);
            currentSpeed = speed;
        }
    }
}