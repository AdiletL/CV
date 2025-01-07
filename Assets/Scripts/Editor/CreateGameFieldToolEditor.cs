using Gameplay.Tool;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreateGameFieldITool))]
public class CreateGameFieldToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var tool = (CreateGameFieldITool)target;

        if (GUILayout.Button("Create Game Field"))
        {
            tool.CreateGameField();
        }
    }
}
