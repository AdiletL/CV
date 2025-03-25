using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Unit.Character
{
    public abstract class CharacterEquipmentController : MonoBehaviour
    {
        [SerializeField] protected CharacterMainController characterMainController;

        [Space] 
        [SerializeField] public Transform weaponParent;
        [SerializeField] public Transform shieldParent;
        
        private List<Equipment.Equipment> equipments;
        
        public bool IsNullEquipment(Equipment.Equipment equipment) => 
            equipments == null || !equipments.Contains(equipment);

        public void Initialize()
        {
            
        }
        
        public virtual void PutOn(Equipment.Equipment equipment)
        {
            Debug.Log("puton");
            equipments ??= new List<Equipment.Equipment>();

            Debug.Log(equipment.EquipmentTypeID);
            switch (equipment.EquipmentTypeID)
            {
                case Equipment.EquipmentType.Weapon:
                    equipment.SetInParent(weaponParent);
                    characterMainController.StateMachine.GetState<CharacterAttackState>()?.SetWeapon((Equipment.Weapon.Weapon)equipment);
                    break;
                case Equipment.EquipmentType.Shield:
                    equipment.SetInParent(shieldParent);
                    equipment.Show();
                    break;
            }

            equipments.Add(equipment);
        }

        public virtual void TakeOff(Equipment.Equipment equipment)
        {
            switch (equipment.EquipmentTypeID)
            {
                case Equipment.EquipmentType.Weapon:
                    characterMainController.StateMachine.GetState<CharacterAttackState>()?.RemoveWeapon();
                    equipment.SetInParent(null);
                    break;
                case Equipment.EquipmentType.Shield:
                    equipment.Hide();
                    equipment.SetInParent(null);
                    break;
            }
            
            equipments?.Remove(equipment);
        }
    }
}