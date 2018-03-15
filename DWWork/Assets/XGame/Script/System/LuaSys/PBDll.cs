/************************************************************
//     文件名      : PBDll.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-04-10 19:41:17.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class PBDLL
{
#if UNITY_EDITOR
        const string LUADLL = "ulua";
#elif UNITY_IPHONE
        const string LUADLL = "__Internal";
#else
    const string LUADLL = "ulua";
#endif
    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int RegisterLuaPB(IntPtr luaState);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Lpb_load_byte(IntPtr luaState, byte[] bytes, int length);
}
