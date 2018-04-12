/************************************************************
//     文件名      : IPack.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2016-12-02 17:13:27.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;

public interface IPack
{
    int ErrorID { get; set; }
    void UnPack(byte[] data, int offset, int length);
    byte[] PackMsg();
    bool HasData { get; }
}
