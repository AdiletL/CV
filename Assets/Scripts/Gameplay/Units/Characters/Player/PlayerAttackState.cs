using Gameplay.Equipment.Weapon;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Gameplay.Unit.Character.Player
{
    public class PlayerAttackState : CharacterAttackState
    {
        private SO_PlayerAttack so_PlayerAttack;
        private PlayerKinematicControl playerKinematicControl;
        private Camera baseCamera;

        private Vector3 direction;
        private Vector3 worldMousePosition;
        private bool isFacingTarget;
        
        public Stat RotationSpeed { get; } = new Stat();
        
        public void SetBaseCamera(Camera camera) => baseCamera = camera;
        public void SetPlayerKinematicControl(PlayerKinematicControl control) => playerKinematicControl = control;

        public override void Initialize()
        {
            base.Initialize();
            so_PlayerAttack = (SO_PlayerAttack)so_CharacterAttack;
            RotationSpeed.AddValue(so_PlayerAttack.RotationSpeed);
        }

        public override void Enter()
        {
            base.Enter();
            playerKinematicControl.SetRotationSpeed(RotationSpeed.CurrentValue);
            UpdateDirection();
            ClearValues();
            SetRotateDirection();
            currentClip = getRandomAnimationClip();
        }

        public override void Update()
        {
            CheckFacingTarget();

            if (isFacingTarget)
            {
                CheckDurationAttack();
                Attack();
            }
        }
        
        protected override void ClearValues()
        {
            base.ClearValues();
            isFacingTarget = false;
            countDurationAttack = 0;
            isFacingTarget = false;
            isAttacked = false;
        }

        private void ClearColorAtTarget()
        {
            if (currentTarget)
            {
                if(currentTarget.TryGetComponent(out UnitRenderer unitRenderer))
                    unitRenderer.ResetColor();
            }
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

        public override void SetWeapon(Equipment.Weapon.Weapon weapon)
        {
            base.SetWeapon(weapon);
            switch (weapon)
            {
                case Sword sword: SetAnimationClip(so_PlayerAttack.SwordAttackClip); break;
                case Bow bow: SetAnimationClip(so_PlayerAttack.BowAttackClip); break;
            }
        }

        protected void SetRotateDirection()
        {
            playerKinematicControl.SetDirectionRotate(direction);
        }

        private void CheckFacingTarget()
        {
            if (Calculate.Rotate.IsFacingTargetUsingAngle(gameObject.transform.position,
                    gameObject.transform.forward, worldMousePosition, 50) && !isFacingTarget)
            {
                this.unitAnimation?.ChangeAnimationWithDuration(null, isDefault: true);
                isFacingTarget = true;
            }
        }

        private void CheckDurationAttack()
        {
            countDurationAttack += Time.deltaTime;
            if (durationAttack < countDurationAttack)
            {
                stateMachine.ExitCategory(Category, null);
                countDurationAttack = 0;
            }
        }

        protected override void DefaultApplyDamage()
        {
            base.DefaultApplyDamage();
            
            if(currentTarget &&
               Calculate.Rotate.IsFacingTargetUsingAngle(gameObject.transform.position,
                   gameObject.transform.forward, currentTarget.transform.position, angleToTarget) &&
               currentTarget.TryGetComponent(out IAttackable attackable) && 
               currentTarget.TryGetComponent(out IHealth health) && health.IsLive)
            {
                Damageable.Value = DamageStat.CurrentValue;
                attackable.TakeDamage(Damageable);
            }
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