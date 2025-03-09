using System.Collections;
using System.Collections.Generic;
using Gameplay.NavMesh;
using Unity.AI.Navigation;
using UnityEngine;

namespace Gameplay.Manager
{
    public class NavMeshManager : MonoBehaviour, IManager
    {
        private NavMeshSurface mainSurface;
        
        private List<NavMeshSurfaceControl> navMeshSurfaceControls = new();
        
        public void Initialize()
        {
            mainSurface = GetComponentInChildren<NavMeshSurface>();
        }

        public void BuildNavMesh(GameObject room)
        {
            StartCoroutine(BuildNavMeshCoroutine(room));
        }

        private IEnumerator BuildNavMeshCoroutine(GameObject room)
        {
            var roomController = room.GetComponent<RoomController>();
            foreach (var VARIABLE in roomController.NavMeshControl.SurfacesControls)
                navMeshSurfaceControls.Add(VARIABLE);
            
            // Обновляем границы основного NavMeshSurface
            UpdateNavMeshBounds();

            yield return null;
            roomController.Activate();
        }

        private void UpdateNavMeshBounds()
        {
            if (navMeshSurfaceControls.Count == 0 || mainSurface == null) return;

            Bounds bounds = new Bounds(navMeshSurfaceControls[0].transform.position, Vector3.zero);
            for (int i = 0; i < navMeshSurfaceControls.Count; i++)
            {
                navMeshSurfaceControls[i].ActivateMeshRenderer();
                bounds.Encapsulate(navMeshSurfaceControls[i].transform.position);
            }

            // Устанавливаем новую зону покрытия
            mainSurface.transform.position = bounds.center;
            mainSurface.size = bounds.size;

            // Перестраиваем общий NavMesh
            mainSurface.BuildNavMesh();

            for (int i = navMeshSurfaceControls.Count - 1; i >= 0; i--)
                navMeshSurfaceControls[i].InActivateMeshRenderer();
        }
    }

}