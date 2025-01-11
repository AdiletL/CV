using UnityEngine;

namespace Calculate
{
    public static class GameLayer
    {
        public static bool IsTarget(LayerMask origin, LayerMask target)
        {
            // Проверяем, входит ли слой в маску. Для нескольких слоев проверяем каждый.
            if ((origin.value & (1 << target)) != 0)
            {
                return true;
            }

            return false;
        }
    }
}