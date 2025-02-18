using System;
using ScriptableObjects.Unit.Portal;
using Unit.Cell;
using Unit.Portal;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Ability
{
    public class SpawnPortal : Ability
    {
        [Inject] private PortalController[] startPortals;
        
        public override AbilityType AbilityType { get; protected set; } = AbilityType.SpawnPortal;
        
        private AssetReferenceT<GameObject> portalObject;
        private Camera baseCamera;
        private Vector3 spawnPosition;
        private string endPortalID;
        
        public void SetBaseCamera(Camera camera) => this.baseCamera = camera;
        public void SetPortalObject(AssetReferenceT<GameObject> portalObject) => this.portalObject = portalObject;
        public void SetEndPortalID(string id) => this.endPortalID = id;
        

        public override void Activate(Action finishedCallBack = null, GameObject target = null, Vector3? point = null)
        {
            base.Activate(finishedCallBack, target, point);
            if(!isActivated) return;
            
            Ray ray = baseCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, Layers.CELL_LAYER))
            {
                if (hit.collider.TryGetComponent(out CellController cellController) && 
                    !cellController.IsBlocked())
                {
                    spawnPosition = hit.point;
                }
            }
            
            CreateTeleport();
            Finish();
        }
        

        private void CreateTeleport()
        {
            var newTeleportObject = Addressables.InstantiateAsync(portalObject).WaitForCompletion();
            newTeleportObject.transform.position = spawnPosition;
            
            var portal = newTeleportObject.GetComponent<PortalController>();
            foreach (var VARIABLE in startPortals)
            {
                if (string.Equals(VARIABLE.ID, endPortalID, StringComparison.Ordinal))
                {
                    portal.SetEndPortal(VARIABLE);
                    VARIABLE.Appear();
                    break;
                }
            }
            portal.Initialize();
            portal.Appear();
        }
    }

    [System.Serializable]
    public class SpawnPortalConfig : AbilityConfig
    {
        public AssetReferenceT<GameObject> SpawnPortalPrefab;
        public SO_Portal StartPortalID;
    }
    
    public class SpawnPortalBuilder : AbilityBuilder<SpawnPortal>
    {
        public SpawnPortalBuilder() : base(new SpawnPortal())
        {
        }

        public SpawnPortalBuilder SetPortalObject(AssetReferenceT<GameObject> portalObject)
        {
            if(ability is SpawnPortal spawnTeleport)
                spawnTeleport.SetPortalObject(portalObject);
            return this;
        }
        public SpawnPortalBuilder SetIDStartPortal(string id)
        {
            if(ability is SpawnPortal spawnTeleport)
                spawnTeleport.SetEndPortalID(id);
            return this;
        }
        public SpawnPortalBuilder SetBaseCamera(Camera camera)
        {
            if(ability is SpawnPortal spawnTeleport)
                spawnTeleport.SetBaseCamera(camera);
            return this;
        }
    }
}