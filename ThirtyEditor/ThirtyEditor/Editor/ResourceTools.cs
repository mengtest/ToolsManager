/************************************************************
//     文件名      : ResourceTools.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-07-18 12:20:07.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.IO;
using LitJson;

public class ResourceTools
{

    [MenuItem("EZFun/Resource/GenResourceList")]
    public static void GenResourceList()
    {
        var path = Application.dataPath + "/XGame/Resources/";

        var dir = new DirectoryInfo(path);
        var fileList = dir.GetFiles("*.*", SearchOption.AllDirectories);
        JsonWriter writer = new JsonWriter();
        writer.WriteArrayStart();
        for (int i = 0; i < fileList.Length; i++)
        {
            if (!fileList[i].Name.EndsWith(".meta"))
            {
                var str = fileList[i].FullName.Replace('\\', '/');
                writer.Write(str.Substring(str.IndexOf("Assets")));
            }
        }
        writer.WriteArrayEnd();
        var fStr = writer.ToString();
        using (Stream file = File.Create(Application.streamingAssetsPath + "/Resources.json"))
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(fStr);
            file.Write(bytes, 0, bytes.Length);
            file.Close();
        }
    }
}
