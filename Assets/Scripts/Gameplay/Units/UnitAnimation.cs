using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Unit
{
    [RequireComponent(typeof(PhotonView))]
    public abstract class UnitAnimation : MonoBehaviour
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected AnimationClip defaultAnimationClip;

        protected PhotonView photonView;
        protected AnimationClip currentClip;
        protected float currentSpeed;
        protected bool isBlock;

        protected List<AnimationClip> allClips;
        
        protected Dictionary<AnimationClip, float> clipLengths = new();

        protected const string TRANSITION_SPEED = "speed";
        
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

        private AnimationClip GetAnimationClip(string clipName)
        {
            foreach (var VARIABLE in allClips)
            {
                if(string.Equals(clipName, VARIABLE.name, StringComparison.Ordinal))
                    return VARIABLE;
            }
            return defaultAnimationClip;
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
            photonView = GetComponent<PhotonView>();
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
        public void ChangeAnimationWithDuration(AnimationClip clip, float duration = 0f, 
            float transitionDuration = 0.1f, bool isForce = false, bool isDefault = false, int layer = 0)
        {
            if (ShouldSkipAnimationChange(clip, isForce, isDefault)) return;

            clip = ResolveAnimationClip(clip, isDefault);
            
            if(photonView.IsMine)
                photonView.RPC(nameof(ChangeAnimationWithDurationRPC), RpcTarget.All, clip.name, duration, transitionDuration, layer);
        }

        public void ChangeAnimationWithSpeed(AnimationClip clip, float speed = 1f, 
            float transitionDuration = 0.1f, bool isForce = false, bool isDefault = false)
        {
            if (ShouldSkipAnimationChange(clip, isForce, isDefault)) return;
            
            clip = ResolveAnimationClip(clip, isDefault);
            
            if(photonView.IsMine)
                photonView.RPC(nameof(ChangeAnimationWithSpeedRPC), RpcTarget.All, clip.name, speed, transitionDuration);
        }

        [PunRPC]
        protected void ChangeAnimationWithDurationRPC(string clipName, float duration, float transitionDuration, int layer)
        {
            var animationClip = GetAnimationClip(clipName);
            SetSpeedClip(animationClip, duration);

            if (gameObject.activeSelf)
                PlayAnimation(animationClip, transitionDuration, layer);
        }

        [PunRPC]
        protected void ChangeAnimationWithSpeedRPC(string clipName, float speed, float transitionDuration)
        {
            animator.SetFloat(TRANSITION_SPEED, speed);
            currentSpeed = speed;
            
            if (gameObject.activeSelf)
            {
                var animationClip = GetAnimationClip(clipName);
                PlayAnimation(animationClip, transitionDuration, 0);
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
        private void PlayAnimation(AnimationClip clip, float transitionDuration, int layer)
        {
            animator.CrossFadeInFixedTime(clip.name, transitionDuration, layer);
            currentClip = clip;
        }


        public void AddClip(AnimationClip clip)
        {
            allClips ??= new List<AnimationClip>();
            allClips.Add(clip);
        }
        public void AddClips(AnimationClip[] clips)
        {
            allClips ??= new List<AnimationClip>();
            allClips.AddRange(clips);
        }

        public void RemoveClip(AnimationClip clip)
        {
            allClips.Remove(clip);
        }
    }
}