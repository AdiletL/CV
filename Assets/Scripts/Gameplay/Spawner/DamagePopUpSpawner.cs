using System;
using Cysharp.Threading.Tasks;
using Gameplay.Manager;
using Gameplay.UI;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Gameplay.Spawner
{
    public class DamagePopUpSpawner
    {
        [Inject] private PoolManager poolManager;
        
        public async void CreatePopUp(Vector3 center, float damage, DamageType damageType)
        {
            GameObject newGameObject = null;
            switch (damageType)
            {
                case DamageType.Nothing: break;
                case DamageType.Physical: newGameObject = await poolManager.GetObjectAsync<PhysicalDamagePopUpUI>(); break;
                case DamageType.Magical: newGameObject = await poolManager.GetObjectAsync<MagicalDamagePopUpUI>(); break;
                case DamageType.Pure: newGameObject = await poolManager.GetObjectAsync<PureDamagePopUpUI>(); break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null);
            }
            if(!newGameObject) return;
            
            newGameObject.transform.position = center;
            newGameObject.GetComponent<PopUpUI>().Play(damage);
        }
    }
}