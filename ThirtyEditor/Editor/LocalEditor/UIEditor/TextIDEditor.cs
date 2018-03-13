using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextID))]
[ExecuteInEditMode]
public class TextIDEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var style = new GUIStyle();
        var texIDOb = (TextID)target;
        EditorGUILayout.LabelField("TextID：", texIDOb.mTextID.ToString());
        EditorGUILayout.LabelField("Text：", texIDOb.mText);
    }
}
