using UnityEditor;
using UnityEngine;

namespace Gameplay.Tool
{
    public class CreateDefaultCellTool : ToolEditor
    {
        [Header("Don't touch!!!")]
        [SerializeField] private GameObject defaultCellToolPrefab;
        [SerializeField] private Transform parent;
        private Vector2Int length = new(100, 100);
        
        [ContextMenu("Create Default Cells")]
        public void Reset()
        {
            CreateCells();
        }
        
        private void CreateCells()
        {
            if (defaultCellToolPrefab.GetComponent<DefaultCell>() == null)
            {
                Debug.LogError("Prefab does not have a DefaultCell");
                return;
            }
            else if (parent == null)
            {
                Debug.LogError("Parent is null");
                return;
            }
            
            float intervalX = 0;
            float intervalZ = 0;
            for (var x = 0; x < length.x; x++)
            {
                intervalX = x * .05f;
                for (var y = 0; y < length.y; y++)
                {
                    intervalZ = y * .05f;
                    
                    var newGameObject = (GameObject)PrefabUtility.InstantiatePrefab(defaultCellToolPrefab.gameObject, parent.transform);
                    newGameObject.transform.localPosition = new Vector3(
                        (x * defaultCellToolPrefab.transform.localScale.x) + intervalX, 0,
                        (y * defaultCellToolPrefab.transform.localScale.z) + intervalZ);
                    
                    newGameObject.GetComponent<DefaultCell>().SetCoordinates(x + 1, y + 1);
                }
            }
            
            MarkDirty();
        }

        public void Setting()
        {
            var defaultCells = parent.GetComponentsInChildren<DefaultCell>();
            foreach (var VARIABLE in defaultCells)
            {
                VARIABLE.SettingGameCell();
            }
            Debug.Log("Setting Completed");
        }
    }
}