using Unit.Cell;
using UnityEngine;

namespace Calculate
{
    public static class FindCell
    {
        private static readonly Vector3 StartRayCheckOffset = Vector3.up;
        private static readonly RaycastHit[] hits = new RaycastHit[1];
        private static readonly Collider[] colliders = new Collider[1];

        public static Vector2Int GetCoordinates(Vector3 origin)
        {
            Vector3 rayOrigin = origin + StartRayCheckOffset;

            int hitCount = Physics.RaycastNonAlloc(rayOrigin, Vector3.down, hits, 100, Layers.CELL_LAYER);

            if (hitCount > 0)
            {
                CellController cell = hits[0].transform.GetComponent<CellController>();
                if (cell != null)
                {
                    return cell.CurrentCoordinates;
                }
            }

            return Vector2Int.zero;
        }

        public static CellController GetCell(Vector3 start, Vector3 rayDirection, bool isUseOverlapSphere = true)
        {
            int hitCount = Physics.RaycastNonAlloc(start + StartRayCheckOffset, rayDirection, hits, 100, Layers.CELL_LAYER);

            if (hitCount > 0)
            {
                return hits[0].transform.GetComponent<CellController>();
            }

            if (!isUseOverlapSphere) return null;
            
            int colliderCount = Physics.OverlapSphereNonAlloc(start, .5f, colliders, Layers.CELL_LAYER);
            
            if(colliderCount > 0)
                return colliders[0].transform.GetComponent<CellController>();

            return null;
        }
    }
}