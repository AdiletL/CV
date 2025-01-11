using System;
using Gameplay;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Unit.Cell
{
    [SelectionBase]
    public class CellController : UnitController
    {
        public static float Radius = .3f;

        [SerializeField] private TextMeshPro platformText;

        private UnitRenderer unitRenderer;
        private Collider[] colliders = new Collider[1];

       [field: SerializeField, HideInInspector] public Vector2Int CurrentCoordinates { get; private set; }

        public bool IsBlocked()
        {
            var colliderCount =
                Physics.OverlapSphereNonAlloc(transform.position, .3f, this.colliders, ~Layers.CELL_LAYER);
            if (colliderCount == 0) return false;

            for (int i = colliders.Length - 1; i >= 0; i--)
            {
                if (colliders[i].TryGetComponent(out BlockGameObject blockGameObject))
                {
                    return blockGameObject.IsBlocked;
                }
            }

            return false;
        }

        public bool IsFreeSpawn()
        {
            var colliderCount =
                Physics.OverlapSphereNonAlloc(transform.position, .3f, this.colliders, ~Layers.CELL_LAYER);
            return colliderCount == 0;
        }

        public void Initialize()
        {
            unitRenderer = GetComponent<UnitRenderer>();
        }

        public override void Appear()
        {
            //Debug.Log(CurrentCoordinates);
        }
        
        public void SetColor(Color color)
        {
            unitRenderer.SetColor(color);
        }

        public void ResetColor()
        {
            unitRenderer.ResetColor();
        }

        public void SetText(string weight)
        {
            platformText.text = weight;
            platformText.enabled = true;
        }

        public void SetCoordinates(Vector2Int coordinates)
        {
            CurrentCoordinates = coordinates;
            MarkDirty();
        }
        
        public void MarkDirty()
        {
            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            #endif
        }
    }
}