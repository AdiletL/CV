using Gameplay.Tool;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreatePlatformsITool))]
public class CreatePlatformsToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var createPlatforms = (CreatePlatformsITool)target;
        
        if (GUILayout.Button("Create Cells"))
        {
            createPlatforms.CreateCells();
        }
        if (GUILayout.Button("Destroy Cells"))
        {
            createPlatforms.DestroyCells();
        }
    }
}
