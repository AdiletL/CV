﻿using System;
using Gameplay;
using Photon.Pun;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Unit.Cell
{
    [SelectionBase]
    public class CellController : UnitController
    {
        [SerializeField] private TextMeshPro platformText;

        private PhotonView photonView;
        private UnitRenderer unitRenderer;
        private Collider[] colliders = new Collider[1];

       [field: SerializeField, HideInInspector] public Vector2Int CurrentCoordinates { get; private set; }

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

        public void InitializeRPC()
        {
            this.photonView = GetComponent<PhotonView>();
            
            this.photonView.RPC(nameof(Trigger), RpcTarget.AllBuffered, this.photonView.ViewID);
        }

        [PunRPC]
        private void Trigger()
        {
            Initialize();
        }
        
        public override void Initialize()
        {
            base.Initialize();
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