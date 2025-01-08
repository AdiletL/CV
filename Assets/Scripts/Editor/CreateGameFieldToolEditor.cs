using Gameplay.Tool;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreateGameFieldTool))]
public class CreateGameFieldToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var tool = (CreateGameFieldTool)target;

        if (GUILayout.Button("Create Game Field"))
        {
            tool.CreateGameField();
        }
    }
}
