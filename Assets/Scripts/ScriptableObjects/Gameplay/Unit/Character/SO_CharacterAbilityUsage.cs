using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableObjects.Unit.Character
{
    public abstract class SO_CharacterAbilityUsage : SerializedScriptableObject
    {
        public Dictionary<AbilityBehaviour, AnimationEventConfig> Animations;
        

        [Button]
        public void InitAnimations()
        {
            foreach (var type in Enum.GetValues(typeof(AbilityBehaviour)).Cast<AbilityBehaviour>())
            {
                if (!Animations.ContainsKey(type))
                {
                    Animations[type] = null; // Или задай дефолтное значение
                    Debug.LogWarning("No animation found for item usage type: " + type);
                }
            }
            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this); // Чтобы Unity сохранил изменения
#endif
        }
    }
}