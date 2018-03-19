/************************************************************
    File      : Utility.cs
    brief     : 一些公共参数,公共函数
    author    : JanusLiu   janusliu@ezfun.cn 
    version   : 1.0
    date      : 2014/10/13 10:1:6
    copyright : Copyright 2014 EZFun Inc.
**************************************************************/


using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using System.Security.Cryptography;
using LitJson;


public enum VersionType : int
{
    /// <summary>
    /// 程序版本号，在Android中是跟随xgame.ezfun走  而在iOS中是固定
    /// </summary>
    App = 0,
    /// <summary>
    /// 
    /// </summary>
    Resource,
    Activity,
    /// <summary>
    /// 当前外网运行的最大版本号  update_config中serverInfo列表中的version
    /// </summary>
	PublishedAppVersion,
    /// <summary>
    /// 包的版本号 即发布时候的GameStart中APP_VERSION 包打出之后固定
    /// </summary>
    PackageVersion,
    Max,
}

public enum ENUM_SDK_PLATFORM
{
    ESP_General = 0,
    ESP_Ios = 100,
    ESP_Android = 200,
    ESP_Debug = 10001
}

public enum EnumAppName
{
    general = 0,
    andriod = 1,
    debug = 2,
    ios = 100
}



public class DWTools
{

    public static void WarmupSerializer()
    {
#if !UNITY_IOS

#endif
    }

    public static bool CaseInsensitiveContains(string text, string value, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
    {
        return text.IndexOf(value, stringComparison) >= 0;
    }

    public static string GetNoColorStr(string str)
    {
        // string format = "<color=#abcedf>text</color>"
        string tempStr = str;
        if (str.Length > 23)
        {
            tempStr = tempStr.Substring(15, tempStr.Length - 23);
        }
        return tempStr;
    }

    public static void ReplaceAnimator(Animator animator, RuntimeAnimatorController replace)
    {
        if (animator == null || replace == null)
            return;
        AnimatorOverrideController overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
        if (overrideController != null)
        {
            overrideController.runtimeAnimatorController = null;
        }
        animator.runtimeAnimatorController = replace;
    }

    public static void HidePlayerPartilce(GameObject target)
    {
        if (target == null)
        {
            return;
        }
        ParticleSystem[] pats = target.GetComponentsInChildren<ParticleSystem>(true);
        for (int i = 0; i < pats.Length; i++)
        {
            pats[i].gameObject.SetActive(false);
        }
    }

    public static int CompressMoveDir(Vector3 moveDir)
    {
        Quaternion quaternion = Quaternion.LookRotation(moveDir);
        return (int)(quaternion.eulerAngles.y * 65535.0f / 360.0f + 0.5f);
    }

    public static Vector3 UncompressMovEulerAngel(int moveDir)
    {
        Vector3 euler = Vector3.zero;
        float delta = 360f / 65535.0f;
        euler.y = moveDir * delta;
        return euler;
    }

    public static Quaternion UncompressMoveDir(int moveDir)
    {
        Vector3 euler = Vector3.zero;
        float delta = 360f / 65535.0f;
        euler.y = moveDir * delta;

        return Quaternion.Euler(euler);
    }

    public static bool IsNumString(string name)
    {
        string patten = @"^[0-9]+$";
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(patten);
        return regex.IsMatch(name);
    }

    static public bool IsInTime(float t, float Start, float end)
    {
        if (t >= Start && t <= end)
            return true;
        return false;
    }


    public static void HandleRendererState(GameObject gb, bool state)
    {
        if (gb == null)
        {
            return;
        }
        ParticleSystem[] particleSyss = gb.GetComponentsInParent<ParticleSystem>();
        for (int i = 0; i < particleSyss.Length; i++)
        {
            particleSyss[i].enableEmission = state;
        }
        Renderer[] renderers = gb.GetComponentsInParent<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = state;
        }
    }

