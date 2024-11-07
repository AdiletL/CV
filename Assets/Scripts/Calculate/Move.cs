using UnityEngine;

namespace Calculate
{
    public static class Move
    {
        public static void Rotate(Transform transform, Transform target, float speed)
        {
            var targetTransform = target;
            var objectTransform = transform;
            
            var direction = targetTransform.position - objectTransform.position;
            if (direction == Vector3.zero) return;
            
            var currentTargetForRotate = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, currentTargetForRotate, speed * Time.deltaTime);
        }
    }
}