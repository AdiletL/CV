using Gameplay.Equipment.Weapon;
using Gameplay.Weapon;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerWeaponAttackState : CharacterWeaponAttackState
    {
        private SO_PlayerAttack so_PlayerAttack;
        private PlayerKinematicControl playerKinematicControl;
        private AnimationClip[] swordAttackClips;
        private AnimationClip[] bowAttackClips;
        private AnimationClip currentClip;
        private Camera baseCamera;

        private Vector3 direction;
        private Vector3 worldMousePosition;
        private bool isFacingTarget;
        
        private const string ATTACK_SPEED_NAME = "SpeedAttack";
        private const int ANIMATION_LAYER = 1;
        
        public Stat RotationSpeed { get; private set; } = new Stat();
        
        public void SetBaseCamera(Camera camera) => baseCamera = camera;
        public void SetPlayerKinematicControl(PlayerKinematicControl control) => playerKinematicControl = control;

        public override void Initialize()
        {
            base.Initialize();
            so_PlayerAttack = (SO_PlayerAttack)so_CharacterAttack;
            swordAttackClips = so_PlayerAttack.SwordAttackClip;
            bowAttackClips = so_PlayerAttack.BowAttackClip;
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
            countDurationAttack += Time.deltaTime;
            if (durationAttack < countDurationAttack)
            {
                stateMachine.ExitCategory(Category, null);
                countDurationAttack = 0;
                return;
            }
            
            if (Calculate.Rotate.IsFacingTargetUsingAngle(gameObject.transform.position,
                    gameObject.transform.forward, worldMousePosition, 50) && !isFacingTarget)
            {
                this.unitAnimation?.ChangeAnimationWithDuration(null, isDefault: true);
                isFacingTarget = true;
            }

            if (isFacingTarget)
            {
                this.unitAnimation?.ChangeAnimationWithDuration(currentClip, duration: durationAttack, ATTACK_SPEED_NAME, layer: ANIMATION_LAYER);
                Attack();
            }
        }

        public override void Exit()
        {
            base.Exit();
            this.unitAnimation?.ExitAnimation(ANIMATION_LAYER);
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

        public override void SetWeapon(Weapon weapon)
        {
            base.SetWeapon(weapon);
            switch (weapon)
            {
                case Sword sword: SetAnimationClip(swordAttackClips); break;
                case Bow bow: SetAnimationClip(bowAttackClips); break;
            }
        }

        protected void SetRotateDirection()
        {
            playerKinematicControl.SetDirectionRotate(direction);
        }
    }

    public class PlayerWeaponAttackStateStateBuilder : CharacterWeaponAttackStateBuilder
    {
        public PlayerWeaponAttackStateStateBuilder() : base(new PlayerWeaponAttackState())
        {
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
    }
}