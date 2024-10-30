using UnityEngine;

namespace Calculate
{
    public static class FindPlatform
    {
        public static Vector2Int GetCoordinates(Vector3 origin)
        {
            Debug.DrawRay(origin + Vector3.up * .5f, Vector3.down, Color.green, 1);
            if (Physics.Raycast(origin + Vector3.up * .5f, Vector3.down, out var hit, 4))
                return hit.transform.GetComponent<Platform>().CurrentCoordinates;

            return Vector2Int.zero;
        }

        public static Platform GetPlatform(Vector3 start, Vector3 rayDirection)
        {
            Debug.DrawRay(start, Vector3.down, Color.green, 1);
            if (Physics.Raycast(start, rayDirection, out var hit, 4))
                return hit.transform.GetComponent<Platform>();

            return null;
        }
    }
}
