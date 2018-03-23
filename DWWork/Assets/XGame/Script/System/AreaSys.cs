/************************************************************
//     文件名      : AreaSys.cs
//     功能描述    : 区域小包资源
//     负责人      : corey
//     参考文档    : 无
//     创建日期    : 2018-03-20 11:34:15.
//     Copyright   : Copyright 2018 DW Inc.
**************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RegisterSystem(typeof(AreaSys))]
public class AreaSys : TCoreSystem<AreaSys>, IInitializeable
{
    public static string AreaRootName = "Area"; //根目录名称
    private static string _NowAreaName = ""; //当前选中的地区玩法
    public static string NowAreaName
    {
        get
        {
            return _NowAreaName;
        }
        set
        {
            _NowAreaName = value;
            if (string.IsNullOrEmpty(_NowAreaName))
            {
                ResourceManager.Instance.RemoveAreaAbDatas();
            }
            else
            {
                ResourceManager.Instance.AddAreaAbDatas(_NowAreaName);
            }

            PlayerPrefs.SetString("NowAreaName", _NowAreaName);
        }
    }

    public void Init()
    {
        string preNowAreaName = PlayerPrefs.GetString("NowAreaName", "");
        NowAreaName = preNowAreaName;
        //先测试
        //NowAreaName = "DouDiZhu";
    }

    public void Release() 
    {
        
    }
}
