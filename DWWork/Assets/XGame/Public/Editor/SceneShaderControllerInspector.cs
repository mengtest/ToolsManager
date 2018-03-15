using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SceneShaderController), true)]
public class SceneShaderControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Space(6f);
        NGUIEditorTools.SetLabelWidth(200f);
        var data = target;

        serializedObject.Update();
        NGUIEditorTools.DrawProperty("lightMapTexture", serializedObject, "m_lightMapTexture");
        var contr = ((SceneShaderController)data);
        NGUIEditorTools.DrawProperty("ShadowColor", serializedObject, "m_ShadowColor");
        DrawRectProperty("LightMapWeight", serializedObject, "m_LightMapWeight", 120, 40f);
        NGUIEditorTools.DrawProperty("GlobalLightColor", serializedObject, "m_GlobalLightColor");
        DrawVector4Property("GlobalLightPos", serializedObject, "m_GlobalLightPos", 200, 40f);

        DrawVector4Property("SunShaftsLightPos", serializedObject, "m_SunShaftsLightPos", 200, 40f);
        NGUIEditorTools.SetLabelWidth(200f);
        NGUIEditorTools.DrawProperty("EnableRayLight", serializedObject, "m_enableRayLight");
        NGUIEditorTools.SetLabelWidth(200f);
        NGUIEditorTools.DrawProperty("LightShaftMaxRadius", serializedObject, "m_MaxRadius");
        NGUIEditorTools.SetLabelWidth(200f);
        NGUIEditorTools.DrawProperty("LightShaftSunColor", serializedObject, "m_SunColor");
        NGUIEditorTools.SetLabelWidth(200f);
        NGUIEditorTools.DrawProperty("LightShaftThreshold", serializedObject, "m_SunThreshold");
        NGUIEditorTools.SetLabelWidth(200f);
        NGUIEditorTools.DrawProperty("LightShaftIntensity", serializedObject, "m_SunShaftIntensity");
        NGUIEditorTools.SetLabelWidth(200f);
        NGUIEditorTools.DrawProperty("LightShaftBlurRadius", serializedObject, "m_SunShaftBlurRadius");
        NGUIEditorTools.SetLabelWidth(200f);
        DrawVector2Property("SunShaftAngelOffset", serializedObject, "m_SunShaftAngelOffset", 200, 40f);
        NGUIEditorTools.SetLabelWidth(200f);
        NGUIEditorTools.DrawProperty("DefaultAmbientColor", serializedObject, "m_DefaultAmbientColor");

        serializedObject.ApplyModifiedProperties();
        ((SceneShaderController)data).SetValue();
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        base.OnPreviewGUI(r, background);
        var data = target;
        ((SceneShaderController)data).SetValue();
    }


    static public void DrawRectProperty(string name, SerializedObject serializedObject, string field, float labelWidth, float spacing)
    {
        if (serializedObject.FindProperty(field) != null)
        {
            GUILayout.BeginHorizontal();
            {
                //GUILayout.Label(name, GUILayout.Width(labelWidth));

                NGUIEditorTools.SetLabelWidth(200f);
                GUILayout.BeginVertical();
                NGUIEditorTools.DrawProperty("DirectLightWeight", serializedObject, field + ".x", GUILayout.MinWidth(50f));
                NGUIEditorTools.DrawProperty("LightMapWeight", serializedObject, field + ".y", GUILayout.MinWidth(50f));
                GUILayout.EndVertical();
                NGUIEditorTools.SetLabelWidth(200f);
                if (spacing != 0f) GUILayout.Space(spacing);
            }
            GUILayout.EndHorizontal();
        }
    }

    static public void DrawVector4Property(string name, SerializedObject serializedObject, string field, float labelWidth, float spacing)
    {
        if (serializedObject.FindProperty(field) != null)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(name, GUILayout.Width(labelWidth));

                NGUIEditorTools.SetLabelWidth(20f);
                GUILayout.BeginVertical();
                NGUIEditorTools.DrawProperty("X", serializedObject, field + ".x", GUILayout.MinWidth(50f));
                NGUIEditorTools.DrawProperty("Y", serializedObject, field + ".y", GUILayout.MinWidth(50f));
                NGUIEditorTools.DrawProperty("Z", serializedObject, field + ".z", GUILayout.MinWidth(50f));
                NGUIEditorTools.DrawProperty("W", serializedObject, field + ".w", GUILayout.MinWidth(50f));
                GUILayout.EndVertical();
                NGUIEditorTools.SetLabelWidth(80f);
                if (spacing != 0f) GUILayout.Space(spacing);
            }
            GUILayout.EndHorizontal();
        }
    }

    static public void DrawVector2Property(string name, SerializedObject serializedObject, string field, float labelWidth, float spacing)
    {
        if (serializedObject.FindProperty(field) != null)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(name, GUILayout.Width(labelWidth));

                NGUIEditorTools.SetLabelWidth(20f);
                GUILayout.BeginVertical();
                NGUIEditorTools.DrawProperty("Horizontal", serializedObject, field + ".x", GUILayout.MinWidth(50f));
                NGUIEditorTools.DrawProperty("Vertical", serializedObject, field + ".y", GUILayout.MinWidth(50f));
                GUILayout.EndVertical();
                NGUIEditorTools.SetLabelWidth(80f);
                if (spacing != 0f) GUILayout.Space(spacing);
            }
            GUILayout.EndHorizontal();
        }
    }
}
