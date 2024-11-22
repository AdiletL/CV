using UnityEngine;

namespace Gameplay.Weapon.Projectile
{
    public class Arrow : Projectile
    {
        private Vector3 targetPosition;
        private Vector3 startPosition;

        private float distance;
        private float timer;
        private float height;
        private float totalDuration;
        private Vector3 previousPosition;

        public override void Move()
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / totalDuration);

            Vector3 horizontalPosition = Vector3.Lerp(startPosition, targetPosition, progress);

            float verticalOffset = moveCurve.Evaluate(progress) * height;
            
            Vector3 currentPosition = new Vector3(horizontalPosition.x, horizontalPosition.y + verticalOffset, horizontalPosition.z);

            transform.position = currentPosition;

            Vector3 direction = (currentPosition - previousPosition).normalized;

            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);

            previousPosition = currentPosition;

            if (progress >= 1f)
            {
                poolable.ReturnToPool(gameObject);
            }
        }

        public void SetTarget(Vector3 target)
        {
            targetPosition = target;
            startPosition = transform.position;
            distance = Vector3.Distance(startPosition, targetPosition);
            height = distance * .1f;
            totalDuration = distance / MovementSpeed;
            timer = 0;

            previousPosition = transform.position;
            
            Vector3 initialDirection = (targetPosition - startPosition).normalized;
            transform.rotation = Quaternion.LookRotation(initialDirection);
        }
    }
}