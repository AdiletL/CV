using System;
using System.Collections.Generic;
using Gameplay.Effect;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Gameplay.Unit.Character
{
    public abstract class CharacterDisableController : MonoBehaviour, IDisableController
    {
        [SerializeField] protected CharacterMainController characterMainController;
        [SerializeField] protected SO_CharacterDisable so_CharacterDisable;
        
        protected CharacterUI characterUI;
        protected EffectHandler effectHandler;
        protected List<IDisable> disables;
        
        public virtual void Initialize()
        {
            characterUI = characterMainController.GetComponentInUnit<CharacterUI>();
            effectHandler = characterMainController.GetComponentInUnit<EffectHandler>();
        }

        public void ActivateDisable(IDisable disableEffect)
        {
            if (characterMainController.StateMachine.TryGetInterfaceImplementingClass(out IDisablelable disablelable))
            {
                disablelable.SetDisableType(disableEffect.DisableTypeID);
                characterMainController.StateMachine.ExitOtherStates(disablelable.GetType(), true);
                
                disables ??= new List<IDisable>();
                if(disables.Count > 0)
                    disables[^1].OnCountTimer -= characterUI.OnTimerDisable;
                
                disableEffect.OnCountTimer += characterUI.OnTimerDisable;
                disableEffect.OnDestroyDisable += DeactivateDisable;
                characterUI.SetTextDisableBar(disableEffect.DisableTypeID.ToString());
                characterUI.ShowDisableBar();
                
                disables.Add(disableEffect);
                if(disableEffect is Effect.Effect effect)
                    effectHandler.AddEffect(effect);
            }
        }
        
        public void DeactivateDisable(IDisable disableEffect)
        {
            if (characterMainController.StateMachine.TryGetInterfaceImplementingClass(out IDisablelable disablelable))
            {
                disablelable.DeactivateDisable(disableEffect.DisableTypeID);
                
                if(disables == null) return;
                for (int i = disables.Count - 1; i >= 0; i--)
                {
                    if (ReferenceEquals(disables[i], disableEffect))
                    {
                        disables.RemoveAt(i);
                        if(disableEffect is Effect.Effect effect)
                            effectHandler.RemoveEffect(effect);
                        
                        disableEffect.OnCountTimer -= characterUI.OnTimerDisable;
                        disableEffect.OnDestroyDisable -= DeactivateDisable;
                        if (disables.Count > 0)
                        {
                            disables[^1].OnCountTimer += characterUI.OnTimerDisable;
                            characterUI.SetTextDisableBar(disables[^1].DisableTypeID.ToString());
                            disablelable.SetDisableType(disables[^1].DisableTypeID);
                        }
                        else
                        {
                            characterUI.HideDisableBar();
                        }

                        return;
                    }
                }
            }
        }
    }
}