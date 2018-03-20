/************************************************************
//     文件名      : GenAreaPack.cs
//     功能描述    : 地区小包制作
//     负责人      : jianing
//     参考文档    : 无
//     创建日期    : 2018-03-20 16:28:28.
//     Copyright   : Copyright 2018 DW Inc.
**************************************************************/

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UpdateDefineSpace;

namespace ThirtyEditor.Editor.AreaAB
{
    public class GenAreaPack : EditorWindow 
    {
        private string m_jsonFilePath;
        private ChildUpdateInfo m_versionInfo = null;

        [MenuItem("EZFun/制作地区包")]
        static void ShowWindow()
        {
            var wr = new Rect(0, 0, 500, 500);
            var w = (GenAreaPack)EditorWindow.GetWindow(typeof(GenAreaPack));
            w.title = "制作地区包";
            w.InitData();
            w.Show();
        }

        void InitData()
        {
            //m_versionInfo = new ChildUpdateInfo();
        }

        void Save()
        {
            var js = JsonMapper.ToJson(m_versionInfo);
            if (string.IsNullOrEmpty(m_jsonFilePath))
            {
                m_jsonFilePath = OpenFolderPanel("选择保存area_config.json文件路径", "", "");
                m_jsonFilePath += "/area_config.json";
            }
            File.WriteAllText(m_jsonFilePath, js);
        }

        protected void LoadVUFile(string path)
        {
            try
            {
                var s = File.ReadAllText(path);
                m_versionInfo = JsonMapper.ToObject<ChildUpdateInfo>(s);
            }
            catch (System.Exception)
            {
                m_versionInfo = new ChildUpdateInfo();
            }
        }

        //绘制窗口时调用
        Vector2 m_ScrollViewPos = Vector2.zero;
        void OnGUI()
        {
            m_ScrollViewPos = EditorGUILayout.BeginScrollView(m_ScrollViewPos, false, false, GUILayout.Height(Screen.height - 30));

            GUILayout.Label("当前游戏程序版本号:" + UnityEditor.PlayerSettings.bundleVersion);
            if (GUILayout.Button("选择area_config文件路径"))
            {
                m_jsonFilePath = EditorUtility.OpenFilePanel("选择area_config文件路径", "", "json");
                LoadVUFile(m_jsonFilePath);
            }

            if (m_versionInfo == null) 
            {
                m_versionInfo = new ChildUpdateInfo();
                m_versionInfo.dynamicUpdateInfo = new List<ChildeDynamicUpdateInfo>();
                ChildeDynamicUpdateInfo childeDynamicUpdateInfo = new ChildeDynamicUpdateInfo();
                childeDynamicUpdateInfo.resInfo = new List<ChildResInfo>();
                m_versionInfo.dynamicUpdateInfo.Add(childeDynamicUpdateInfo);
            }

            var s = new GUIStyle();
            s.richText = true;
            GUILayout.Label("");

            int areaID;
            if (int.TryParse(EditorGUILayout.TextField("    areaID:", m_versionInfo.areaID.ToString()), out areaID))
            {
                m_versionInfo.areaID = areaID;
            }
            m_versionInfo.localName = EditorGUILayout.TextField("  localName：", m_versionInfo.localName);
            m_versionInfo.baseVersion = EditorGUILayout.TextField("  baseVersion：", m_versionInfo.baseVersion);

            GUILayout.Label("---------更新包------------");
            var delGameVerIndex = new List<int>();
            for (int i = 0; m_versionInfo != null && m_versionInfo.dynamicUpdateInfo != null && i < m_versionInfo.dynamicUpdateInfo.Count; ++i)
            {
                var vui = m_versionInfo.dynamicUpdateInfo[i];
     
                int platform;
                vui.resVersion = EditorGUILayout.TextField("  兼容资源版本：", vui.resVersion);
                if (int.TryParse(EditorGUILayout.TextField("    平台:", vui.basePlatform.ToString()), out platform))
                {
                    vui.basePlatform = platform;
                }

                //for (int j = 0; j < vui.resInfo.Count; ++j)
                {
                    Action<string, string> ressCB = (string buttonName, string ressName) =>
                    {
                        EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb, GUILayout.ExpandWidth(true));
                        if (GUILayout.Button(buttonName, GUILayout.Width(120)))
                        {
                            ChildResInfo fi = null;
                            switch (ressName)
                            {
                                case "Table":
                                    fi = GenTableData();
                                    break;
                                case "AssetBundles":
                                    fi = GenAssetsBundle();
                                    break;
                                case "Lua":
                                    fi = GenLua();
                                    break;
                                case "Audio":
                                    fi = GenAudio();
                                    break;
                            }
                            if (fi != null)
                            {
                                AddUpdateInfo(ref vui, fi);
                            }
                        }
                        ChildResInfo ressInfo = vui.resInfo.Find((ChildResInfo findvalue) =>
                        {
                            return findvalue.type == ressName;
                        });

                        if (ressInfo != null)
                        {
                            ressInfo.md5 = EditorGUILayout.TextField("", ressInfo.md5);
                            if (string.IsNullOrEmpty(ressInfo.md5))
                            {
                                vui.resInfo.Remove(ressInfo);
                            }
                        }
                        else
                        {
                            EditorGUILayout.TextField("", "");
                        }
                        EditorGUILayout.EndHorizontal();
                    };

                    ressCB("选择TableData文件夹", "Table");
                    ressCB("选择AssetBundles文件夹", "AssetBundles");
                    ressCB("选择Lua文件夹", "Lua");
                    ressCB("选择Audio文件夹", "Audio");
                }
            }

