//========================================================================
// Copyright(C): EZFun
//
// CLR Version : 4.0.30319.34209
// NameSpace : Assets.XGame.Script.System
// FileName : Version
//
// Created by : dhf at 1/12/2015 2:44:55 PM
//
// Function : APP与res版本与活动版本
//
//========================================================================

using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;

//public enum VersionType : int
//{
//    /// <summary>
//    /// 程序版本号，在Android中是跟随xgame.ezfun走  而在iOS中是固定
//    /// </summary>
//    App = 0,
//    /// <summary>
//    /// 
//    /// </summary>
//    Resource,
//    Activity,
//    /// <summary>
//    /// 当前外网运行的最大版本号  update_config中serverInfo列表中的version
//    /// </summary>
//	PublishedAppVersion,
//    /// <summary>
//    /// 包的版本号 即发布时候的GameStart中APP_VERSION 包打出之后固定
//    /// </summary>
//    PackageVersion,
//    Max,
//}

//public enum ENUM_SDK_PLATFORM
//{
//    ESP_General = 0,
//    ESP_Ios = 100,
//    ESP_Android = 200,
//    ESP_Debug = 10001
//}

//public enum EnumAppName
//{
//    general = 0,
//    andriod = 1,
//    debug = 2,
//    ios = 100
//}

public class VersionData
{
    public List<string> versions;
    public string announce;
    public int platform;
    public EnumAppName m_appName;
    public Dictionary<string, int> act_pop_versions;
    public Dictionary<string, string> m_packageInfo;
    public int m_questionnaireBeginTime;
    public bool m_questionnaireClicked;
    public VersionData()
    {
        versions = new List<string>();
        announce = "";
        platform = 0;
        m_appName = EnumAppName.andriod;
        act_pop_versions = new Dictionary<string, int>();
        m_packageInfo = new Dictionary<string, string>();
        m_questionnaireBeginTime = 0;
        m_questionnaireClicked = false;
    }
}

[LuaWrap]
public class Version
{
    private static Version m_Instance;
    private static VersionData m_versionData;
    private static int m_netActivityVersion = 0;
    private string m_verFilePath = null;

    private static string m_appLocalVersion = Constants.APP_VERSION;
    private static string m_ressLocalVersion = Constants.RESS_VERSION;
    private static string m_activityLocalVersion = "0";
    private static string m_publishAppVersion = "";
    private static string m_packageVersion = "";
    private static Dictionary<string, int> m_actPopVersions = new Dictionary<string, int>();
    private static Dictionary<string, string> m_packageInfo = new Dictionary<string, string>();
    private static int m_questionnaireBeginTime = 0;
    private static bool m_questionnaireClicked = false;

    public bool m_isPrePublish
    {
        get;
        private set;
    }

    public static Version Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new Version();
                m_versionData = new VersionData();

                m_versionData.versions.Add(m_appLocalVersion);
                m_versionData.versions.Add(m_ressLocalVersion);
                m_versionData.versions.Add(m_activityLocalVersion);
                m_versionData.versions.Add(m_publishAppVersion);
                m_versionData.versions.Add(m_packageVersion);

                m_versionData.platform = (int)ENUM_SDK_PLATFORM.ESP_Debug;

