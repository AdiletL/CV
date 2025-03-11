using UnityEngine;

namespace Calculate
{
    public static class Rotate
    {
        public static bool IsFacingTargetXZ(Vector3 origin, Vector3 forward, Vector3 target, float thresholdDot = 10f)
        {
            // Проецируем forward и directionToTarget на плоскость XZ (обнуляем Y)
            Vector3 flatForward = new Vector3(forward.x, 0, forward.z).normalized;
            Vector3 flatDirectionToTarget = new Vector3(target.x - origin.x, 0, target.z - origin.z).normalized;

            // Вычисляем угол между ними
            float angle = Vector3.Angle(flatForward, flatDirectionToTarget);

            return angle <= thresholdDot;
        }

        public static bool IsFacingTargetY(Vector3 origin, Vector3 target, float thresholdDot = 10f)
        {
            // Берём только направление по вертикали
            Vector3 direction = target - origin;
            float angle = Mathf.Atan2(direction.y, direction.z) * Mathf.Rad2Deg;
            
            return angle <= thresholdDot;
        }
    }
}