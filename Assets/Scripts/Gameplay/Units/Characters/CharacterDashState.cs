using Machine;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterDashState : State
    {
        public override StateCategory Category { get; } = StateCategory.action;
        
        public float moveSpeed = 5f; // Обычная скорость движения
        public float dashSpeed = 20f; // Скорость рывка
        public float dashDuration = 0.2f; // Длительность рывка
        public float dashCooldown = 1f; // Время перезарядки рывка

        private Vector3 moveDirection;
        private bool isDashing = false;
        private float dashTimer = 0f;
        private float cooldownTimer = 0f;
        
        public GameObject GameObject { get; set; }
        
        public override void Initialize()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        public override void LateUpdate()
        {
            // Обновление таймера перезарядки
            if (cooldownTimer > 0f)
            {
                cooldownTimer -= Time.deltaTime;
            }
             
            // Обработка рывка
            if (isDashing)
            {
                DashUpdate();
                return;
            }

            // Вызов рывка по кнопке (например, Left Shift)
            if (Input.GetKeyDown(KeyCode.LeftShift) && cooldownTimer <= 0f)
            {
                StartDash();
            }
        }
        
        private void StartDash()
        {
            isDashing = true;
            dashTimer = dashDuration;
            cooldownTimer = dashCooldown;
        }

        private void DashUpdate()
        {
            if (dashTimer > 0f)
            {
                dashTimer -= Time.deltaTime;
                //CharacterController.Move(moveDirection * dashSpeed * Time.deltaTime);
            }
            else
            {
                isDashing = false;
            }
        }
        
    }
}