using UnityEngine;

namespace Calculate
{
    public static class Rotate
    {
        public static bool IsFacingTargetUsingAngle(Vector3 origin, Vector3 forward, Vector3 target, float thresholdDot = 10f)
        {
            // Направление на цель
            Vector3 directionToTarget = (target - origin).normalized;

            // Нормализуем forward
            Vector3 normalizedForward = forward.normalized;

            // Вычисляем угол между векторами
            float angle = Vector3.Angle(normalizedForward, directionToTarget);

            // Проверяем, находится ли угол в пределах допустимого порога
            return angle <= thresholdDot;
        }

    }
}