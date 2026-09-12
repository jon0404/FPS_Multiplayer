using UnityEngine;
using System.Collections;
using UnityEditor;

public class ColoredHeaderAttribute : PropertyAttribute
{
    public string text;
    public Color color;
    public int fontSize;
    public int indent;

    public ColoredHeaderAttribute(
        string text,
        float r,
        float g,
        float b,
        int fontSize = 12,
        int indent = 0)
    {
        this.text = text;
        this.color = new Color(r, g, b);
        this.fontSize = fontSize;
        this.indent = indent;
    }
}

/*public class HorizontalSpaceAttribute : PropertyAttribute
{
    public float width;

    public HorizontalSpaceAttribute(float width = 10f)
    {
        this.width = width;
    }
}*/

public class IndentAttribute : PropertyAttribute
{
    public int levels;

    public IndentAttribute(int levels = 1)
    {
        this.levels = levels;
    }
}

// Name this script "ColorSpacerExample"



// This class defines the ColorSpacer attribute, so that
// it can be used in your regular MonoBehaviour scripts:

public class ColorSpacer : PropertyAttribute
{
    public float xPos;
    public float spaceHeight;
    public float lineHeight;
    public float lineWidth;
    public Color lineColor = Color.red;


    public ColorSpacer(float xPos, float spaceHeight, float lineHeight, float lineWidth, float r, float g, float b)
    {
        this.xPos = xPos;
        this.spaceHeight = spaceHeight;
        this.lineHeight = lineHeight;
        this.lineWidth = lineWidth;

        // unfortunately we can't pass a color through as a Color object
        // so we pass as 3 floats and make the object here
        this.lineColor = new Color(r, g, b);
    }
}


// This defines how the ColorSpacer should be drawn
// in the inspector, when inspecting a GameObject with
// a MonoBehaviour which uses the ColorSpacer attribute

[CustomPropertyDrawer(typeof(ColorSpacer))]
public class ColorSpacerDrawer : DecoratorDrawer
{
    ColorSpacer colorSpacer
    {
        get { return ((ColorSpacer)attribute); }
    }

    public override float GetHeight()
    {
        return base.GetHeight() + colorSpacer.spaceHeight;
    }

    public override void OnGUI(Rect position)
    {
        // calculate the rect values for where to draw the line in the inspector
        float lineX = colorSpacer.xPos;
        float lineY = position.y + (colorSpacer.spaceHeight);
        float lineWidth = colorSpacer.lineWidth;
        float lineHeight = colorSpacer.lineHeight;

        // Draw the line in the calculated place in the inspector
        EditorGUI.DrawRect(new Rect(lineX, lineY, lineWidth, lineHeight), colorSpacer.lineColor);
    }
}
