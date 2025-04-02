using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gameplay.Resistance
{
    public class ResistanceHandler : MonoBehaviour, IHandler
    {
        private Dictionary<ResistanceType, List<IResistance>> resistances;

        public bool IsResistanceNull(ResistanceType resistanceType)
        {
            if(resistances == null
               || !resistances.ContainsKey(resistanceType)
               || resistances[resistanceType].Count == 0) 
                return true;
            
            return false;
        }

        public List<IResistance> GetResistances(ResistanceType resistanceType)
        {
            if (IsResistanceNull(resistanceType))
            {
                return null;
            }

            return resistances[resistanceType];
        }

        public DamageData DamageModifiers(DamageData damageData)
        {
            var list = GetResistances(ResistanceType.Damage);
            if (list != null)
            {
                foreach (IDamageResistance VARIABLE in list)
                {
                    if (damageData.DamageTypeID.HasFlag(VARIABLE.DamageTypeID))
                    {
                        return VARIABLE.DamageModify(damageData);
                    }
                }
            }
            return damageData;
        }
            
        public void Initialize()
        {
          
        }

        public void AddResistance(IResistance resistance)
        {
            resistances ??= new Dictionary<ResistanceType, List<IResistance>>();

            var resistanceType = resistance.ResistanceTypeID;
            if (!IsResistanceNull(resistance.ResistanceTypeID))
            {
                for (int i = resistances[resistanceType].Count - 1; i >= 0; i--)
                {
                    if (Object.ReferenceEquals(resistance, resistances[resistanceType][i]))
                        return;
                }
            }
            else
            {
                resistances.Add(resistanceType, new List<IResistance>());
            }
            resistances[resistanceType].Add(resistance);
        }

        public void RemoveResistance(IResistance resistance)
        {
            var resistanceType = resistance.ResistanceTypeID;
            if(IsResistanceNull(resistanceType)) return;
            for (int i = resistances[resistanceType].Count - 1; i >= 0; i--)
            {
                if (Object.ReferenceEquals(resistance, resistances[resistanceType][i]))
                {
                    resistances[resistanceType].RemoveAt(i);
                    return;
                }
            }
        }
    }
}