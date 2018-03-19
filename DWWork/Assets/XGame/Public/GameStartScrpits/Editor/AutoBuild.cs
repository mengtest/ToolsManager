using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine.Rendering;

using System.Text;

public enum eAutoBuild
{
    eBuild_Debug = -1,
}
public class AutoBuild
{
    static string m_outPath;
    static string[] SceneList = null;
    static string m_projName = "";
    static bool m_isAB = false;
    static void searchUnity(List<string> list, string dirPath)
    {
        var files = Directory.GetFiles(dirPath, "*.unity");
        foreach (var file in files)
        {
            if (!list.Contains(file))
            {
                list.Add(file);
            }
        }
        var dirs = Directory.GetDirectories(dirPath);
        foreach (var dir in dirs)
        {
            searchUnity(list, dir);
        }
    }

    // Use this for initialization
    static void InitArgs()
    {

        string[] args = System.Environment.GetCommandLineArgs();

        /*
		BuildOptions op = BuildOptions.None;
		if (isDebug) {
			op |= BuildOptions.AllowDebugging;
			op |= BuildOptions.Development;
			op |= BuildOptions.ConnectWithProfiler;
		}
		*/

        s_buildOptions = BuildOptions.None;

        for (int i = 0; i < args.Length; ++i)
        {
            if (args[i].Equals("-outPath"))
            {
                if (i + 1 < args.Length)
                {
                    m_outPath = args[i + 1];
                    Debug.Log("m_outPath:" + m_outPath);

                    i++;
                    continue;
                }
            }

            if (args[i].Equals("U3DAllowDebugging=true"))
            {
                s_buildOptions |= BuildOptions.AllowDebugging;
                Debug.Log("BuildOptions.AllowDebugging enable");
            }

            if (args[i].Equals("U3DDevelopment=true"))
            {
                s_buildOptions |= BuildOptions.Development;
                Debug.Log("BuildOptions.Development enable");
            }

            if (args[i].Equals("U3DConnectWithProfiler=true"))
            {
                s_buildOptions |= BuildOptions.ConnectWithProfiler;
                Debug.Log("BuildOptions.Development enable");
            }

            if (args[i].Equals("U3DGoogleProject=true"))
            {
                s_buildOptions |= BuildOptions.AcceptExternalModificationsToPlayer;
                Debug.Log("BuildOptions.GoogleProject enable");
            }

            if (args[i].Contains("ProjectName="))
            {
                var names = args[i].Split('=');
                if (names.Length == 2 && !string.IsNullOrEmpty(names[1]))
                {
                    m_projName = names[1];
                }
            }
            if (args[i].Contains("U3DUSE_AB=true"))
            {
                m_isAB = true;
                Debug.Log("U3DUSE_AB:true");
            }
        }

        var list = new List<string>();
        list.Add("Assets/XGame/Scene/Release/GameRoot.unity");
        if (!m_isAB)
        {
            searchUnity(list, "Assets/XGame/Scene/Release/");
        }
        SceneList = list.ToArray();
    }

    private static string GetApkOutputPath()
    {
        return null;
    }

    [MenuItem("EZFun/build/build Anroid Debug")]
    public static void BuildAndroidAPKDebug()
    {
        InitArgs();

        Build_Debug_APK();
        BuildAndroidEnd();
    }

    public static void BuildAndroidABAPK()
    {
        m_isAB = true;
        BuildAndroidBefore(eAutoBuild.eBuild_Debug);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "RELEASE;UNITY;DW_Android");

        PlayerSettings.bundleIdentifier = "com.dawang.jxqp";
        PlayerSettings.productName = "聊聊江西麻将";

