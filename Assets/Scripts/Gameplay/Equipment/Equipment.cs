using System.Collections.Generic;
using Calculate;
using ScriptableObjects.Gameplay.Equipment;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gameplay.Equipment
{
    public enum EquipmentType
    {
        Nothing,
        Weapon,
        Shield,
        Armor,
        Helmet
    }
    public abstract class Equipment
    {
        public abstract EquipmentType EquipmentTypeID { get; protected set; }

        protected SO_Equipment so_Equipment;
        protected Transform ownerCenter;
        protected GameObject equipment;
        private int ownerLayer;
        private bool isAddedStat;

        private List<float> addedStats;
        
        public GameObject Owner { get; protected set; }
        
        public void SetOwner(GameObject gameObject)
        {
            this.Owner = gameObject;
            ownerLayer = Owner.layer;
        }

        public void SetConfig(SO_Equipment so_Equipment) => this.so_Equipment = so_Equipment;
        public void SetOwnerCenter(Transform ownerCenter) => this.ownerCenter = ownerCenter;

        public virtual void Initialize()
        {
            equipment = Addressables.InstantiateAsync(so_Equipment.EquipmentPrefab).WaitForCompletion();
            equipment.gameObject.layer = ownerLayer;
            Hide();
        }
        
        public void Show()
        {
            addedStats ??= new List<float>();
            AddRegenerationStatToOwner();
            equipment.SetActive(true);
        }

        public void Hide()
        {
            RemoveRegenerationStatFromOwner();
            equipment.SetActive(false);
        }

        private void AddRegenerationStatToOwner()
        {
            if(isAddedStat) return;
            var endurance = Owner.GetComponent<IEndurance>();
            foreach (var VARIABLE in so_Equipment.StatConfigs)
            {
                foreach (var statValue in VARIABLE.StatValuesConfig)
                {
                    var gameValue = new GameValue(statValue.Value, statValue.ValueTypeID);
                    var result = gameValue.Calculate(endurance.RegenerationStat.GetValue(statValue.StatValueTypeID));
                    endurance.RegenerationStat.AddValue(result, statValue.StatValueTypeID);
                    addedStats.Add(result);
                }
            }

            isAddedStat = true;
        }

        private void RemoveRegenerationStatFromOwner()
        {
            if(!isAddedStat) return;
            var endurance = Owner.GetComponent<IEndurance>();
            int index = 0;
            foreach (var VARIABLE in so_Equipment.StatConfigs)
            {
                foreach (var statValue in VARIABLE.StatValuesConfig)
                {
                    endurance.RegenerationStat.RemoveValue(addedStats[index], statValue.StatValueTypeID);
                    index++;
                }
            }

            isAddedStat = false;
        }
        
        public void SetInParent(Transform parent)
        {
            equipment.transform.SetParent(parent);
            if(!parent) return;
            equipment.transform.localPosition = Vector3.zero;
            equipment.transform.localRotation = Quaternion.identity;
        }
    }

    public abstract class EquipmentBuilder
    {
        protected Equipment equipment;

        public EquipmentBuilder(Equipment equipment)
        {
            this.equipment = equipment;
        }

        public EquipmentBuilder SetConfig(SO_Equipment so_Equipment)
        {
            equipment.SetConfig(so_Equipment);
            return this;
        }
        public Equipment Build()
        {
            return equipment;
        }
    }
}