/************************************************************
//     文件名      : STMUIAtlasDataInspector.cs
//     功能描述    : 
//     负责人      : Administrator   Administrator@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-06-30 20:13:33.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

[CustomEditor(typeof(STMUIAtlasData))]
public class STMUIAtlasDataInspector : Editor
{
    STMUIAtlasData m_atlasData;
    // Use this for initialization
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        m_atlasData = target as STMUIAtlasData;
        SerializedProperty atlas = serializedObject.FindProperty("m_atlas");
        SerializedProperty type = serializedObject.FindProperty("m_type");
        SerializedProperty spriteName = serializedObject.FindProperty("m_spriteName");
        SerializedProperty size = serializedObject.FindProperty("m_size");
        SerializedProperty offset = serializedObject.FindProperty("offset");
        SerializedProperty advance = serializedObject.FindProperty("advance");
        //the rest:
        UIAtlas tempAtlas = EditorGUILayout.ObjectField("UI Atlas", m_atlasData.m_atlas, typeof(UIAtlas), false) as UIAtlas;
        if (tempAtlas != null)
        {
            if (tempAtlas.spriteList.Count < 1)

                EditorGUILayout.HelpBox("No Elements in atlas", MessageType.Warning, true);
            else
            {
                if (m_atlasData.m_atlas != tempAtlas)
                {
                    m_atlasData.m_atlas = tempAtlas;
                    m_atlasData.m_spriteData = tempAtlas.spriteList[tempAtlas.spriteList.Count - 1];
                    m_atlasData.m_spriteName = m_atlasData.m_spriteData.name;
                    m_atlasData.m_size = new Vector2(m_atlasData.m_spriteData.width, m_atlasData.m_spriteData.height);
                }
                EditorGUILayout.BeginHorizontal();
                NGUIEditorTools.DrawAdvancedSpriteField(atlas.objectReferenceValue as UIAtlas, spriteName.stringValue, SelectSprite, false);
                EditorGUILayout.EndHorizontal();
            }
            
        }
        EditorGUILayout.PropertyField(spriteName);
        EditorGUILayout.Space(); //////////////////SPACE

        SerializedProperty sp = NGUIEditorTools.DrawProperty("Type", serializedObject, "m_type", GUILayout.MinWidth(20f));

        EditorGUILayout.PropertyField(size);
        EditorGUILayout.Space(); //////////////////SPACE
        EditorGUILayout.PropertyField(offset);
        EditorGUILayout.Space(); //////////////////SPACE
        EditorGUILayout.PropertyField(advance);
        EditorGUILayout.Space(); //////////////////SPACE
                                 //FixColumnCount();
        serializedObject.ApplyModifiedProperties(); //since break; cant be called

    }

    void SelectSprite(string spriteName)
    {
        serializedObject.Update();
        UISpriteData spriteData = m_atlasData.m_atlas.GetSprite(spriteName);
        if ( m_atlasData.m_spriteName != spriteName)
        {
            SerializedProperty sp = serializedObject.FindProperty("m_spriteName");
            sp.stringValue = spriteName;
            serializedObject.ApplyModifiedProperties();
            NGUITools.SetDirty(serializedObject.targetObject);
            NGUISettings.selectedSprite = spriteName;
            m_atlasData.m_spriteName = spriteName;
            m_atlasData.m_spriteData = spriteData;
            m_atlasData.m_size = new Vector2(m_atlasData.m_spriteData.width, m_atlasData.m_spriteData.height);
        }
        serializedObject.Update();
    }
}
