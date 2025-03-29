using System;
using System.Collections.Generic;
using Gameplay.Equipment.Weapon;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Unit.Character.Player
{
    public class PlayerAttackState : CharacterAttackState
    {
        private SO_PlayerAttack so_PlayerAttack;
        private PlayerKinematicControl playerKinematicControl;
        private Camera baseCamera;

        private Vector3 direction;
        private Vector3 worldMousePosition;
        private int counterSpecialAttack;
        private const int SPECIAL_ATTACK_INDEX = 2;
        private bool isFacingTarget;
        
        public Stat RotationSpeed { get; } = new Stat();
        
        public void SetBaseCamera(Camera camera) => baseCamera = camera;
        public void SetPlayerKinematicControl(PlayerKinematicControl control) => playerKinematicControl = control;

        protected override AnimationEventConfig getAnimationEventConfig()
        {
            if (CurrentWeapon == null)
                return so_PlayerAttack.DefaultAnimations[Random.Range(0, so_PlayerAttack.DefaultAnimations.Length)];

            AnimationEventConfig config = CurrentWeapon switch
            {
                _ when CurrentWeapon.GetType() == typeof(Sword) => so_PlayerAttack.SwordAnimations[
                    Random.Range(0, so_PlayerAttack.SwordAnimations.Length)],
                _ when CurrentWeapon.GetType() == typeof(Bow) => so_PlayerAttack.BowAnimations[
                    Random.Range(0, so_PlayerAttack.BowAnimations.Length)],
                _ => throw new ArgumentException($"Unknown state type: {CurrentWeapon.GetType()}")
            };
            return config;
        }

        protected override AnimationEventConfig getSpecialAnimationEventConfig()
        {
            AnimationEventConfig config = CurrentWeapon switch
            {
                _ when CurrentWeapon.GetType() == typeof(Sword) => so_PlayerAttack.SpecialSwordAnimations[specialActionConfigIndex],
                _ when CurrentWeapon.GetType() == typeof(Bow) => so_PlayerAttack.BowAnimations[specialActionConfigIndex],
                _ => throw new ArgumentException($"Unknown state type: {CurrentWeapon.GetType()}")
            };
            return config;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            so_PlayerAttack = (SO_PlayerAttack)so_CharacterAttack;
            RotationSpeed.AddCurrentValue(so_PlayerAttack.RotationSpeed);
            
            for (int i = 0; i < so_PlayerAttack.SwordAnimations.Length; i++)
                unitAnimation.AddClip(so_PlayerAttack.SwordAnimations[i].Clip);
            
            for (int i = 0; i < so_PlayerAttack.SpecialSwordAnimations.Length; i++)
                unitAnimation.AddClip(so_PlayerAttack.SpecialSwordAnimations[i].Clip);
            
            for (int i = 0; i < so_PlayerAttack.BowAnimations.Length; i++)
                unitAnimation.AddClip(so_PlayerAttack.BowAnimations[i].Clip);
        }
        
        public override void Enter()
        {
            base.Enter();
            playerKinematicControl.SetRotationSpeed(RotationSpeed.CurrentValue);
            UpdateDirection();
            SetRotateDirection();
            UpdateCurrentClip();
        }

        public override void Update()
        {
            CheckFacingOnTarget();

            if (isFacingTarget)
            {
                Attack();
            }
        }

        public override void Exit()
        {
            base.Exit();
            unitAnimation.ExitAnimation(2);
        }

        protected override void SubscribeStatEvent()
        {
            base.SubscribeStatEvent();
            RangeStat.OnChangedCurrentValue += OnChangedRangeStatCurrentValue;
        }

        protected override void UnsubscribeStatEvent()
        {
            base.UnsubscribeStatEvent();
            RangeStat.OnChangedCurrentValue -= OnChangedRangeStatCurrentValue;
        }

        private void OnChangedRangeStatCurrentValue()
        {
            var totalRange = RangeStat.CurrentValue;
            if(CurrentWeapon != null) totalRange += CurrentWeapon.RangeStat.CurrentValue;
            unitRenderer.SetRangeAttackScale(totalRange);
        }
        
        protected override void ClearValues()
        {
            base.ClearValues();
            isFacingTarget = false;
        }

        private void UpdateDirection()
        {
            // Получаем позицию мыши в мировых координатах
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = baseCamera.transform.position.y - gameObject.transform.position.y; // Используем высоту камеры
            worldMousePosition = baseCamera.ScreenToWorldPoint(mousePosition);
            
            // Вычисляем направление
            direction = worldMousePosition - gameObject.transform.position;
            direction.y = 0; // Игнорируем высоту, вращаем только по Y
        }

        protected void SetRotateDirection()
        {
            playerKinematicControl.SetDirectionRotate(direction);
        }

        private void CheckFacingOnTarget()
        {
            if (!isFacingTarget)
            {
                if (Calculate.Rotate.IsFacingTargetXZ(gameObject.transform.position,
                        gameObject.transform.forward, worldMousePosition, 500))
                {
                    isFacingTarget = true;
                }
                else
                {
                    this.unitAnimation?.ChangeAnimationWithDuration(null, isDefault: true);
                }
            }
        }

        protected override void DefaultApplyDamage()
        {
            currentTarget = FindUnitInRange<IPlayerAttackable>();
            base.DefaultApplyDamage();
        }
    }

    public class PlayerAttackStateBuilder : CharacterAttackStateBuilder
    {
        public PlayerAttackStateBuilder() : base(new PlayerAttackState())
        {
        }
        
        public PlayerAttackStateBuilder SetBaseCamera(Camera camera)
        {
            if(state is PlayerAttackState playerWeaponAttackState)
                playerWeaponAttackState.SetBaseCamera(camera);

            return this;
        }
        public PlayerAttackStateBuilder SetPlayerKinematicControl(PlayerKinematicControl control)
        {
            if(state is PlayerAttackState playerWeaponAttackState)
                playerWeaponAttackState.SetPlayerKinematicControl(control);

            return this;
        }
    }
}