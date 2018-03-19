/************************************************************
//     文件名      : FileUtil.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2016-12-09 15:13:09.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;

public class EZFunFileUtil
{

    public static byte[] ReadFile(string filename)
    {
        int index = filename.IndexOf("/assets/");
        if (index > 0)
            filename = filename.Substring(index + "/assets/".Length);
        int strlen = LoadFileSize(filename);
        if (strlen == -1)
        {
            Debug.LogError("xgame2  nulll" + filename);
            return null;
        }
        byte[] data = new byte[strlen];
        LoadFileByData(filename, data);
        return data;
    }

    public static bool IsExsits(string filename)
    {
        int index = filename.IndexOf("/assets/");
        if (index > 0)
            filename = filename.Substring(index + "/assets/".Length);
        int strlen = LoadFileSize(filename);
        return strlen != -1;
    }

    [DllImport("XGame2File", CallingConvention = CallingConvention.Cdecl)]
    public static extern void LoadFileByData(string fileName, byte[] data);

    [DllImport("XGame2File", CallingConvention = CallingConvention.Cdecl)]
    public static extern int LoadFileSize(string fileName);

}
