using UnityEngine;

namespace Movement
{
    public class Rotation
    {
        private Transform transform;
        private Transform target;
        private Vector3 direction = Vector3.up;
        private float rotationSpeed;
        private bool isIgonreX = true;
        private bool isIgonreY = false;
        private bool isIgonreZ = true;

        public Rotation(Transform transform, float rotationSpeed)
        {
            this.transform = transform;
            this.rotationSpeed = rotationSpeed;
        }
        
        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public void SetDirection(Vector3 direction)
        {
            this.direction = direction;
        }

        public void SetIgnoreDirection(bool isIgnoreX, bool isIgnoreY, bool isIgnoreZ)
        {
            this.isIgonreX = isIgnoreX;
            this.isIgonreY = isIgnoreY;
            this.isIgonreZ = isIgnoreZ;
        }
        
        public void Rotate()
        {
            var direction = target.position - transform.position;
            if (direction == Vector3.zero) return;
            
            // Задаем направлениe
            var targetRotation = Quaternion.LookRotation(direction, this.direction);

            // Игнорируем выбранные оси
            Vector3 targetEulerAngles = targetRotation.eulerAngles;
            Vector3 currentEulerAngles = transform.rotation.eulerAngles;

            if (isIgonreX) targetEulerAngles.x = currentEulerAngles.x;
            if (isIgonreY) targetEulerAngles.y = currentEulerAngles.y;
            if (isIgonreZ) targetEulerAngles.z = currentEulerAngles.z;

            // Обновляем поворот с учетом игнорируемых осей
            Quaternion finalRotation = Quaternion.Euler(targetEulerAngles);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRotation, rotationSpeed * Time.deltaTime);
        }
    }
}