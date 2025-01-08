using Gameplay.Tool;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreateCellTool))]
public class CreateCellToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var createCell = (CreateCellTool)target;
        
        if (GUILayout.Button("Create Cells"))
        {
            createCell.CreateCells();
        }
        if (GUILayout.Button("Destroy Cells"))
        {
            createCell.DestroyCells();
        }
    }
}
