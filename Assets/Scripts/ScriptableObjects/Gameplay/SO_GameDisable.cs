using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Unit.Character;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "SO_GameDisable", menuName = "SO/Gameplay/Disable", order = 51)]
    public class SO_GameDisable : SerializedScriptableObject
    {
        public Dictionary<DisableType, DisableCategory> BlockActions;
        
        [Button]
        public void InitBlockActions()
        {
            foreach (var type in Enum.GetValues(typeof(DisableType)).Cast<DisableType>())
            {
                if (!BlockActions.ContainsKey(type))
                {
                    BlockActions[type] = DisableCategory.Nothing; // Или задай дефолтное значение
                    Debug.LogWarning("No animation found for item usage type: " + type);
                }
            }
            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this); // Чтобы Unity сохранил изменения
#endif
        }
    }
}