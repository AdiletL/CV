﻿using UnityEngine;

namespace Calculate
{
    public static class Move
    {
        public static void Rotate(Transform transform, Transform target, float speed, Vector3 upwards = default, bool ignoreX = false, bool ignoreY = false, bool ignoreZ = false)
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
    }
}