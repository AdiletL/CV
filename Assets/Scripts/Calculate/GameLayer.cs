using UnityEngine;

namespace Calculate
{
    public static class GameLayer
    {
        public static bool IsTarget(LayerMask[] origin, LayerMask target)
        {
            if(origin is null) return false;
            
            foreach (var VARIABLE in origin)
            {
                // Проверяем, входит ли слой в маску
                if ((VARIABLE.value & (1 << target)) != 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}