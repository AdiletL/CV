using UnityEngine;

namespace Gameplay.Weapon.Projectile
{
    public class ArrowController : ProjectileController
    {
        private Vector3 startPosition;
        private Vector3 targetPosition;
        private Vector3 previousPosition;

        private float distance;
        private float timerHeight;
        private float totalHeight;
        private float totalDuration;
        private float countTimerDestroy;
        private float timerDestroy;
        
        public override void Initialize()
        {
            base.Initialize();
            timerDestroy = so_Projectile.TimerDestroy;
        }


        public override void Move()
        {
            if (isMoveable)
            {
                timerHeight += Time.deltaTime;
                float progress = Mathf.Clamp01(timerHeight / totalDuration);

                Vector3 horizontalPosition = Vector3.Lerp(startPosition, targetPosition, progress);

                float verticalOffset = moveCurve.Evaluate(progress) * totalHeight;

                Vector3 currentPosition = new Vector3(horizontalPosition.x, horizontalPosition.y + verticalOffset,
                    horizontalPosition.z);

                transform.position = currentPosition;

                Vector3 direction = (currentPosition - previousPosition).normalized;

                if (direction != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(direction);

                previousPosition = currentPosition;

                if (progress >= 1f)
                    isMoveable = false;
            }
            else
            {
                countTimerDestroy += Time.deltaTime;
                if (countTimerDestroy > timerDestroy)
                {
                    ReturnToPool();
                }
            }
        }

        public void SetTargetPosition(Vector3 target)
        {
            targetPosition = target;
            UpdateData();
        }

        private void UpdateData()
        {
            startPosition = transform.position;
            distance = Vector3.Distance(startPosition, targetPosition);
            totalHeight = (distance * .1f) * height;
            totalDuration = distance / MovementSpeed;
            timerHeight = 0;
            countTimerDestroy = 0;
            isMoveable = true;

            previousPosition = transform.position;
            
            Vector3 initialDirection = (targetPosition - startPosition).normalized;
            transform.rotation = Quaternion.LookRotation(initialDirection);
        }
    }
}