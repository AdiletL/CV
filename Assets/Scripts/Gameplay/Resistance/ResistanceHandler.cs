using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Resistance
{
    public class ResistanceHandler : MonoBehaviour, IHandler
    {
        private Dictionary<Type, IResistance> resistances = new();

        public IResistance GetResistance(Type resistanceType)
        {
            if (resistanceType == null)
                throw new ArgumentNullException(nameof(resistanceType));

            if (resistances.TryGetValue(resistanceType, out var resistance))
                return resistance;

            return null;
        }
        
        public bool TryGetResistance<T>(out IResistance resistance) where T : IResistance
        {
            var resistanceType = typeof(T);

            if (resistances.TryGetValue(resistanceType, out resistance))
                return true;

            resistance = null;
            return false;
        }
            
        public void Initialize()
        {
           
        }

        public void AddResistance(IResistance resistance)
        {
            if(resistances.ContainsKey(resistance.GetType())) return;
            resistances[resistance.GetType()] = resistance;
        }

        public void RemoveResistance(IResistance resistance)
        {
            if(!resistances.ContainsKey(resistance.GetType())) return;
            resistances.Remove(resistance.GetType());
        }
    }
}