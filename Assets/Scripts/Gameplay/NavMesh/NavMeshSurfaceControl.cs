using Unity.AI.Navigation;
using UnityEngine;

namespace Gameplay.NavMesh
{
    [RequireComponent(typeof(NavMeshSurface))]
    public class NavMeshSurfaceControl : MonoBehaviour
    {
        public NavMeshSurface GetSurface() => GetComponent<NavMeshSurface>();
        public void ActivateMeshRenderer() => GetComponent<MeshRenderer>().enabled = true;
        public void InActivateMeshRenderer() => GetComponent<MeshRenderer>().enabled = false;
    }
}