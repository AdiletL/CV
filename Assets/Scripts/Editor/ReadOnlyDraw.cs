#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false; // Отключаем возможность взаимодействия
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true; // Включаем обратно
    }
}
#endif