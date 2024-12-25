using System.Collections.Generic;
using UnityEngine;

namespace Unit
{
    public class UnitAnimation : MonoBehaviour
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected AnimationClip defaultAnimationClip;
        
        protected AnimationClip currentClip;
        protected float currentSpeed;

        protected Dictionary<AnimationClip, float> clipLengths = new(); // Кэш длины клипов
        protected const string TRANSITION_SPEED = "speed";

        private bool isBlock;
        
        public virtual void Initialize()
        {
            
        }

        public void ChangeAnimation(AnimationClip clip, float duration = 0,
            float transitionDuration = .1f, bool isForce = false, bool isDefault = false)
        {
            if(this.isBlock && !isForce) return;
            
            if (!isDefault)
            {
                if (clip == null || (!isForce && currentClip == clip)) return;
            }
            else
            {
                if(!isForce && currentClip == clip) return;
                clip = defaultAnimationClip;
            }
            
            SetSpeedAnimation(clip, duration);
            
            if(!gameObject.activeSelf) return;
            
            animator.CrossFadeInFixedTime(clip.name, transitionDuration, 0);
            currentClip = clip;
        }


        public virtual void SetSpeedAnimation(AnimationClip clip, float duration)
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

        public void SetBlock(bool isBlock) => this.isBlock = isBlock;
    }
}