    public static Vector2 ListToVect2(List<int> list)
    {
        Vector2 vect = Vector2.zero;
        if (list != null && list.Count == 2)
        {
            vect.x = list[0];
            vect.y = list[1];
        }
        else if (list != null && list.Count == 1)
        {
            vect.x = list[0];
        }
        else
        {
            //			Debug.LogWarning("[Con not convert to vector2]");
        }
        return vect;
    }

    public static Vector2 ListToVect2(List<float> list)
    {
        Vector2 vect = Vector2.zero;
        if (list != null && list.Count == 2)
        {
            vect.x = list[0];
            vect.y = list[1];
        }
        else
        {
            Debug.LogWarning("[Con not convert to vector2]");
        }
        return vect;
    }

    public static Vector3 LerpAngle(Vector3 from, Vector3 to, float delta, float minDelta = 0, float maxDelta = 360)
    {
        float x = LerpAngleClamp(from.x, to.x, delta, minDelta, maxDelta);
        float y = LerpAngleClamp(from.y, to.y, delta, minDelta, maxDelta);
        float z = LerpAngleClamp(from.z, to.z, delta, minDelta, maxDelta);
        return new Vector3(x, y, z);
    }

    public static float LerpAngleClamp(float from, float to, float delta, float minDelta, float maxDelta)
    {
        float result;
        if (Mathf.Abs(Mathf.DeltaAngle(to, from)) < minDelta)
        {
            result = to;
        }
        else
        {
            result = Mathf.LerpAngle(from, to, delta);
            float absChange = Mathf.Abs(result - from);
            int x = (int)((result - from) / absChange);
            if (absChange < minDelta)
            {
                result = from + minDelta * x;
            }
            else if (absChange > maxDelta)
            {
                result = from + maxDelta * x;
            }
        }
        return result;
    }

    public static string EZSubString(string str, int left, int right)
    {
        if (string.IsNullOrEmpty(str))
        {
            return "";
        }

        if (left < 0 || left >= right)
        {
            return "";
        }

        if (str.Length <= left || str.Length < (right - left))
        {
            return "";
        }

        return str.Substring(left, right);
    }

    public static float CalculateByFormulaID(int ID, float values0, float values1, float values2)
    {
        float result = 0;
        if (ID == 1)
        {
            return values1 * values0 + values2;
        }
        return result;
    }

    public enum AxleType
    {
        x,
        y,
        z,
    }
    public static float Get2DDistance(Vector3 pos1, Vector3 pos2, AxleType type)
    {
        Vector3 vec = new Vector3(1, 1, 1);
        switch (type)
        {
            case AxleType.x:
                vec.x = 0;
                break;
            case AxleType.y:
                vec.y = 0;
                break;
            case AxleType.z:
                vec.z = 0;
                break;
        }

        pos1 = new Vector3(pos1.x * vec.x, pos1.y * vec.y, pos1.z * vec.z);
        pos2 = new Vector3(vec.x * pos2.x, vec.y * pos2.y, vec.z * pos2.z);

        return (pos1 - pos2).magnitude;
    }

    /// <summary>
    /// 以后优化可以改成队列的，同GetTrans
    /// </summary>
    /// <param name="gb"></param>
    /// <param name="layer"></param>
    public static void SetLayer(GameObject gb, int layer, bool setChildren = false)
    {
        if (gb == null)
        {
            return;
        }
        SetLayer(gb.transform, layer, setChildren);
    }

    public static void SetLayer(Transform gb, int layer, bool setChildren = false)
    {
        if (gb == null)
        {
            return;
        }
        gb.gameObject.layer = layer;
        if (setChildren)
        {
            for (int i = 0; i < gb.transform.childCount; i++)
            {
                SetLayer(gb.transform.GetChild(i), layer, true);
            }
        }
    }

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

    public static Component GetOrAddComponent(GameObject gb, string typeStr)
    {
        if (gb == null)
        {
            return null;
        }
        Component t = gb.GetComponent(typeStr);
        if (t == null)
        {
            Type type = Type.GetType(typeStr);
            t = gb.AddComponent(type);
        }
        return t;
    }

