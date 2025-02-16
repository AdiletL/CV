using System;
using System.Collections.Generic;
using System.IO;
using Gameplay.Skill;
using Sirenix.OdinInspector;
using Unit.Item;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjects.Unit.Item
{
    [CreateAssetMenu(fileName = "SO_Item_", menuName = "SO/Gameplay/Item", order = 51)]
    public class SO_Item : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public ItemType ItemTypeID { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public int Amount { get; private set; } = 1;
        [field: SerializeField] public bool IsCanSelect { get; private set; }
        [field: SerializeField, Space(10)] public float JumpPower { get; private set; } = 1.5f;
        [field: SerializeField] public float JumpDuration { get; private set; } = .5f;
        [field: SerializeField, Space(15)] public SkillConfigData SkillConfigData { get; private set; }
        
        
        public List<SkillConfig> GetSkillConfigs()
        {
            var skillConfigs = new List<SkillConfig>();
            foreach (SkillType VARIABLE in Enum.GetValues(typeof(SkillType)))
            {
                if (VARIABLE == SkillType.nothing || (SkillConfigData.SkillTypeID & VARIABLE) == 0) continue;

                if (SkillConfigData.ApplyDamageHealConfig.SkillType == SkillConfigData.SkillTypeID)
                    skillConfigs.Add(SkillConfigData.ApplyDamageHealConfig);
                if(SkillConfigData.SpawnPortalConfig.SkillType == SkillConfigData.SkillTypeID)
                    skillConfigs.Add(SkillConfigData.SpawnPortalConfig);
            }

            return skillConfigs;
        }
        
        #if UNITY_EDITOR
        [Button]
        public void RenameFile()
        {
            if (string.IsNullOrEmpty(Name)) return;

            string assetPath = AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrEmpty(assetPath)) return;

            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid)) return; // На случай, если GUID не найден

            string newDirectory = Path.GetDirectoryName(assetPath);
            string newPath = Path.Combine(newDirectory, $"SO_Item_{Name}.asset");

            if (assetPath != newPath && !AssetDatabase.LoadAssetAtPath<SO_Item>(newPath))
            {
                string error = AssetDatabase.MoveAsset(assetPath, newPath);
                if (string.IsNullOrEmpty(error))
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    // Получаем актуальный путь через GUID после переименования
                    string updatedPath = AssetDatabase.GUIDToAssetPath(guid);
                    SO_Item renamedItem = AssetDatabase.LoadAssetAtPath<SO_Item>(updatedPath);
                
                    if (renamedItem != null)
                    {
                        EditorUtility.SetDirty(renamedItem);
                        AssetDatabase.ImportAsset(updatedPath);
                    }
                }
                else
                {
                    Debug.LogError($"Ошибка переименования: {error}");
                }
            }
        }
        #endif

        
    }
}