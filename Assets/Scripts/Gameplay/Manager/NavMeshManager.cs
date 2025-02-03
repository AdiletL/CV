using System.Collections;
using UnityEngine;

namespace Gameplay.Manager
{
    public class NavMeshManager : MonoBehaviour, IManager
    {
        public void Initialize()
        {
            
        }

        public void BuildNavMesh(GameObject room)
        {
            StartCoroutine(BuildNavMeshCoroutine(room));
        }

        private IEnumerator BuildNavMeshCoroutine(GameObject room)
        {
            yield return null;
            var roomController = room.GetComponent<RoomController>();
            foreach (var VARIABLE in roomController.NavMeshControl.SurfacesControls)
            {
                VARIABLE.ActivateMeshRenderer();
                VARIABLE.GetSurface().BuildNavMesh();
                VARIABLE.InActivateMeshRenderer();
                yield return null;
            }
        }
    }
}