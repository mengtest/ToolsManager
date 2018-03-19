/************************************************************
//     文件名      : CheckUnityPro.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-08-10 20:12:46.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CheckUnityPro : MonoBehaviour
{

    [MenuItem("EZFun/Meta/CheckPro")]
    public static void CheckPro()
    {

        var path = Application.dataPath;

        var dir = new DirectoryInfo(path);

        var files = dir.GetFiles("*.meta", SearchOption.AllDirectories);
        Dictionary<string, string> fileList = new Dictionary<string, string>();

        List<string> contextStr = new List<string>();
        for (int i = 0; i < files.Length; i++)
        {
            using (var s = new FileStream(files[i].FullName, FileMode.Open))
            {
                if (files[i].FullName.Contains(".cs.") || files[i].FullName.Contains("Script") || files[i].FullName.Contains("Plugins")
                    || files[i].FullName.Contains("GameStartScrpits"))
                {
                    continue;
                }
                contextStr.Clear();
                var reder = new StreamReader(s);
                var text = "";
                bool isContainsPro = false;
                while (!string.IsNullOrEmpty((text = reder.ReadLine())))
                {
                    if (text.Contains("Pro") || text.Contains("folderAsset: yes"))
                    {
                        isContainsPro = true;
                        break;
                    }
                    contextStr.Add(text);
                }
                reder.Close();
                if (isContainsPro)
                {
                    continue;
                }
                string content = contextStr[0] + "\n";
                content += "licenseType: Pro\n";
                for(int m = 1; m < contextStr.Count; m ++)
                {
                    content += contextStr[m] + "\n";
                }
                fileList.Add(files[i].FullName, content);
            }
        }
        var enumerator = fileList.GetEnumerator();
        while (enumerator.MoveNext())
        {
            using (var s = new FileStream(enumerator.Current.Key, FileMode.OpenOrCreate))
            {
                var reder = new StreamWriter(s);
                reder.Write(enumerator.Current.Value);
                reder.Flush();
                reder.Close();
            }
        }
    }
}
