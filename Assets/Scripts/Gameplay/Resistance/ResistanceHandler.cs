using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gameplay.Resistance
{
    public class ResistanceHandler : MonoBehaviour, IHandler
    {
        private Dictionary<Type, List<IResistance>> resistances;


        public bool IsResistanceNull(Type type)
        {
            if (!resistances.ContainsKey(type)) return true;
            if(resistances[type].Count == 0) return true;
            return false;
        }
        public List<IResistance> GetResistances(Type type)
        {
            return resistances[type];
        }
            
        public void Initialize()
        {
           
        }

        public void AddResistance(IResistance resistance)
        {
            resistances ??= new Dictionary<Type, List<IResistance>>();

            var type = resistance.GetType();
            if (!IsResistanceNull(type))
            {
                for (int i = resistances[type].Count - 1; i >= 0; i--)
                {
                    if (Object.ReferenceEquals(resistance, resistances[type][i]))
                        return;
                }
            }
            else
            {
                resistances.Add(type, new List<IResistance>());
            }
            resistances[type].Add(resistance);
        }

        public void RemoveResistance(IResistance resistance)
        {
            var type = resistance.GetType();
            if(IsResistanceNull(type)) return;
            for (int i = resistances[type].Count - 1; i >= 0; i--)
            {
                if (Object.ReferenceEquals(resistance, resistances[type][i]))
                {
                    resistances[type].RemoveAt(i);
                    return;
                }
            }
        }
    }
}