using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ColoredHeaderAttribute))]
public class ColoredHeaderDrawer : DecoratorDrawer
{
    public override float GetHeight()
    {
        return 24f;
    }

    public override void OnGUI(Rect position)
    {
        var attribute = (ColoredHeaderAttribute)base.attribute;

        position.x += attribute.indent * 15f;
        position.width -= attribute.indent * 15f;

        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.fontSize = attribute.fontSize;
        style.normal.textColor = attribute.color;

        EditorGUI.LabelField(position, attribute.text, style);
    }
}