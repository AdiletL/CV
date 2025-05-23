﻿using System.Collections.Generic;
using Calculate;
using Gameplay.Unit;
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


        public Equipment(SO_Equipment so_Equipment)
        {
            this.so_Equipment = so_Equipment;   
        }
        
        public void SetOwner(GameObject gameObject)
        {
            this.Owner = gameObject;
            ownerLayer = Owner.layer;
            ownerCenter = Owner.GetComponent<UnitCenter>().Center;
        }

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
                    var gameValue = new GameValue(statValue.GameValueConfig.Value, statValue.GameValueConfig.ValueTypeID);
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
            equipment.transform.localScale = Vector3.one;
        }
    }
}