/************************************************************
//     文件名      : X2BundleControl.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-07-20 12:33:15.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Collections.Generic;
public enum ABTargetGroup
{
    Android,
    Window,
    iOS,
}
public class X2BundleDataControl {

    private static X2BundleDataControl m_instance;

    public static X2BundleDataControl Instance {
        get
        {
            if (m_instance == null)
            {
                m_instance = new X2BundleDataControl();
            }
            return m_instance;
        }
    }

    private TableView m_bundleTable;
    private TableView m_assetTable;
    EditorWindow m_editorWindow;

    ABAssetInfo texInfo;
    public void SetViewer(TableView bundleTable, TableView assetTable, EditorWindow editorWindow)
    {
        m_bundleTable = bundleTable;
        m_assetTable = assetTable;
        this.m_editorWindow = editorWindow;
        RefreshDataWithSelect();
    }


    private List<object> m_abList = new List<object>();

    public void RefreshDataWithSelect()
    {
        m_abList.Clear();
        X2AssetsBundleEditor.GetBundleList(m_abList);
        m_bundleTable.RefreshData(m_abList);
    }

    public void OnBundleSelected(object selected, int col)
    {
        ABBundleInfo bundleData = selected as ABBundleInfo;
        if (bundleData == null)
            return;

        if (m_assetTable != null)
        {
            List<object> list = new List<object>();
            var enemerator = bundleData.fileList.GetEnumerator();
            while(enemerator.MoveNext())
            {
                var pathInfo = X2AssetsBundleEditor.GetABAsset(enemerator.Current);
                if (pathInfo != null)
                {
                    list.Add(pathInfo);
                }
            }
            m_assetTable.RefreshData(list);
        }
    }


    public void OnAssetSelected(object selected, int col)
    {
        texInfo = selected as ABAssetInfo;
        if (texInfo == null)
            return;
        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(texInfo.AssetPath, typeof(UnityEngine.Object));
        if (obj != null)
        {
            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
        }
        InitChildrenOrParent();
        InitChildrenOrParentData();
    }

    private int m_currentVersion = 10000;
    private bool m_isUpdate;
    private ABTargetGroup m_abbuildTarget;
    private BuildTarget m_targetGroup;


    private int m_state = 0;

    private int state
    {
        get { return m_state; }
        set
        {

            if(value == 3)
            {
                //收集资源
                state = 1;
                state = 2;
                X2AssetsBundleEditor.BuildAssetBuindles(m_targetGroup, m_currentVersion);
                m_state = 3;
            }
            else
            if (value == 2)
            {
                //收集资源
                state = 1;
                X2AssetsBundleEditor.ProcessCacheDic(m_isUpdate);
                RefreshDataWithSelect();
                m_state = 2;
            }
            else if (value == 1)
            {
                X2AssetsBundleEditor.LoadAndVerions(m_targetGroup, m_isUpdate, m_currentVersion);
                //收集资源
                X2AssetsBundleEditor.CollectionAllAssets(m_isUpdate);
                RefreshDataWithSelect();
                m_state = 1;
            }
            else
            {
                m_state = 0;
            }
        }
    }
    public void Draw()
    {
        GUILayout.BeginHorizontal(TableStyles.Toolbar);
        {
            GUILayout.Label("Version: ", GUILayout.Width(60));
            m_currentVersion = EditorGUILayout.IntField(m_currentVersion, TableStyles.TextField, GUILayout.Width(150));
            float spacePixel = 10.0f;
            GUILayout.Space(spacePixel);
            var isUpdate = GUILayout.Toggle(m_isUpdate, " IsUpdate");
            if (isUpdate != m_isUpdate)
            {
                m_isUpdate = isUpdate;
                state = state;
            }
            var target = (ABTargetGroup)EditorGUILayout.EnumPopup(m_abbuildTarget);
          
            switch (target)
            {
                case ABTargetGroup.Android:
                    m_targetGroup = BuildTarget.Android;
                    break;
                case ABTargetGroup.iOS:
                    m_targetGroup = BuildTarget.iOS;
                    break;
                case ABTargetGroup.Window:
                    m_targetGroup = BuildTarget.StandaloneWindows64;
                    break;
            }
            if (m_abbuildTarget != target)
            {
                m_abbuildTarget = target;
                state = state;
            }
            if (GUILayout.Button("Load", TableStyles.ToolbarButton, GUILayout.MaxWidth(140)))
            {
                state = 1;
            }
            if (GUILayout.Button("Process Ab Files", TableStyles.ToolbarButton, GUILayout.MaxWidth(140)))
            {
                state = 2;
            }

            if (GUILayout.Button("Build AssetBundles", TableStyles.ToolbarButton, GUILayout.MaxWidth(140)))
            {
                state = 3;
            }

            if (GUILayout.Button("Clear All", TableStyles.ToolbarButton, GUILayout.MaxWidth(140)))
            {
                state = 0;
                X2AssetsBundleEditor.Clear();
            }
            GUILayout.FlexibleSpace();
         }
        GUILayout.EndHorizontal();
    }


    public void DrawFileInfo(Rect rect)
    {
        GUILayout.BeginArea(rect);
        if (texInfo != null)
        {
            float spacePixel = 10.0f;
            GUILayout.BeginHorizontal(TableStyles.ToolbarButton);
            {
                GUILayout.Label("FilePath:", GUILayout.Width(100));
                GUILayout.Space(spacePixel);
                GUILayout.Label(texInfo.AssetPath);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(TableStyles.ToolbarButton);
            {
                GUILayout.Label("HashCode:", GUILayout.Width(100));
                GUILayout.Space(spacePixel);
                GUILayout.Label(texInfo.m_cacheData.fileHash);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(TableStyles.ToolbarButton);
            {
                GUILayout.Label("OldHashCode:", GUILayout.Width(100));
                GUILayout.Space(spacePixel);
                string oldFileHash = texInfo.m_cacheData.fileHash;
                var oldData = X2AssetsBundleEditor.GetOldData(texInfo.AssetPath);
                if (oldData != null )
                {
                    oldFileHash = oldData.fileHash;
                }
                GUILayout.Label(oldFileHash);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(TableStyles.Toolbar);
            GUILayout.Space(spacePixel);
            texInfo.m_isIgnore = GUILayout.Toggle(texInfo.m_isIgnore, "是否忽略这个文件md5不一致");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(TableStyles.ToolbarButton);
            {
                GUILayout.Label("Version:", GUILayout.Width(80));
                GUILayout.Space(spacePixel);
                string oldFileHash = texInfo.m_cacheData.fileHash;
                var oldData = X2AssetsBundleEditor.GetOldData(texInfo.AssetPath);
                if (oldData != null)
                {
                    oldFileHash = oldData.fileHash;
                }
                GUILayout.Label(texInfo.m_cacheData.version.ToString(), GUILayout.Width(100));
                GUILayout.Space(spacePixel);
                GUILayout.Label("BundleType:", GUILayout.Width(80));
                GUILayout.Space(spacePixel);
                GUILayout.Label(texInfo.m_cacheData.bundleId.buildType.ToString(), GUILayout.Width(100));
            }

            var r = rect;
            int border = 10;
            float splitH = 0.8f;
            float splitW = 0.5f;
            int toolbarHeight = 80;
            int startY = toolbarHeight + border;
            int height = (int)(r.height - startY - border * 2);
            if (m_childrenTable != null)
            {
                m_childrenTable.Draw(new Rect(border, startY,
                   (int)(r.width * splitW) - border * 1.5f, 
                   (int)(height * splitH - border * 1.5f)));
            }

            if (m_parentTable != null)
            {
                m_parentTable.Draw(new Rect((int)(r.width * splitW) + border * 0.5f, startY,
                    (int)(r.width * splitW) - border * 1.5f,
                    (int)(height * (splitH) - border * 1.5f)));
            }

        }
        GUILayout.EndArea();
    }


    private TableView m_childrenTable;
    private TableView m_parentTable;
    private List<object> m_children = new List<object>();
    private List<object> m_parent = new List<object>();

    private void InitChildrenOrParent()
    {
        if (m_childrenTable != null)
        {
            return;
        }
        m_childrenTable = new TableView(m_editorWindow, typeof(ABAssetInfo));
        m_childrenTable.AddColumn("AssetPath", "ChildrenFiles", 0.7f);
        m_childrenTable.AddColumn("IsCaseUpdate", "Case Update", 0.15f);
        m_childrenTable.AddColumn("m_hasChildrenUpdate", "Child Update", 0.15f);
        m_childrenTable.OnSelected += OnAssetSelected;
        m_parentTable = new TableView(m_editorWindow, typeof(ABAssetInfo));
        m_parentTable.AddColumn("AssetPath", "ParentFiles", 0.8f);
        m_childrenTable.AddColumn("IsCaseUpdate", "Case Update", 0.2f);
        m_parentTable.OnSelected += OnAssetSelected;
    }

    private void InitChildrenOrParentData()
    {
        m_children.Clear();
        m_parent.Clear();
        if (texInfo!= null)
        {
            var enumerator = texInfo.m_children.GetEnumerator();
            while(enumerator.MoveNext())
            {
                m_children.Add(enumerator.Current);
            }
            enumerator = texInfo.m_parent.GetEnumerator();
            while (enumerator.MoveNext())
            {
                m_parent.Add(enumerator.Current);
            }
        }
        m_childrenTable.RefreshData(this.m_children);
        m_parentTable.RefreshData(this.m_parent);
    }
}
