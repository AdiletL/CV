using Gameplay.Common;
using Gameplay.Factory.Weapon;
using Gameplay.Units.Item.ContextMenu;
using ScriptableObjects.Gameplay.Equipment;
using Unit;
using Unit.Character;
using UnityEngine;
using Zenject;

namespace Gameplay.Units.Item
{
    public abstract class EquipmentItem : Item
    {
        [Inject] private DiContainer diContainer;
        
        private SO_Equipment so_Equipment; 
        private Equipment.Equipment equipment;
        private EquipmentContextMenu equipmentContextMenu;
        private CharacterMainController currentCharacter;
        
        public void SetEquipmentConfig(SO_Equipment config) => so_Equipment = config;
        
        public override void Initialize()
        {
            base.Initialize();
            currentCharacter = OwnerGameObject.GetComponent<CharacterMainController>();
            EquipmentFactory equipmentFactory = new EquipmentFactoryBuilder()
                .Build();
            equipment = (Equipment.Weapon.Sword)equipmentFactory.CreateEquipment(so_Equipment);
            diContainer.Inject(equipment);
            equipment.SetOwner(OwnerGameObject);
            equipment.SetOwnerCenter(OwnerGameObject.GetComponent<UnitCenter>().Center);
            equipment.Initialize();
        }

        public override void PutOn()
        {
            if (IsCooldown)
            {
                Debug.Log($"{ItemBehaviourID} на перезарядке!");
                return;
            }
            
            if(!currentCharacter.IsNullEquipment(equipment)) return;
            StartEffect();
        }

        public override void TakeOff()
        {
            if(currentCharacter.IsNullEquipment(equipment)) return;
            RemoveStatsFromUnit();
            currentCharacter.TakeOffEquipment(equipment);
            Exit();
        }
        
        protected override void AfterCast()
        {
            base.AfterCast();
            currentCharacter.PutOnEquipment(equipment);
            AddStatsFromUnit();
            Exit();
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