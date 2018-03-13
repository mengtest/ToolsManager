/************************************************************
//     文件名      : BuildTools.cs
//     功能描述    : 构建工具
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/09/24.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using LitJson;
using System.Threading;

public static class BuildTools
{
    
    public enum BuildFlag
    {
        None = 0,
        BuildScene = 1 << 0,
        BuildAssetBundle = 1 << 1,
        Development = 1 << 2
    }

    static void ClearAssetBundleNames()
    {
        string[] allAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < allAssetBundleNames.Length; ++i)
        {
            AssetDatabase.RemoveAssetBundleName(allAssetBundleNames[i], true);
        }
        AssetDatabase.RemoveUnusedAssetBundleNames();
    }

    static void GetAllFileAtPath(string path, ref List<string> allFiles)
    {
        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (string fi in files)
            {
                string completePath = fi.Replace('\\', '/');
                completePath = completePath.Remove(0, PathUtils.ProjectPath.Length);
                allFiles.Add(completePath);
            }
        }
    }

    public static void GetBuildSceneNames(ref List<string> listSceneNames)
    {
        foreach (EditorBuildSettingsScene sceneSetting in EditorBuildSettings.scenes)
        {
            if (sceneSetting == null)
            {
                continue;
            }
            if (sceneSetting.enabled)
            {
                listSceneNames.Add(sceneSetting.path);
            }
        }
    }

    //建立 Resources 目录下的被打包到 AssetBundle 中的文件夹排除列表
    public static void BuildExcludingResourcePathes(ref List<string> buildEntries, ref List<string> excludingResourcePathes)
    {
        if (excludingResourcePathes == null)
        {
            return;
        }

        //遍历每个需要打包的资源目录
        for (int nBundleEntry = 0; nBundleEntry < buildEntries.Count; ++nBundleEntry)
        {
            var path = buildEntries[nBundleEntry];
            if (path.Length <= 0)
            {
                continue;
            }

            //为当前路径加上结尾，以免混淆开头相同的目录。
            string buildPath;
            if (path[path.Length - 1] != '/')
            {
                buildPath = path + '/';
            }
            else
            {
                buildPath = path;
            }

            //如果当前打包路径在 Resources 目录下
            if (buildPath.StartsWith(PathUtils.ResourceRefPath, System.StringComparison.OrdinalIgnoreCase))
            {
                //判断当前打包目录或其根目录是否已经被加入排除列表
                bool bIsPathExisting = false;

                for (int nExistingPathIndex = excludingResourcePathes.Count - 1; nExistingPathIndex >= 0; --nExistingPathIndex)
                {
                    string existingPath = excludingResourcePathes[nExistingPathIndex];

                    //如果当前目录与现有目录相同
                    if (0 == string.Compare(buildPath, existingPath, true))
                    {
                        bIsPathExisting = true;
                    }
                    //如果现有目录是当前目录的根目录
                    else if (existingPath.StartsWith(buildPath, System.StringComparison.OrdinalIgnoreCase))
                    {
                        //删除现有目录条目
                        excludingResourcePathes.RemoveAt(nExistingPathIndex);
                    }
                }

                //将当前目录加入排除列表
                if (!bIsPathExisting)
                {
                    excludingResourcePathes.Add(buildPath);
                }
            }
        }
    }

    static void CreateAssetFolder(string destDir)
    {
        //逐级创建文件夹
        string[] directories = destDir.Split('/');
        for (int nLevel = 0; nLevel < directories.Length; ++nLevel)
        {
            //当前层级的父文件夹路径
            string parentDir = "";
            for (int nParent = 0; nParent < nLevel; ++nParent)
            {
                if (parentDir.Length > 0)
                {
                    parentDir += '/';
                }
                parentDir += directories[nParent];
            }

            //当前级别需要创建的文件夹名称
            string dirToCreate = directories[nLevel];

            //当前级别的文件夹全路径
            string dirToLevel = parentDir;
            if (dirToLevel.Length > 0)
            {
                dirToLevel += '/';
            }
            dirToLevel += dirToCreate;

            //验证当前级别的文件夹是否已创建
            string dirToLevelFullPath = PathUtils.ProjectPath + dirToLevel;
            if (!System.IO.Directory.Exists(dirToLevelFullPath))
            {
                System.IO.Directory.CreateDirectory(dirToLevelFullPath);
                //AssetDatabase.CreateFolder(parentDir, dirToCreate);
            }
        }
    }

    //移动指定目录及其子目录下的资源到指定文件夹
    static bool MoveAssets(string sourceDir, string destDir)
    {
        //获取指定目录及其子目录下的所有文件路径
        string[] fileEntries = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);
        for (int i = 0; i < fileEntries.Length; ++i)
        {
            string filePath = fileEntries[i];
            FileAttributes fa = File.GetAttributes(filePath);
            if ((fa & FileAttributes.Hidden) != 0 || filePath.Contains(".svn"))
                continue;

            //替换 Unity 不支持的路径中的反斜杠
            string sourcePath = filePath.Replace('\\', '/');

            //创建移动目标目录
            string destPath = destDir + sourcePath.Remove(0, sourceDir.Length);
            int nDirEndIndex = destPath.LastIndexOf('/');
            if (nDirEndIndex >= 0)
            {
                string destAssetDir = destPath.Remove(nDirEndIndex);
                CreateAssetFolder(destAssetDir);
            }

            string fullSourcePath = PathUtils.ProjectPath + sourcePath;
            string fullDestPath = PathUtils.ProjectPath + destPath;

            //移动文件
            if (File.Exists(fullDestPath))
            {
                File.Delete(fullDestPath);
            }
            File.Move(fullSourcePath, fullDestPath);
        }

        return true;
    }

    //将资源还原到 Resources 目录
    static bool RestoreAssetsInResourceDir(List<string> excludingResourcePathes)
    {
        int nResourceDirLength = PathUtils.ResourceRefPath.Length;

        foreach (var excludingPath in excludingResourcePathes)
        {
            string originalResourcesDirPath = excludingPath;
            string buildTempResourcesDirPath = "Resources_BuildTemp/" + excludingPath.Substring(nResourceDirLength);

            UnityEngine.Debug.Log("Moving assets from '" + buildTempResourcesDirPath + "' to '" + originalResourcesDirPath + "'...");

            //从 Resources 目录移动到 Resource_BuildTemp 下
            if (!MoveAssets(buildTempResourcesDirPath, originalResourcesDirPath))
            {
                UnityEngine.Debug.Log("Failed to move assets from '" + buildTempResourcesDirPath + "' to '" + originalResourcesDirPath + "', reverting files...");

                //如果发生错误则将已经移动的文件还原
                MoveAssets(originalResourcesDirPath, buildTempResourcesDirPath);

                UnityEngine.Debug.Log("Assets reverted.");

                return false;
            }

            //删除 Resource_BuildTemp 目录
            string destDirFullPath = PathUtils.ProjectPath + buildTempResourcesDirPath;
            PathUtils.DeleteDirectory(destDirFullPath);

            UnityEngine.Debug.Log("Completed moving assets from '" + buildTempResourcesDirPath + "' to '" + originalResourcesDirPath + "'.");
        }

        return true;
    }

    //将 Resources 目录的资源移出
    static bool MoveOutAssetsInResourcesDir(List<string> excludingResourcePathes)
    {
        int nResourceDirLength = PathUtils.ResourceRefPath.Length;

        foreach (var excludingPath in excludingResourcePathes)
        {
            string originalResourcesDirPath = excludingPath;
            string buildTempResourcesDirPath = "Resources_BuildTemp/" + excludingPath.Substring(nResourceDirLength);

            UnityEngine.Debug.Log("Moving assets from '" + originalResourcesDirPath + "' to '" + buildTempResourcesDirPath + "'...");

            //从 Resources 目录移动到 Resource_BuildTemp 下
            if (!MoveAssets(originalResourcesDirPath, buildTempResourcesDirPath))
            {
                UnityEngine.Debug.Log("Failed to move assets from '" + originalResourcesDirPath + "' to '" + buildTempResourcesDirPath + "', reverting files...");

                //如果发生错误则将已经移动的文件还原
                MoveAssets(buildTempResourcesDirPath, originalResourcesDirPath);

                string destDirFullPath = PathUtils.ProjectPath + buildTempResourcesDirPath;
                PathUtils.DeleteDirectory(destDirFullPath);

                UnityEngine.Debug.Log("Assets reverted.");

                return false;
            }

            UnityEngine.Debug.Log("Completed moving assets from '" + originalResourcesDirPath + "' to '" + buildTempResourcesDirPath + "'.");
        }

        return true;
    }

    public static bool BuildAssetBundleByFlatform(BuildTarget target, BuildFlag buildFlags, List<string> excludingResourcePathes, List<string> buildingScenes)
    {
        bool hasBuild = false;
        if (buildFlags == BuildFlag.None)
        {
            return hasBuild;
        }
        ClearAssetBundleNames();

        List<string> buildEntries = new List<string>();
        List<AssetBundleDescObject.AssetBundleDesc> assetBundleDescs = new List<AssetBundleDescObject.AssetBundleDesc>();
        List<AssetBundleDescObject.SceneBundleDesc> sceneBundleDescs = new List<AssetBundleDescObject.SceneBundleDesc>();
        JsonData jsonData = LoadAssetBundleConfig();

        #region build resources
        if ((buildFlags & BuildFlag.BuildAssetBundle) != 0)
        {
            JsonData resourceConfig = jsonData["Resources"];
            for (int i = 0; i < resourceConfig.Count; ++i)
            {
                string folder = resourceConfig[i].ToString();
                if (folder.StartsWith(PathUtils.ResourceRefPath, System.StringComparison.OrdinalIgnoreCase))
                {
                    string path = PathUtils.ProjectPath + folder;
                    string assetBundleName = folder.Substring(PathUtils.ResourceRefPath.Length);
                    assetBundleName = assetBundleName.ToLower();
                    AssetBundleDescObject.AssetBundleDesc assetBundleDesc = new AssetBundleDescObject.AssetBundleDesc();
                    assetBundleDesc.m_Name = assetBundleName;
                    List<string> resFiles = new List<string>();
                    GetAllFileAtPath(path, ref resFiles);
                    foreach (string fi in resFiles)
                    {
                        AssetImporter importer = AssetImporter.GetAtPath(fi);
                        if (importer != null)
                        {
                            importer.assetBundleName = assetBundleName;
                            importer.assetBundleVariant = "";
                            assetBundleDesc.m_Assets.Add(fi);
                        }
                    }
                    if (assetBundleDesc.m_Assets.Count > 0)
                    {
                        assetBundleDescs.Add(assetBundleDesc);
                        buildEntries.Add(folder);
                    }
                }
                else
                {
                    Debug.LogError("File path not in resources folder " + folder);
                }
            }

            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/AssetBundles", BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle, target);
            hasBuild = true;
        }

        #endregion

        #region build scenes
        if ((buildFlags & BuildFlag.BuildScene) != 0)
        {
            JsonData sceneConfig = jsonData["Scenes"];
            for (int i = 0; i < sceneConfig.Count; ++i)
            {
                string sceneName = sceneConfig[i].ToString();
                string bundleName = PathUtils.GetSceneNameFromPath(sceneName);
                bundleName = bundleName.ToLower();

                AssetImporter importer = AssetImporter.GetAtPath(sceneName);
                if (importer != null)
                {
                    AssetBundleDescObject.SceneBundleDesc desc = new AssetBundleDescObject.SceneBundleDesc();
                    desc.m_BundleName = bundleName;
                    //use bundle name to scene name
                    desc.m_SceneName = bundleName;

                    importer.assetBundleName = bundleName;
                    importer.assetBundleVariant = "";
                    string assetBundlePath = PathUtils.AssetBundleRefPath + bundleName;

                    string strError = BuildPipeline.BuildPlayer(new string[] { sceneName }, assetBundlePath, target, BuildOptions.BuildAdditionalStreamedScenes);
                    if (!string.IsNullOrEmpty(strError))
                    {
                        UnityEngine.Debug.LogError("Failed to build asset bundle for scene '" + sceneName + "'.");
                    }
                    buildingScenes.Add(bundleName);
                    sceneBundleDescs.Add(desc);
                    buildEntries.Add(sceneName);
                    hasBuild = true;
                }
            }
        }

        #endregion

        if (excludingResourcePathes != null)
        {
            BuildExcludingResourcePathes(ref buildEntries, ref excludingResourcePathes);
        }

        AssetBundleDescObject assetBundleDescObject = ScriptableObject.CreateInstance<AssetBundleDescObject>();
        assetBundleDescObject.m_AssetBundleDesc = assetBundleDescs;
        assetBundleDescObject.m_SceneBundleDesc = sceneBundleDescs;
        const string databasePath = PathUtils.ResourceRefPath + "AssetBundleDatabase.asset";
        AssetDatabase.CreateAsset(assetBundleDescObject, databasePath);

        ClearAssetBundleNames();
        return hasBuild;
    }

    static JsonData LoadAssetBundleConfig()
    {
        string xmlFilePath = PathUtils.ProjectPath + "Config/AssetBundlesResource.json";

        if (File.Exists(xmlFilePath))
        {
            StreamReader reader = File.OpenText(xmlFilePath);
            if (reader != null)
            {
                string content = reader.ReadToEnd();
                JsonData json = JsonMapper.ToObject(content);
                reader.Close();
                return json;
            }
        }
        return null;
    }

    public static void AddDefine(string str, BuildTargetGroup buildTargetGroup)
    {
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        symbols += ";";
        symbols += str;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbols);
    }

    public static void BuildAppImpl(BuildTarget buildTarget, BuildOptions buildOptions, string outputPath)
    {
        string buildPath = outputPath;
        if (string.IsNullOrEmpty(buildPath) == false)
        {
            buildPath += "/";
        }
        switch (buildTarget)
        {
            case BuildTarget.Android:
                {
                    buildPath += "XGame2.apk";
                    string androidSkdRoot = GetCommandLineArg("-androidsdkroot");
                    if (!string.IsNullOrEmpty(androidSkdRoot))
                        EditorPrefs.SetString("AndroidSdkRoot", androidSkdRoot);
                    break;
                }
            case BuildTarget.iOS:
                {
                    buildPath += "XCodeProject";
                    break;
                }
            case BuildTarget.StandaloneWindows:
                {
                    buildPath += "XGame2_x86.exe";
                    break;
                }
            case BuildTarget.StandaloneWindows64:
                {
                    buildPath += "XGame2_x64.exe";
                    break;
                }
            default:
                {
                    UnityEngine.Debug.LogError("RD_LOG: Unsupported build target.");
                    return;
                }
        }
        PathUtils.EnsureFilePathExist(buildPath);

        List<string> excludingResourcePathes = new List<string>();
        List<string> buildingScenes = new List<string>();
        BuildAssetBundleByFlatform(buildTarget, BuildFlag.BuildScene | BuildFlag.BuildAssetBundle, excludingResourcePathes, buildingScenes);

        EditorApplication.LockReloadAssemblies();

        MoveOutAssetsInResourcesDir(excludingResourcePathes);

        //根据是否使用 AssetBundle 设置构建场景列表
        List<string> listSceneNames = new List<string>();
        GetBuildSceneNames(ref listSceneNames);
        if (buildingScenes != null)
        {
            foreach (string buildingSceneName in buildingScenes)
            {
                for (int i = 0; i < listSceneNames.Count; ++i)
                {
                    string sceneName = listSceneNames[i].ToLower();
                    sceneName = PathUtils.GetSceneNameFromPath(sceneName);
                    if (buildingSceneName == sceneName)
                    {
                        listSceneNames.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        string disableLog = GetCommandLineArg("-disablelog");
        if (!string.IsNullOrEmpty(disableLog))
        {
            AddDefine("DISABLE_LOG", EditorUserBuildSettings.selectedBuildTargetGroup);
        }
        string buildError = BuildPipeline.BuildPlayer(listSceneNames.ToArray(), buildPath, buildTarget, buildOptions);
        if (!string.IsNullOrEmpty(buildError))
        {
            UnityEngine.Debug.LogError(buildError);
        }

        RestoreAssetsInResourceDir(excludingResourcePathes);

        EditorApplication.UnlockReloadAssemblies();
    }

    //[MenuItem("Build Tools/Build app", false, 20)]
    public static void BuildApp()
    {
        BuildAppImpl(
            EditorUserBuildSettings.activeBuildTarget,
            //BuildOptions.AllowDebugging | BuildOptions.Development
            BuildOptions.None,
            "bin"
            );
    }
    public static string handleLongStr(string val)
    {
        int index = val.IndexOf('.');
        if (index != -1)
        {
            string sstr = val.Substring(index);
            if (sstr.Length > 4)
            {
                val = val.Substring(0, index + 4);
            }
        }
        return val;
    }
    //[MenuItem("Build Tools/Test", false, 20)]
    public static void Test()
    {
        //AnimationClip animclip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/XGame/Data/Animation/monster/monster_boss_zuoci/zuoci_Skill03.anim");
        AnimationClip animclip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/XGame/baimaqibing_attack01.anim");
        string val = "";
        foreach (EditorCurveBinding theCurveBinding in AnimationUtility.GetCurveBindings(animclip))
        {
            AnimationCurve curve = AnimationUtility.GetEditorCurve(animclip, theCurveBinding);

            string name = theCurveBinding.propertyName.ToLower();
            //去掉scalecurve
            //if (name.Contains("scale"))
            //{
            //    AnimationUtility.SetEditorCurve(animclip, theCurveBinding, null);
            //    continue;
            //}
            //减小精度
            for (int i = 0; i < curve.keys.Length; i++)
            {
                Keyframe key = new Keyframe();
                key.value = curve.keys[i].value;
                key.inTangent = curve.keys[i].inTangent;
                key.outTangent = curve.keys[i].outTangent;
                val = curve.keys[i].time.ToString();
                val = handleLongStr(val);
                key.time = float.Parse(val);
                val = curve.keys[i].value.ToString();
                val = handleLongStr(val);
                key.value = float.Parse(val);
                val = curve.keys[i].inTangent.ToString();
                val = handleLongStr(val);
                key.inTangent = float.Parse(val);
                val = curve.keys[i].outTangent.ToString();
                val = handleLongStr(val);
                key.outTangent = float.Parse(val);
                curve.MoveKey(i, key);
            }
            AnimationUtility.SetEditorCurve(animclip, theCurveBinding, curve);
        }
        //删掉编辑器绑定关系部分
        var so = new SerializedObject(animclip);
        SerializedProperty m_EulerEditorCurves = so.FindProperty("m_EulerEditorCurves");
        SerializedProperty m_EditorCurves = so.FindProperty("m_EditorCurves");
        int count = m_EulerEditorCurves.arraySize;
        for (int i = 0; i < count; i++)
        {
            m_EulerEditorCurves.DeleteArrayElementAtIndex(0);
            so.ApplyModifiedProperties();
        }
        count = m_EditorCurves.arraySize;
        for (int i = 0; i < count; i++)
        {
            m_EditorCurves.DeleteArrayElementAtIndex(0);
            so.ApplyModifiedProperties();
        }

        //AssetDatabase.SaveAssets();

        Debug.Log("success");
    }


    //获取脚本命令行参数
    public static string GetCommandLineArg(string name)
    {
        string[] args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; ++i)
        {
            if (args[i].ToLower() == name.ToLower())
            {
                if (args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }
        }

        return "";
    }

    //判断指定的命令行参数是否存在
    public static bool HasCommandLineArg(string name)
    {
        string[] args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; ++i)
        {
            if (args[i].ToLower() == name.ToLower())
            {
                return true;
            }
        }

        return false;
    }

    public static void BuildForAndroid()
    {
        //PlayerSettings.Android.keystoreName = "keystore";   //"key.store"
        //PlayerSettings.Android.keystorePass = "XGame2";           //"key.store.password"
        //PlayerSettings.Android.keyaliasName = "keystore";   //"key.alias"
        //PlayerSettings.Android.keyaliasPass = "XGame2";           //"key.alias.password"
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);

        bool bDevelopment = HasCommandLineArg("-development");

        BuildOptions developmentBuildOptions = bDevelopment ? (BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler) : BuildOptions.None;

        string outputPath = GetCommandLineArg("-outputpath");

        BuildAppImpl(EditorUserBuildSettings.activeBuildTarget, developmentBuildOptions, outputPath);
    }

    public static void BuildForPC_X86()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);

        bool bDevelopment = HasCommandLineArg("-development");

        BuildOptions developmentBuildOptions = bDevelopment ? (BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler) : BuildOptions.None;

        string outputPath = GetCommandLineArg("-outputpath");

        BuildAppImpl(EditorUserBuildSettings.activeBuildTarget, developmentBuildOptions, outputPath);
    }

    public static void BuildForPC_X64()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows64);

        bool bDevelopment = HasCommandLineArg("-development");

        BuildOptions developmentBuildOptions = bDevelopment ? (BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler) : BuildOptions.None;

        string outputPath = GetCommandLineArg("-outputpath");
        BuildAppImpl(EditorUserBuildSettings.activeBuildTarget, developmentBuildOptions, outputPath);
    }
}
