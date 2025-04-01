using System;
using Gameplay.Manager;
using Gameplay.UI;
using UnityEngine;
using Zenject;

namespace Gameplay.Spawner
{
    public class ProtectionPopUpSpawner
    {
        [Inject] private PoolManager poolManager;
        
        public async void CreatePopUp(Vector3 center, float damage, DamageType damageType)
        {
            GameObject newGameObject = null;
            switch (damageType)
            {
                case DamageType.Nothing: break;
                case DamageType.Physical: newGameObject = await poolManager.GetObjectAsync<PhysicalDamageResistancePopUpUI>(); break;
                case DamageType.Magical: newGameObject = await poolManager.GetObjectAsync<MagicalDamageResistancePopUpUI>(); break;
                case DamageType.Pure: newGameObject = await poolManager.GetObjectAsync<PureDamageResistancePopUpUI>(); break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null);
            }
            if(!newGameObject) return;
            
            newGameObject.transform.position = center;
            newGameObject.GetComponent<PopUpUI>().Play(damage);
        }
    }
}