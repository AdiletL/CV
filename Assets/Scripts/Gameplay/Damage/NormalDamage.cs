using Calculate;
using Gameplay.Resistance;
using Gameplay.Spawner;
using Unit;
using UnityEngine;
using Zenject;

namespace Gameplay.Damage
{
    public class NormalDamage : Damage
    {
        [Inject] private DamagePopUpPopUpSpawner damagePopUpPopUpSpawner;

        
        public NormalDamage(int amount, GameObject gameObject) : base(amount, gameObject)
        {

        }

        
        public override int GetTotalDamage(GameObject gameObject)
        {
            base.GetTotalDamage(gameObject);
            var targetUnitCenter = gameObject.GetComponent<UnitCenter>();
            
            CheckAbility(result, targetUnitCenter);
            CheckItem(result, targetUnitCenter);

            if (damagePopUpPopUpSpawner)
                damagePopUpPopUpSpawner.CreatePopUp(targetUnitCenter.Center.position, result);
            
            return result;
        }
    }
}