    public static T DisableComponent<T>(GameObject gb)
        where T : UnityEngine.Behaviour
    {
        if (gb == null)
        {
            return null;
        }
        T t = gb.GetComponent<T>();
        if (t != null)
        {
            t.enabled = false;
        }

        return t;
    }

    static bool isLegalCharacters(String strLegal)
    {
        // 合法的字符为;
        // 字母,数字,字符:  a-z      A-Z     0-9      -   .   @    _;
        // 对应的ASCII码：  97-123   65-90   48-57   45  46  64   95;

        char[] chCheck = strLegal.ToLower().ToCharArray();
        for (int i = 0; i < strLegal.Length; i++)
        {
            // 转换成数值型判断其ASCII码;
            try
            {
                int sOneCharacter = Convert.ToInt16(chCheck[i]);
                if (!((sOneCharacter >= 97 && sOneCharacter <= 123) || (sOneCharacter >= 48 && sOneCharacter <= 57) ||
                      sOneCharacter == 45 || sOneCharacter == 46 || sOneCharacter == 64 || sOneCharacter == 95))
                {
                    return false;
                }
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        return true;
    }

    public static string GetSecToStr(int time, bool withHour = true)
    {
        string str = "";

        int h = time / 3600;
        if (withHour)
        {
            str += GetNormalStr(h) + ":";
        }

        int m = (time - h * 3600) / 60;
        str += GetNormalStr(m) + ":";

        int s = time - h * 3600 - m * 60;
        str += GetNormalStr(s);

        return str;
    }

    public static string GetSecToShortStr(int time)
    {
        string str = "";

        int h = time / 3600;
        //str += GetNormalStr(h) + ":";

        int m = (time - h * 3600) / 60;
        str += GetNormalStr(m) + ":";

        int s = time - h * 3600 - m * 60;
        str += GetNormalStr(s);

        return str;
    }

    static string GetNormalStr(int time)
    {
        string str = "";

        if (time >= 10)
        {
            str = time.ToString();
        }
        else if (time > 0)
        {
            str = "0" + time.ToString();
        }
        else
        {
            str = "00";
        }

        return str;
    }
  
    // 获取assetbundle路径
    static public string AssetBundlePath
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }

    private static string m_avaliablePath = "";

    // 获取可写路径
    static public string AvailablePath
    {
        get
        {
            if (string.IsNullOrEmpty(m_avaliablePath))
            {
                m_avaliablePath = Application.persistentDataPath;
            }
            return m_avaliablePath;
        }
    }

    private static string m_cachePath = "";

    /// <summary>
    /// 因为苹果要求，在iphone下 此目录在Application.temporaryCachePath 
    /// 其他情况在Application.persistentDataPath
    /// </summary>
    static public string CachePath
    {
        get
        {
            if (string.IsNullOrEmpty(m_cachePath))
            {
                //if (Application.platform == RuntimePlatform.IPhonePlayer)
                //{
                //    m_cachePath = Application.temporaryCachePath;
                //}
                //else
                {
                    m_cachePath = Application.persistentDataPath;
                }
            }
            return m_cachePath;
        }
    }

    static private string m_streamPath;
    static public string StreamPath
    {
        get
        {
            if (string.IsNullOrEmpty(m_streamPath))
            {
                m_streamPath = Application.streamingAssetsPath;
            }
            return m_streamPath;
        }
        set
        {
            m_streamPath = value;
        }
    }
    // 压缩
    static public void FastZipCompress(string path)
    {
        var zipName = path + ".zip";
        (new FastZip()).CreateZip(zipName, path, true, "");
    }

    static public void FastZipCompress(string path,string zipName)
    {
        (new FastZip()).CreateZip(zipName, path, true, "");
    }

    // 解压缩
    static public void FastZipUnCompress(string path)
    {
        var directName = path.Substring(0, path.Length - 4);
        (new FastZip()).ExtractZip(path, directName, "");
    }

    static public void FastZipUnCompress(string path, string directName)
    {
        (new FastZip()).ExtractZip(path, directName, "");
    }

    // 下载文件
    static public IEnumerator WWWDownloadFile(string addr, Action<byte[]> onDownloaded)
    {
        WWW w = new WWW(addr);
        while (!w.isDone) { yield return new WaitForEndOfFrame(); }
        if (w.error != null) { Debug.LogError(w.error); }
        onDownloaded(w.bytes);
    }

    static public IEnumerator WWWDownloadFile(string addr, Action<string> onDownloaded)
    {
        WWW w = new WWW(addr);
        while (!w.isDone) { yield return new WaitForEndOfFrame(); }
        if (w.error != null) { Debug.LogError(w.error); }
        onDownloaded(w.text);
    }

    // md5
    static public bool VerifyMD5(string fn, string srcMD5)
    {
        return GenMD5(fn) == srcMD5;
    }

    static public string GenMD5(string fn)
    {
        var md5CSP = new System.Security.Cryptography.MD5CryptoServiceProvider();
        var fs = new System.IO.FileStream(fn, FileMode.Open, FileAccess.Read);
        var arrbytHashValue = md5CSP.ComputeHash(fs);
        var strHashData = System.BitConverter.ToString(arrbytHashValue).Replace("-", "");
        fs.Close();
        return strHashData;
    }

    public static int GetLineNum()
    {
        System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1, true);
        return st.GetFrame(0).GetFileLineNumber();
    }

