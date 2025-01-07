using Gameplay;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameFieldController))]
public class GameFieldEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var gameField = (GameFieldController)target;

        if (GUILayout.Button("SortingArray"))
        {
            gameField.SortingArray();
        }
    }
}
