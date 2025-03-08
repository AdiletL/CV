using Calculate;
using Gameplay.Resistance;
using Gameplay.Unit;
using UnityEngine;

namespace Gameplay.Damage
{
    public class NormalDamage : Damage
    {
        public NormalDamage(GameObject owner, float damage) : base(owner, damage)
        {

        }

        public override int GetTotalDamage(GameObject gameObject)
        {
            result = Value;
            
            var resistanceHandler = gameObject.GetComponent<ResistanceHandler>();
            if (resistanceHandler && !resistanceHandler.IsResistanceNull(typeof(NormalDamageResistance)))
            {
                var resistances = resistanceHandler.GetResistances(typeof(NormalDamageResistance));
                for (int i = resistances.Count - 1; i >= 0; i--)
                {
                    var resistanceValue = new GameValue(resistances[i].Value, resistances[i].ValueType);
                    result -= (int)resistanceValue.Calculate(result);
                }
                if (result < 0) result = 0;
            }
            
            var targetUnitCenter = gameObject.GetComponent<UnitCenter>();
            
            CheckAbility(result, targetUnitCenter);
            CheckItem(result, targetUnitCenter);
            
            damagePopUpPopUpSpawner?.CreatePopUp(targetUnitCenter.Center.position, result);
            return (int)result;
        }
    }
}