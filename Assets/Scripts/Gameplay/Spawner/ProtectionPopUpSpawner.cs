using System;
using Gameplay.UI;
using UnityEngine;

namespace Gameplay.Spawner
{
    public class ProtectionPopUpSpawner : PopUpSpawner
    {
        public async void CreatePopUp(Vector3 center, float value, DamageType damageType)
        {
            GameObject newGameObject = null;
            
            if (damageType.HasFlag(DamageType.Physical))
                newGameObject = await poolManager.GetObjectAsync<PhysicalDamageResistancePopUpUI>();

            if (damageType.HasFlag(DamageType.Magical))
                newGameObject = await poolManager.GetObjectAsync<MagicalDamageResistancePopUpUI>();

            if (damageType.HasFlag(DamageType.Pure))
                newGameObject = await poolManager.GetObjectAsync<PureDamageResistancePopUpUI>();
            
            if(!newGameObject) return;
            
            newGameObject.transform.position = center;
            newGameObject.GetComponent<PopUpUI>().Play(value);
        }
    }
}