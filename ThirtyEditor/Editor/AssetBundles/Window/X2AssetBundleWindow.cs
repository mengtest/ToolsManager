/************************************************************
//     文件名      : X2AssetBundleWindow.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-07-20 12:28:42.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

public class X2AssetBundleWindow : EditorWindow
{

    [MenuItem("EZFun/X2Bundle/BundleBuildWin")]
    public static void Create()
    {
        X2AssetBundleWindow bundleManagerWin = EditorWindow.GetWindow<X2AssetBundleWindow>();
        bundleManagerWin.minSize = new Vector2(1000, 800);
    }

    void OnEnable()
    {
        if (m_bundleDataViewer == null)
        {
            //BundleDataManager.Init();
            m_bundleDataViewer = new X2BundleViewer(this);
        }
    }

    void OnGUI()
    {
        if (m_bundleDataViewer != null)
        {
            Rect viewRect = new Rect(0, 10, position.width, position.height - 10);
            m_bundleDataViewer.Draw(viewRect);
        }
    }

    private X2BundleViewer m_bundleDataViewer = null;
}
