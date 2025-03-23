using System;
using System.IO;
using Gameplay;
using Gameplay.Ability;
using Gameplay.Effect;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScriptableObjects.Unit.Item
{
    public abstract class SO_Item : ScriptableObject
    {
        public abstract string ItemName { get; protected set; }
        [field: SerializeField, PreviewField] public Sprite Icon { get; private set; }
        [field: SerializeField] public AssetReferenceT<GameObject> Prefab { get; private set; }
        [field: SerializeField] public ItemCategory ItemCategoryID { get; private set; }
        [field: SerializeField] public ItemBehaviour ItemBehaviourID { get; private set; }
        [field: SerializeField] public float Cooldown { get; private set; }
        [field: SerializeField] public float TimerCast { get; private set; }
        
        [field: SerializeField, Space(10)] public float JumpPower { get; private set; } = 1.5f;
        [field: SerializeField] public float JumpDuration { get; private set; } = .5f;
        
        [field: SerializeField, Space(15)] public StatConfig[] UnitStatsConfigs { get; private set; }
        [field: SerializeField, Space(15)] public AbilityConfigData AbilityConfigData { get; private set; }
        [field: SerializeField, Space(15)] public EffectConfigData EffectConfigData { get; private set; }
        
        #if UNITY_EDITOR
        //[Button]
        public void RenameFile()
        {
            string assetPath = AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrEmpty(assetPath)) return;

            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid)) return; // На случай, если GUID не найден

            string newDirectory = Path.GetDirectoryName(assetPath);
            string newPath = Path.Combine(newDirectory, $"SO_Item_{ItemCategoryID}.asset");

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

    [System.Serializable]
    public class ItemConfigData
    {
        public SO_Item SO_Item;
        public int amount;
    }
}