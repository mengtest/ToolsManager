using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.GZip;

public class Util : MonoBehaviour
{

    public static RuntimePlatform m_platformId;

    public static string m_dataPath;

    public static string m_persistPath;

    public static string m_streamPath;

    public static void Init()
    {
 //       Debug.LogError("Util Init");
        m_platformId = Application.platform;
        m_dataPath = Application.dataPath;
        m_persistPath = Application.persistentDataPath;
        m_streamPath = Application.streamingAssetsPath;
    }

    public static int Int(object o)
    {
        return Convert.ToInt32(o);
    }

    public static float Float(object o)
    {
        return (float)Math.Round(Convert.ToSingle(o), 2);
    }

    public static long Long(object o)
    {
        return Convert.ToInt64(o);
    }

    public static int Random(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static float Random(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static string Uid(string uid)
    {
        int position = uid.LastIndexOf('_');
        return uid.Remove(0, position + 1);
    }

    public static long GetTime()
    {
        TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
        return (long)ts.TotalMilliseconds;
    }

    /// <summary>
    /// 搜索子物体组件-GameObject版
    /// </summary>
    public static T Get<T>(GameObject go, string subnode) where T : Component
    {
        if (go != null)
        {
            Transform sub = go.transform.Find(subnode);
            if (sub != null) return sub.GetComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// 搜索子物体组件-Transform版
    /// </summary>
    public static T Get<T>(Transform go, string subnode) where T : Component
    {
        if (go != null)
        {
            Transform sub = go.Find(subnode);
            if (sub != null) return sub.GetComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// 搜索子物体组件-Component版
    /// </summary>
    public static T Get<T>(Component go, string subnode) where T : Component
    {
        return go.transform.Find(subnode).GetComponent<T>();
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    public static T Add<T>(GameObject go) where T : Component
    {
        if (go != null)
        {
            T[] ts = go.GetComponents<T>();
            for (int i = 0; i < ts.Length; i++)
            {
                if (ts[i] != null) Destroy(ts[i]);
            }
            return go.gameObject.AddComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    public static T Add<T>(Transform go) where T : Component
    {
        return Add<T>(go.gameObject);
    }

    /// <summary>
    /// 查找子对象
    /// </summary>
    public static GameObject Child(GameObject go, string subnode)
    {
        return Child(go.transform, subnode);
    }

    /// <summary>
    /// 查找子对象
    /// </summary>
    public static GameObject Child(Transform go, string subnode)
    {
        Transform tran = go.Find(subnode);
        if (tran == null) return null;
        return tran.gameObject;
    }

    /// <summary>
    /// 取平级对象
    /// </summary>
    public static GameObject Peer(GameObject go, string subnode)
    {
        return Peer(go.transform, subnode);
    }

    /// <summary>
    /// 取平级对象
    /// </summary>
    public static GameObject Peer(Transform go, string subnode)
    {
        Transform tran = go.parent.Find(subnode);
        if (tran == null) return null;
        return tran.gameObject;
    }

    /// <summary>
    /// 手机震动
    /// </summary>
    public static void Vibrate()
    {
        //int canVibrate = PlayerPrefs.GetInt(Const.AppPrefix + "Vibrate", 1);
        //if (canVibrate == 1) iPhoneUtils.Vibrate();
    }

    /// <summary>
    /// Base64编码
    /// </summary>
    public static string Encode(string message)
    {
        byte[] bytes = Encoding.GetEncoding("utf-8").GetBytes(message);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Base64解码
    /// </summary>
    public static string Decode(string message)
    {
        byte[] bytes = Convert.FromBase64String(message);
        return Encoding.GetEncoding("utf-8").GetString(bytes);
    }

    /// <summary>
    /// 判断数字
    /// </summary>
    public static bool IsNumeric(string str)
    {
        if (str == null || str.Length == 0) return false;
        for (int i = 0; i < str.Length; i++)
        {
            if (!Char.IsNumber(str[i])) { return false; }
        }
        return true;
    }

    /// <summary>
    /// HashToMD5Hex
    /// </summary>
    public static string HashToMD5Hex(string sourceStr)
    {
        byte[] Bytes = Encoding.UTF8.GetBytes(sourceStr);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte[] result = md5.ComputeHash(Bytes);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
                builder.Append(result[i].ToString("x2"));
            return builder.ToString();
        }
    }

    /// <summary>
    /// 计算字符串的MD5值
    /// </summary>
    public static string md5(string source)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
        byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
        md5.Clear();

        string destString = "";
        for (int i = 0; i < md5Data.Length; i++)
        {
            destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
        }
        destString = destString.PadLeft(32, '0');
        return destString;
    }

    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    public static string md5file(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }

    /// <summary>
    /// 功能：压缩字符串
    /// </summary>
    /// <param name="infile">被压缩的文件路径</param>
    /// <param name="outfile">生成压缩文件的路径</param>
    public static void CompressFile(string infile, string outfile)
    {
        Stream gs = new GZipOutputStream(File.Create(outfile));
        FileStream fs = File.OpenRead(infile);
        byte[] writeData = new byte[fs.Length];
        fs.Read(writeData, 0, (int)fs.Length);
        gs.Write(writeData, 0, writeData.Length);
        gs.Close(); fs.Close();
    }

    /// <summary>
    /// 功能：输入文件路径，返回解压后的字符串
    /// </summary>
    public static string DecompressFile(string infile)
    {
        string result = string.Empty;
        Stream gs = new GZipInputStream(File.OpenRead(infile));
        MemoryStream ms = new MemoryStream();
        int size = 2048;
        byte[] writeData = new byte[size];
        while (true)
        {
            size = gs.Read(writeData, 0, size);
            if (size > 0)
            {
                ms.Write(writeData, 0, size);
            }
            else
            {
                break;
            }
        }
        result = new UTF8Encoding().GetString(ms.ToArray());
        gs.Close(); ms.Close();
        return result;
    }

    /// <summary>
    /// 压缩字符串
    /// </summary>
    public static string Compress(string source)
    {
        byte[] data = Encoding.UTF8.GetBytes(source);
        MemoryStream ms = null;
        using (ms = new MemoryStream())
        {
            using (Stream stream = new GZipOutputStream(ms))
            {
                try
                {
                    stream.Write(data, 0, data.Length);
                }
                finally
                {
                    stream.Close();
                    ms.Close();
                }
            }
        }
        return Convert.ToBase64String(ms.ToArray());
    }

    /// <summary>
    /// 解压字符串
    /// </summary>
    public static string Decompress(string source)
    {
        string result = string.Empty;
        byte[] buffer = null;
        try
        {
            buffer = Convert.FromBase64String(source);
        }
        catch
        {
            Debug.LogError("Decompress---->>>>" + source);
        }
        using (MemoryStream ms = new MemoryStream(buffer))
        {
            using (Stream sm = new GZipInputStream(ms))
            {
                StreamReader reader = new StreamReader(sm, Encoding.UTF8);
                try
                {
                    result = reader.ReadToEnd();
                }
                finally
                {
                    sm.Close();
                    ms.Close();
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 清除所有子节点
    /// </summary>
    public static void ClearChild(Transform go)
    {
        if (go == null) return;
        for (int i = go.childCount - 1; i >= 0; i--)
        {
            Destroy(go.GetChild(i).gameObject);
        }
    }


    /// <summary>
    /// 清理内存
    /// </summary>
    public static void ClearMemory()
    {
        GC.Collect(); Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// 是否为数字
    /// </summary>
    public static bool IsNumber(string strNumber)
    {
        Regex regex = new Regex("[^0-9]");
        return !regex.IsMatch(strNumber);
    }

    /// <summary>
    /// 取得App包里面的读取目录
    /// </summary>
    public static Uri AppContentDataUri
    {
        get
        {
            string dataPath = Application.dataPath;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                var uriBuilder = new UriBuilder();
                uriBuilder.Scheme = "file";
                uriBuilder.Path = Path.Combine(dataPath, "Raw");
                return uriBuilder.Uri;
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                return new Uri("jar:file://" + dataPath + "!/assets");
            }
            else
            {
                var uriBuilder = new UriBuilder();
                uriBuilder.Scheme = "file";
                uriBuilder.Path = Path.Combine(dataPath, "StreamingAssets");
                return uriBuilder.Uri;
            }
        }
    }


    /// <summary>
    /// 取得行文本
    /// </summary>
    public static string GetFileText(string path)
    {
        return File.ReadAllText(path);
    }

    /// <summary>
    /// 网络可用
    /// </summary>
    public static bool NetAvailable
    {
        get
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }

    /// <summary>
    /// 是否是无线
    /// </summary>
    public static bool IsWifi
    {
        get
        {
            return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        }
    }

    /// <summary>
    /// 添加lua单机事件
    /// </summary>
    public static void AddClick(GameObject go, System.Object luafuc)
    {
        UIEventListener.Get(go).onClick += delegate (GameObject o)
        {
            LuaInterface.LuaFunction func = (LuaInterface.LuaFunction)luafuc;
            func.Call();
        };
    }

    /// <summary>
    /// 取得Lua路径
    /// </summary>
    public static string AppContentPathA()
    {
        return Application.dataPath + Path.DirectorySeparatorChar;
        //string path = string.Empty;
        //switch (Application.platform)
        //{
        //    case RuntimePlatform.Android:
        //        path = Application.dataPath + "!/assets/";
        //        break;
        //    case RuntimePlatform.IPhonePlayer:
        //        path = Application.dataPath + "/Raw/";
        //        break;
        //    default:
        //        path = Application.dataPath + "/StreamingAssets/";
        //        break;
        //}

        //return path;
    }

    public static string LuaPath(string name,out bool needCry, bool isXGame = true, string endWith = ".lua", string subPath = "Lua")
    {
        if (!name.EndsWith(endWith))
        {
            name = name.Replace('.', Path.DirectorySeparatorChar);
        }
        else
        {
            int index = name.LastIndexOf('.');
            name = name.Substring(0, index);
            name = name.Replace('.', Path.DirectorySeparatorChar);
        }
        name += endWith;

        var p = m_persistPath + Path.DirectorySeparatorChar;
       // var p = EZFunTools.CachePath + Path.DirectorySeparatorChar;
        p += subPath + Path.DirectorySeparatorChar + name;
        if (File.Exists(p))
        {
            needCry = true;
        }
        else
        {
            if ((m_platformId == RuntimePlatform.OSXEditor || m_platformId == RuntimePlatform.WindowsEditor) && isXGame)
            {
                p = m_dataPath + Path.DirectorySeparatorChar +
                    "XGame" + Path.DirectorySeparatorChar +
                "Script" + Path.DirectorySeparatorChar +
                "Lua" + Path.DirectorySeparatorChar + name;
                needCry = false;
            }
            else
            {
                p = m_streamPath + Path.DirectorySeparatorChar +
                    subPath + Path.DirectorySeparatorChar + name;
                needCry = true;
            }
        }
        return p;
    }

    public static bool isApplePlatform
    {
        get
        {
            return Application.platform == RuntimePlatform.IPhonePlayer ||
                   Application.platform == RuntimePlatform.OSXEditor ||
                   Application.platform == RuntimePlatform.OSXPlayer;
        }
    }

    public static void DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                File.Delete(path);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    public static void DeleteFolder(string path)
    {
        var dir = new DirectoryInfo(path);
        if (dir.Exists)
        {
            try
            {
                dir.Delete(true);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    public static bool CheckFileExists(string filePath)
    {
        return System.IO.File.Exists(filePath);
    }

    /// <summary>
    /// 创建目录 给lua使用
    /// </summary>
    public static void CreateFolder(string newDirName) 
    {
        var dir = new DirectoryInfo(newDirName);
        if (!dir.Exists)
        {
            dir.Create();
        }
    }

    public static void DeleteFolderFile(string path)
    {
        var dir = new DirectoryInfo(path);
        var files = dir.GetFiles("*.*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            DeleteFile(file.FullName);
        }
        var cdirs = dir.GetDirectories();
        foreach (var cdir in cdirs)
        {
            DeleteFolderFile(cdir.FullName);
        }
    }
}