    public static string GetCurSourceFileName()
    {
        System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1, true);

        return st.GetFrame(0).GetFileName();

    }

    public static bool CheckLoadAllAB()
    {
        return false;
        //if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        //{
        //    return Constants.FORCE_LOAD_AB;
        //}
        //else if (Application.platform == RuntimePlatform.Android)
        //{
        //    return false;
        //}
        //else
        //{
        //    return false;
        //}
    }


    public static byte[] ReadFileStream(string path)
    {
        byte[] b = null;
        if (File.Exists(path))
        {
            using (Stream file = File.OpenRead(path))
            {
                b = new byte[(int)file.Length];
                file.Read(b, 0, b.Length);
                file.Close();
                file.Dispose();
            }
        }
        if (b == null)
            Debug.LogError("ReadFileStream Read file failed! " + path);
        return b;
    }

    public static int GetCharacterCount(string str)
    {
        int chrCount = 0;
        foreach (var s in str)
        {
            if (Convert.ToInt32(s) > 127)
            {
                chrCount += 2;
            }
            else
            {
                ++chrCount;
            }
        }
        return chrCount;
    }

    public static string GetMD5(string sDataIn)
    {
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] bytValue, bytHash;
        bytValue = System.Text.Encoding.UTF8.GetBytes(sDataIn);
        bytHash = md5.ComputeHash(bytValue);
        md5.Clear();
        string sTemp = "";
        for (int i = 0; i < bytHash.Length; i++)
        {
            sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
        }
        return sTemp.ToLower();
    }

    public static string GetMD5(byte[] sDataIn)
    {
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] bytHash;
        bytHash = md5.ComputeHash(sDataIn);
        md5.Clear();
        string sTemp = "";
        for (int i = 0; i < bytHash.Length; i++)
        {
            sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
        }
        return sTemp.ToLower();
    }


    public static void SetActive(GameObject go, bool state)
    {
        if (go)
        {
            go.SetActive(state);
        }
    }

    public static void SetActive(Transform trans, bool state)
    {
        if (trans)
        {
            trans.gameObject.SetActive(state);
        }
    }

    static public float Get2MonsterDis(Transform attacker, Transform defenser)
    {
        if (attacker == null || defenser == null)
        {
            return 0;
        }

        CharacterController cc = defenser.GetComponent<CharacterController>();
        float TargetScale;
        if (cc != null)
        {
            TargetScale = cc.radius * defenser.transform.localScale.z;
        }
        else
        {
            TargetScale = 0;
        }

        float distance = Get2DDistance(attacker.position, defenser.position, AxleType.y) - TargetScale;

        return distance;
    }



    // 获取当前时间的unix时间
    static public long GetNowUnixTime()
    {
        return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
    }

    // 获取一个时间与当前时间的间隔
    static public long GetTimeInterval(long time)
    {
        if (time == 0)
        {
            return 0;
        }
        var now = GetNowUnixTime();
        return now - time;
    }

    //秒数转换成00:00形式
    public static string GetTimeStringBySecond(int secondTotal)
    {
        int minus = (secondTotal - (secondTotal % 60)) / 60;
        int second = secondTotal % 60;
        string minusStr = "";
        string secondStr = "";
        if (minus < 10)
        {
            minusStr = "0" + minus;
        }
        else
        {
            minusStr = minus.ToString();
        }
        if (second < 10)
        {
            secondStr = "0" + second;
        }
        else
        {
            secondStr = second.ToString();
        }
        return minusStr + ":" + secondStr;
    }



    public static System.DateTime ConvertIntDateTime(long d)
    {
        System.DateTime time = System.DateTime.MinValue;
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        time = startTime.AddSeconds(d);
        return time;
    }

    // 只精确到X分X秒的
    public static string GetTimeStrMinSec(long time)
    {
        var min = time / 60;
        var sec = time % 60;
        return string.Format("{0:D2}", min) + ":" + string.Format("{0:D2}", sec);
    }

    // 精确到时分秒
    public static string GetTimeStrHourMinSec(long time)
    {
        var h = time / 3600;
        var m = time % 3600 / 60;
        var s = time % 3600 % 60;
        string strm = m.ToString();
        if (m < 10)
        {
            strm = "0" + strm;
        }
        string strs = s.ToString();
        if (s < 10)
        {
            strs = "0" + strs;
        }
        return h + ":" + strm + ":" + strs;
    }

    // 精确到X时X分的 24小时制
    public static string GetTimeStrHourMin(long time)
    {
        var hour = time / 3600 % 24;
        var min = time % 3600 / 60;
        string strm = min.ToString();
        if (min < 10)
        {
            strm = "0" + strm;
        }
        return hour + ":" + strm;
    }

    static public bool JsonDataContainsKey(LitJson.JsonData data, string key)
    {
        bool result = false;
        if (data == null)
        {
            return result;
        }

        if (!data.IsObject)
        {
            return result;
        }

        IDictionary tdictionary = data as IDictionary;
        if (tdictionary == null)
        {
            return result;
        }

        if (tdictionary.Contains(key))
        {
            result = true;
        }
        return result;
    }

    public static string GetJsonTablePath(string fileName)
    {
        string update_path = Application.persistentDataPath + "/Table/" + fileName + ".json";
        string use_path = DWTools.StreamPath + "/Table/" + fileName + ".json";

        if (File.Exists(update_path))
        {
            use_path = update_path;
        }

        return use_path;
    }

    private static Queue<Transform> m_recursiveQueue = new Queue<Transform>();
    // Mingrui Jiang CGG
    // Set layer recursively, DO NOT USE FOR LARGE HEIARCHY
    public static void SetLayerRecursive(Transform trans, int layer)
    {
        //Debug.Log(trans.name + " " + LayerMask.LayerToName(layer));
        m_recursiveQueue.Enqueue(trans);
        //改递归，消除函数调用消耗
        while (m_recursiveQueue.Count > 0)
        {
            var childTrans = m_recursiveQueue.Dequeue();
            childTrans.gameObject.layer = layer;
            for (int i = 0; i < childTrans.childCount; i++)
            {
                m_recursiveQueue.Enqueue(childTrans.GetChild(i));
            }
        }
    }
    //拼接字符串
    public static string LinkStr(params object[] ob)
    {
        StringBuilder sbuild = new StringBuilder();
        for (int obIndex = 0; ob != null && obIndex < ob.Length; obIndex++)
        {
            sbuild.Append(ob[obIndex].ToString());
        }
        return sbuild.ToString();
    }

   
    public static void SetMainCityLightingMap(Texture2D targetTexture)
    {
        LightmapData[] DataTemp = new LightmapData[LightmapSettings.lightmaps.Length];
        for (int i = 0; i < DataTemp.Length; i++)
        {
            LightmapData temp = new LightmapData();
#if UNITY_5_5
            temp.lightmapLight = targetTexture;
#elif UNITY_5_6
	 		temp.lightmapColor = targetTexture;
#else
            temp.lightmapFar = targetTexture;
#endif
            DataTemp[i] = temp;
        }
        LightmapSettings.lightmaps = DataTemp;
    }

    /// <summary>
    /// 检查是否到达指定位置
    /// </summary>
    public static bool IsGetPosition(Vector3 pos, Vector3 targetPos, Vector3 dir, int needCheckAngelLv = 0)
    {
        if (pos != targetPos && dir == Vector3.zero)
        {
            return false;
        }
        Vector3 origV = targetPos - pos;
        Vector3 curDir = origV.normalized;
        curDir.y = 0;
        float dotV = Vector3.Dot(curDir, dir);
        float angle = Mathf.Acos(dotV) * Mathf.Rad2Deg;
        float distance = origV.sqrMagnitude;
        if (distance < 0.05 || (angle >= GetAngelLimit(needCheckAngelLv)))//距离小于0.05，或者移动方向发生变化时
        {
            return true;
        }
        return false;
    }


    private static int GetAngelLimit(int needCheckAngelLv)
    {
        if (needCheckAngelLv == 0)
        {
            return 5;//同方向的向量精度缺失，1->0.99935居然有两度的误差
        }
        else if (needCheckAngelLv == 1)
        {
            return 90;
        }
        return 0;
    }


    /// <summary>
    /// 检测碰撞
    /// </summary>
    public static bool CheckHasCollider(Transform trans, float radius, int layer, string targetTag,string targetTag_sec = "")
    {
        Collider[] triggerCollider = Physics.OverlapSphere(trans.position, radius, layer);
        bool isHitCollider = false;
        for (int i = 0; i < triggerCollider.Length; i++)
        {
            if (triggerCollider[i] is BoxCollider
                && !triggerCollider[i].name.Contains("_a")
                && (triggerCollider[i].gameObject.CompareTag(targetTag) || (targetTag_sec.Length > 0 && triggerCollider[i].gameObject.CompareTag(targetTag_sec))))
            {
                isHitCollider = true;
                break;
            }
        }
        return isHitCollider;
    }
}
/// <summary>
///  将一些标记写入文件头 文件尾  防止写入一半是中断  导致后续读取文件导致crash
/// </summary>
public static class HeadUtil
{
    private const int m_num = 0x11111111;

