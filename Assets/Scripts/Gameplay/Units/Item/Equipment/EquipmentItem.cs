using System;
using Gameplay.Equipment;
using Gameplay.Factory.Weapon;
using Gameplay.Unit.Character;
using Gameplay.Unit.Item.ContextMenu;
using ScriptableObjects.Gameplay.Equipment;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit.Item
{
    public abstract class EquipmentItem : Item
    {
        [Inject] private DiContainer diContainer;
        [Inject] private EquipmentFactory equipmentFactory;
        
        public abstract EquipmentType EquipmentTypeID { get; protected set; }
        
        protected SO_Equipment so_Equipment; 
        protected Equipment.Equipment equipment;
        private EquipmentContextMenu equipmentContextMenu;
        private CharacterEquipmentController characterEquipmentController;

        private bool isUse;
        
        public void SetEquipmentConfig(SO_Equipment config) => so_Equipment = config;
        
        public override void Initialize()
        {
            base.Initialize();
            characterEquipmentController = Owner.GetComponent<CharacterEquipmentController>();
            
            equipment = equipmentFactory.CreateEquipment(so_Equipment);
            diContainer.Inject(equipment);
            equipment.SetOwner(Owner);
            equipment.SetOwnerCenter(Owner.GetComponent<UnitCenter>().Center);
            equipment.Initialize();
            equipment.Hide();
        }

        public override void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null)
        {
            base.Enter(finishedCallBack, target, point);
            if(characterEquipmentController.IsNullEquipment(equipment)) 
                PutOn();
            else
                TakeOff();
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
    
    public abstract class EquipmentItemBuilder : ItemBuilder<EquipmentItem>
    {
        protected EquipmentItemBuilder(Item item) : base(item)
        {
        }

        public EquipmentItemBuilder SetEquipmentConfig(SO_Equipment config)
        {
            if(item is EquipmentItem equipmentItem)
                equipmentItem.SetEquipmentConfig(config);
            return this;
        }
    }
}