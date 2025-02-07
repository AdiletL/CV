using Gameplay.Weapon;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerWeaponAttackState : CharacterWeaponAttackState
    {
        private AnimationClip[] swordAttackClip;
        private AnimationClip[] bowAttackClip;
        private Camera baseCamera;
        private PlayerKinematicControl playerKinematicControl;

        private float rotationSpeed;

        public void SetSwordAttackClip(AnimationClip[] clips) => swordAttackClip = clips;
        public void SetBowAttackClip(AnimationClip[] clips) => bowAttackClip = clips;
        public void SetBaseCamera(Camera camera) => baseCamera = camera;
        public void SetPlayerKinematicControl(PlayerKinematicControl control) => playerKinematicControl = control;
        public void SetRotationSpeed(float rotationSpeed) => this.rotationSpeed = rotationSpeed;

        
        public override void Enter()
        {
            base.Enter();
            this.unitAnimation?.ChangeAnimationWithDuration(getRandomAnimationClip(), duration: durationAttack);
            playerKinematicControl.SetRotationSpeed(rotationSpeed);
        }

        public override void Exit()
        {
            ClearColorAtTarget();
            base.Exit();
        }

        private void ClearColorAtTarget()
        {
            if (currentTarget)
            {
                if(currentTarget.TryGetComponent(out UnitRenderer unitRenderer))
                    unitRenderer.ResetColor();
            }
        }

        public override void SetTarget(GameObject target)
        {
            if(target == null) return;
            ClearColorAtTarget();
            
            base.SetTarget(target);
        }
        

        public override void SetWeapon(Weapon weapon)
        {
            base.SetWeapon(weapon);
            
            switch (weapon)
            {
                case Sword sword:
                    SetAnimationClip(swordAttackClip);
                    break;
                case Bow bow:
                    SetAnimationClip(bowAttackClip);
                    break;
            }
        }

        protected override void RotateToTarget()
        {
            // Получаем позицию мыши в мировых координатах
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = baseCamera.transform.position.y - gameObject.transform.position.y; // Используем высоту камеры
            Vector3 worldMousePosition = baseCamera.ScreenToWorldPoint(mousePosition);

            // Вычисляем направление
            Vector3 direction = worldMousePosition - gameObject.transform.position;
            direction.y = 0; // Игнорируем высоту, вращаем только по Y
            playerKinematicControl.SetDirectionRotate(direction);
        }

        protected override void Fire()
        {
            FindUnit();
            
            if(!currentTarget ||
               !Calculate.Move.IsFacingTargetUsingAngle(gameObject.transform.position,
                   gameObject.transform.forward, currentTarget.transform.position, angleToTarget))
            {
                return;
            }
            
            if (currentTarget&& currentTarget.TryGetComponent(out IHealth health))
            {
                if (health.IsLive)
                    CurrentWeapon.FireAsync();
                else
                    currentTarget = null;
            }
        }
    }

    public class PlayerWeaponAttackStateStateBuilder : CharacterWeaponAttackStateBuilder
    {
        public PlayerWeaponAttackStateStateBuilder() : base(new PlayerWeaponAttackState())
        {
        }

        public PlayerWeaponAttackStateStateBuilder SetSwordAttackClip(AnimationClip[] clips)
        {
            if(state is PlayerWeaponAttackState playerWeaponAttackState)
                playerWeaponAttackState.SetSwordAttackClip(clips);

            return this;
        }
        public PlayerWeaponAttackStateStateBuilder SetBowAttackClip(AnimationClip[] clips)
        {
            if(state is PlayerWeaponAttackState playerWeaponAttackState)
                playerWeaponAttackState.SetBowAttackClip(clips);

            return this;
        }
        public PlayerWeaponAttackStateStateBuilder SetBaseCamera(Camera camera)
        {
            if(state is PlayerWeaponAttackState playerWeaponAttackState)
                playerWeaponAttackState.SetBaseCamera(camera);

            return this;
        }
        public PlayerWeaponAttackStateStateBuilder SetPlayerKinematicControl(PlayerKinematicControl control)
        {
            if(state is PlayerWeaponAttackState playerWeaponAttackState)
                playerWeaponAttackState.SetPlayerKinematicControl(control);

            return this;
        }
        public PlayerWeaponAttackStateStateBuilder SetRotationSpeed(float rotationSpeed)
        {
            if(state is PlayerWeaponAttackState playerWeaponAttackState)
                playerWeaponAttackState.SetRotationSpeed(rotationSpeed);

            return this;
        }
    }
}