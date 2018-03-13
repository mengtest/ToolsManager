

/************************************************************
//     文件名      : ReportUnableUseMaterialName.cs
//     功能描述    : 打印所有不能用的材质的名称
//     负责人      : lezen   lezenzengr@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-06-24 16:38:15.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;
using Core.CAssetBundleData;
using LitJson;
using System.Security.Cryptography;
using System.Text;
public class ReportUnableUseMaterialName : MonoBehaviour
{
    #region report matiral name
    private static Dictionary<string, ABAssetInfo> m_dependsDic = new Dictionary<string, ABAssetInfo>();
    [MenuItem("EZFun/ReportAllCanNotUse MaterialName")]
    public static void ReportAllMaterial()
    {
        m_dependsDic.Clear();
        CollectFromDirectinfo_Report("XGame/Scene", false);
        CollectFromDirectinfo_Report("XGame/Resources", false);
        foreach (var keyvalue in m_dependsDic)
        {
            var cacheData = keyvalue.Value.ToCacheData();
            for (int i = 0; i < cacheData.depends.Length; i++)
            {
                ReportMaterial(cacheData.depends[i]);
            }
        }
        m_dependsDic.Clear();
    }
    private static List<string> unityBuildInShaderNameList = new List<string>(){
        "Standard",
        "Legacy Shaders/"

    };
    static void ReportMaterial(string prefabPath)
    {
        var data = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(UnityEngine.Object));
        if (prefabPath.EndsWith(".prefab"))
        {
            GameObject gb = data as GameObject;
            MeshRenderer[] mr = gb.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < mr.Length; i++)
            {
                var materials = mr[i].sharedMaterials;
                for (int j = 0; j < materials.Length; j++)
                {
                    if (CheckNeedReport(materials[j].shader.name))
                        Debug.LogError(gb.name + " ->  " + "material name  ->  " + materials[j].shader.name);
                }
            }
        }
        else if (prefabPath.EndsWith(".mat"))
        {
            Material material = data as Material;
            if (CheckNeedReport(material.shader.name))
                Debug.LogError(prefabPath + " ->  " + " material name  ->  " + material.shader.name);
        }

    }
    static bool CheckNeedReport(string shaderName)
    {
        for (int i = 0; i < unityBuildInShaderNameList.Count; i++)
        {
            if (shaderName.Contains(unityBuildInShaderNameList[i]))
                return true;
        }
        return false;
    }
    static void CollectFromDirectinfo_Report(string path,
        bool isUpdate = false, string endWith = "prefab")
    {
        DirectoryInfo direc;
        direc = new DirectoryInfo(Application.dataPath + "/" + path);
        if (!direc.Exists)
        {
            return;
        }
        FileInfo[] fileArray = direc.GetFiles("*.*", SearchOption.AllDirectories);
        for (int fileIndex = 0; fileArray != null && fileIndex < fileArray.Length; fileIndex++)
        {
            FileInfo file = fileArray[fileIndex];
            string fileName = file.FullName.Replace("\\", "/");
            string prefabName = fileName.Substring(fileName.IndexOf("Assets"));
            if (prefabName.Contains("GameRoot.unity"))
            {
                continue;
            }
            if (prefabName.EndsWith(endWith) || prefabName.EndsWith("mat")
                || prefabName.EndsWith("ttf"))
            {
                ColloectDependecies_Report(prefabName, file.Name);
            }
        }
    }

    private static ABAssetInfo ColloectDependecies_Report(string prefabPath, string fileName = "")
    {
        prefabPath = prefabPath.ToLower();
        string[] depends = AssetDatabase.GetDependencies(prefabPath, false);
        if (prefabPath.EndsWith(".unity"))
        {
            if (fileName.Contains("."))
            {
                fileName = fileName.Substring(0, fileName.IndexOf("."));
            }
        }
        ABAssetInfo parentAssetInfo = null;
        if (!m_dependsDic.ContainsKey(prefabPath))
        {
            parentAssetInfo = X2AssetsBundleEditor.LoadAssetData(prefabPath);
            m_dependsDic.Add(prefabPath, parentAssetInfo);
            if (depends != null)
            {
                for (int i = 0; i < depends.Length; i++)
                {
                    depends[i] = depends[i].ToLower().Replace("\\", "/");
                    if (!depends[i].EndsWith("dll") && !depends[i].EndsWith("cs") && !depends[i].EndsWith("js"))
                    {
                        var childAssetInfo = ColloectDependecies_Report(depends[i]);
                        parentAssetInfo.AddChild(childAssetInfo);

                    }
                }
            }
        }
        else
        {
            parentAssetInfo = m_dependsDic[prefabPath];
        }
        return parentAssetInfo;
    }

    #endregion

}
