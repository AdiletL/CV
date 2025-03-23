using System;
using Gameplay.Equipment.Shield;
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
                _ when config.GetType() == typeof(SO_NormalShield) => CreateShield(config),
                _ => throw new ArgumentOutOfRangeException(nameof(config), config, null)
            };
            return result;
        }

        private Sword CreateSword(SO_Equipment config)
        {
            return new Sword(config as SO_Sword);
        }
        
        private Bow CreateBow(SO_Equipment config)
        {
            return new Bow(config as SO_Bow);
        }

        private Shield CreateShield(SO_Equipment config)
        {
            return new Shield(config as SO_Shield);
        }
    }
}
