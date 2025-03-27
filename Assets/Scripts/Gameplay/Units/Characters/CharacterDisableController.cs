using System;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Gameplay.Unit.Character
{
    public abstract class CharacterDisableController : MonoBehaviour, IDisableController
    {
        [SerializeField] protected CharacterMainController characterMainController;
        [SerializeField] protected SO_CharacterDisable so_CharacterDisable;
        
        public virtual void Initialize()
        {
        }

        public void ActivateDisable(DisableType disableType)
        {
            if (characterMainController.StateMachine.TryGetInterfaceImplementingClass(out IDisablelable disablelable))
            {
                disablelable.SetDisableType(disableType);
                characterMainController.StateMachine.ExitOtherStates(disablelable.GetType(), true);
            }
        }

        public void DeactivateDisable(DisableType disableType)
        {
            if (characterMainController.StateMachine.TryGetInterfaceImplementingClass(out IDisablelable disablelable))
            {
                disablelable.DeactivateDisable(disableType);
            }
        }
    }
}