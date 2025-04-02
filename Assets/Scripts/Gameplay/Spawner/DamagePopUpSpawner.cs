using System;
using Gameplay.UI;
using UnityEngine;

namespace Gameplay.Spawner
{
    public class DamagePopUpSpawner : PopUpSpawner
    {
        public async void CreatePopUp(Vector3 center, float value, DamageType damageType)
        {
            GameObject newGameObject = null;
            
            if (damageType.HasFlag(DamageType.Physical))
                newGameObject = await poolManager.GetObjectAsync<PhysicalDamagePopUpUI>();

            if (damageType.HasFlag(DamageType.Magical))
                newGameObject = await poolManager.GetObjectAsync<MagicalDamagePopUpUI>();

            if (damageType.HasFlag(DamageType.Pure))
                newGameObject = await poolManager.GetObjectAsync<PureDamagePopUpUI>();
            
            if(!newGameObject) return;
            
            newGameObject.transform.position = center;
            newGameObject.GetComponent<PopUpUI>().Play(value);
        }
    }
}