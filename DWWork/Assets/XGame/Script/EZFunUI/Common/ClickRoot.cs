/************************************************************
//     文件名      : ClickWindow.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2016-09-21 18:05:49.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;

public class ClickRoot : BaseUI {


    protected override void HandleWidgetClick(GameObject gameObj)
    {
        if (gameObj == null)
        {
            return;
        }
    }

}
