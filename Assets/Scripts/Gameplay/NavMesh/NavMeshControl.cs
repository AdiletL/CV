using Unity.AI.Navigation;
using UnityEngine;

namespace Gameplay.NavMesh
{
    public class NavMeshControl : MonoBehaviour
    {
        [field: SerializeField] public NavMeshSurfaceControl[] SurfacesControls { get; private set; }

        [ContextMenu("Get Surfaces")]
        private void GetSurfaces()
        {
            SurfacesControls = GetComponentsInChildren<NavMeshSurfaceControl>();
        }
    }
}