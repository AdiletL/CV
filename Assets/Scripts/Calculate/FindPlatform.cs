using UnityEngine;

namespace Calculate
{
    public static class FindPlatform
    {
        private static readonly Vector3 StartRayCheckOffset = Vector3.up * 0.5f;
        private static readonly RaycastHit[] hits = new RaycastHit[1];

        public static Vector2Int GetCoordinates(Vector3 origin)
        {
            Vector3 rayOrigin = origin + StartRayCheckOffset;

            int hitCount = Physics.RaycastNonAlloc(rayOrigin, Vector3.down, hits, 4, Layers.PLATFORM_LAYER);

            if (hitCount > 0)
            {
                Platform platform = hits[0].transform.GetComponent<Platform>();
                if (platform != null)
                {
                    return platform.CurrentCoordinates;
                }
            }

            return Vector2Int.zero;
        }

        public static Platform GetPlatform(Vector3 start, Vector3 rayDirection)
        {
            int hitCount = Physics.RaycastNonAlloc(start, rayDirection, hits, 4, Layers.PLATFORM_LAYER);

            if (hitCount > 0)
            {
                return hits[0].transform.GetComponent<Platform>();
            }

            return null;
        }
    }
}