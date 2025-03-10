﻿using System;
using Gameplay.Unit.Portal;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Unit.Item
{
    public class TeleportationScrollItem : Item
    {
        [Inject] private PortalController[] startPortals;

        public override ItemName ItemNameID { get; protected set; } = ItemName.TeleportationScroll;
        
        private AssetReferenceT<GameObject> portalObject;
        private Camera baseCamera;
        private Vector3? spawnPosition;
        private string endPortalID;

        public void SetPortalObject(AssetReferenceT<GameObject> portalObject) => this.portalObject = portalObject;
        public void SetEndPortalID(string id) => this.endPortalID = id;

        public override void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null)
        {
            base.Enter(finishedCallBack, target, point);
            if (!isActivated || point == null) return;
            spawnPosition = point;
            StartEffect();
        }

        protected override void AfterCast()
        {
            CreateTeleport();
            base.AfterCast();
            Exit();
        }
        
        private void CreateTeleport()
        {
            var newTeleportObject = Addressables.InstantiateAsync(portalObject).WaitForCompletion();
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
    
    
    public class TeleportationScrollItemBuilder : ItemBuilder<TeleportationScrollItem>
    {
        public TeleportationScrollItemBuilder() : base(new TeleportationScrollItem())
        {
        }

        public TeleportationScrollItemBuilder SetPortalObject(AssetReferenceT<GameObject> portalObject)
        {
            if(item is TeleportationScrollItem teleportationScroll)
                teleportationScroll.SetPortalObject(portalObject);
            return this;
        }
        
        public TeleportationScrollItemBuilder SetEndPortalID(string id)
        {
            if(item is TeleportationScrollItem teleportationScroll)
                teleportationScroll.SetEndPortalID(id);
            return this;
        }
    }
}