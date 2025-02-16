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

        protected const string EMPTY_ANIMATION_NAME = "New State";

        private AnimationClip ResolveAnimationClip(AnimationClip clip, bool isDefault) =>
            isDefault ? defaultAnimationClip : clip;

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
            foreach (var clip in allClips)
            {
                if (string.Equals(clipName, clip.name, StringComparison.Ordinal))
                    return clip;
            }
            return defaultAnimationClip;
        }

        private bool ShouldSkipAnimationChange(AnimationClip clip, bool isForce, bool isDefault) =>
            (isBlock && !isForce) || 
            (isDefault ? !isForce && currentClip == defaultAnimationClip : clip == null || (!isForce && currentClip == clip));

        public virtual void Initialize()
        {
            photonView = GetComponent<PhotonView>();
        }

        public void ChangeAnimationWithDuration(AnimationClip clip, float duration = 0f, string speedName = null,
            float transitionDuration = 0.1f, bool isForce = false, bool isDefault = false, int layer = 0)
        {
            if (ShouldSkipAnimationChange(clip, isForce, isDefault)) return;
            clip = ResolveAnimationClip(clip, isDefault);

            if (photonView.IsMine)
                photonView.RPC(nameof(ChangeAnimationWithDurationRPC), RpcTarget.All, clip.name, duration, speedName, transitionDuration, layer);
        }

        public void ChangeAnimationWithSpeed(AnimationClip clip, float speed = 1f, string speedName = null,
            float transitionDuration = 0.1f, bool isForce = false, bool isDefault = false)
        {
            if (ShouldSkipAnimationChange(clip, isForce, isDefault)) return;
            clip = ResolveAnimationClip(clip, isDefault);

            if (photonView.IsMine)
                photonView.RPC(nameof(ChangeAnimationWithSpeedRPC), RpcTarget.All, clip.name, speed, speedName, transitionDuration);
        }

        [PunRPC]
        protected void ChangeAnimationWithDurationRPC(string clipName, float duration, string speedName, float transitionDuration, int layer)
        {
            var animationClip = GetAnimationClip(clipName);
            SetSpeedClip(animationClip, duration, speedName);

            if (gameObject.activeSelf)
                PlayAnimation(animationClip, transitionDuration, layer);
        }

        [PunRPC]
        protected void ChangeAnimationWithSpeedRPC(string clipName, float speed, string speedName, float transitionDuration)
        {
            animator.SetFloat(speedName, speed);
            currentSpeed = speed;

            if (gameObject.activeSelf)
            {
                var animationClip = GetAnimationClip(clipName);
                PlayAnimation(animationClip, transitionDuration, 0);
            }
        }

        public void SetBlock(bool isBlock) => this.isBlock = isBlock;

        public virtual void SetSpeedClip(AnimationClip clip, float duration, string speedName)
        {
            float clipLength = GetClipLength(clip);
            duration = duration > 0 ? duration : clipLength;

            float speed = Calculate.Animation.TotalAnimationSpeed(clipLength, duration);

            if (!Mathf.Approximately(currentSpeed, speed))
            {
                if(!string.IsNullOrEmpty(speedName))
                    animator.SetFloat(speedName, speed);
                currentSpeed = speed;
            }
        }

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

        public void ExitAnimation(int layer, float transitionDuration = .1f)
        {
            animator.CrossFadeInFixedTime(EMPTY_ANIMATION_NAME, transitionDuration, layer);
        }
    }
}