    public static void WriteDatas(byte[] datas, Stream sw)
    {
        if (sw == null)
        {
            return;
        }
        sw.Write(HeadUtil.head, 0, HeadUtil.head.Length);
        if (datas != null && datas.Length > 0)
            sw.Write(datas, 0, datas.Length);
        sw.Write(HeadUtil.head, 0, HeadUtil.head.Length);
    }

    private static byte[] m_buff = new byte[1024];

    public static void WriteStream(Stream stream, Stream sw)
    {
        lock (m_buff)
        {
            if (sw == null || stream == null)
            {
                return;
            }
            sw.Write(HeadUtil.head, 0, HeadUtil.head.Length);
            int lenght = 0;
            while ((lenght = stream.Read(m_buff, 0, m_buff.Length)) > 0)
            {
                sw.Write(m_buff, 0, lenght);
            }
            sw.Write(HeadUtil.head, 0, HeadUtil.head.Length);
        }
    }

    public static bool IsContains(byte[] datas, out int offset, out int lenght)
    {
        offset = 0;
        lenght = 0;
        if (datas.Length < head.Length * 2)
        {
            return false;
        }
        for (int i = 0; i < head.Length; i++)
        {
            if (head[i] != datas[i] || head[i] != datas[datas.Length - head.Length + i])
            {
                return false;
            }
        }
        offset = head.Length;
        lenght = datas.Length - 2 * offset;
        return true;
    }

