/************************************************************
//     文件名      : ShaderAddToShaderVariants.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-07-26 10:28:12.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ShaderAddToShaderVariants
{

    [MenuItem("EZFun/Shader/AddShaderToVariants")]
    public static void AddShaderToVarints()
    {
        var shaderVarints = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>("Assets/XGame/Resources/Shaders/X2ShaderVariants.shadervariants");
        string dirPath = EditorUtility.OpenFolderPanel("选择AB所在文件夹", Application.dataPath, "");
        if (string.IsNullOrEmpty(dirPath))
        {
            return;
        }
        var dir = new DirectoryInfo(dirPath);
        var files = dir.GetFiles("*.shader", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var path = file.FullName;
            path = path.Replace('\\', '/');
            var ab = AssetDatabase.LoadAssetAtPath<Shader>(path.Substring(path.IndexOf("Assets")));
            if (ab != null)
            {
                for (int i = 0; i <= (int)UnityEngine.Rendering.PassType.MotionVectors; i++)
                {
                    ShaderVariantCollection.ShaderVariant shaderv;
                    try
                    {
                        shaderv = new ShaderVariantCollection.ShaderVariant(ab, (UnityEngine.Rendering.PassType)i);
                        if (shaderv.shader != null)
                        {
                            shaderVarints.Add(shaderv);
                        }
                    }
                    catch
                    {
                        shaderv.shader = null;
                    }
                }
            }
        }
       // File.Delete(Application.dataPath + "/XGame/Resources/Shaders/X2ShaderVariants.shadervariant");
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
        //AssetDatabase.CreateAsset(shaderVarints, "Assets/XGame/Resources/Shaders/X2ShaderVariants.shadervariants");
        //AssetDatabase.SaveAssets(shaderVarints, )
    }
}