                m_versionData.act_pop_versions = m_actPopVersions;
                m_versionData.m_packageInfo = m_packageInfo;
                m_versionData.m_questionnaireBeginTime = m_questionnaireBeginTime;
                m_versionData.m_questionnaireClicked = m_questionnaireClicked;
            }

            return m_Instance;
        }
    }

    public void Init()
    {
        m_verFilePath = Application.persistentDataPath + Path.DirectorySeparatorChar
            + "ver";
        ReadVersionInfoFromFile();
    }
    /// <summary>
    /// 加了一个，主要是因为这个大陆版本，提审只屏蔽了很少功能，而海外版本屏蔽了很多功能，所以海外需要屏蔽的都用的是LocalSys.Instance.CheckIsPrePublish()
    /// </summary>
    /// <returns></returns>
	public bool CheckIsPrePublish()
    {
        if (!Constants.RELEASE)
        {
            return false;
        }

        //if (!Constants.M_IS_IOS_PRE)
        //{
        //    return false;
        //}

        return m_isPrePublish;
    }

    public int GetRessVersion()
    {
        return GetVersion(m_ressLocalVersion);
    }

    public static int GetVersion(string str)
    {
        return Version.GetVersionCode(str);
    }

    public static int GetVersionCode(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return 0;
        }
        string[] strArray = str.Split('.');
        int version = int.Parse(strArray[0]) * 10000 + int.Parse(strArray[1]) * 100 + int.Parse(strArray[2]);
        return version;
    }

    public static string VersionIntToStr(int version)
    {
        int ppint = version / 10000;
        int pint = (version % 10000) / 100;
        int cint = (version % 100);
        return string.Format("{0}.{1}.{2}", ppint, pint, cint);
    }

    #region Activity Pop Version
    public void StoreActivityPopVersion()
    {
        WriteVersionInfoToFile();
    }

    public void SetActivityPopVersion(int id, int version)
    {
        if (m_versionData.act_pop_versions.ContainsKey(id.ToString()))
        {
            m_versionData.act_pop_versions[id.ToString()] = version;
        }
        else
        {
            m_versionData.act_pop_versions.Add(id.ToString(), version);
        }
    }

    //return true:need download; false:don't need download
    public bool CheckActivityPopVersion(int id, int version)
    {
        if (m_versionData.act_pop_versions.ContainsKey(id.ToString()))
        {
            if (m_versionData.act_pop_versions[id.ToString()] < version)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }
    #endregion Activity Pop Version

    public bool CheckActivityVersion()
    {
        return m_netActivityVersion > int.Parse(m_versionData.versions[(int)VersionType.Activity]);
    }

    public void StoreActivityVersion()
    {
        SetVersion(VersionType.Activity, m_netActivityVersion.ToString());
        m_netActivityVersion = 0;
    }


    public void SetNetActivityVersion(int ver)
    {
        m_netActivityVersion = ver;
    }

    public int GetNetActivityVersion()
    {
        return m_netActivityVersion;
    }

    public string GetVersion(VersionType vt)
    {
        if ((int)vt < m_versionData.versions.Count)
        {
            return m_versionData.versions[(int)vt];
        }
        else
        {
            return "";
        }
    }

    public string GetVersion(int vt)
    {
        if (vt < m_versionData.versions.Count)
        {
            return m_versionData.versions[vt];
        }
        else
        {
            return "";
        }
    }

    public void SetVersion(VersionType vt, string version)
    {
        if ((int)vt < m_versionData.versions.Count)
        {
            m_versionData.versions[(int)vt] = version;
        }
        else
        {
            m_versionData.versions.Add(version);
        }
        WriteVersionInfoToFile();
    }

    // 设置版本信息
    private void WriteVersionInfoToFile()
    {
        try
        {
            FileStream sw = new FileStream(m_verFilePath, FileMode.Create);
            var js = JsonMapper.ToJson(m_versionData);
            HeadUtil.WriteDatas(System.Text.Encoding.Default.GetBytes(js), sw);
            sw.Close();
        }
        catch (System.Exception)
        {
            //Debug.LogError("version file error:" + ex.Message);
        }

        if (!string.IsNullOrEmpty(m_verFilePath))
        {
            SaveAppCfg();
        }
    }


    // 从文件读取版本信息
    private void ReadVersionInfoFromFile()
    {
        if (File.Exists(m_verFilePath))
        {
            var b = EZFunTools.ReadFileStream(m_verFilePath);

            if (b == null || b.Length == 0)
            {
                WriteVersionInfoToFile();
                return;
            }
            int offset, length;
            if (!HeadUtil.IsContains(b, out offset, out length))
            {
                WriteVersionInfoToFile();
                return;
            }
            var str = System.Text.Encoding.Default.GetString(b, offset, length);
            bool needWriteVersionInfoFile = false;
            var versionData = JsonMapper.ToObject<VersionData>(str);

            for (int index = 0; index < versionData.versions.Count; index++)
            {
                int result = CompareVersion(versionData.versions[index], m_versionData.versions[index]);
                if (result == -1)
                {
                    needWriteVersionInfoFile = true;
                }
                else if (result == 1)
                {
                    m_versionData.versions[index] = versionData.versions[index];
                }
            }

            if (versionData.act_pop_versions == null)
            {
                needWriteVersionInfoFile = true;
            }
            else
            {
                string key;
                int value = 0;
                foreach (var kv in versionData.act_pop_versions)
                {
                    key = (string)kv.Key;
                    value = (int)kv.Value;

                    if (m_versionData.act_pop_versions.ContainsKey(key))
                    {
                        if (m_versionData.act_pop_versions[key] < value)
                        {
                            m_versionData.act_pop_versions[key] = value;
                        }
                        else
                        {
                            needWriteVersionInfoFile = true;
                        }
                    }
                    else
                    {
                        m_versionData.act_pop_versions.Add(key, value);
                    }
                }
            }

            m_versionData.m_questionnaireBeginTime = versionData.m_questionnaireBeginTime;
            m_versionData.m_questionnaireClicked = versionData.m_questionnaireClicked;
            if (needWriteVersionInfoFile)
            {
                WriteVersionInfoToFile();
            }

            SetNoticeURL();
        }
        else
        {
            WriteVersionInfoToFile();
        }
    }

    public void SetNoticeURL()
    {
        //Constants.NOTICE_URL = Constants.RELEASE_NOTICE_URL + "cgg/test1/";
        if (GetVersion(GetVersion(VersionType.PublishedAppVersion)) >= GetVersion(m_packageVersion))
        {
            //Constants.NOTICE_URL = Constants.RELEASE_NOTICE_URL + "cgg/";
            m_isPrePublish = false;
        }
        else
        {
            //Constants.NOTICE_URL = Constants.RELEASE_NOTICE_URL + "cgg/tsf/";
            m_isPrePublish = true;
        }
    }

    private void SaveAppCfg()
    {
        string path = Application.persistentDataPath + "/localAppVer.cfg";
        try
        {
            StreamWriter sw = new StreamWriter(path);
            sw.Write(m_versionData.versions[(int)VersionType.Resource]);
            sw.Close();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("version file error:" + ex.Message);
        }
    }

    private void DeleteLocalRessCache()
    {
        for (int i = 0; i < (int)UpdateType.MAX; i++)
        {
            DirectoryInfo direct = new DirectoryInfo(EZFunTools.CachePath + "/" + ((UpdateType)i).ToString());
            if (direct.Exists)
            {
                try
                {
                    direct.Delete(true);
                }
                catch
                {
                }
            }
        }

        //added by janus 修复覆盖安装没有删除ress配置文件，导致新版本的相同版本号的更新没有更新到的bug
        if (File.Exists(EZFunTools.AvailablePath + "/ress.cf"))
        {
            File.Delete(EZFunTools.AvailablePath + "/ress.cf");
        }
    }

    /// <summary>
    /// gameStart调用
    /// </summary>
    /// <param name="platformId"></param>
    public void SetPlatformID(int platformId)
    {
        m_versionData.platform = platformId;
        SetAppName();
        WriteVersionInfoToFile();
    }

    public string getMSDKPlatformName()
    {
        ENUM_SDK_PLATFORM platformSDK = GetCurrentSdkPlatform();
        string strName = "";
        return strName;
    }

    private void SetAppName()
    {
        switch ((ENUM_SDK_PLATFORM)m_versionData.platform)
        {
            case ENUM_SDK_PLATFORM.ESP_Debug:
                m_versionData.m_appName = EnumAppName.debug;
                break;
            case ENUM_SDK_PLATFORM.ESP_Android:
                m_versionData.m_appName = EnumAppName.andriod;
                break;
            case ENUM_SDK_PLATFORM.ESP_Ios:
                m_versionData.m_appName = EnumAppName.ios;
                break;
            default:
                m_versionData.m_appName = EnumAppName.andriod;
                break;
        }
    }

    public ENUM_SDK_PLATFORM GetCurrentSdkPlatform()
    {
        return (ENUM_SDK_PLATFORM)m_versionData.platform;
    }

    public ENUM_SDK_PLATFORM GetPublishSDKPlatform()
    {
        return (ENUM_SDK_PLATFORM)m_versionData.platform;
    }

    public string GetAppName()
    {
        return (m_versionData.m_appName).ToString();

        if (GetVersion(GetVersion(VersionType.PublishedAppVersion)) >= GetVersion(m_packageVersion))
        {
            return (m_versionData.m_appName).ToString();
        }
        else
        {
            if (m_versionData.m_appName == EnumAppName.andriod)
            {
                return (m_versionData.m_appName).ToString();
            }
            else
            {
                if (Constants.USE_PRE_VERSION_2 && Application.platform == RuntimePlatform.Android)
                {
                    return "tsf_xgame";
                }
                else
                {
                    return "tsf_" + (m_versionData.m_appName).ToString();
                }
            }
        }
    }

    public EnumAppName GetAppNameEnum()
    {
        return m_versionData.m_appName;
    }

    public void SetAnnounce(string announce)
    {
        m_versionData.announce = announce;
    }

    public string GetAnnounce()
    {
        return m_versionData.announce;
    }

    public static int CompareVersion(string localVer, string cmpVer)
    {
        var lvs = localVer.Split('.');
        var cvs = cmpVer.Split('.');

        for (int index = 0; index < lvs.Length; ++index)
        {
            if (index >= lvs.Length)
            {
                return -1;
            }
            if (index >= cvs.Length)
            {
                return 1;
            }
            if (lvs[index].Equals("") || lvs[index].Equals(" "))
            {
                return -1;
            }
            if (cvs[index].Equals("") || cvs[index].Equals(" "))
            {
                return 1;
            }
            if (int.Parse(lvs[index]) < int.Parse(cvs[index]))
            {
                return -1;
            }
            else if (int.Parse(lvs[index]) > int.Parse(cvs[index]))
            {
                return 1;
            }
        }
        return 0;
    }

    public void SetPackageVersion(string version)
    {
        m_packageVersion = version;

        if (m_versionData.versions.Count >= 5)
        {
            string localPackageVersion = m_versionData.versions[(int)VersionType.PackageVersion];
            //当当前包的版本号比记录本地的版本号大时，删掉本地的资源文件夹
            if (GetVersion(localPackageVersion) < GetVersion(m_packageVersion))
            {
                DeleteLocalRessCache();
            }
        }
        else
        {
            DeleteLocalRessCache();
        }
        SetVersion(VersionType.PackageVersion, m_packageVersion);
    }

    public string GetPackagetVersion()
    {
        return m_packageVersion;
    }

    public int GetPackageVersionInt()
    {
        return GetVersion(m_packageVersion);
    }

    public void SetPackageInfo(string desc, string url)
    {
        m_versionData.m_packageInfo["desc"] = desc;
        m_versionData.m_packageInfo["url"] = url;
        WriteVersionInfoToFile();
    }

    public string GetPackageURL()
    {
        if (m_versionData.m_packageInfo.ContainsKey("url"))
        {
            return m_versionData.m_packageInfo["url"];
        }
        else
        {
            return "";
        }
    }

    public int GetQuestionnaireBegineTime()
    {
        return m_versionData.m_questionnaireBeginTime;
    }

    public void SetQuestionnaireBegineTime()
    {
        m_versionData.m_questionnaireBeginTime = TimerSys.Instance.GetCurrentDateTime();
        WriteVersionInfoToFile();
    }

    public bool GetQuestionnaireClicked()
    {
        return m_versionData.m_questionnaireClicked;
    }

    public void SetQuestionnaireClicked(bool clicked)
    {
        m_versionData.m_questionnaireClicked = clicked;
        WriteVersionInfoToFile();
    }

}
