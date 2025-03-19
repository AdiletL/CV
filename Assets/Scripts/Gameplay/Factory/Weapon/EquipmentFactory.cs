using System;
using Gameplay.Equipment.Weapon;
using ScriptableObjects.Gameplay.Equipment;
using ScriptableObjects.Gameplay.Equipment.Shield;
using ScriptableObjects.Gameplay.Equipment.Weapon;

namespace Gameplay.Factory.Weapon
{
    public class EquipmentFactory : Factory
    {
        public Equipment.Equipment CreateEquipment(SO_Equipment config)
        {
            Equipment.Equipment result = config switch
            {
                _ when config.GetType() == typeof(SO_Sword) => CreateSword(config),
                _ when config.GetType() == typeof(SO_Bow) => CreateBow(config),
                //_ when config.GetType() == typeof(SO_NormalShield) => CreateNormalShield(config),
                _ => throw new ArgumentOutOfRangeException(nameof(config), config, null)
            };
            return result;
        }

        private Sword CreateSword(SO_Equipment config)
        {
            var sword = (Sword)new SwordBuilder()
                .SetConfig(config)
                .Build();
            return sword;
        }
        
        private Bow CreateBow(SO_Equipment config)
        {
            var bow = (Bow)new BowBuilder()
                .SetConfig(config)
                .Build();
            return bow;
        }
        
    }
}
