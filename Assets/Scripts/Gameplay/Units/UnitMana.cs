using ScriptableObjects.Unit;
using UnityEngine;

namespace Gameplay.Unit
{
    public abstract class UnitMana : MonoBehaviour, IMana, IActivatable
    {
        [SerializeField] protected UnitController unitController;
        [SerializeField] protected SO_UnitMana so_UnitMana;
        
        public Stat ManaStat { get; }
        public Stat RegenerationStat { get; }
        
        public bool IsActive { get; protected set; }
        
        public virtual void Initialize()
        {
            ManaStat.AddMaxValue(so_UnitMana.MaxMana);
            ManaStat.AddValue(so_UnitMana.MaxMana);
            
            
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
    } 
}