        BuildAndroid(eAutoBuild.eBuild_Debug);
    }

    public static void BuildAndroidABAPK_Debug()
    {
        m_isAB = true;
        BuildAndroidBefore(eAutoBuild.eBuild_Debug);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "UNITY;DW_Android");

        PlayerSettings.bundleIdentifier = "com.dawang.jxqp";
        PlayerSettings.productName = "聊聊江西麻将";

        BuildAndroid(eAutoBuild.eBuild_Debug);
    }

    static void RemoveDirect(string name)
    {
        string path = Application.dataPath + name;
        string destPath = "F/" + name;

        DirectoryInfo direct = new DirectoryInfo(path);
        direct.MoveTo(destPath);
    }

    static void RecoveryDirect(string name)
    {
        string path = "F/" + name;
        string destPath = Application.dataPath + name;

        DirectoryInfo direct = new DirectoryInfo(path);
        direct.MoveTo(destPath);
    }

    public static void Build_Debug_APK()
    {
        BuildAndroidBefore(eAutoBuild.eBuild_Debug);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "BUILD_EXP;PLAt_SDK_DEBUG;UNITY;DW_Android");
        PlayerSettings.bundleIdentifier = "com.dawang.jxqp";
        PlayerSettings.productName = "聊聊江西麻将";
        BuildAndroid(eAutoBuild.eBuild_Debug);
    }


    public static void BuildAndroidCommon()
    {
        GraphicsSettings.SetShaderMode(BuiltinShaderType.DeferredShading, BuiltinShaderMode.Disabled);
        GraphicsSettings.SetShaderMode(BuiltinShaderType.DeferredReflections, BuiltinShaderMode.Disabled);
        GraphicsSettings.SetShaderMode(BuiltinShaderType.LegacyDeferredLighting, BuiltinShaderMode.Disabled);
        GraphicsSettings.SetShaderMode(BuiltinShaderType.ScreenSpaceShadows, BuiltinShaderMode.Disabled);
        GraphicsSettings.SetShaderMode(BuiltinShaderType.DepthNormals, BuiltinShaderMode.Disabled);
        GraphicsSettings.SetShaderMode(BuiltinShaderType.LightHalo, BuiltinShaderMode.Disabled);
        GraphicsSettings.SetShaderMode(BuiltinShaderType.LensFlare, BuiltinShaderMode.Disabled);
        GraphicsSettings.SetShaderMode(BuiltinShaderType.Sprite, BuiltinShaderMode.Disabled);
        GraphicsSettings.SetShaderMode(BuiltinShaderType.MotionVectors, BuiltinShaderMode.Disabled);

        InitArgs();
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "BUILD_EXP;BEHAVIAC_RELEASE;UNITY;DW_Android");
        PlayerSettings.bundleIdentifier = "com.da.lljxqp";
        PlayerSettings.bundleIdentifier = "com.dawang.jxqp";
        PlayerSettings.productName = "聊聊江西麻将";
        BuildPipeline.BuildPlayer(SceneList, m_outPath, BuildTarget.Android,
            BuildOptions.AcceptExternalModificationsToPlayer);
    }


    static void BuildAndroidBefore(eAutoBuild eType)
    {
        //SerVersion();
        InitArgs();
        string datapath = Application.dataPath;
        string[] strArray = datapath.Split('/');
        string parentPath = "";
        for (int i = 0; i < strArray.Length - 1; i++)
        {
            parentPath += strArray[i] + "/";
        }

        //       ReplaceSDKPlatformIcon(eType, parentPath);

        PlayerSettings.Android.keystoreName = parentPath + "tools/dw20170816.keystore";
        PlayerSettings.Android.keyaliasName = "dwkey";

        //if (eType == eAutoBuild.ebuild_dangle)
        //{
        //    PlayerSettings.keystorePass = "downjoy_345";
        //    PlayerSettings.keyaliasPass = "downjoy_345";
        //}
        //else if (eType == eAutoBuild.ebuild_sina)
        //{
        //    PlayerSettings.keystorePass = "android";
        //    PlayerSettings.keyaliasPass = "android";
        //}
        //else
        {
            PlayerSettings.keystorePass = "dw888888";
            PlayerSettings.keyaliasPass = "dw888888";
        }
    }

    static void BuildAndroid(eAutoBuild sdkType)
    {
        //       MovePluginDir(sdkType);
        //ReplaceDll("Android", "nguiLib");
        //ReplaceDll("Android", "DataEyeDll");
        BuildPipeline.BuildPlayer(SceneList, m_outPath, BuildTarget.Android,s_buildOptions);
        //       RecoveryDll("nguiLib");
        //        RecoveryDll("DataEyeDll");
        //       RecoveryPluginDir();
    }

    public static void BuildAndroidEnd()
    {
        RecoveryRess();
        //		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");
    }




    public static void BuildIOS()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "RELEASE;UNITY;DW_IOS");
        PlayerSettings.bundleIdentifier = "com.da.jxqp";
        PlayerSettings.productName = "聊聊江西麻将";

        BuildIosCommon(false, false);
    }

    public static void BuildIOSDebug()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "UNITY;DW_IOS");

        PlayerSettings.bundleIdentifier = "com.da.jxqp";
        PlayerSettings.productName = "聊聊江西麻将";

        BuildIosCommon(false, true);
    }

    public static void BuildIOSHD()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "PLAT_SDK_TT_HD");
        PlayerSettings.bundleIdentifier = "com.da.jxqp";
        PlayerSettings.productName = "聊聊江西麻将HD";

        BuildIosCommon();
    }


    public static void BuildIOSAB()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "RELEASE;UNITY;DW_IOS");

        PlayerSettings.bundleIdentifier = "com.da.jxqp";
        PlayerSettings.productName = "聊聊江西麻将";

        BuildIosCommon(true, false);
    }

    public static void BuildIOSAB_Debug()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "UNITY;DW_IOS");
        PlayerSettings.bundleIdentifier = "com.da.jxqp";
        PlayerSettings.productName = "聊聊江西麻将";

        BuildIosCommon(true, false);
    }





    private static BuildOptions s_buildOptions;
    static void BuildIosCommon(bool isAb = false, bool isDebug = false)
    {
        GraphicsSettings.SetShaderMode(BuiltinShaderType.DeferredShading, BuiltinShaderMode.Disabled);
        GraphicsSettings.SetShaderMode(BuiltinShaderType.DeferredReflections, BuiltinShaderMode.Disabled);
        GraphicsSettings.SetShaderMode(BuiltinShaderType.LegacyDeferredLighting, BuiltinShaderMode.Disabled);
        GraphicsSettings.SetShaderMode(BuiltinShaderType.ScreenSpaceShadows, BuiltinShaderMode.Disabled);
        GraphicsSettings.SetShaderMode(BuiltinShaderType.DepthNormals, BuiltinShaderMode.Disabled);
        GraphicsSettings.SetShaderMode(BuiltinShaderType.LightHalo, BuiltinShaderMode.Disabled);
        GraphicsSettings.SetShaderMode(BuiltinShaderType.LensFlare, BuiltinShaderMode.Disabled);
        GraphicsSettings.SetShaderMode(BuiltinShaderType.Sprite, BuiltinShaderMode.Disabled);
        GraphicsSettings.SetShaderMode(BuiltinShaderType.MotionVectors, BuiltinShaderMode.Disabled);

        PlayerSettings.iOS.scriptCallOptimization = ScriptCallOptimizationLevel.SlowAndSafe;
        PlayerSettings.iOS.microphoneUsageDescription = "是否允许使用麦克风";

        DeleteXGameDll();
        //ReplaceDll("iOS", "nguiLib");
        //ReplaceDll("iOS", "DataEyeDll");
        m_isAB = isAb;
        InitArgs();
        //GenLuaFiles();
        //BuildAB(1);
        //SerVersion();

        BuildPipeline.BuildPlayer(SceneList, m_outPath, BuildTarget.iOS, s_buildOptions);
        //RecoveryDll("nguiLib");
        //RecoveryDll("DataEyeDll");
        //RecoveryRess();
    }

    static void BuildAB(int state)
    {
        System.Type type = System.Type.GetType("BuildAssetsBundleEditor");
        var v = System.Activator.CreateInstance(type);
        System.Reflection.MethodInfo method = type.GetMethod("BuildAssetBundle");
        method.Invoke(v, new object[] { state });
    }

    static void GenLuaFiles()
    {
        System.Type type = System.Type.GetType("BuildAssetsBundleEditor");
        var v = System.Activator.CreateInstance(type);
        System.Reflection.MethodInfo method = type.GetMethod("GenLuaFiles");
        method.Invoke(v, null);
    }

    public static void BuildWin()
    {
        InitArgs();
        BuildPipeline.BuildPlayer(SceneList, m_outPath, BuildTarget.StandaloneWindows64, BuildOptions.Development | BuildOptions.AllowDebugging);
    }

    private static void ReplaceDll(string platPath, string dllName)
    {
        string datapath = Application.dataPath;
        string[] strArray = datapath.Split('/');
        string parentPath = "";
        for (int i = 0; i < strArray.Length - 1; i++)
        {
            parentPath += strArray[i] + "/";
        }
        string sourceFileName = parentPath + "tools/" + platPath + "/" + dllName + ".dll";
        string direcFileName = Application.dataPath + "/Plugins/ios/" + dllName + ".dll";

        try
        {
            //File.Copy(direcFileName, direcFileName + "temp", true);
            File.Copy(sourceFileName, direcFileName, true);
        }
        catch (IOException ex)
        {
            Debug.Log("[exception]" + ex.ToString());
        }
    }

    private static void DeleteXGameDll()
    {
        try
        {
            File.Delete(Application.streamingAssetsPath + "/xgame.ezfun");
        }
        catch (IOException)
        {

        }
    }

    private static void RecoveryDll(string dllName)
    {
        string sourceFileName = Application.dataPath + "/Plugins/" + dllName + ".dlltemp";
        string direcFileName = Application.dataPath + "/Plugins/" + dllName + ".dll";

        try
        {
            File.Copy(sourceFileName, direcFileName, true);
        }
        catch (IOException ex)
        {
            Debug.Log("[exception]" + ex.ToString());
        }

        try
        {
            File.Delete(sourceFileName);
        }
        catch (IOException ex)
        {
            Debug.Log("[exception]" + ex.ToString());
        }
    }

    public static void RemoveRessToTmp()
    {
        string sourceFileName = Application.dataPath + "/XGame/Resources/Prefab";
        string direcFileName = Application.dataPath + "/XGame/Temp";

        DirectoryInfo sourceDirect = new DirectoryInfo(sourceFileName);
        DirectoryInfo direct = new DirectoryInfo(direcFileName);
        if (!sourceDirect.Exists)
        {
            return;
        }

        if (direct.Exists)
        {
            direct.Delete(true);
        }
        sourceDirect.MoveTo(direcFileName);

        AssetDatabase.Refresh();
    }

    public static void RecoveryRess()
    {
        string sourceFileName = Application.dataPath + "/XGame/Temp";
        string direcFileName = Application.dataPath + "/XGame/Resources/Prefab";

        DirectoryInfo sourceDirect = new DirectoryInfo(sourceFileName);
        DirectoryInfo direct = new DirectoryInfo(direcFileName);
        if (!sourceDirect.Exists)
        {
            return;
        }

        if (direct.Exists)
        {
            direct.Delete();
        }
        sourceDirect.MoveTo(direcFileName);
    }

    private static void SerVersion()
    {
        PlayerSettings.bundleVersion = GameStart.PACKAGE_VERSION;
        //PlayerSettings.shortBundleVersion = GameStart.APP_VERSION;

        int ver = GetVersion();
        string verStr = ver.ToString() + GetStrInt(System.DateTime.Now.DayOfYear, 3) + GetStrInt(System.DateTime.Now.Hour, 2);
        PlayerSettings.Android.bundleVersionCode = int.Parse(verStr);
    }

    static string GetStrInt(int vint, int count)
    {
        string vstr = vint.ToString();
        while (vstr.Length < count)
        {
            vstr = "0" + vstr;
        }
        return vstr;
    }

    static int GetVersion()
    {
        string str = GameStart.PACKAGE_VERSION;
        if (string.IsNullOrEmpty(str))
        {
            return 0;
        }
        string[] strArray = str.Split('.');
        int version = int.Parse(strArray[0]) * 10000 + int.Parse(strArray[1]) * 100 + int.Parse(strArray[2]);
        return version;
    }

    //根据不同平台，添加不同的plugin 目录
    #region MovePluginDir
    static Dictionary<string, string> m_pluginDirDic = new Dictionary<string, string>();
    static void MovePluginDir(eAutoBuild sdkType)
    {
        m_pluginDirDic.Clear();

        string sourcePath = Application.dataPath + "/PlatformPlugin/";
        string destPath = Application.dataPath + "/Plugins/Android/";


        PlayerSettings.productName = "CGG";

        //switch (sdkType)
        //{
        //    case eAutoBuild.eBuild_MSDK:
        //        sourcePath += "YSDK_yyb";
        //        PlayerSettings.productName = "大圣伏魔";
        //        break;
        //}

        DirectoryInfo direc = new DirectoryInfo(sourcePath);
        DirectoryInfo[] childDirect = direc.GetDirectories();

        for (int childIndex = 0; childDirect != null && childIndex < childDirect.Length; childIndex++)
        {
            string childPath = sourcePath + "/" + childDirect[childIndex].Name;
            string childToPath = destPath + childDirect[childIndex].Name;

            DirectoryInfo childFromDirec = new DirectoryInfo(childPath);
            DirectoryInfo childToDirec = new DirectoryInfo(childToPath);

            if (childToDirec.Exists)
            {
                childToDirec.Delete(true);
            }

            m_pluginDirDic.Add(childToPath, childPath);
            childFromDirec.MoveTo(childToPath);
        }

        FileInfo[] fileArray = direc.GetFiles();
        for (int fileIndex = 0; fileArray != null && fileIndex < fileArray.Length; fileIndex++)
        {
            FileInfo file = fileArray[fileIndex];
            string fileName = file.Name;
            if (fileName.EndsWith("meta"))
            {
                continue;
            }

            string fileFromPath = sourcePath + "/" + fileName;
            string fileToPath = destPath + fileName;
            FileInfo fileToFile = new FileInfo(fileToPath);
            if (fileToFile.Exists)
            {
                fileToFile.Delete();
            }

            m_pluginDirDic.Add(fileToPath, fileFromPath);
            file.MoveTo(fileToPath);
        }
    }

    static void RecoveryPluginDir()
    {
        if (m_pluginDirDic.Count == 0)
        {
            return;
        }

        foreach (var kv in m_pluginDirDic)
        {
            string fromPath = kv.Key;
            string toPath = kv.Value;

            if (fromPath.EndsWith("xml"))
            {
                FileInfo fromFile = new FileInfo(fromPath);
                FileInfo toFile = new FileInfo(toPath);

                if (toFile.Exists)
                {
                    toFile.Delete();
                }

                fromFile.MoveTo(toPath);
            }
            else
            {
                DirectoryInfo fromDirect = new DirectoryInfo(fromPath);
                DirectoryInfo toDirect = new DirectoryInfo(toPath);

                if (toDirect.Exists)
                {
                    toDirect.Delete(true);
                }

                fromDirect.MoveTo(toPath);
            }
        }
    }

    #endregion

    //根据不同平台，添加不同的ICON目录
    #region SDKPlatformIcon

    public static void ReplaceSDKPlatformIcon(eAutoBuild etype, string path, BuildTargetGroup btg = BuildTargetGroup.Android)
    {
        string[] strIconName = new string[6] { "192.png", "144.png", "96.png", "72.png", "48.png", "36.png" };
        Texture2D[] textIcon = new Texture2D[6];
        int i = 0;

        {
            for (i = 0; i < strIconName.Length; ++i)
            {
                string strPath = "Assets/SDK_ICON/icon/" + strIconName[i];
                textIcon[i] = AssetDatabase.LoadAssetAtPath(strPath, typeof(Texture2D)) as Texture2D;
            }
        }

        for (i = 0; i < textIcon.Length; ++i)
        {
            if (textIcon[i] == null)
            {
                Debug.LogError("Icon没找到");
                return;
            }
        }

        PlayerSettings.SetIconsForTargetGroup(btg, textIcon);
    }
    #endregion
}
