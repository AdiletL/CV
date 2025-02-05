using UnityEngine;

namespace Gameplay.NavMesh
{
    public class NavMeshSurfaceControl : MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        public void ActivateMeshRenderer()
        {
            if(meshRenderer == null)
                meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.enabled = true;
        }

        public void InActivateMeshRenderer()
        {
            if(meshRenderer == null)
                meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.enabled = false;
        }
    }
}