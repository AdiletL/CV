using UnityEngine;

namespace Calculate
{
    public static class Move
    {
        public static void Rotate(Transform transform, Transform target, float speed, Vector3 upwards = new Vector3(), bool ignoreX = false, bool ignoreY = false, bool ignoreZ = false)
        {
            var direction = target.position - transform.position;
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
        
        public static bool IsFacingTargetUsingDot(Transform objectTransform, Transform targetTransform, float thresholdDot = 1f)
        {
            // Направление на цель
            Vector3 directionToTarget = (targetTransform.position - objectTransform.position).normalized;

            // Значение Dot между forward направлением объекта и направлением на цель
            float dotToTarget = Vector3.Dot(objectTransform.forward, directionToTarget);

            // Если Dot больше порога, объект смотрит на цель
            return dotToTarget >= thresholdDot;
        }
    }
}