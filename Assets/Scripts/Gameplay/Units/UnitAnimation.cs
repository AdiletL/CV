using System.Collections.Generic;
using UnityEngine;

namespace Unit
{
    public abstract class UnitAnimation : MonoBehaviour
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected AnimationClip defaultAnimationClip;

        protected AnimationClip currentClip;
        protected float currentSpeed;

        // Кэш для хранения длительности анимационных клипов
        protected Dictionary<AnimationClip, float> clipLengths = new();
        protected const string TRANSITION_SPEED = "speed";

        private bool isBlock;
        
        /// <summary>
        /// Возвращает анимационный клип для использования.
        /// </summary>
        private AnimationClip ResolveAnimationClip(AnimationClip clip, bool isDefault)
        {
            return isDefault ? defaultAnimationClip : clip;
        }

        /// <summary>
        /// Возвращает длину анимационного клипа, используя кэш.
        /// </summary>
        private float GetClipLength(AnimationClip clip)
        {
            if (!clipLengths.TryGetValue(clip, out float clipLength))
            {
                clipLength = clip.length;
                clipLengths[clip] = clipLength;
            }

            return clipLength;
        }

        /// <summary>
        /// Проверяет, нужно ли пропустить изменение анимации.
        /// </summary>
        private bool ShouldSkipAnimationChange(AnimationClip clip, bool isForce, bool isDefault)
        {
            if (isBlock && !isForce) return true;

            if (isDefault)
            {
                return !isForce && currentClip == defaultAnimationClip;
            }

            return clip == null || (!isForce && currentClip == clip);
        }
        
        #region Initialization

        /// <summary>
        /// Метод для инициализации, может быть переопределён в наследниках.
        /// </summary>
        public virtual void Initialize()
        {
            // Переопределите в наследниках при необходимости
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Изменение текущей анимации.
        /// </summary>
        /// <param name="clip">Клип для проигрывания.</param>
        /// <param name="duration">Продолжительность анимации.</param>
        /// <param name="transitionDuration">Длительность перехода между анимациями.</param>
        /// <param name="isForce">Принудительное изменение анимации.</param>
        /// <param name="isDefault">Установить анимацию по умолчанию.</param>
        public void ChangeAnimationWithDuration(AnimationClip clip, float duration = 0, 
            float transitionDuration = 0.1f, bool isForce = false, bool isDefault = false)
        {
            if (ShouldSkipAnimationChange(clip, isForce, isDefault)) return;

            clip = ResolveAnimationClip(clip, isDefault);
            SetSpeedClip(clip, duration);

            if (gameObject.activeSelf)
            {
                PlayAnimation(clip, transitionDuration);
            }
        }

        public void ChangeAnimationWithSpeed(AnimationClip clip, float speed = 1, 
            float transitionDuration = 0.1f, bool isForce = false, bool isDefault = false)
        {
            if (ShouldSkipAnimationChange(clip, isForce, isDefault)) return;
            
            clip = ResolveAnimationClip(clip, isDefault);
            animator.SetFloat(TRANSITION_SPEED, speed);
            currentSpeed = speed;
            
            if (gameObject.activeSelf)
            {
                PlayAnimation(clip, transitionDuration);
            }
        }

        /// <summary>
        /// Блокировка/разблокировка изменений анимации.
        /// </summary>
        /// <param name="isBlock">Флаг блокировки.</param>
        public void SetBlock(bool isBlock) => this.isBlock = isBlock;

        #endregion
        
        /// <summary>
        /// Установка скорости анимации.
        /// </summary>
        /// <param name="clip">Клип, для которого устанавливается скорость.</param>
        /// <param name="duration">Продолжительность воспроизведения.</param>
        public virtual void SetSpeedClip(AnimationClip clip, float duration)
        {
            float clipLength = GetClipLength(clip);
            duration = duration > 0 ? duration : clipLength;

            float speed = Calculate.Animation.TotalAnimationSpeed(clipLength, duration);

            if (!Mathf.Approximately(currentSpeed, speed))
            {
                animator.SetFloat(TRANSITION_SPEED, speed);
                currentSpeed = speed;
            }
        }

        /// <summary>
        /// Запускает воспроизведение анимации с указанной длительностью перехода.
        /// </summary>
        private void PlayAnimation(AnimationClip clip, float transitionDuration)
        {
            animator.CrossFadeInFixedTime(clip.name, transitionDuration, 0);
            currentClip = clip;
        }
    }
}