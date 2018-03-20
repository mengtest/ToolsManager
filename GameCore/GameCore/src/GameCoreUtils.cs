using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Core.CAssetBundleData;

public class GameCoreUtils
{
    public static T GetOrAddComponent<T>(GameObject gb)
    where T : UnityEngine.Component
    {
        return GetOrAddComponent(gb, typeof(T)) as T;
    }

    public static Component GetOrAddComponent(GameObject gb, Type type)
    {
        if (gb == null)
        {
            return null;
        }
        Component t = gb.GetComponent(type);
        if (t == null)
        {
            t = gb.AddComponent(type);
        }

        return t;
    }

    public static ABDesc StrToResCABDataStructLs(string str)
    {
        ABDesc abStruct = new ABDesc();
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            var obj = LitJson.JsonMapper.ToObject(str);
            if (obj["m_files"] != null && obj["m_files"].Count > 0)
            {
                var list = obj["m_files"];
                for (int i = 0; i < list.Count; i++)
                {
                    var row = list[i];
                    ABFileDesc data = new ABFileDesc();
                    if (row["m_gbName"] != null)
                    {
                        data.m_gbName = row["m_gbName"].ToString();
                    }
                    if (row["m_ressVersion"] != null && row["m_ressVersion"].IsInt)
                    {
                        data.m_ressVersion = (int)row["m_ressVersion"];
                    }
                    if (row["m_bundleName"] != null)
                    {
                        data.m_bundleName = list[i]["m_bundleName"].ToString();
                    }
                    abStruct.m_files.Add(data);
                }
            }
            if (obj["m_scenes"] != null && obj["m_scenes"].Count > 0)
            {
                var list = obj["m_scenes"];
                for (int i = 0; i < list.Count; i++)
                {
                    var row = list[i];
                    ABSceneDesc data = new ABSceneDesc();
                    if (row["m_sceneName"] != null)
                    {
                        data.m_sceneName = row["m_sceneName"].ToString();
                    }
                    if (row["m_ressVersion"] != null && row["m_ressVersion"].IsInt)
                    {
                        data.m_ressVersion = (int)row["m_ressVersion"];
                    }
                    if (row["m_bundleName"] != null)
                    {
                        data.m_bundleName = list[i]["m_bundleName"].ToString();
                    }
                    abStruct.m_scenes.Add(data);
                }
            }
            if (obj["m_abDepends"] != null && obj["m_abDepends"].Count > 0)
            {
                var list = obj["m_abDepends"];
                for (int i = 0; i < list.Count; i++)
                {
                    var row = list[i];
                    ABDependencies data = new ABDependencies();
                    if (row["m_dependenciesAb"] != null && row["m_dependenciesAb"].IsArray)
                    {
                        var deList = row["m_dependenciesAb"];
                        for (int j = 0; j < deList.Count; j++)
                        {
                            data.m_dependenciesAb.Add(deList[j].ToString());
                        }
                    }
                    if (row["m_ressVersion"] != null && row["m_ressVersion"].IsInt)
                    {
                        data.m_ressVersion = (int)row["m_ressVersion"];
                    }
                    if (row["m_abName"] != null)
                    {
                        data.m_abName = row["m_abName"].ToString();
                    }
                    //if (row["m_firstVersion"] != null && row["m_firstVersion"].IsBoolean)
                    //{
                    //    data.m_firstVersion = (int)row["m_firstVersion"];
                    //}
                    abStruct.m_abDepends.Add(data);
                }
            }
        }
        else
        {
            abStruct = LitJson.JsonMapper.ToObject<ABDesc>(str);
        }
       
        return abStruct;
    }

    public const float m_DownScaleUpdate = 0.2f;
}


public class PathUtils
{
    public const string ResourceRefPath = "Assets/XGame/Resources/";
    public const string AssetBundleRefPath = "Assets/StreamingAssets/AssetBundles/";
    private static string _ProjectPath = null;
    public static string ProjectPath
    {
        get
        {
            if (string.IsNullOrEmpty(_ProjectPath))
            {

                const string assetDirName = "Assets";
                int nAssetDirIndex = Application.dataPath.LastIndexOf(assetDirName, System.StringComparison.OrdinalIgnoreCase);
                if (nAssetDirIndex < 0)
                {
                    _ProjectPath = Application.dataPath;
                }
                else
                {
                    _ProjectPath = Application.dataPath.Remove(nAssetDirIndex, Application.dataPath.Length - nAssetDirIndex);
                }
            }
            return _ProjectPath;
        }
    }

    private static string _LuaPath = null;
    public static string LuaPath
    {
        get
        {
            if (string.IsNullOrEmpty(_LuaPath))
            {
                _LuaPath = Application.streamingAssetsPath + "/Lua/";
            }
            return _LuaPath;
        }
    }

