using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gameplay.Equipment
{
    public abstract class Equipment
    {
        protected Transform ownerCenter;
        protected GameObject equipment;
        protected AssetReferenceGameObject equipmentPrefab;
        public GameObject Owner { get; protected set; }
        
        public void SetOwner(GameObject gameObject) => this.Owner = gameObject;
        public void SetOwnerCenter(Transform ownerCenter) => this.ownerCenter = ownerCenter;
        public void SetEquipmentPrefab(AssetReferenceGameObject equipmentPrefab) => this.equipmentPrefab = equipmentPrefab;

        public virtual void Initialize()
        {
            equipment = Addressables.InstantiateAsync(equipmentPrefab).WaitForCompletion();
            Hide();
        }
        
        public void Show() => equipment.SetActive(true);
        public void Hide() => equipment.SetActive(false);
        
        public void SetInParent(Transform parent)
        {
            equipment.transform.SetParent(parent);
            equipment.transform.localPosition = Vector3.zero;
            equipment.transform.localRotation = Quaternion.identity;
        }
    }

    public abstract class EquipmentBuilder<T> where T : Equipment
    {
        protected Equipment equipment;

        public EquipmentBuilder(Equipment equipment)
        {
            this.equipment = equipment;
        }

        public EquipmentBuilder<T> SetEquipmentPrefab(AssetReferenceGameObject equipmentPrefab)
        {
            equipment.SetEquipmentPrefab(equipmentPrefab);
            return this;
        }

        public Equipment Build()
        {
            return equipment;
        }
    }
}