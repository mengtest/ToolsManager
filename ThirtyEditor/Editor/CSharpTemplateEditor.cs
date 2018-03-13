/************************************************************
     File      : CSharpTemplateEditor.cs
     author    : shandong   shandong@ezfun.cn
     version   : 1.0
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class CSharpTemplateEditor : UnityEditor.AssetModificationProcessor
{
    public static void OnWillCreateAsset(string path)
    {
        if (path.Contains("StreamingAssets"))
        {
            return;
        }
        path = path.Replace(".meta", "");
        int index = path.LastIndexOf(".");
        if (index == -1)
        {
            return;
        }
        string file = path.Substring(index);
        if (file != ".cs" && file != ".js" && file != ".boo" && file != ".lua") return;
        string fileExtension = file;
        if (path.Contains("ezfun.lua"))
        {
            return;
        }
        index = Application.dataPath.LastIndexOf("Assets");
        path = Application.dataPath.Substring(0, index) + path;
        file = System.IO.File.ReadAllText(path);

        file = file.Replace("#CREATIONDATE#", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        file = file.Replace("#CREATOR#", Environment.UserName);

        System.IO.File.WriteAllText(path, file);
        AssetDatabase.Refresh();
    }
}