    private static byte[] m_heads = null;

    public static byte[] head
    {
        get
        {
            if (m_heads == null)
            {
                m_heads = BitConverter.GetBytes(m_num);
            }
            return m_heads;
        }
    }
}
public static class FloatUtil
{
    public static bool Equals0(this float f)
    {
        return Mathf.Abs(f) <= 0.00001F;
    }
}

public static class CommonStringBuilder
{
    private static StringBuilder _sb = null;
    private static StringBuilder m_sb
    {
        get {
            if (_sb == null)
                _sb = new StringBuilder();
            return _sb;
        }
    }

    public static void Clear()
    {
        m_sb.Remove(0, m_sb.Length);
    }

    public static string GetString()
    {
        return m_sb.ToString();
    }

    public static void Append(bool value, bool needClear = false)
    {
        if (needClear)
        {
            Clear();
        }
        m_sb.Append(value);
    }
    public static void Append(byte value, bool needClear = false)
    {
        if (needClear)
        {
            Clear();
        }
        m_sb.Append(value);
    }
    public static void Append(char value, bool needClear = false)
    {
        if (needClear)
        {
            Clear();
        }
        m_sb.Append(value);
    }

    public static void AppendFormat(string format, params object[] args)
    {
        m_sb.AppendFormat(format, args);
    }

