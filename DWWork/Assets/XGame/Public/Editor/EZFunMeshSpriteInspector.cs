/************************************************************
     File      : NewBehaviourScript.cs
     author    : Administrator   Administrator@ezfun.cn
     version   : 1.0
     date      : 2017-01-11 21:58:45.
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEditor;
using UnityEngine;

/// <summary>
/// Inspector class used to edit UITextures.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(EZFunMeshSprite), true)]
public class EZFunMeshSpriteInspector : Editor
{

    /// <summary>
    /// Atlas selection callback.
    /// </summary>

    void OnSelectAtlas(Object obj)
    {
        serializedObject.Update();
        SerializedProperty sp = serializedObject.FindProperty("mAtlas");
        sp.objectReferenceValue = obj;
        serializedObject.ApplyModifiedProperties();
        NGUITools.SetDirty(serializedObject.targetObject);
        NGUISettings.atlas = obj as UIAtlas;
        //(target as EZFunMeshSprite).MarkAsChanged();
    }

    /// <summary>
    /// Sprite selection callback function.
    /// </summary>

    void SelectSprite(string spriteName)
    {
        serializedObject.Update();
        SerializedProperty sp = serializedObject.FindProperty("mSpriteName");
        sp.stringValue = spriteName;
        serializedObject.ApplyModifiedProperties();
        NGUITools.SetDirty(serializedObject.targetObject);
        NGUISettings.selectedSprite = spriteName;
        //(target as EZFunMeshSprite).MarkAsChanged();
    }

    public override void OnInspectorGUI()
    {
        NGUIEditorTools.SetLabelWidth(80f);
        EditorGUILayout.Space();

        serializedObject.Update();

        DrawCustomProperties();

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Draw the atlas and sprite selection fields.
    /// </summary>

    protected void DrawCustomProperties()
    {
        SerializedProperty height = NGUIEditorTools.DrawProperty("Height", serializedObject, "mIconHeight");
        SerializedProperty size = NGUIEditorTools.DrawProperty("Size", serializedObject, "m_Size");
        GUILayout.BeginHorizontal();
        if (NGUIEditorTools.DrawPrefixButton("Atlas"))
            ComponentSelector.Show<UIAtlas>(OnSelectAtlas);
        SerializedProperty atlas = NGUIEditorTools.DrawProperty("", serializedObject, "mAtlas", GUILayout.MinWidth(20f));

        if (GUILayout.Button("Edit", GUILayout.Width(40f)))
        {
            if (atlas != null)
            {
                UIAtlas atl = atlas.objectReferenceValue as UIAtlas;
                NGUISettings.atlas = atl;
                if (atl != null) NGUIEditorTools.Select(atl.gameObject);
            }
        }
        GUILayout.EndHorizontal();

        SerializedProperty sp = serializedObject.FindProperty("mSpriteName");
        NGUIEditorTools.DrawAdvancedSpriteField(atlas.objectReferenceValue as UIAtlas, sp.stringValue, SelectSprite, false);
    }

    /// <summary>
    /// All widgets have a preview.
    /// </summary>

    public override bool HasPreviewGUI()
    {
        return (Selection.activeGameObject == null || Selection.gameObjects.Length == 1);
    }

    /// <summary>
    /// Draw the sprite preview.
    /// </summary>

    public override void OnPreviewGUI(Rect rect, GUIStyle background)
    {
        EZFunMeshSprite sprite = target as EZFunMeshSprite;
        if (sprite == null) return;

        UIAtlas atlas = sprite.atlas;
        if (atlas == null) return;

        Texture2D tex = atlas.texture as Texture2D;
        if (tex == null) return;

        UISpriteData sd = sprite.atlas.GetSprite(sprite.spriteName);
        NGUIEditorTools.DrawSprite(tex, rect, sd, Color.white);
    }
}