    private static string _AssetBundlePath = null;
    public static string AssetBundlePath
    {
        get
        {
            //unity 5.3 版本不能直接用Application.StreamingAssets读取StreamingAssets下文件，5.4版本以后才支持此功能，所以这里需要拼接读取路径
            if (string.IsNullOrEmpty(_AssetBundlePath))
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    _AssetBundlePath = Application.streamingAssetsPath + "/AssetBundles/";
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    _AssetBundlePath = Application.streamingAssetsPath + "/AssetBundles/";
                }
                else
                {
                    _AssetBundlePath = Application.streamingAssetsPath + "/AssetBundles/";
                }
                return _AssetBundlePath;
            }
            return _AssetBundlePath;
        }
    }


    public static string AssetBundlePersistPath
    {
        get
        {
            //if (Application.platform == RuntimePlatform.IPhonePlayer)
            //{
            //    return Application.temporaryCachePath + "/AssetBundles/";
            //}
            //else
            {
                return Application.persistentDataPath + "/AssetBundles/";
            }
        }
    }


    private static string _MapFilePath = null;
    public static string MapFilePath
    {
        get
        {
            if (string.IsNullOrEmpty(_MapFilePath))
            {
                _MapFilePath = Application.streamingAssetsPath + "/MapFile/";
            }
            return _MapFilePath;
        }
    }

    private static string _TableDataPath = null;
    public static string TableDataPath
    {
        get
        {
            if (string.IsNullOrEmpty(_TableDataPath))
            {
                _TableDataPath = Application.streamingAssetsPath + "/TableData/";
            }
            return _TableDataPath;
        }
    }

    public static void EnsureFilePathExist(string relativePathIncludingFileName)
    {
        int nFileNameStart = relativePathIncludingFileName.LastIndexOf('/');
        if (nFileNameStart > 0)
        {
            string relativePath = relativePathIncludingFileName.Remove(nFileNameStart);
            string absolutePath = ProjectPath + relativePath;
            Directory.CreateDirectory(absolutePath);
        }
    }

    public static string GetExtension(string filePath)
    {
        string extWithDot = Path.GetExtension(filePath);
        return extWithDot;
    }

    public static string ChangeExtension(string filePath, string ext)
    {
        return Path.ChangeExtension(filePath, ext);
    }

    /// <summary>
    /// 后缀名转换字典
    /// </summary>
    public static Dictionary<string, string> ExtCovertDic = new Dictionary<string, string>{
            {".bin" , ".bytes"}
        };

    /// <summary>
    /// 后缀名修改(.bin ==> .bytes)
    /// </summary>
    /// <param name="oldFilePath"></param>
    /// <returns></returns>
    public static string TryConvertFileExt(string oldFilePath)
    {
        string fileExt = Path.GetExtension(oldFilePath);
        string convExt = string.Empty;

        if (PathUtils.ExtCovertDic.TryGetValue(fileExt, out convExt))
        {
            string newFilePath = Path.ChangeExtension(oldFilePath, convExt);
            if (File.Exists(newFilePath) == false)
            {
                File.Copy(oldFilePath, newFilePath);
            }

            return newFilePath;
        }

        return oldFilePath;
    }

    /// <summary>
    /// 获取修改后的后缀名(.bin ==> .bytes)
    /// </summary>
    /// <param name="oldFilePath"></param>
    /// <returns></returns>
    public static string GetConvertFileExt(string oldFilePath)
    {
        string fileExt = Path.GetExtension(oldFilePath);
        string convExt = string.Empty;

        if (PathUtils.ExtCovertDic.TryGetValue(fileExt, out convExt))
        {
            string newFilePath = Path.ChangeExtension(oldFilePath, convExt);
            return newFilePath;
        }

        return oldFilePath;
    }

    public static void DeleteDirectory(string dirPath)
    {
        string[] files = Directory.GetFiles(dirPath);
        string[] dirs = Directory.GetDirectories(dirPath);

        for (int nFile = 0; nFile < files.Length; ++nFile)
        {
            File.SetAttributes(files[nFile], FileAttributes.Normal);
            File.Delete(files[nFile]);
        }

        for (int nDir = 0; nDir < dirs.Length; ++nDir)
        {
            DeleteDirectory(dirs[nDir]);
        }

        Directory.Delete(dirPath, false);
    }

    public static string GetSceneNameFromPath(string sceneFilePath)
    {
        int nPreSceneNameStart = sceneFilePath.LastIndexOf('/');
        if (nPreSceneNameStart >= 0)
        {
            sceneFilePath = sceneFilePath.Substring(nPreSceneNameStart + 1);
        }

        int nSceneNameEnd = sceneFilePath.LastIndexOf(".unity", System.StringComparison.OrdinalIgnoreCase);
        if (nSceneNameEnd >= 0)
        {
            sceneFilePath = sceneFilePath.Substring(0, nSceneNameEnd);
        }

        return sceneFilePath;
    }

    public static bool HasExtension(string str)
    {
        return str.LastIndexOf('.') > 0;
    }
}