    public static void AppendFormat(string format, object arg0)
    {
        m_sb.AppendFormat(format, arg0);
    }

    public static void Append(char[] value, bool needClear = false)
    {
        if (needClear)
        {
            Clear();
        }
        m_sb.Append(value);
    }
    public static void Append(decimal value, bool needClear = false)
    {
        if (needClear)
        {
            Clear();
        }
        m_sb.Append(value);
    }
    public static void Append(double value, bool needClear = false)
    {
        if (needClear)
        {
            Clear();
        }
        m_sb.Append(value);
    }
    public static void Append(float value, bool needClear = false)
    {
        if (needClear)
        {
            Clear();
        }
        m_sb.Append(value);
    }
    public static void Append(int value, bool needClear = false)
    {
        if (needClear)
        {
            Clear();
        }
        m_sb.Append(value);
    }
    public static void Append(long value, bool needClear = false)
    {
        if (needClear)
        {
            Clear();
        }
        m_sb.Append(value);
    }
    //public static void Append(object value, bool needClear = false)
    //{
    //        if (needClear)
    //        {
    //            Clear();
    //        }
    //    m_sb.Append(value);
    //}
    public static void Append(sbyte value, bool needClear = false)
    {
        if (needClear)
        {
            Clear();
        }
        m_sb.Append(value);
    }
    public static void Append(short value, bool needClear = false)
    {
        if (needClear)
        {
            Clear();
        }
        m_sb.Append(value);
    }
    public static void Append(string value, bool needClear = false)
    {
        if (needClear)
        {
            Clear();
        }
        m_sb.Append(value);
    }
    public static void Append(uint value, bool needClear = false)
    {
        if (needClear)
        {
            Clear();
        }
        m_sb.Append(value);
    }
    public static void Append(ulong value, bool needClear = false)
    {
        if (needClear)
        {
            Clear();
        }
        m_sb.Append(value);
    }
    public static void Append(ushort value, bool needClear = false)
    {
        if (needClear)
        {
            Clear();
        }
        m_sb.Append(value);
    }

