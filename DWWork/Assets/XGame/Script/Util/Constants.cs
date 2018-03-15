// /************************************************************
//     File      : Constants.cs
//     brief     : 这里定义的都是一些系统开关和关键id,为了防止误改，所有值全部移到表中，改用读表的方式 
//     author    : JanusLiu   janusliu@ezfun.cn
//     version   : 1.0
//     date      : 2016/2/1 15:17:58
//     copyright : Copyright 2014 EZFun Inc.
// **************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

[LuaWrap]
public class Constants
{
#if UNITY_EDITOR
    public const string NATIVE_IMPORT = "XGame2";
#elif UNITY_IOS
    public const string NATIVE_IMPORT = "__Internal";
#else
    public const string NATIVE_IMPORT = "XGame2";
#endif

    public static string CryptoIV = "dd7fd4a156d28bade96f816db1d18609";
    public static string CryptoKey = "dd7fd4a156d28bade96f816db1d18609";


    //发布的时候，必须设置为TRUE
	public static bool RELEASE = true;

    //改成正式环境目前应用宝和iOS都有用到
    public static bool WITH_RELEASE_EXP = true;
    //是否使用提审服2,默认情况为false,只有当前打包时，外网还有一些渠道正在审核时，才置为true
    public static bool USE_PRE_VERSION_2 = false;
    //程序版本号 DLL 版本号
    public static string APP_VERSION = "1.3.0";
    //资源版本号
    public static string RESS_VERSION = "1.3.0";
    //DataEye ID
    public static string DATAEYE_ID = "442BC4D556B13607EC6217FB1308CC0D";
    // 客服系统key
    public static string SERVICE_KEY = "dfws2_fdsfdkljrle20161014jjlkwqre";
    //bugly appid(抓取崩溃用的)
#if UNITY_IOS
    public static string BUGLY_APPID = "bc6d0adc0e";
#else
    public static string BUGLY_APPID = "d0b5b1ce3c";
#endif
#if UNITY_IOS
    //更新地址
    public static string UPDATE_URL = "http://dl.dw7758.com/jxqp/Update/";
#else
    public static string UPDATE_URL = "http://dl.dw7758.com/jxqp/Update/";
#endif

    //更新vu文件名
    public static string UPDATE_CONFIG_FILE = "update_config.json";
    //是否有iOS预发布状态
    public static bool M_IS_IOS_PRE = true;

    public static float SCREEN_HEIGHT = 640f;
    public static bool FORCE_LOAD_AB = false;
    public static bool RUN_WITH_EN_LUA = false; //是否用加密lua
    public static bool FORCE_DEBUG_PLATFORM = false; //强制使用debug平台
    public static bool EnableIM = true; //是否开放语言聊天


    private static Dictionary<int, string> m_valueDic = new Dictionary<int, string>();
    private static void SetValue(int ID, ref bool ob)
    {
        if (m_valueDic.ContainsKey(ID) && !string.IsNullOrEmpty(m_valueDic[ID]))
        {
            ob = m_valueDic[ID] == "1";
        }
    }

    private static void SetValue(int ID, ref string ob)
    {
        if (m_valueDic.ContainsKey(ID) && !string.IsNullOrEmpty(m_valueDic[ID]))
        {
            ob = m_valueDic[ID];
        }
    }

    private static void SetValue(int ID, ref float ob)
    {
        if (m_valueDic.ContainsKey(ID) && !string.IsNullOrEmpty(m_valueDic[ID]))
        {
            float.TryParse(m_valueDic[ID], out ob);
        }
    }

    private static void SetValue(int ID, ref int ob)
    {
        if (m_valueDic.ContainsKey(ID) && !string.IsNullOrEmpty(m_valueDic[ID]))
        {
            int.TryParse(m_valueDic[ID], out ob);
        }
    }

    public static void InitValue()
    {
		Debug.Log ("Constant InitValue");

        byte[] b = EZFunTools.ReadFile(EZFunTools.GetJsonTablePath("ResClientImportValueList"));
        if (b != null)
        {
            JsonData json = JsonMapper.ToObject(System.Text.Encoding.UTF8.GetString(b))["list"];

            for (int i = 0; i < json.Count; i++)
            {
                JsonData item = json[i];
                if (EZFunTools.JsonDataContainsKey(item, "ID") && EZFunTools.JsonDataContainsKey(item, "value"))
                {
                    if (!m_valueDic.ContainsKey((int)(item["ID"])))
                    {
                        m_valueDic.Add((int)(item["ID"]), item["value"].ToString());
                    }
                }
            }

			//是否RELEASE由打包时决定
//			SetValue(1, ref RELEASE);
            SetValue(2, ref WITH_RELEASE_EXP);
            SetValue(3, ref USE_PRE_VERSION_2);
            SetValue(4, ref APP_VERSION);
            SetValue(5, ref RESS_VERSION);
            SetValue(6, ref DATAEYE_ID);
            SetValue(7, ref UPDATE_URL);
            SetValue(8, ref UPDATE_CONFIG_FILE);
            SetValue(9, ref BUGLY_APPID);
            SetValue(10, ref M_IS_IOS_PRE);
            SetValue(11, ref SCREEN_HEIGHT);
            if (!RUN_WITH_EN_LUA)
            {
                SetValue(12, ref RUN_WITH_EN_LUA);
            }
            SetValue(13, ref FORCE_DEBUG_PLATFORM);
        }
    }
    public static bool TEMP_CLOSE = true; // CGG临时关闭的功能开关
}

class EZFunSysInfo
{
    public static int SystemMemSize = 1024;

    public static void Init()
    {
        SystemMemSize = SystemInfo.systemMemorySize;
    }
}

