using System;
using Gameplay;
using Photon.Pun;
using TMPro;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;

namespace Unit.Cell
{
    [SelectionBase]
    public class CellController : UnitController
    {
        [SerializeField] private TextMeshPro platformText;
       [field: SerializeField, HideInInspector] public Vector2Int CurrentCoordinates { get; private set; }

        private PhotonView photonView;
        private Collider[] colliders = new Collider[1];

        public bool IsBlocked()
        {
            var colliderCount =
                Physics.OverlapSphereNonAlloc(transform.position + Vector3.up, .3f, this.colliders, ~Layers.CELL_LAYER);
            if (colliderCount == 0) return false;

            for (int i = colliders.Length - 1; i >= 0; i--)
            {
                if (colliders[i].TryGetComponent(out ObstacleGameObject blockGameObject))
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

        public override void Appear()
        {
            //Debug.Log(CurrentCoordinates);
        }

        public override void Disappear()
        {
            throw new NotImplementedException();
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