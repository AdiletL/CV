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
            var result = CurrentDamage + AdditionalDamage;
            
            var resistanceHandler = gameObject.GetComponent<ResistanceHandler>();
            if (resistanceHandler && resistanceHandler.TryGetResistance<NormalDamageResistance>(out var normalResistance))
            {
                var resistanceValue = new GameValue(normalResistance.Value, normalResistance.ValueType);
                result -= resistanceValue.Calculate(result);
                if (result < 0) result = 0;
            }

            var targetUnitCenter = gameObject.GetComponent<UnitCenter>();
            var ownerUnitCenter = Owner.GetComponent<UnitCenter>();
            CheckSkill(result, targetUnitCenter);

            if (damagePopUpPopUpSpawner)
                damagePopUpPopUpSpawner.CreatePopUp(targetUnitCenter.Center.position, result);
            
            return result;
        }
    }
}