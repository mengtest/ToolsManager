/************************************************************
//     文件名      : SeprateSelectTextureAlphaEditor.cs
//     功能描述    : 分离指定路径的纹理图alpha通道
//     负责人      : lezen   lezenzeng@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-07-14 16:25:15.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.IO;
public class SeprateSelectTextureAlphaEditor : EditorWindow {

    public SeprateSelectTextureAlphaEditor instance;

    public static Texture2D selectTexture;


    [MenuItem("EZFun/UI/分离指定路径图集alpha通道")]
    public static void MainFunc()
    { 
        SeprateSelectTextureAlphaEditor window = (SeprateSelectTextureAlphaEditor)EditorWindow.GetWindow(typeof(SeprateSelectTextureAlphaEditor));
        window.titleContent = new GUIContent("纹理通道分离编辑器");
    }

    static void SeperateRGBAandlphaChannel(string _texPath)
    {
        string assetRelativePath = GetRelativeAssetPath(_texPath);
        SetTextureReadableEx(assetRelativePath);    //set readable flag and set textureFormat TrueColor  
        Texture2D sourcetex = AssetDatabase.LoadAssetAtPath(assetRelativePath, typeof(Texture2D)) as Texture2D;  //not just the textures under Resources file    
        if (!sourcetex)
        {
            Debug.LogError("Load Texture Failed : " + assetRelativePath);
            return;
        }

        TextureImporter ti = null;
        try
        {
            ti = (TextureImporter)TextureImporter.GetAtPath(assetRelativePath);
        }
        catch
        {
            Debug.LogError("Load Texture failed: " + assetRelativePath);
            return;
        }
        if (ti == null)
        {
            return;
        }

        Texture2D rgbTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGB24, false);
        rgbTex.SetPixels(sourcetex.GetPixels());
        rgbTex.Apply();

        Color[] colors = sourcetex.GetPixels();
        Color[] colorsAlpha = new Color[colors.Length];

        for (int i = 0; i < colors.Length; ++i)
        {
            colorsAlpha[i].r = colors[i].a;
            colorsAlpha[i].g = colors[i].a;
            colorsAlpha[i].b = colors[i].a;
            colorsAlpha[i].a = colors[i].a;
        }
        Texture2D alphaTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGB24, false);
        alphaTex.SetPixels(colorsAlpha);
        alphaTex.Apply();
        byte[] alphabytes = alphaTex.EncodeToPNG();
        string alphaTexRelativePath = GetAlphaTexPath(_texPath);
        byte[] bytes = rgbTex.EncodeToPNG();

        File.WriteAllBytes(GetRGBTexPath(assetRelativePath), bytes);
        File.WriteAllBytes(alphaTexRelativePath, alphabytes);
        ReImportAsset(GetRelativeAssetPath(alphaTexRelativePath), sourcetex.width, sourcetex.height);
        ReImportAsset(GetRGBTexPath(assetRelativePath), sourcetex.width, sourcetex.height);
    }

    static void ReImportAsset(string path, int width, int height)
    {
        try
        {
            AssetDatabase.ImportAsset(path);
        }
        catch
        {
            Debug.LogError("Import Texture failed: " + path);
            return;
        }

        TextureImporter importer = null;
        try
        {
            importer = (TextureImporter)TextureImporter.GetAtPath(path);
        }
        catch
        {
            Debug.LogError("Load Texture failed: " + path);
            return;
        }
        if (importer == null)
        {
            return;
        }
        importer.maxTextureSize = Mathf.Max(width, height);
        importer.anisoLevel = 0;
        importer.filterMode = FilterMode.Bilinear;
        importer.isReadable = false;  //increase memory cost if readable is true  
        importer.textureFormat = TextureImporterFormat.AutomaticCompressed;
#if UNITY_5_5 || UNITY_5_6
        importer.textureType = TextureImporterType.Default;
#else
        importer.textureType = TextureImporterType.Advanced;
#endif
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = false;
        AssetDatabase.ImportAsset(path);
    }

    static void SetTextureReadableEx(string _relativeAssetPath)    //set readable flag and set textureFormat TrueColor  
    {
        TextureImporter ti = null;
        try
        {
            ti = (TextureImporter)TextureImporter.GetAtPath(_relativeAssetPath);
        }
        catch
        {
            Debug.LogError("Load Texture failed: " + _relativeAssetPath);
            return;
        }
        if (ti == null)
        {
            return;
        }
        ti.isReadable = true;
        ti.textureFormat = TextureImporterFormat.AutomaticTruecolor;      //this is essential for departing Textures for ETC1. No compression format for following operation.  
        AssetDatabase.ImportAsset(_relativeAssetPath);
    }

    static string GetRGBTexPath(string _texPath)
    {
        return GetTexPath(_texPath, "_RGB.");
    }

    static string GetAlphaTexPath(string _texPath)
    {
        return GetTexPath(_texPath, "_Alpha.");
    }

    static string GetRelativeAssetPath(string _fullPath)
    {
        _fullPath = GetRightFormatPath(_fullPath);
        int idx = _fullPath.IndexOf("Assets");
        string assetRelativePath = _fullPath.Substring(idx);
        return assetRelativePath;
    }

    static string GetTexPath(string _texPath, string _texRole)
    {
        string dir = System.IO.Path.GetDirectoryName(_texPath);
        string filename = System.IO.Path.GetFileNameWithoutExtension(_texPath);
        string result = dir + "/" + filename + _texRole + "png";
        return result;
    }

    static string GetRightFormatPath(string _path)
    {
        return _path.Replace("\\", "/");
    }

    void OnSelectTexture(UnityEngine.Object obj)
    {
        if (selectTexture != obj)
        {
            selectTexture = obj as Texture2D;
            Repaint();
        }
    }



    void OnEnable() { instance = this; }
    void OnDisable() { instance = null; }

    private string path = "";

    Rect rect;
    void OnGUI()
    {
        NGUIEditorTools.SetLabelWidth(84f);
        GUILayout.Space(3f);

        NGUIEditorTools.DrawHeader("Input", true);
        NGUIEditorTools.BeginContents(false);


        EditorGUILayout.LabelField("输入文件路径（可以拖拽文件至输入框）");
        rect = EditorGUILayout.GetControlRect(GUILayout.Width(800));
        GUILayout.Space(20f);
        path = EditorGUI.TextField(rect, path);

        if ((Event.current.type == EventType.dragUpdated ||
            Event.current.type == EventType.DragExited) &&
        rect.Contains(Event.current.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                path = DragAndDrop.paths[0];
                return;
            }
        }

        if (path.Length > 0)
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("Seperate"))
            {
                SeperateRGBAandlphaChannel(path);
            }
            GUILayout.EndVertical();
        }
    }

}
