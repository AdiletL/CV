using System;
using Gameplay.UI.ScreenSpace;
using Unit.Cell;
using Unit.Portal;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Units.Item
{
    public class TeleportationScroll : Item
    {
        [Inject] private PortalController[] startPortals;

        public override ItemName ItemNameID { get; protected set; } = ItemName.TeleportationScroll;
        
        private AssetReferenceT<GameObject> portalObject;
        private Camera baseCamera;
        private Vector3 spawnPosition;
        private Texture2D selectTargetCursor;
        private string endPortalID;

        public void SetBaseCamera(Camera camera) => this.baseCamera = camera;
        public void SetPortalObject(AssetReferenceT<GameObject> portalObject) => this.portalObject = portalObject;
        public void SetEndPortalID(string id) => this.endPortalID = id;
        public void SetTargetCursor(Texture2D texture) => this.selectTargetCursor = texture;

        public override void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null)
        {
            base.Enter(finishedCallBack, target, point);
            if(isActivated) SetCursor(selectTargetCursor);
        }

        public override void Update()
        {
            base.Update();
            if (isActivated && Input.GetMouseButtonDown(0) && !isCasting)
            {
                Ray ray = baseCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, Layers.CELL_LAYER))
                {
                    if (hit.collider.TryGetComponent(out CellController cellController) && 
                        !cellController.IsBlocked())
                    {
                        StartEffect();
                        spawnPosition = hit.point;
                    }
                }
            }
        }

        protected override void AfterCast()
        {
            CreateTeleport();
            FinishEffect();
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
    
    
    public class TeleportationScrollBuilder : ItemBuilder<TeleportationScroll>
    {
        public TeleportationScrollBuilder() : base(new TeleportationScroll())
        {
        }

        public TeleportationScrollBuilder SetPortalObject(AssetReferenceT<GameObject> portalObject)
        {
            if(item is TeleportationScroll teleportationScroll)
                teleportationScroll.SetPortalObject(portalObject);
            return this;
        }
        
        public TeleportationScrollBuilder SetBaseCamera(Camera baseCamera)
        {
            if(item is TeleportationScroll teleportationScroll)
                teleportationScroll.SetBaseCamera(baseCamera);
            return this;
        }
        
        public TeleportationScrollBuilder SetEndPortalID(string id)
        {
            if(item is TeleportationScroll teleportationScroll)
                teleportationScroll.SetEndPortalID(id);
            return this;
        }
        
        public TeleportationScrollBuilder SetSelectTargetCursor(Texture2D texture)
        {
            if(item is TeleportationScroll teleportationScroll)
                teleportationScroll.SetTargetCursor(texture);
            return this;
        }
    }
}