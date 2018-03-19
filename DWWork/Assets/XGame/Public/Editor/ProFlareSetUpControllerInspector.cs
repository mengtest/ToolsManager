/************************************************************
//     文件名      : ProFlareSetUpControllerInspector.cs
//     功能描述    : 
//     负责人      : lezen   lezenzeng@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-06-30 14:22:46.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

[CustomEditor(typeof(ProFlareSetUpController), true)]
public class ProFlareSetUpControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Space(6f);
        NGUIEditorTools.SetLabelWidth(200f);
        var data = target;

        serializedObject.Update();
        NGUIEditorTools.DrawProperty("UpdateInterval", serializedObject, "m_updateInterval");
       
        NGUIEditorTools.DrawProperty("AppearSpeed", serializedObject, "m_appearSpeed", GUILayout.MinWidth(120f));
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        base.OnPreviewGUI(r, background);
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
}
