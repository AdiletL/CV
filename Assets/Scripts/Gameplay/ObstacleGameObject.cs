using System;
using UnityEngine;

namespace Gameplay
{
    public class ObstacleGameObject : MonoBehaviour
    {
        [SerializeField] protected Renderer[] renderers;
        public bool IsBlocked = true;

        protected void Start()
        {
            Material mat = null;
            foreach (var VARIABLE in renderers)
            {
                mat = VARIABLE.material;
                mat.SetFloat("_Stencil", 0);              // Сравниваем с 1
            }
        }
    }
}