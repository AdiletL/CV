using System.IO;
using ScriptableObjects.Unit.Item;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjects.Unit.Portal
{
    [CreateAssetMenu(fileName = "SO_Portal_", menuName = "SO/Gameplay/Unit/Portal", order = 51)]
    public class SO_Portal : ScriptableObject
    {
        [field: SerializeField] public string ID { get; private set; }
        
#if UNITY_EDITOR
        [Button]
        public void RenameFile()
        {
            if (string.IsNullOrEmpty(ID)) return;

            string assetPath = AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrEmpty(assetPath)) return;

            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid)) return; // На случай, если GUID не найден

            string newDirectory = Path.GetDirectoryName(assetPath);
            string newPath = Path.Combine(newDirectory, $"SO_Portal_{ID}.asset");

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