            GUILayout.Label("");
            if (GUILayout.Button("保存"))
            {
                Save();
            }

            EditorGUILayout.EndScrollView();
        }

        void AddUpdateInfo(ref ChildeDynamicUpdateInfo rv, ChildResInfo updateInfo)
        {
            ChildResInfo value = rv.resInfo.Find((ChildResInfo findValue) =>
            {
                return findValue.type == updateInfo.type;
            });

            if (value != null)
            {
                rv.resInfo.Remove(value);
            }
            rv.resInfo.Add(updateInfo);
        }

        void OnInspectorUpdate()
        {
            //Debug.Log("窗口面板的更新");
            //这里开启窗口的重绘，不然窗口信息不会刷新
            this.Repaint();
        }

        void OnSelectionChange()
        {
            //当窗口出去开启状态，并且在Hierarchy视图中选择某游戏对象时调用
            foreach (Transform t in Selection.transforms)
            {
                //有可能是多选，这里开启一个循环打印选中游戏对象的名称
                Debug.Log("OnSelectionChange" + t.name);
            }
        }

        void OnDestroy()
        {
            Debug.Log("当窗口关闭时调用");
        }

////////////////////////////////////////////////////////////////////////////////////////////////
        static ChildResInfo GenTableData()
        {
            var targetPath = OpenFolderPanel("选择TableData文件夹", "", "");
            return GenFiles(targetPath, false);
        }

        static ChildResInfo GenLua()
        {
            var targetPath = OpenFolderPanel("选择Lua文件夹", "", "");
            return GenFiles(targetPath, true);
        }

        static ChildResInfo GenAudio()
        {
            var targetPath = OpenFolderPanel("选择Audio文件夹", "", "");
            return GenFiles(targetPath, false);
        }

        static ChildResInfo GenAssetsBundle()
        {
            var targetPath = OpenFolderPanel("选择AssetsBundle文件夹", "", "");
            return GenFiles(targetPath, false);
        }

        static ChildResInfo GenFiles(string inputPath, bool isCry)
        {
            if (inputPath == null || inputPath.Equals(""))
            {
                return null;
            }
            var fi = new ChildResInfo();
            var dir = inputPath.Split('/');
            //压缩出来的文件加个随机数
            int randomNum = UnityEngine.Random.Range(0, 999999999);
            fi.type = dir[dir.Length - 1];
            fi.packageName = fi.type + "_" + randomNum;

            ForeachFile(inputPath, (string f) =>
            {
                if (Path.GetExtension(f).Equals("meta"))
                {
                    File.Delete(f);
                }
                else
                {
                    if (isCry)
                    {
                        CryFile(f);
                    }
                }
            });

            var outPath = inputPath + "_" + randomNum + ".zip";
            if (File.Exists(outPath))
            {
                File.Move(outPath, outPath);
            }

            Debug.LogError("zipfile " + inputPath);
            DWTools.FastZipCompress(inputPath, outPath);

            fi.md5 = DWTools.GenMD5(outPath);
            fi.size = GetFileLength(outPath);

            return fi;
        }

        public static int GetFileLength(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return (int)fileInfo.Length;
        }

        public static void ForeachFile(string path, Action<string> genFile)
        {
            if (!Directory.Exists(path))
                return;

            foreach (var fileStr in Directory.GetFiles(path))
            {
                if (fileStr.Substring(fileStr.Length - 4) == "meta")
                {
                    File.Delete(fileStr);
                    continue;
                }
                genFile(fileStr);
            }
            foreach (var dirStr in Directory.GetDirectories(path))
            {
                if (Directory.Exists(dirStr))
                {
                    ForeachFile(dirStr, genFile);
                }
            }
        }

        public static string OpenFolderPanel(string title, string folder, string defaultName)
        {
            var targetPath = EditorUtility.OpenFolderPanel(title, folder, defaultName);
            if (string.IsNullOrEmpty(targetPath))
            {
                return null;
            }
            return targetPath;
        }
        static void CryFile(string path)
        {
            Debug.LogWarning("cry path =" + path);
            var b = DWFileUtil.ReadFileStream(path);

            GlobalCrypto.InitCry("dd7fd4a156d28bade96f816db1d18609", "dd7fd4a156d28bade96f816db1d18609");
            var cb = GlobalCrypto.Encrypte(b);

            File.WriteAllBytes(path, cb);
        }
    }
}
