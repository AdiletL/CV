using UnityEngine;

namespace Calculate
{
    public static class Move
    {
        public static void Rotate(Transform transform, Transform target, float speed, Vector3 upwards = new Vector3())
        {
            var direction = target.position - transform.position;
            if (direction == Vector3.zero) return;

            if (upwards == Vector3.zero) upwards = Vector3.up;
            // Задаем направлениe
            var targetRotation = Quaternion.LookRotation(direction, upwards);

            // Игнорируем выбранные оси
            Vector3 targetEulerAngles = targetRotation.eulerAngles;

            // Обновляем поворот с учетом игнорируемых осей
            Quaternion finalRotation = Quaternion.Euler(targetEulerAngles);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRotation, speed * Time.deltaTime);
        }
        
        public static void Rotate(Transform transform, Vector3 target, float speed, bool ignoreX, bool ignoreY, bool ignoreZ, Vector3 upwards = new Vector3())
        {
            var direction = target - transform.position;
            if (direction == Vector3.zero) return;

            if (upwards == Vector3.zero) upwards = Vector3.up;
            // Задаем направлениe
            var targetRotation = Quaternion.LookRotation(direction, upwards);

            // Игнорируем выбранные оси
            Vector3 targetEulerAngles = targetRotation.eulerAngles;
            Vector3 currentEulerAngles = transform.rotation.eulerAngles;

            if (ignoreX) targetEulerAngles.x = currentEulerAngles.x;
            if (ignoreY) targetEulerAngles.y = currentEulerAngles.y;
            if (ignoreZ) targetEulerAngles.z = currentEulerAngles.z;

            // Обновляем поворот с учетом игнорируемых осей
            Quaternion finalRotation = Quaternion.Euler(targetEulerAngles);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRotation, speed * Time.deltaTime);
        }
        
        public static bool IsFacingTargetUsingAngle(Vector3 origin, Vector3 forward, Vector3 target, float thresholdDot = 0.05f)
        {
            // Направление на цель
            Vector3 directionToTarget = target - origin;

            // Игнорируем ось Y (проекция на плоскость XZ)
            directionToTarget.y = 0;

            // Нормализуем проекцию
            directionToTarget.Normalize();

            // Forward направление объекта, игнорируя ось Y
            Vector3 forwardXZ = forward;
            forwardXZ.y = 0;
            forwardXZ.Normalize();

            // Скалярное произведение между forward направлением объекта и направлением на цель
            float dotProduct = Vector3.Angle(forwardXZ, directionToTarget);

            // Если скалярное произведение больше или равно порогу, объект смотрит на цель
            return dotProduct <= thresholdDot;
        }

    }
}