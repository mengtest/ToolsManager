/************************************************************
//     文件名      : X2AssetsBundleEditor.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-02-20 11:37:53.
//     Copyright   : Copyright 2016 EZFun Inc.
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



public struct BundleID
{
    public EnumAB buildType;
    public int version;
    public string sceneName;

    public BundleID(int version, EnumAB bundeType, string tsceneName = "")
    {
        this.buildType = bundeType;
        this.version = version;
        this.sceneName = tsceneName;
    }

    public override int GetHashCode()
    {
        return (int)buildType * 100000 + version + sceneName.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj is BundleID)
        {
            var temp = (BundleID)obj;
            return temp.buildType == this.buildType && temp.version == this.version && this.sceneName == temp.sceneName;
        }
        return false;
    }
}
/// 
/// AB 内部关系不加后缀，但是实际文件有后缀，其实ab之间引用关系之间也不是以文件名为依据，而是以之前
/// 注意，在StreamingAssets目录下的ab是后后缀的，因为android打gradlew需要后缀区分那些文件压缩
public class X2AssetsBundleEditor : MonoBehaviour
{

    private static ABAssetDataList m_currentCache;

    private static Dictionary<string, ABAssetData> m_oldDic = new Dictionary<string, ABAssetData>();

    private static Dictionary<string, ABAssetInfo> m_dependsDic = new Dictionary<string, ABAssetInfo>();

    public static Dictionary<string, bool> m_isIgnoreDic = new Dictionary<string, bool>();

    private static int m_curVersion = 0;

    private static int m_firstVersion = 0;

    private static BundleID m_curretBundleID;

    private static bool m_isUpdateShader = false;

    private static Dictionary<BundleID, ABBundleInfo> m_abbuildDic = new Dictionary<BundleID, ABBundleInfo>();

    [MenuItem("EZFun/X2Bundle/All/BuildAssetBundleStandalone")]
    public static void BuildAllAssetBundle()
    {
        BuildAllAssetBundle(BuildTarget.StandaloneWindows);
    }

    [MenuItem("EZFun/X2Bundle/All/BuildAssetBundleiOS")]
    public static void BuildAllAssetBundleiOS()
    {
        BuildAllAssetBundle(BuildTarget.iOS);
    }


    [MenuItem("EZFun/X2Bundle/All/BuildAssetBundleAndroid")]
    public static void BuildAllAssetBundleAndroid()
    {
        BuildAllAssetBundle(BuildTarget.Android);
    }

    [MenuItem("EZFun/X2Bundle/Update/BuildAssetBundleStandalone")]
    public static void BuildUpdateAssetBundle()
    {
        BuildAllAssetBundle(BuildTarget.StandaloneWindows, true);
    }

    [MenuItem("EZFun/X2Bundle/Update/BuildAssetBundleiOS")]
    public static void BuildUpdateAssetBundleiOS()
    {
        BuildAllAssetBundle(BuildTarget.iOS, true);
    }


    [MenuItem("EZFun/X2Bundle/Update/BuildAssetBundleAndroid")]
    public static void BuildUpdateAssetBundleAndroid()
    {
        BuildAllAssetBundle(BuildTarget.Android, true);
    }


    [MenuItem("EZFun/X2Bundle/TestRedundancy")]
    public static void TestRedundancy()
    {
        string dirPath = EditorUtility.OpenFolderPanel("选择AB所在文件夹", Application.dataPath, "");
        if (string.IsNullOrEmpty(dirPath))
        {
            return;
        }
        var dir = new DirectoryInfo(dirPath);

        var files = dir.GetFiles("*.*", SearchOption.AllDirectories);

        Dictionary<string, List<string>> m_fileAbDic = new Dictionary<string, List<string>>();
        Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();
        foreach (var file in files)
        {
            var ab = AssetBundle.LoadFromFile(file.FullName);
            if (ab != null)
            {
                bundles.Add(file.Name, ab);
            }
        }

        foreach (var ab in bundles)
        {
            var allAssets = ab.Value.GetAllAssetNames();
            foreach (var asset in allAssets)
            {
                if (!m_fileAbDic.ContainsKey(asset))
                {
                    m_fileAbDic.Add(asset, new List<string>());
                }
                m_fileAbDic[asset].Add(ab.Key);
            }
            ab.Value.Unload(true);
        }

        foreach (var fileEntry in m_fileAbDic)
        {
            if (fileEntry.Value.Count > 1)
            {
                var abStr = "";
                foreach (var abName in fileEntry.Value)
                {
                    abStr += abName + "  ";
                }
                Debug.LogError("Redundancy file  fileName:" + fileEntry.Key + "  in Ab:" + abStr);
            }
        }

    }


    public static void LoadAndVerions(BuildTarget buildTargetGroup, bool isUpdate = false,
        int version = 10000)
    {
        m_isUpdateShader = false;
        m_curVersion = version;
        if (isUpdate)
        {
            m_currentCache = ABAssetDataUtil.ReloadCache(buildTargetGroup);
            if (m_curVersion <= m_currentCache.firstVersion || m_curVersion <= m_currentCache.newVersion)
            {
                m_curVersion = m_currentCache.newVersion + 1;
            }
            for (int i = 0; i < m_currentCache.assetList.Count; i++)
            {
                if (!m_oldDic.ContainsKey(m_currentCache.assetList[i].filePath))
                {
                    m_oldDic.Add(m_currentCache.assetList[i].filePath, m_currentCache.assetList[i]);

                    if (!m_abbuildDic.ContainsKey(m_currentCache.assetList[i].bundleId))
                    {
                        ABBundleInfo buildInfo = new ABBundleInfo();
                        buildInfo.bundleId = m_currentCache.assetList[i].bundleId;
                        buildInfo.firstVersion = m_currentCache.firstVersion;
                        m_abbuildDic.Add(buildInfo.bundleId, buildInfo);
                    }
                }
            }
        }
        else
        {
            m_currentCache = new ABAssetDataList();

            m_currentCache.firstVersion = m_curVersion;
        }
    }


    public static void CollectionAllAssets(bool isUpdate)
    {
        m_abbuildDic.Clear();
        m_dependsDic.Clear();
        m_curretBundleID = new BundleID(m_currentCache.firstVersion, EnumAB.shaders_ab);
        CollectFromDirectinfo("XGame/Public/Resources", isUpdate);
        CollectFromDirectinfo("XGame/Resources/Shaders", isUpdate);

        //主要图集 和 字体
        m_curretBundleID = new BundleID(m_currentCache.firstVersion, EnumAB.uialtas_ab);
        CollectFromDirectinfo("XGame/Resources/EZFunUI/Altas/Release", isUpdate);
        CollectFromDirectinfo("XGame/Data/UI/AltasRef", isUpdate);
        CollectFromDirectinfo("XGame/Resources/EZFunUI/Font", isUpdate);

        //主要ui
        m_curretBundleID = new BundleID(m_currentCache.firstVersion, EnumAB.ui_ab);
        CollectFromDirectinfo("XGame/Resources/EZFunUI", isUpdate);
        CollectFromDirectinfo("XGame/Resources/SuperTextMesh", isUpdate);

        //主要是角色模型技能等
        m_curretBundleID = new BundleID(m_currentCache.firstVersion, EnumAB.model_ab);
        CollectFromDirectinfo("XGame/Resources/Prefab", isUpdate);


        m_curretBundleID = new BundleID(m_currentCache.firstVersion, EnumAB.scene_ab);
        CollectFromDirectinfo("XGame/Scene/Release", isUpdate, "unity");

        // 场景prefab
        m_curretBundleID = new BundleID(m_currentCache.firstVersion, EnumAB.scenemodle_ab);
        CollectFromDirectinfo("XGame/Resources/Scene", isUpdate);
    }

    public static void BuildAllAssetBundle(BuildTarget buildTargetGroup, bool isUpdate = false,
        int version = 10000)
    {
        //加载原来文件
        LoadAndVerions(buildTargetGroup, isUpdate, version);
        //收集资源
        CollectionAllAssets(isUpdate);
        // 场景
        ProcessCacheDic(isUpdate);

        BuildAssetBuindles(buildTargetGroup, version);
    }

     [MenuItem("EZFun/X2Bundle/BuildSpecialBundleAndriod")]
    public static void BuildSpecialBundleAndriod()
    {
        BuildTarget buildTargetGroup = BuildTarget.Android;
        string buildPath = EditorUtility.OpenFolderPanel("选择目录", Application.dataPath, "");
        if (string.IsNullOrEmpty(buildPath))
        {
            return;
        }
        LoadAndVerions(buildTargetGroup, false, 99999);
        m_curretBundleID = new BundleID(99999, EnumAB.ui_ab);
        CollectFromDirectinfo(buildPath, false);

        List<AssetBundleBuild> buildsList = new List<AssetBundleBuild>();
        foreach (var keyValue in m_abbuildDic)
        {
            buildsList.Add(keyValue.Value.GetBundleBuild());
        }
        ProcessCacheDic(false);
        BuildAssetBuindles(buildTargetGroup, 99999);
    }


    public static void BuildAssetBuindles(BuildTarget buildTargetGroup, int version)
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

        m_currentCache.newVersion = m_curVersion;
        //1存储ResourceManager的描述文件，
        SaveABDataFile(structs, buildTargetGroup);
        //2将相应的ab拷贝到对应的文件夹中
        CopyABToUpdateForlder(buildTargetGroup);

        m_currentCache.assetList.Clear();
        foreach (var keyvalue in m_dependsDic)
        {
            m_currentCache.assetList.Add(keyvalue.Value.ToCacheData());
        }
        ABAssetDataUtil.SaveAssetCache(buildTargetGroup, m_currentCache);
    }

    private static void AddResrouceToAbDataStruct(AssetBundleManifest manifest,
        ABDesc structs)
    {
        foreach (var build in m_abbuildDic)
        {
            if (build.Value.bundleId.version >= m_curVersion)
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
                    else if (fileName.EndsWith(".prefab") ||
                        fileName.EndsWith("shader") || fileName.EndsWith("cginc")
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
        }
        var data = manifest.GetAllAssetBundles();
        for (int i = 0; i < data.Length; i++)
        {
            ABDependencies ad = new ABDependencies();
            var abs = manifest.GetAllDependencies(data[i]);
            ad.m_abName = data[i];
            if (ad.m_abName == EnumAB.shaders_ab.ToString())
            {
                if (m_isUpdateShader)
                {
                    ad.m_ressVersion = m_curVersion;
                }
                else
                {
                    ad.m_ressVersion = m_currentCache.firstVersion;
                }
            }
            else
            {
                var spliteStrList = ad.m_abName.Split('_');
                if (spliteStrList.Length >= 2)
                {
                    int tversion = 0;
                    if (int.TryParse(spliteStrList[0], out tversion))
                    {
                        ad.m_ressVersion = tversion;
                    }
                    else
                    {
                        ad.m_ressVersion = m_currentCache.firstVersion;
                    }
                }
                else
                {
                    ad.m_ressVersion = m_currentCache.firstVersion;
                }
            }

            for (int j = 0; j < abs.Length; j++)
            {
                ad.m_dependenciesAb.Add(abs[j]);
            }
            structs.m_abDepends.Add(ad);
        }
    }




    #region tools

    /// <summary>
    /// 收集资源
    /// </summary>
    /// <param name="path"></param>
    /// <param name="isUpdate"></param>
    /// <param name="endWith"></param>
    static void CollectFromDirectinfo(string path,
        bool isUpdate = false, string endWith = "prefab")
    {
        if (!m_abbuildDic.ContainsKey(m_curretBundleID))
        {
            ABBundleInfo buildInfo = new ABBundleInfo();
            buildInfo.bundleId = m_curretBundleID;
            buildInfo.firstVersion = m_currentCache.firstVersion;
            m_abbuildDic.Add(m_curretBundleID, buildInfo);
        }

        DirectoryInfo direc;
        direc = new DirectoryInfo(Application.dataPath + "/" + path);
        if (!direc.Exists)
        {
            direc = new DirectoryInfo(path);
            if (!direc.Exists)
            {
                return;
            }
            //return;
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
            if (prefabName.Contains("GameRoot.unity"))
            {
                continue;
            }
            if (prefabName.EndsWith(".meta"))
            {
                continue;
            }
            if (prefabName.EndsWith(endWith) || prefabName.EndsWith("shader") || prefabName.EndsWith("cginc")
                || prefabName.EndsWith("ttf")
                || prefabName.EndsWith("shadervariants")
                || prefabName.EndsWith("png"))
            {
                ColloectDependecies(prefabName, file.Name);
            }
        }
    }

    private static ABAssetInfo ColloectDependecies(string prefabPath, string fileName = "")
    {
        prefabPath = prefabPath.ToLower();
        string[] depends = AssetDatabase.GetDependencies(prefabPath, false);
        //场景比较特殊是因为场景ab不能包含其他的资源，场景自己一个ab，所以得把场景引用的资源挪到scenemodle_ab中去
        if (prefabPath.EndsWith(".unity"))
        {
            if (fileName.Contains("."))
            {
                fileName = fileName.Substring(0, fileName.IndexOf("."));
            }
            m_curretBundleID = new BundleID(m_currentCache.firstVersion, EnumAB.scene_ab, fileName);
            if (!m_abbuildDic.ContainsKey(m_curretBundleID))
            {
                ABBundleInfo buildInfo = new ABBundleInfo();
                buildInfo.bundleId = m_curretBundleID;
                buildInfo.firstVersion = m_currentCache.firstVersion;
                m_abbuildDic.Add(m_curretBundleID, buildInfo);
            }
        }
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
                        //场景比较特殊是因为场景ab不能包含其他的资源，场景自己一个ab，所以得把场景引用的资源挪到scenemodle_ab中去
                        if (prefabPath.EndsWith(".unity"))
                        {
                            m_curretBundleID = new BundleID(m_currentCache.firstVersion, EnumAB.scenemodle_ab);
                        }
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


    public static void ProcessCacheDic(bool isUpdate = false)
    {
        List<ABAssetData> shaderlist = new List<ABAssetData>();

        foreach (var keyvalue in m_abbuildDic)
        {
            keyvalue.Value.fileList.Clear();
        }

        foreach (var keyvalue in m_dependsDic)
        {
            if (!isUpdate)
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
            else
            {
                ABAssetData oldCache = null;
                var newCache = keyvalue.Value.m_cacheData;
                var assetInfo = keyvalue.Value;

                ABBundleInfo build = null;
                if (!m_abbuildDic.TryGetValue(newCache.bundleId, out build))
                {
                    build = new ABBundleInfo();
                    build.bundleId = newCache.bundleId;
                    m_abbuildDic.Add(newCache.bundleId, build);
                }
                m_oldDic.TryGetValue(keyvalue.Key, out oldCache);
                ABBundleInfo oldBuild = null;
                if (oldCache != null && m_abbuildDic.ContainsKey(oldCache.bundleId))
                {
                    oldBuild = m_abbuildDic[oldCache.bundleId];
                }
                if (newCache.bundleType == EnumAB.shaders_ab)
                {
                    if (oldCache != null)
                        continue;

                    //shader有修改  重新build所有shader就好
                    if (!newCache.Equals(oldCache))
                    {
                        //重新打 将已经处理过的shader都移动到最新的version
                        m_isUpdateShader = true;
                        if (shaderlist.Count != 0)
                        {
                            foreach (var shader in shaderlist)
                            {
                                build.bundleId = new BundleID(m_curVersion, EnumAB.shaders_ab);
                                AddAsset(build, shader);
                            }
                            shaderlist.Clear();
                        }
                    }
                    if (m_isUpdateShader)
                    {
                        build.bundleId = new BundleID(m_curVersion, EnumAB.shaders_ab);
                        AddAsset(build, newCache);
                    }
                    else
                    {
                        // shaderlist 用来处理原来的没有修改过的shader，当后面发现有shader修改，那么需要把之前觉得不需要的改的shader都修改了
                        shaderlist.Add(newCache);
                        AddAsset(oldBuild, newCache);
                    }
                }
                else if (newCache.bundleType == EnumAB.uialtas_ab)
                {
                    //图集的定义是图集除了引用shader之外不会引用任何其他图集，而且有另外一套图集引用关系
                    var newBuildCache = assetInfo.m_cacheData;
                    if (newBuildCache.Equals(oldCache))
                    {
                        AddAsset(oldBuild, newCache);
                    }
                    else
                    {
                        //图集有修改  新建一个 version_uiAtlas_ab的assetbundle 并且把所有相关的放到这儿来
                        build = GetUpdateABBuild(EnumAB.uialtas_ab);
                        AddAsset(build, newBuildCache);
                        TransToSelf(assetInfo, EnumAB.uialtas_ab);
                    }
                }
                else if (newCache.bundleType == EnumAB.scene_ab)
                {
                    var newBuildCache = assetInfo.m_cacheData;
                    if (newBuildCache.Equals(oldCache))
                    {
                        AddAsset(oldBuild, newCache);
                    }
                    else
                    {
                        //场景有改变 需要重新打包这个场景它的和它的子引用，要更新它的子引用是怕它有增加一个模型
                        build = GetUpdateABBuild(EnumAB.scene_ab, newCache.bundleId.sceneName);
                        AddAsset(build, newBuildCache);
                        var scenePrefabBuild = GetUpdateABBuild(EnumAB.scenemodle_ab);
                        foreach (var child in assetInfo.m_children)
                        {
                            AddAsset(scenePrefabBuild, child.m_cacheData);
                        }
                    }
                }
                else if (newCache.bundleType == EnumAB.scenemodle_ab)
                {
                    var newBuildCache = assetInfo.m_cacheData;
                    if (newBuildCache.Equals(oldCache))
                    {
                        AddAsset(oldBuild, newCache);
                    }
                    else
                    {
                        TransToParent(assetInfo, EnumAB.scenemodle_ab);
                    }
                }
                else
                {
                    var newBuildCache = assetInfo.m_cacheData;
                    if (newBuildCache.Equals(oldCache))
                    {
                        AddAsset(oldBuild, newCache);
                    }
                    else
                    {
                        //上面是三个并不通用的规则，这里是通用规则，如果一个文件有改变，那么谁引用它，那么谁就要重新build咯
                        TransToParent(assetInfo, assetInfo.m_cacheData.bundleId.buildType);
                    }
                }
                //以上结构不好，我就不整理了
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

    public static void GetBundleList(List<object> list)
    {
        var enumerator = m_abbuildDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            list.Add(enumerator.Current.Value);
        }
    }

    /// <summary>
    /// 只改引用自己  这儿用到的主要是uiatlas
    /// </summary>
    /// <param name="parentInfo"></param>
    /// <param name="prefabType"></param>
    private static void TransToSelf(ABAssetInfo parentInfo, EnumAB prefabType)
    {
        Queue<ABAssetInfo> queue = new Queue<ABAssetInfo>();
        queue.Enqueue(parentInfo);
        var buildinfo = GetUpdateABBuild(prefabType);
        while (queue.Count != 0)
        {
            var assetInfo = queue.Dequeue();
            if (assetInfo.m_cacheData.bundleType == prefabType)
            {
                AddAsset(buildinfo, assetInfo.m_cacheData);
                foreach (var parent in assetInfo.m_parent)
                {
                    queue.Enqueue(parent);
                }
            }
        }
    }


    static Queue<ABAssetInfo> queue = new Queue<ABAssetInfo>();
    static HashSet<string> m_handleFiles = new HashSet<string>();
    /// <summary>
    /// 一个资源改变了，会导致引用它的资源也改变，但是希望限制影响等级
    /// </summary>
    /// <param name="parentInfo"></param>
    /// <param name="prefabType">影响等级</param>
    private static void TransToParent(ABAssetInfo parentInfo, EnumAB prefabType)
    {
        queue.Clear();
        m_handleFiles.Clear();
        queue.Enqueue(parentInfo);
        //m_handleFiles 这个是用来破除 资源之间引用循环关系的
        while (queue.Count != 0)
        {
            var assetInfo = queue.Dequeue();
            if (assetInfo.m_cacheData.bundleType >= prefabType)
            {
                assetInfo.m_hasChildrenUpdate = true;
                var buildinfo = GetUpdateABBuild(assetInfo.m_cacheData.bundleType, assetInfo.m_cacheData.bundleId.sceneName);
                AddAsset(buildinfo, assetInfo.m_cacheData);
                m_handleFiles.Add(assetInfo.AssetPath);
                foreach (var parent in assetInfo.m_parent)
                {
                    if (!m_handleFiles.Contains(parent.AssetPath))
                    {
                        queue.Enqueue(parent);
                    }
                }
            }
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

    private static ABBundleInfo GetUpdateABBuild(EnumAB bundleType, string sceneName = "")
    {
        ABBundleInfo build = null;
        var bundleId = new BundleID(m_curVersion, bundleType, sceneName);
        if (m_abbuildDic.ContainsKey(bundleId))
        {
            build = m_abbuildDic[bundleId];
        }
        else
        {
            build = new ABBundleInfo();
            build.bundleId = bundleId;
            m_abbuildDic.Add(bundleId, build);
        }
        return build;
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


    static void SaveABDataFile(ABDesc abData, BuildTarget buildTargetGroup)
    {
        string outPath = PreOutPath(buildTargetGroup);
        string fileName = "ABDatabase";
        if (m_currentCache.firstVersion != m_curVersion)
        {
            fileName = m_curVersion + "_ABDatabase";
        }
        outPath += "/" + m_curVersion;
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
        if (m_currentCache.firstVersion == m_currentCache.newVersion)
        {
            var newPath = Application.streamingAssetsPath + "/AssetBundles/" + fileName + ".abMap";
            if (File.Exists(newPath))
            {
                File.Delete(newPath);
            }
            using (Stream file = File.Create(newPath))
            {
                file.Write(bytes, 0, bytes.Length);
                file.Close();
            }
        }
    }


    static void CopyABToUpdateForlder(BuildTarget buildTargetGroup)
    {
        string outPath = PreOutPath(buildTargetGroup);
        var versionPath = outPath + "/" + m_curVersion;
        //Directory.CreateDirectory(versionPath);
        foreach (var keyValue in m_abbuildDic)
        {
            var bundle = keyValue.Value;
            //新打的要拷贝到对应文件夹
            if (bundle.bundleId.version == m_curVersion)
            {
                string bundleName = bundle.BundleName;
                //给shader改个名字
                if (bundle.bundleId.buildType == EnumAB.shaders_ab
                    && m_currentCache.firstVersion != m_currentCache.newVersion)
                {
                    bundleName = m_curVersion + "_" + bundleName;
                }
                FileInfo file = new FileInfo(outPath + "/" + bundle.BundleName);
                var dstPath = versionPath + "/" + bundleName;
                if (File.Exists(dstPath))
                {
                    File.Delete(dstPath);
                }
                if (file.Exists)
                {
                    Debug.LogError("CopyPath ConfigPath:" + dstPath);
                    file.CopyTo(dstPath);
                    //如果是第一次打全资源包 那么也要移动到StreamingAssetsPath下面
                    if (m_currentCache.firstVersion == m_currentCache.newVersion)
                    {
                        /// 注意，在StreamingAssets目录下的ab是后后缀的，因为android打gradlew需要后缀区分那些文件压缩
                        ///
                        ///AB 内部关系不加后缀，但是实际文件有后缀，其实ab之间引用关系之间也不是以文件名为依据，而是以之前
                        ///BuildPipeline.BuildAssetBundles 中名字作为判断条件
                        dstPath = Application.streamingAssetsPath + "/AssetBundles/" + bundleName + ".ezfunab";
                        if (File.Exists(dstPath))
                        {
                            File.Delete(dstPath);
                        }
                        Debug.LogError("CopyPath Application.streamingAssetsPath:" + dstPath);
                        file.CopyTo(dstPath);
                    }
                }
            }
        }
    }



    public static string PreOutPath(BuildTarget buildTargetGroup)
    {
        var path = Application.dataPath + "/../Config/" + buildTargetGroup.ToString();
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }
    #endregion

    public static ABAssetInfo GetABAsset(string path)
    {
        if (m_dependsDic.ContainsKey(path))
        {
            return m_dependsDic[path];
        }
        return null;
    }

    public static ABAssetData GetOldData(string path)
    {
        if (m_oldDic.ContainsKey(path))
        {
            return m_oldDic[path];
        }
        return null;
    }

    public static void Clear()
    {
        m_abbuildDic.Clear();
        m_dependsDic.Clear();
        m_isIgnoreDic.Clear();
        m_oldDic.Clear();
        HashUtil.ClearHash();
    }
}
