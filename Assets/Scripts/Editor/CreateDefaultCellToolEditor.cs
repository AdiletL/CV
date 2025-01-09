using Gameplay.Tool;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreateDefaultCellTool))]
public class CreateDefaultCellToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CreateDefaultCellTool defaultCellTool = (CreateDefaultCellTool)target;

        if (GUILayout.Button("Create Default Cells"))
        {
            defaultCellTool.CreateCells();
        }
        else if (GUILayout.Button("Setting game cell"))
        {
            defaultCellTool.Setting();
        }
    }
}
