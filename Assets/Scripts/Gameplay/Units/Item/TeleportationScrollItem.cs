using System;
using Gameplay.Unit.Portal;
using ScriptableObjects.Unit.Item;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Unit.Item
{
    public class TeleportationScrollItem : Item
    {
        [Inject] private PortalController[] startPortals;

        public override ItemUsageType ItemUsageTypeID { get; } = ItemUsageType.ApplyToPoint;
        public override string ItemName { get; protected set; } = nameof(TeleportationScrollItem);
        
        private AssetReferenceT<GameObject> portalObjectPrefab;
        private Camera baseCamera;
        private Vector3? spawnPosition;
        private string endPortalID;


        public TeleportationScrollItem(SO_TeleportationScrollItem so_TeleportationScrollItem) : base(so_TeleportationScrollItem)
        {
            portalObjectPrefab = so_TeleportationScrollItem.PortalObject;
            endPortalID = so_TeleportationScrollItem.EndPortalID.ID;
        }

        public override void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null)
        {
            base.Enter(finishedCallBack, target, point);
            if (!isActivated || point == null) return;
            spawnPosition = point;
            //StartEffect();
        }

        protected override void AfterCast()
        {
            CreateTeleport();
            base.AfterCast();
            Exit();
        }
        
        private void CreateTeleport()
        {
            var newTeleportObject = Addressables.InstantiateAsync(portalObjectPrefab).WaitForCompletion();
            newTeleportObject.transform.position = spawnPosition.Value;
            
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
}