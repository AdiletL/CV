using System;
using Gameplay.Equipment.Weapon;
using ScriptableObjects.Equipment.Weapon;
using ScriptableObjects.Gameplay.Equipment;

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
                _ => throw new ArgumentOutOfRangeException(nameof(config), config, null)
            };
            return result;
        }

        private Sword CreateSword(SO_Equipment config)
        {
            var so_Sword = config as SO_Sword;
            var sword = (Sword)new SwordBuilder()
                .SetReduceEndurance(so_Sword.ReduceEndurance)
                .SetAngleToTarget(so_Sword.AngleToTarget)
                .SetEquipmentPrefab(so_Sword.EquipmentPrefab)
                .Build();
            return sword;
        }
        
        private Bow CreateBow(SO_Equipment config)
        {
            var so_Bow = config as SO_Bow;
            var bow = (Bow)new BowBuilder()
                .SetReduceEndurance(so_Bow.ReduceEndurance)
                .SetAngleToTarget(so_Bow.AngleToTarget)
                .SetEquipmentPrefab(so_Bow.EquipmentPrefab)
                .Build();
            return bow;
        }
    }

    public class EquipmentFactoryBuilder
    {
        private EquipmentFactory factory = new();

        public EquipmentFactory Build()
        {
            return factory;
        }
    }
}
