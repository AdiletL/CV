using Unit.Cell;
using UnityEngine;

namespace Calculate
{
    public static class FindCell
    {
        private static readonly Vector3 StartRayCheckOffset = Vector3.up;
        
        public static CellController GetCell(Vector3 start, Vector3 rayDirection, bool isUseOverlapSphere = true)
        {
            RaycastHit[] hits = new RaycastHit[1];
            int hitCount = Physics.RaycastNonAlloc(start + StartRayCheckOffset, rayDirection, hits, 100, Layers.CELL_LAYER);

            if (hitCount > 0)
            {
                return hits[0].transform.GetComponent<CellController>();
            }

            if (!isUseOverlapSphere) return null;

            Collider[] colliders = new Collider[1];
            int colliderCount = Physics.OverlapSphereNonAlloc(start, .5f, colliders, Layers.CELL_LAYER);
            
            if(colliderCount > 0)
                return colliders[0].transform.GetComponent<CellController>();

            return null;
        }
    }
}