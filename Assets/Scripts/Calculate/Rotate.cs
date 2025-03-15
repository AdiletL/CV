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

        public static bool IsFacingTargetY(Transform origin, Vector3 target, float thresholdDot = 10f)
        {
            // Проецируем forward и directionToTarget на плоскость YZ (обнуляем X)
            Vector3 flatForward = new Vector3(0, origin.forward.y, origin.forward.z).normalized;
            Vector3 flatDirectionToTarget = new Vector3(0, target.y - origin.position.y, target.z - origin.position.z).normalized;

            // Вычисляем угол между ними
            float angle = (Vector3.Angle(flatForward, flatDirectionToTarget) / 2);

            return angle <= thresholdDot;
           // Debug.Log((angle, thresholdDot));
        }
        
        public static bool IsFacingTarget(Transform origin, Vector3 target, float angleThreshold = 10f)
        {
            // Направление взгляда объекта
            Vector3 forward = origin.forward.normalized;

            // Вектор в сторону цели
            Vector3 directionToTarget = (target - origin.position).normalized;

            // Вычисляем угол между forward и направлением к цели
            float angle = Vector3.Angle(forward, directionToTarget);

            // Проверяем, укладывается ли угол в порог
            return angle <= angleThreshold;
        }
    }
}