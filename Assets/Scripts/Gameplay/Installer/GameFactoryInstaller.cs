using Gameplay.Factory;
using Gameplay.Factory.Weapon;
using Zenject;

namespace Gameplay.Installer
{
    public class GameFactoryInstaller
    {
        public void Install(DiContainer diContainer)
        {
            EquipmentFactory equipmentFactory = new EquipmentFactory();
            diContainer.Inject(equipmentFactory);
            diContainer.Bind(equipmentFactory.GetType()).FromInstance(equipmentFactory).AsSingle();
            
            InventoryItemFactory inventoryItemFactory = new InventoryItemFactory();
            diContainer.Inject(inventoryItemFactory);
            diContainer.Bind(inventoryItemFactory.GetType()).FromInstance(inventoryItemFactory).AsSingle();
            
            AbilityFactory abilityFactory = new AbilityFactory();
            diContainer.Inject(abilityFactory);
            diContainer.Bind(abilityFactory.GetType()).FromInstance(abilityFactory).AsSingle();
        }
    }
}