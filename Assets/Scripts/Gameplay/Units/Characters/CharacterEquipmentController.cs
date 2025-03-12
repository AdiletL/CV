using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Unit.Character
{
    public abstract class CharacterEquipmentController : MonoBehaviour
    {
        [SerializeField] protected CharacterMainController characterMainController;
        
        [field: SerializeField, Space] public Transform WeaponParent { get; private set; }
        
        private List<Equipment.Equipment> equipments;
        
        public bool IsNullEquipment(Equipment.Equipment equipment) => 
            equipments == null || !equipments.Contains(equipment);

        public void Initialize()
        {
            
        }
        
        public virtual void PutOn(Equipment.Equipment equipment)
        {
            equipments ??= new List<Equipment.Equipment>();
            
            if (equipment is Equipment.Weapon.Weapon weapon)
                characterMainController.StateMachine.GetState<CharacterAttackState>()?.SetWeapon(weapon);
            
            equipments.Add(equipment);
        }

        public virtual void TakeOff(Equipment.Equipment equipment)
        {
            if (equipment is Equipment.Weapon.Weapon weapon)
                characterMainController.StateMachine.GetState<CharacterAttackState>()?.RemoveWeapon();
            
            equipments?.Remove(equipment);
        }
    }
}