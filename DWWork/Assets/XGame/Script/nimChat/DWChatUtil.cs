using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DWChatUtils
{

    public static long toTicks(DateTime dt)
    {
        TimeSpan ts = dt.ToUniversalTime() - new DateTime(1970, 1, 1);
        Int32 ticks = System.Convert.ToInt32(ts.TotalSeconds);
        return ticks;
    }

    public static Int64 createUniqueID()
    {
        byte[] buffer = Guid.NewGuid().ToByteArray();
        return BitConverter.ToInt64(buffer, 0);
    }

    public static string getStreamingAssets()
    {
        string path = null;
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            path = Application.streamingAssetsPath;
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            //_appDataPath = Application.dataPath + "/NimUnityDemo";
            path = "jar: file://" + Application.dataPath + "!/assets/";
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            path = "StreamingAssets";
        }

        return path;
    }

    private static string _appDataPath;

    public static string AppDataPath
    {
        get
        {
            if (string.IsNullOrEmpty(_appDataPath))
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    _appDataPath = Application.persistentDataPath + "/NimUnityDemo";
                }
                else if (Application.platform == RuntimePlatform.Android)
                {
                    string androidPathName = "com.netease.nim_unity_android_demo";
                    if (System.IO.Directory.Exists("/sdcard"))
                        _appDataPath = Path.Combine("/sdcard", androidPathName);
                    else
                        _appDataPath = Path.Combine(Application.persistentDataPath, androidPathName);
                }
                else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    _appDataPath = "NimUnityDemo";
                }
                else
                {
                    _appDataPath = Application.persistentDataPath + "/NimUnityDemo";
                }
                UnityEngine.Debug.Log("AppDataPath:" + _appDataPath);
            }
            return _appDataPath;
        }
    }

    public static string WriteablePath
    {
        get
        {
            string path = null;
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                path = "";
            else
                path = AppDataPath;
            return path;
        }
    }

    public static void DebugLog(string log)
    {

        Debug.Log(log);
    }

    public static void DebugLogFormat(string format, params object[] args) 
    {

        Debug.LogFormat(format, args);
    }
}

