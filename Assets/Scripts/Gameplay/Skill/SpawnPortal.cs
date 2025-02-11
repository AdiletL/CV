using System;
using Unit.Cell;
using Unit.Portal;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using Object = UnityEngine.Object;

namespace Gameplay.Skill
{
    public class SpawnPortal : Skill
    {
        [Inject] private PortalController[] startPortals;
        
        public override SkillType SkillType { get; protected set; } = SkillType.spawnTeleport;
        
        private AssetReferenceT<GameObject> portalObject;
        private Vector3 spawnPosition;
        private string ID;
        
        public void SetPortalObject(AssetReferenceT<GameObject> portalObject) => this.portalObject = portalObject;
        public void SetIDEndPortal(string id) => this.ID = id;
        
        public override void Update()
        {
            
        }

        public override void LateUpdate()
        {
           
        }

        public override void Execute(Action exitCallBack = null)
        {
            base.Execute(exitCallBack);
            CreateTeleport();
            Exit();
        }

        private void CreateTeleport()
        {
            var newTeleportObject = Addressables.InstantiateAsync(portalObject).WaitForCompletion();
            newTeleportObject.transform.position = spawnPosition;
            
            var portal = newTeleportObject.GetComponent<PortalController>();
            foreach (var VARIABLE in startPortals)
            {
                if (string.Equals(VARIABLE.ID, ID, StringComparison.Ordinal))
                {
                    portal.SetEndPortal(VARIABLE);
                    VARIABLE.Appear();
                    break;
                }
            }
            portal.Initialize();
            portal.Appear();
        }
        
        public override void CheckTarget()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, Layers.CELL_LAYER))
            {
                if (hit.collider.TryGetComponent(out CellController cellController) && 
                    !cellController.IsBlocked())
                {
                    spawnPosition = hit.point;
                    IsCanUseSkill = true;
                }
            }
        }
    }
    
    public class SpawnPortalBuilder : SkillBuilder<SpawnPortal>
    {
        public SpawnPortalBuilder() : base(new SpawnPortal())
        {
        }

        public SpawnPortalBuilder SetPortalObject(AssetReferenceT<GameObject> portalObject)
        {
            if(skill is SpawnPortal spawnTeleport)
                spawnTeleport.SetPortalObject(portalObject);
            return this;
        }
        public SpawnPortalBuilder SetIDStartPortal(string id)
        {
            if(skill is SpawnPortal spawnTeleport)
                spawnTeleport.SetIDEndPortal(id);
            return this;
        }
    }
}