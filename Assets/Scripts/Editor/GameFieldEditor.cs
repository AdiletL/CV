using Gameplay;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomController))]
public class GameFieldEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var gameField = (RoomController)target;

        if (GUILayout.Button("SortingArray"))
        {
            gameField.SortingArray();
        }
    }
}
