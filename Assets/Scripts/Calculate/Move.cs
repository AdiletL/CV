using UnityEngine;

namespace Calculate
{
    public static class Move
    {
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