using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IndentAttribute))]
public class IndentDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attribute = (IndentAttribute)base.attribute;
        EditorGUI.indentLevel = 1;
        EditorGUI.indentLevel += attribute.levels;

        EditorGUI.PropertyField(position, property, label);

        EditorGUI.indentLevel -= attribute.levels;
    }

    
}

