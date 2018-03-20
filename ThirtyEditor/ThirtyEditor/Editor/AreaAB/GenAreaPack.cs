/************************************************************
//     文件名      : GenAreaPack.cs
//     功能描述    : 地区小包AB包制作
//     负责人      : jianing
//     参考文档    : 无
//     创建日期    : 2018-03-15 16:28:28.
//     Copyright   : Copyright 2018 DW Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;
using Core.CAssetBundleData;
using LitJson;
using System.Security.Cryptography;
using System.Text;

namespace ThirtyEditor.Editor.AreaAB
{
    public class GenAreaPack
    {
        private static ABAssetDataList m_currentCache;
        private static BundleID m_curretBundleID;
        private static Dictionary<BundleID, ABBundleInfo> m_abbuildDic = new Dictionary<BundleID, ABBundleInfo>();
        private static Dictionary<string, ABAssetInfo> m_dependsDic = new Dictionary<string, ABAssetInfo>();
        private static int newVersion = 1000;

        private static string nowSelectPath = "";
        private static string areaName = "";

        [MenuItem("EZFun/Area/BuildAssetAndroid")]
        public static void BuildAssetBundleAndroid()
        {
            nowSelectPath = EditorUtility.OpenFolderPanel("选择文件夹", "", "");
            CollectionAllAssets(BuildTarget.Android);
        }

        [MenuItem("EZFun/Area/BuildAssetIOS")]
        public static void BuildAssetBundleIOS()
        {
            nowSelectPath = EditorUtility.OpenFolderPanel("选择文件夹", "", "");
            CollectionAllAssets(BuildTarget.iOS);
        }

        public static void CollectionAllAssets(BuildTarget buildTargetGroup)
        {
            m_abbuildDic.Clear();
            m_dependsDic.Clear();
            m_currentCache = new ABAssetDataList();
            m_curretBundleID = new BundleID(newVersion, EnumAB.ui_ab);
            //nowSelectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            //nowSelectPath = nowSelectPath.Replace("Assets", "");
            areaName = nowSelectPath.Substring(nowSelectPath.LastIndexOf("/") + 1);

            CollectFromDirectinfo(nowSelectPath);
            ProcessCacheDic();
            BuildAssetBuindles(buildTargetGroup);
        }

        public static void ProcessCacheDic(bool isUpdate = false)
        {
            List<ABAssetData> shaderlist = new List<ABAssetData>();

            foreach (var keyvalue in m_abbuildDic)
            {
                keyvalue.Value.fileList.Clear();
            }

            foreach (var keyvalue in m_dependsDic)
            {
                if (m_abbuildDic.ContainsKey(keyvalue.Value.m_cacheData.bundleId))
                {
                    var buildInfo = m_abbuildDic[keyvalue.Value.m_cacheData.bundleId];
                    if (buildInfo != null)
                    {
                        AddAsset(buildInfo, keyvalue.Value.m_cacheData);
                    }
                }
            }
            foreach (var keyvalue in m_dependsDic)
            {
                if (!m_abbuildDic.ContainsKey(keyvalue.Value.m_cacheData.bundleId))
                {
                    var ab = new ABBundleInfo();
                    ab.bundleId = keyvalue.Value.m_cacheData.bundleId;
                    m_abbuildDic.Add(keyvalue.Value.m_cacheData.bundleId, ab);
                }
                m_abbuildDic[keyvalue.Value.m_cacheData.bundleId].fileList.Add(keyvalue.Key);

            }
        }

        private static void AddAsset(ABBundleInfo buildInfo, ABAssetData cacheData)
        {
            if (buildInfo != null && cacheData != null
                && buildInfo.bundleId.version >= cacheData.version)
            {
                cacheData.bundleId = buildInfo.bundleId;
            }
        }

        public static void BuildAssetBuindles(BuildTarget buildTargetGroup)
        {
            List<AssetBundleBuild> buildsList = new List<AssetBundleBuild>();
            foreach (var keyValue in m_abbuildDic)
            {
                buildsList.Add(keyValue.Value.GetBundleBuild());
            }
            BuildAssetBundleOptions m_options = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression;
            var assetBundleManifest = BuildPipeline.BuildAssetBundles(PreOutPath(buildTargetGroup), buildsList.ToArray(),
               m_options, buildTargetGroup);

            ABDesc structs = new ABDesc();
            AddResrouceToAbDataStruct(assetBundleManifest, structs);

            m_currentCache.newVersion = newVersion;
            ////1存储ResourceManager的描述文件，
            SaveABDataFile(structs, buildTargetGroup);

            m_currentCache.assetList.Clear();
            foreach (var keyvalue in m_dependsDic)
            {
                m_currentCache.assetList.Add(keyvalue.Value.ToCacheData());
            }
            SaveAssetCache(buildTargetGroup, m_currentCache);
        }

        static void SaveABDataFile(ABDesc abData, BuildTarget buildTargetGroup)
        {
            string outPath = PreOutPath(buildTargetGroup);
            string fileName = "ABDatabase";
            Directory.CreateDirectory(outPath);
            string path = outPath + "/" + fileName + ".abMap";
            string json = JsonMapper.ToJson(abData);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (Stream file = File.Create(path))
            {
                file.Write(bytes, 0, bytes.Length);
                file.Close();
            }
        }

        private static void AddResrouceToAbDataStruct(AssetBundleManifest manifest, ABDesc structs)
        {
            foreach (var build in m_abbuildDic)
            {
                foreach (string fileName in build.Value.fileList)
                {
                    if (fileName.EndsWith("unity"))
                    {
                        ABSceneDesc sceneData = new ABSceneDesc();
                        sceneData.m_sceneName = fileName;
                        sceneData.m_bundleName = build.Value.BundleName;
                        sceneData.m_ressVersion = build.Value.bundleId.version;
                        structs.m_scenes.Add(sceneData);
                    }
                    else if (
                        fileName.EndsWith(".prefab")
                        //|| fileName.EndsWith("shader") 
                        || fileName.EndsWith("cginc")
                        || fileName.EndsWith("ttf")
                        || fileName.EndsWith("shadervariants")
                        || fileName.EndsWith("png"))
                    {
                        ABFileDesc sceneData = new ABFileDesc();
                        sceneData.m_gbName = fileName;
                        sceneData.m_bundleName = build.Value.BundleName;
                        sceneData.m_ressVersion = build.Value.bundleId.version;
                        structs.m_files.Add(sceneData);
                    }
                }
            }
            var data = manifest.GetAllAssetBundles();
            for (int i = 0; i < data.Length; i++)
            {
                ABDependencies ad = new ABDependencies();
                var abs = manifest.GetAllDependencies(data[i]);
                ad.m_abName = data[i];
                ad.m_ressVersion = newVersion;

                for (int j = 0; j < abs.Length; j++)
                {
                    ad.m_dependenciesAb.Add(abs[j]);
                }
                structs.m_abDepends.Add(ad);
            }
        }

        /// <summary>
        /// 收集资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isUpdate"></param>
        /// <param name="endWith"></param>
        static void CollectFromDirectinfo(string path)
        {
            DirectoryInfo direc;
            direc = new DirectoryInfo(path);
            if (!direc.Exists)
            {
                direc = new DirectoryInfo(path);
                if (!direc.Exists)
                {
                    return;
                }
            }
            FileInfo[] fileArray = direc.GetFiles("*.*", SearchOption.AllDirectories);
            if (fileArray == null)
            {
                return;
            }

            for (int fileIndex = 0; fileIndex < fileArray.Length; fileIndex++)
            {
                FileInfo file = fileArray[fileIndex];
                string fileName = file.FullName.Replace("\\", "/");
                string prefabName = fileName.Substring(fileName.IndexOf("Assets"));
                if (prefabName.Contains(".unity")
                    || prefabName.EndsWith(".meta"))
                {
                    continue;
                }

                if (prefabName.EndsWith("cginc")
                    || prefabName.EndsWith("ttf")
                    || prefabName.EndsWith("shadervariants")
                    || prefabName.EndsWith("png")
                    || prefabName.EndsWith("prefab")
                    //||prefabName.EndsWith("shader")
                    )
                {
                    ColloectDependecies(prefabName);
                }
            }
        }

        private static ABAssetInfo ColloectDependecies(string prefabPath)
        {
            if (!m_abbuildDic.ContainsKey(m_curretBundleID))
            {
                ABBundleInfo buildInfo = new ABBundleInfo();
                buildInfo.bundleId = m_curretBundleID;
                buildInfo.firstVersion = newVersion;
                m_abbuildDic.Add(m_curretBundleID, buildInfo);
            }

            prefabPath = prefabPath.ToLower();
            string[] depends = AssetDatabase.GetDependencies(prefabPath, false);

            ABAssetInfo parentAssetInfo = null;
            if (!m_dependsDic.ContainsKey(prefabPath))
            {
                parentAssetInfo = LoadAssetData(prefabPath);
                m_dependsDic.Add(prefabPath, parentAssetInfo);
                if (depends != null)
                {
                    for (int i = 0; i < depends.Length; i++)
                    {
                        depends[i] = depends[i].ToLower().Replace("\\", "/");
                        if (!depends[i].EndsWith("dll") && !depends[i].EndsWith("cs") && !depends[i].EndsWith("js"))
                        {
                            var childAssetInfo = ColloectDependecies(depends[i]);
                            parentAssetInfo.AddChild(childAssetInfo);
                        }
                    }
                }
            }
            else
            {
                parentAssetInfo = m_dependsDic[prefabPath];
            }
            return parentAssetInfo;
        }

        public static ABAssetInfo LoadAssetData(string prefabPath)
        {
            string[] depends = AssetDatabase.GetDependencies(prefabPath, true);
            ABAssetData assetCache = new ABAssetData();
            assetCache.filePath = prefabPath;
            assetCache.fileHash = HashUtil.GetByPath(Application.dataPath + "/../" + prefabPath);
            assetCache.depends = depends;
            assetCache.bundleId = m_curretBundleID;
            return new ABAssetInfo(assetCache);
        }

        public static void BuildAllAssetBundle(BuildTarget buildTargetGroup)
        {
            List<AssetBundleBuild> buildsList = new List<AssetBundleBuild>();
            foreach (var keyValue in m_abbuildDic)
            {
                buildsList.Add(keyValue.Value.GetBundleBuild());
            }
            BuildAssetBundleOptions m_options = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression;
            var assetBundleManifest = BuildPipeline.BuildAssetBundles(PreOutPath(buildTargetGroup), buildsList.ToArray(),
               m_options, buildTargetGroup);
        }

        public static string PreOutPath(BuildTarget buildTargetGroup)
        {
            var path = Application.dataPath + "/../Area/" + areaName + "/" + buildTargetGroup.ToString();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static void SaveAssetCache(BuildTarget buildTarget, ABAssetDataList cache)
        {
            var data = LitJson.JsonMapper.ToJson(cache);
            var path = PreOutPath(buildTarget);
            using (Stream file = File.Create(path + "/AssetDatas.json"))
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(data);
                file.Write(bytes, 0, bytes.Length);
                file.Close();
            }
        }

    }
}
