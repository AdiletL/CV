﻿using UnityEngine;

namespace Calculate
{
    public static class Distance
    {
        public static bool IsNearUsingSqr(Vector3 current, Vector3 target, float threshold = 0.01f)
        {
            return (current - target).sqrMagnitude <= threshold;
        }
    }
}