    public static string BuildString(string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8, string s9, string s10)
    {
        Clear();
        m_sb.Append(s1);
        m_sb.Append(s2);
        m_sb.Append(s3);
        m_sb.Append(s4);
        m_sb.Append(s5);
        m_sb.Append(s6);
        m_sb.Append(s7);
        m_sb.Append(s8);
        m_sb.Append(s9);
        m_sb.Append(s10);
        return m_sb.ToString();
    }

    public static string BuildString(string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8, string s9)
    {
        Clear();
        m_sb.Append(s1);
        m_sb.Append(s2);
        m_sb.Append(s3);
        m_sb.Append(s4);
        m_sb.Append(s5);
        m_sb.Append(s6);
        m_sb.Append(s7);
        m_sb.Append(s8);
        m_sb.Append(s9);
        return m_sb.ToString();
    }

    public static string BuildString(string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8)
    {
        Clear();
        m_sb.Append(s1);
        m_sb.Append(s2);
        m_sb.Append(s3);
        m_sb.Append(s4);
        m_sb.Append(s5);
        m_sb.Append(s6);
        m_sb.Append(s7);
        m_sb.Append(s8);
        return m_sb.ToString();
    }

    public static string BuildString(string s1, string s2, string s3, string s4, string s5, string s6, string s7)
    {
        Clear();
        m_sb.Append(s1);
        m_sb.Append(s2);
        m_sb.Append(s3);
        m_sb.Append(s4);
        m_sb.Append(s5);
        m_sb.Append(s6);
        m_sb.Append(s7);
        return m_sb.ToString();
    }

    public static string BuildString(string s1, string s2, string s3, string s4, string s5, string s6)
    {
        Clear();
        m_sb.Append(s1);
        m_sb.Append(s2);
        m_sb.Append(s3);
        m_sb.Append(s4);
        m_sb.Append(s5);
        m_sb.Append(s6);
        return m_sb.ToString();
    }

    public static string BuildString(string s1, string s2, string s3, string s4, string s5)
    {
        Clear();
        m_sb.Append(s1);
        m_sb.Append(s2);
        m_sb.Append(s3);
        m_sb.Append(s4);
        m_sb.Append(s5);
        return m_sb.ToString();
    }

    public static string BuildString(string s1, string s2, string s3, string s4)
    {
        Clear();
        m_sb.Append(s1);
        m_sb.Append(s2);
        m_sb.Append(s3);
        m_sb.Append(s4);
        return m_sb.ToString();
    }

    public static string BuildString(string s1, string s2, string s3)
    {
        Clear();
        m_sb.Append(s1);
        m_sb.Append(s2);
        m_sb.Append(s3);
        return m_sb.ToString();
    }

    public static string BuildString(string s1, string s2)
    {
        Clear();
        m_sb.Append(s1);
        m_sb.Append(s2);
        return m_sb.ToString();
    }
}

public static class EnumParser<T>
{
    private static readonly Dictionary<string, T> _dictionary = new Dictionary<string, T>();

    static EnumParser()
    {
        if (!typeof(T).IsEnum)
            throw new NotSupportedException("Type " + typeof(T).FullName + " is not an enum.");

        string[] names = Enum.GetNames(typeof(T));
        T[] values = (T[])Enum.GetValues(typeof(T));

        int count = names.Length;
        for (int i = 0; i < count; i++)
            _dictionary.Add(names[i], values[i]);
    }

    public static bool TryParse(string name, out T value)
    {
        return _dictionary.TryGetValue(name, out value);
    }

    public static T Parse(string name)
    {
        return _dictionary[name];
    }
}

public static class EnumHelp
{
    public static T GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
    {
        var type = enumVal.GetType();
        var memInfo = type.GetMember(enumVal.ToString());
        if (memInfo == null || memInfo.Length == 0)
        {
            return null;
        }
        var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
        return (attributes.Length > 0) ? (T)attributes[0] : null;
    }
}