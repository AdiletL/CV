using System;
using Gameplay.Equipment;
using Gameplay.Factory.Weapon;
using Gameplay.Unit.Character;
using Gameplay.Unit.Item.ContextMenu;
using ScriptableObjects.Gameplay.Equipment;
using ScriptableObjects.Unit.Item;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit.Item
{
    public abstract class EquipmentItem : Item
    {
        [Inject] private DiContainer diContainer;
        [Inject] private EquipmentFactory equipmentFactory;
        
        public abstract EquipmentType EquipmentTypeID { get; protected set; }
        public override ItemUsageType ItemUsageTypeID { get; } = ItemUsageType.Equip;

        protected SO_EquipmentItem so_Equipment; 
        protected Equipment.Equipment equipment;
        private EquipmentContextMenu equipmentContextMenu;
        private CharacterEquipmentController characterEquipmentController;

        private bool isUse;


        public EquipmentItem(SO_EquipmentItem so_EquipmentItem) : base(so_EquipmentItem)
        {
            this.so_Equipment = so_EquipmentItem;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            characterEquipmentController = Owner.GetComponent<CharacterEquipmentController>();
            
            equipment = equipmentFactory.CreateEquipment(so_Equipment.SO_Equipment);
            diContainer.Inject(equipment);
            equipment.SetOwner(Owner);
            equipment.Initialize();
            equipment.Hide();
        }

        public override void StartEffect()
        {
            if (IsCooldown)
            {
                Debug.Log($"{ItemBehaviourID} на перезарядке!");
                return;
            }

            if (isCasting)
            {
                Debug.Log($"{ItemBehaviourID} кастуется!");
                return;
            }

            if (!characterEquipmentController.IsNullEquipment(equipment))
            {
                TakeOff();
                return;
            }
            base.StartEffect();
        }

        public virtual void PutOn()
        {
            if (IsCooldown)
            {
                Debug.Log($"{ItemBehaviourID} на перезарядке!");
                return;
            }

            if (isCasting)
            {
                Debug.Log($"{ItemBehaviourID} кастуется!");
                return;
            }
            
            if(!characterEquipmentController.IsNullEquipment(equipment)) return;
            StartEffect();
        }

        public virtual void TakeOff()
        {
            if(characterEquipmentController.IsNullEquipment(equipment)) return;
            RemoveStatsFromUnit();
            characterEquipmentController.TakeOff(equipment);
            isUse = false;
            Exit();
        }
        
        protected override void AfterCast()
        {
            base.AfterCast();
            isUse = true;
            characterEquipmentController.PutOn(equipment);
            AddStatsToUnit();
            Exit();
        }

        public override void AddStatsToUnit()
        {
            if(!isUse) return;
            base.AddStatsToUnit();
        }

        public override void RemoveStatsFromUnit()
        {
            if(!isUse) return;
            base.RemoveStatsFromUnit();
        }

        public override void Exit()
        {
            base.Exit();
            HideContextMenu();
        }

        private void CreateContextMenu()
        {
            if (equipmentContextMenu == null)
            {
                equipmentContextMenu = new EquipmentContextMenu(this);
                diContainer.Inject(equipmentContextMenu);
            }
        }

        public override void ShowContextMenu()
        {
            CreateContextMenu();
            equipmentContextMenu.Show();
        }

        public override void HideContextMenu()
        {
            equipmentContextMenu?.Hide();
        }
    }
}