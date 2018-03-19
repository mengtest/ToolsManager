using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ezfun_resource;
using LuaInterface;
using System.Security.Cryptography;
using System.Reflection;

//PS：自己在对应的类区域内新添类的对应方法，暂不处理的情况：函数返回类型是用户自定义的类结构
public delegate void BaseLuaCallBack();
[LuaWrap]
public static class WrapSys
{
    #region unity parm
    public static bool IsAndroid
    {
        get
        {
            return Application.platform == RuntimePlatform.Android;
        }
    }

    public static bool IsIOS
    {
        get
        {
            return Application.platform == RuntimePlatform.IPhonePlayer;
        }
    }

    public static bool IsEditor
    {
        get
        {
            return Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor;
        }
    }

    public static void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    #endregion

    #region MD5
    public static string ComputeHash(string str)
    {
        return MD5Util.ComputeHash(str);
    }
    #endregion

    #region mem sys

    public static string LuaPersistPath
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return Application.persistentDataPath;
            }
            else
            {
                return Application.dataPath;
            }
        }
    }

    #endregion

    #region CNetSys System
    public static object CNetSys_GetInstance()
    {
        return CNetSys.Instance;
    }

    public static string CNetSys_GetServerName()
    {
        return "";
    }

    public static void CNetSys_AddNetConnectInstance(string netName)
    {
        CNetSys.Instance.AddNetConnectInstance(netName);
    }

    public static void CNetSys_RemoveNetConnectInstance(string netName)
    {
        CNetSys.Instance.RemoveNetConnectInstance(netName);
    }

    public static void CNetSys_ConnectSvr(string netName,string ip,int port)
    {
        LuaRootSys.Instance.ConnectGameSvr(netName, ip, port);
    }

    public static void CNetSys_SendLuaNet(string netName,int areaID,int cmd, int seq, bool iscb, bool needLockScreen,LuaStringBuffer datas)
    {
        LuaRootSys.Instance.SendNetMsgByLua(netName,areaID, cmd, seq, iscb, needLockScreen, datas);
    }
    public static void CNetSys_SendLuaNet(string netName,int areaID,int cmd, int seq, bool iscb, bool needLockScreen)
    {
        LuaRootSys.Instance.SendNetMsgByLua(netName,areaID, cmd, seq, iscb, needLockScreen, null);
    }

    public static void CNetSys_SendHttpLuaNet(string msgKey,string url,string param,bool isPost,int seq,int timeOut)
    {
        WebSocketSys.Instance.SendMsg(msgKey, url, param,isPost,seq,timeOut);
    }

    public static void CNetSys_SetAccessToken(string accessToken)
    {
        CNetSys.Instance.m_Token.SetAccessToken(accessToken);
    }

    public static void CNetSys_GiveUpAutoConnect(string netName)
    {
        CNetSys.Instance.GiveUpAutoConnect(netName);
    }

    public static void CNetSys_ResetAutoConnectt(string netName)
    {
        CNetSys.Instance.ResetAutoConnect(netName);
    }

    public static void CNetSys_RestartHttpWork()
    {
        WebSocketSys.Instance.ReStartWork();
    }

    public static bool CNetSys_CheckNetWorkIsReachable()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    public static void CNetSys_CloseHttpWork()
    {
        WebSocketSys.Instance.Release();
    }

    //...
    // To Add New Method Before
    #endregion

    #region EventSys System
    public static void EventSys_AddEvent(EEventType eventId, object param1 = null, object param2 = null)
    {
        EventSys.Instance.AddEvent(eventId, param1, param2);
    }
    public static void EventSys_AddEventNow(EEventType eventId, object param1 = null, object param2 = null)
    {
        EventSys.Instance.AddEventNow(eventId, param1, param2);
    }

    public static void EventSys_AddEventNow(int eventId, object param1 = null, object param2 = null)
    {
        EventSys.Instance.AddEventNow((EEventType)eventId, param1, param2);
    }

    public static void EventSys_AddEvent(int eventId, object param1 = null, object param2 = null)
    {
        EventSys.Instance.AddEvent((EEventType)eventId, param1, param2);
    }

    public static object EventSys_GetInstance()
    {
        return EventSys.Instance;
    }
    #endregion

    #region LuaRoot System

    public static void LuaRootSys_AddLuaEvent(int eventid)
    {
        LuaRootSys.Instance.AddEventHandler(eventid);
    }

    public static void LuaRootSys_RemoveLuaEvent(int eventid)
    {
        LuaRootSys.Instance.RemoveEventHandle(eventid);
    }


    public static int LuaRootSys_LoadPB(string pbName)
    {
        return LuaRootSys.Instance.RegisterPBFlie(pbName);
    }

    #endregion

    #region  EZFunWindowMgr System

    public static void EZFunWindowMgr_SetWindowStatusForLua(EZFunWindowEnum windowEnum, RessType ressType, bool open, int state, string luaWindowName, object param)
    {
        EZFunWindowMgr.Instance.SetWindowStatusForLua(windowEnum, ressType, open, state, luaWindowName, param);
    }

    public static void EZFunWindowMgr_SetWindowStatusForLua(EZFunWindowEnum windowEnum, RessType ressType, bool open = true, int state = 0, string luaWindowName = "")
    {
        EZFunWindowMgr.Instance.SetWindowStatusForLua(windowEnum, ressType, open, state, luaWindowName, null);
    }

    public static void EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum windowEnum, bool open = true, int state = 0, string windowName = "")
    {
        EZFunWindowMgr.Instance.SetWindowStatus(windowEnum, open, state, windowName);
    }

    public static void EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum windowEnum, bool open, int state, string windowName, bool animated, object others = null)
    {
        EZFunWindowMgr.Instance.SetWindowStatus(windowEnum, open, state, windowName, animated, others);
    }
    public static void EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum windowEnum, RessType ressType, bool open = true, int state = 0, string luaWindowName = "")
    {
        EZFunWindowMgr.Instance.SetWindowStatus(windowEnum, ressType, open, state, luaWindowName);
    }
    public static int EZFunWindowMgr_GetScreenWidth()
    {
        return EZFunWindowMgr.Instance.GetScreenWidth();
    }
    public static int EZFunWindowMgr_GetScreenHeight()
    {
        return EZFunWindowMgr.Instance.GetScreenHeight();
    }
    public static int EZFunWindowMgr_getUnusedLayer(int type, Transform winTrans)
    {
        return EZFunWindowMgr.Instance.getUnusedLayer(type, winTrans);
    }
    public static bool EZFunWindowMgr_CheckWindowOpen(EZFunWindowEnum enumWindow, string luaWindowName = "")
    {
        return EZFunWindowMgr.Instance.CheckWindowOpen(enumWindow, luaWindowName);
    }
    public static bool EZFunWindowMgr_InitWindowDic(EZFunWindowEnum windowEnum, RessType ressType, RessStorgeType ressStorgeType, string luaWindowName)
    {
        return EZFunWindowMgr.Instance.InitWindowDic(windowEnum, ressType, ressStorgeType, luaWindowName);
    }
    public static void EZFunWindowMgr_RegisterWindowInfo(string windowName, string resPath, string resLuaName, bool isNeedCreate, bool isClosePrimary)
    {
        EZFunWindowMgr.Instance.RegisterWindowInfo(windowName, null, resPath, resLuaName, isNeedCreate);
    }
    //...
    // To Add New Method Before
    #endregion

    #region  ResourceMgr System
    //public OBContainer ResourceMgr_InitAssetWith2Param(RessType ressType, string ressName)
    //{
    //	return ResourceMgr.InitAssetWith2Param(ressType,ressName);
    //}
    public static UnityEngine.GameObject ResourceMgr_GetInstantiateAsset(RessType ressType, string ressName, RessStorgeType ressStorgeType = RessStorgeType.RST_Never, bool forceInit = false)
    {
        return (GameObject)ResourceMgr.GetInstantiateAsset(ressType, ressName, ressStorgeType);
    }

    public static UnityEngine.GameObject ResourceMgr_GetInsAssetInParent(RessType ressType, string ressName, Transform parent, RessStorgeType ressStorgeType = RessStorgeType.RST_Never)
    {
        GameObject go = (GameObject)ResourceMgr.GetInstantiateAsset(ressType, ressName, ressStorgeType);
        var scale = go.transform.localScale;
        go.transform.parent = parent;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = scale;
        NGUITools_SetLayer(go, parent.gameObject.layer);
        return go;
    }
    public static int ResourceMgr_CacheTextureNum
    {
        get { return ResourceMgr.CacheTextureNum; }
        set { ResourceMgr.CacheTextureNum = value; }
    }

    //...
    // To Add New Method Before
    #endregion

    #region  GameRoot Common
    public static object GameRoot_GetInstance()
    {
        return GameRoot.Instance;
    }

    public static float GameRoot_Screen_width()
    {
        return Screen.width;
        //return GameRoot.screen_width;
    }

    public static float GameRoot_Screen_height()
    {
        return Screen.height;
        //return GameRoot.screen_height;
    }

    public static float GameRoot_GameTime
    {
        get { return GameRoot.m_gameTime; }
    }
    public static EGameStateType GameRoot_GetCurStateType()
    {
        return GameStateMgr.Instance.GetCurStateType();
    }
    //...
    // To Add New Method Before
    #endregion

    #region  TextData Common
    public static string TextData_GetText(int id)
    {
        return TextData.GetText(id);
    }
    public static string TextData_GetText(int id, params string[] stringArray)
    {
        return TextData.GetText(id, stringArray);
    }
    public static string TextData_GetText(string text, params string[] stringArray)
    {
        return TextData.GetText(text, stringArray);
    }
    //...
    // To Add New Method Before
    #endregion

    #region  Constants Common
    public static bool Constants_RELEASE
    {
        get { return Constants.RELEASE; }
    }
    //...
    // To Add New Method Before
    #endregion

    #region  Util Common
    public static void Util_Log(string str)
    {
        Debug.Log(str);
    }
    public static void Util_LogError(string str)
    {
        Debug.LogError(str);
    }
    public static void Util_LogWarning(string str)
    {
        Debug.LogWarning(str);
    }
    public static void Util_SetParent(Transform trans, Transform childTrans)
    {
        EZFunUITools.SetParent(trans, childTrans);
    }
    //...
    // To Add New Method Before

    #endregion

    #region  Version Common
    public static bool Version_CheckIsPrePublish()
    {
        return Version.Instance.CheckIsPrePublish();
    }
    public static int Version_GetQuestionnaireBegineTime()
    {
        return Version.Instance.GetQuestionnaireBegineTime();
    }
    public static void Version_SetQuestionnaireBegineTime()
    {
        Version.Instance.SetQuestionnaireBegineTime();
    }
    public static bool Version_GetQuestionnaireClicked()
    {
        return Version.Instance.GetQuestionnaireClicked();
    }
    public static void Version_SetQuestionnaireClicked(bool clicked)
    {
        Version.Instance.SetQuestionnaireClicked(clicked);
    }
    #endregion

    #region  WindowBaseLua Common
    public static void WindowBaseLua_LoadLua(string fileName)
    {
        ///WindowBaseLua.LoadLua(fileName);
    }
    //...
    // To Add New Method Before
    #endregion

    #region  NGUITools Tool
    public static void NGUITools_SetLayer(GameObject gb, int layer)
    {
        WindowRoot.SetLayer(gb, layer);
    }
    public static bool NGUITools_GetActive(Transform trans)
    {
        return NGUITools.GetActive(trans.gameObject);
    }

    public static void NGUITools_ActiveOutLine(Transform trans, bool isOutline)
    {
        if (trans == null)
        {
            return;
        }
        var label = trans.GetComponent<UILabel>();
        if (label == null)
        {
            return;
        }
        label.effectStyle = isOutline ? UILabel.Effect.Outline : UILabel.Effect.None;
    }

    //...
    // To Add New Method Before
    #endregion

    #region EZFunTools Tool
    public static string EZFunTools_ReadLua(string fileName)
    {
        return EZFunTools.ReadLua(fileName);
    }
    public static string EZFunTools_GetSecToStr(int sec)
    {
        return EZFunTools.GetSecToStr(sec);
    }

    public static string EZFunTools_GetSecToStrWithoutHour(int sec)
    {
        return EZFunTools.GetSecToStr(sec, false);
    }
    public static double EZFunTools_LuaDecryptDES(double value)
    {
        return EZFunTools.LuaDecryptDES(value);
    }
    public static double EZFunTools_LuaEncryptDES(double value)
    {
        return EZFunTools.LuaEncryptDES(value);
    }
    public static int EZFunTools_GetCharacterCount(string str)
    {
        return EZFunTools.GetCharacterCount(str);
    }
    public static void EZFunTools_SetCameraDepth(Camera cam, float depth)
    {
        EZFunTools.SetCameraDepth(cam, depth);
    }
    public static string EZFunTools_GetLeftTimeStr(int time)
    {
        return EZFunTools.GetLeftTimeStr(time);
    }

    public static float GetRealTime()
    {
        return GameRoot.m_realTime;
    }

    public static Component EZFunTools_GetOrAddComponent(GameObject gb, string typeStr)
    {
        return EZFunTools.GetOrAddComponent(gb, typeStr);
    }


    //...
    // To Add New Method Before
    #endregion

    #region  PortingForLua Tool
    public static object PortingForLua_GetClassType()
    {
        return typeof(PortingForLua);
    }
    public static Camera PortingForLua_GetOrAddCamera(Transform trans)
    {
        return PortingForLua.GetOrAddCamera(trans);
    }
    public static UICamera PortingForLua_GetOrAddUICamera(Transform trans)
    {
        return PortingForLua.GetOrAddUICamera(trans);
    }
    public static int PortingForLua_BinaryMoveLeft(int value, int count)
    {
        return PortingForLua.BinaryMoveLeft(value, count);
    }
    public static int PortingForLua_BinaryMoveRight(int value, int count)
    {
        return PortingForLua.BinaryMoveRight(value, count);
    }
    public static void PortingForLua_PlayAnimInAnimator(Transform trans, string name)
    {
        PortingForLua.PlayAnimInAnimator(trans, name);
    }
    public static void PortingForLua_PlayAnimEndPlayIdle(Transform trans)
    {
        PortingForLua.PlayAnimEndPlayIdle(trans);
    }
    public static void PortingForLua_PlayTweensInChildren(Transform trans)
    {
        PortingForLua.PlayTweensInChildren(trans);
    }
    public static void PortingForLua_PlayAnimationByName(Transform trans, string ani)
    {
        PortingForLua.PlayAnimationByName(trans, ani);
    }
    //...
    // To Add New Method Before
    #endregion

    #region Unity Screen  Tool
    public static int UnityScreen_width
    {
        get { return UnityEngine.Screen.width; }
    }
    public static int UnityScreen_height
    {
        get { return UnityEngine.Screen.height; }
    }
	public static Vector3 ScreenToWorldPoint(Camera cam, Vector3 src)
	{
		if(cam != null){
			return cam.ScreenToWorldPoint(src);
		}
		return Vector3.zero;
	}
	public static Vector3 WorldToScreenPoint(Camera cam, Vector3 src)
	{
		if(cam != null){
			return cam.WorldToScreenPoint(src);
		}
		return Vector3.zero;
	}

    //获取当前鼠标屏幕坐标的 世界坐标
    public static Vector3 GetCurrentTouchPos(int layer) 
    {
        var m_currentPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Camera camera = EZFunWindowMgr.Instance.GetCameraByLayer(layer);
        return ScreenToWorldPoint(camera, m_currentPos);
    }
    #endregion

    #region AudioSys
    public static void AudioSys_LoadBanks(string effectName)
    {
        AudioSys.Instance.LoadBank(effectName);
    }

    public static void AudioSys_UnLoadBank(string effectName)
    {
        AudioSys.Instance.UnLoadBank(effectName);
    }

    public static FmodAudioSys GetFmodAudioSys() 
    {
        return AudioSys.Instance.GetFmodAudioSys();
    }

    public static FMOD.Studio.EventInstance AudioSys_PlayEffect(string effectName)
    {
        return AudioSys.Instance.PlayEffect(effectName);
    }
    public static void AudioSys_PauseMusic()
    {
        AudioSys.Instance.PauseMusic();
    }
    public static void AudioSys_ResumeMusic()
    {
        AudioSys.Instance.ResumeMusic();
    }
	public static void AudioSys_SetMusicVolumn(float vol)
	{
		AudioSys.Instance.SetMusicVolumn (vol);
	}

	public static void AudioSys_SetEffectVolumn(float vol)
	{
		AudioSys.Instance.SetEffectVolumn (vol);
	}

	public static float AudioSys_GetEffectVolumn()
	{
		return AudioSys.Instance.GetEffectVolumn ();
	}

	public static float AudioSys_GetMusicVolumn()
	{
		return AudioSys.Instance.GetMusicVolumn ();
	}

    public static float AudioSys_GetSaveEffectVolumn()
    {
        return PlayerPrefs.GetFloat("EffectVolume", 1f);
    }

    public static float AudioSys_GetSaveMusicVolumn()
    {
        return PlayerPrefs.GetFloat("MusicVolume", 1f);
    }

    public static void PauseVoice(bool pause) 
    {
        if (pause)
        {
            AudioSys.Instance.SetMusicVolumn(0);
            AudioSys.Instance.SetEffectVolumn(0);
        }
        else 
        {
            AudioSys.Instance.SetMusicVolumn(PlayerPrefs.GetFloat("MusicVolume", 1f));
            AudioSys.Instance.SetEffectVolumn(PlayerPrefs.GetFloat("EffectVolume", 1f));
        }
    }


    public static void AudioSys_PlayMusic(string musicName)
    {
        if(musicName != null)
        {
            AudioSys.Instance.PlayMusic(musicName);
        }
    }

	//use these two for now,the four functions above are designed for expand or bugfix
	public static void InitMusicVolumnSlider(Transform trans)
	{
		UISlider slider = trans.GetComponentInChildren<UISlider> ();

		if (slider == null) {
			return;
		}

		slider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
		slider.onChange.Clear ();
		slider.onChange.Add( new EventDelegate(() => {
			AudioSys.Instance.SetMusicVolumn (slider.value);
			PlayerPrefs.SetFloat("MusicVolume", slider.value);
		}));
	}
	public static void InitEffectVolumnSlider(Transform trans)
	{
		UISlider slider = trans.GetComponentInChildren<UISlider> ();

		if (slider == null) {
			return;
		}

		slider.value = PlayerPrefs.GetFloat("EffectVolume", 1f);
		slider.onChange.Clear ();
		slider.onChange.Add( new EventDelegate(() => {
			AudioSys.Instance.SetEffectVolumn (slider.value);
			PlayerPrefs.SetFloat("EffectVolume", slider.value);
		}));
	}

    public static void BeginDetecation()
    {
        RBDViewController.BeginDetecation();
    }

    public static bool isMuted()
    {
       return RBDViewController.isMuted();
    }
    #endregion

	#region netsys
	public static void NetSys_SetForceOut(bool flag)
	{
		CNetSys.Instance.GameNetStateConnect.m_isForceOut = flag;
	}
	#endregion

    #region NGUI 

    public static string GetUILabelText(Transform trans)
    {
        if(trans == null)
        {
            return "";
        }
        var label = trans.GetComponent<UILabel>();
        if(label != null)
        {
            return label.text;
        }
        return "";
    }

	public static ComponentBaseLua GetOrAddComponentBaseLua(GameObject gb)
	{
		return EZFunTools.GetOrAddComponent<ComponentBaseLua>(gb);
	}

    #endregion

    #region Timer
    public static void AddTimerEVentByLeftTime(float time, LuaInterface.LuaFunction luaFunc)
    {
        TimerSys.Instance.AddTimerEventByLeftTime(() =>
        {
            luaFunc.Call();
            luaFunc = null;
        }, time);
    }

    public static int GetCurrentDateTime()
    {
        return TimerSys.Instance.GetCurrentDateTime();
    }

    public static string TimerSys_GetCurrentTimeStrHM()
    {
        return TimerSys.Instance.GetCurrentTimeStrHM();
    }

    #endregion

    #region  HandleErrorWindow Window
    public static String HandleErrorWindow_TitleStr
    {
        set { HandleErrorWindow.m_titleStr = value; }
    }
    public static string HandleErrorWindow_ContentSt
    {
        set { HandleErrorWindow.m_contentStr = value; }
    }
    public static object HandleErrorWindow_GetClassType()
    {
        return typeof(HandleErrorWindow);
    }
    public static void HandleErrorWindow_SetOkCallBack(LuaFunction func)
    {
        HandleErrorWindow.m_okCallBack = (p1, p2) =>
        {
            if (func != null)
            {
                func.Call2Args(p1, p2);
            }
        };
    }
    public static void HandleErrorWindow_SetNoCallBack(LuaFunction func)
    {
        HandleErrorWindow.m_noCallBack = (p1, p2) =>
        {
            if (func != null)
            {
                func.Call2Args(p1, p2);
            }
        };
    }
    public static void HandleErrorWindow_SetCancelCallBack(LuaFunction func)
    {
        HandleErrorWindow.m_cancelCallBack = (p1, p2) =>
        {
            if (func != null)
            {
                func.Call2Args(p1, p2);
            }
        };
    }
    #endregion

    #region UMengSys
    public static void WeiXinLogin() 
    {
        if (UMengSys.Instance == null)
        {
            Debug.LogError("UMengSys no Instance!");
            return;
        }

        UMengSys.Instance.WeiXinLogin();
    }

    public static void WeiXinLogout()
    {
        if (UMengSys.Instance == null)
        {
            Debug.LogError("UMengSys no Instance!");
            return;
        }
        UMengSys.Instance.Logout();
    }

    public static void LoadPic(string picUrl, string picPath, LuaInterface.LuaFunction luaFunc)
    {
        if (UMengSys.Instance == null)
        {
            Debug.LogError("UMengSys no Instance!");
            return;
        }
        UMengSys.Instance.LoadPic(picUrl, picPath,luaFunc);
    }

    //直接分享url 不下载
    public static void WeiXinShareLocalUrl(int platForm, string text, string title, string picUrl, string targeturl, LuaInterface.LuaFunction luaFunc)
    {
        if (UMengSys.Instance == null)
        {
            Debug.LogError("UMengSys no Instance!");
            return;
        }
        Platform myPlatform = (Platform)platForm;
        UMengSys.Instance.WeiXinShareLocalUrl(myPlatform, text, title, picUrl, targeturl, luaFunc);
    }

    //获取截屏图片
    public static void GetScreenTexturePic(int capx, int capy, int capwidth, int capheight, string picPath, LuaInterface.LuaFunction luaFunc)
    {
        if (UMengSys.Instance == null)
        {
            Debug.LogError("UMengSys no Instance!");
            return;
        }
        UMengSys.Instance.GetScreenTexturePic(capx, capy, capwidth, capheight, picPath, luaFunc);
    }

    public static void GetScreenTexturePic(Camera camera, int capx, int capy, int capwidth, int capheight, string picPath, LuaInterface.LuaFunction luaFunc)
    {
        if (UMengSys.Instance == null)
        {
            Debug.LogError("UMengSys no Instance!");
            return;
        }
        UMengSys.Instance.GetScreenTexturePic(camera, capx, capy, capwidth, capheight, picPath, luaFunc);
    }

    public static void SetScreenShotWaitTime(float waitTime) 
    {
        UMengSys.waitTime = waitTime;
    }

    public static UMengSys GetUmengSys() 
    {
        return UMengSys.Instance;
    }
    #endregion

    #region  PlatformInterface
    public static int GetWifiRssi()
    {
        return DWPlatformInfo.rssi;
    }

    public static int GetNetType()
    {
        return DWPlatformInfo.netType;
    }

    public static double GetBattery()
    {
        return DWPlatformInfo.battery;
    }

    public static string GetDeviceID()
    {
        return DWPlatformInfo.deviceid;
    }

    public static string GetLatitude()
    {
		#if UNITY_EDITOR
		return "22.546804";
		#else
        return DWPlatformInfo.latitude;
		#endif
    }

    public static string GetLongitude()
    {
		#if UNITY_EDITOR
		return "113.959438";
		#else
		return DWPlatformInfo.longitude;
		#endif
    }

    public static string GetCountry()
    {
        return DWPlatformInfo.country;
    }

    public static string GetCountryCode()
    {
        return DWPlatformInfo.countrycode;
    }

    public static string GetState()
    {
        return DWPlatformInfo.state;
    }

    public static string GetCity()
    {
        return DWPlatformInfo.city;
    }

    public static string GetSubLocality()
    {
        return DWPlatformInfo.subLocality;
    }

    public static string GetStreet()
    {
        return DWPlatformInfo.street;
    }

    public static string GetThoroughfare()
    {
        return DWPlatformInfo.thoroughfare;
    }

    public static string GetSubThoroughfare()
    {
        return DWPlatformInfo.subThoroughfare;
    }

    public static string GetLocationName()
    {
        return DWPlatformInfo.name;
    }

    public static string GetFormattedAddressLine()
    {
        return DWPlatformInfo.formattedAddressLine;
    }
    #endregion

    #region PlatInterface
    public static void PlatInterface_RefreshBattery()
    {
        PlatInterface.Instance.getBattery();
    }

    public static void PlatInterface_RefreshLocation()
    {
        PlatInterface.Instance.getLocation();
    }

    public static void PlatInterface_RefreshNetStatus()
    {
        PlatInterface.Instance.getWifiRssi();
    }

    public static void PlatInterface_CopyStr(string str)
    {
        PlatInterface.Instance.systemCopyStr(str);
    }

    #endregion

    #region CIAPSys
    private static bool m_hasInitUnlegalCountry = false;
    public static void CIAPSys_InitUnlegalCountry()
    {
        if (m_hasInitUnlegalCountry)
        {
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            DWIAPLib._InitUnlegalCountry("test");
            m_hasInitUnlegalCountry = true;
        }
    }

    public static void CIAPSys_PurchaseSucceed(string sPid,string serialNumber)
    {
        DWIAPLib._PurchaseSucceed(sPid, serialNumber);
    }

    public static void CIAPSys_SendBuyDiamond(string purchaseIdentify)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Debug.LogError("CIAPSys_SendBuyDiamond " + purchaseIdentify);
            if (DWIAPLib._CanPurchase())
            {
                DWIAPLib._BuyIAP(purchaseIdentify);
            }
            else
            {
                LuaRootSys.Instance.CallLuaFunc("CIAPSys.HandleBuyIAPCB", "0");
            }
        }
        else
        {
            LuaRootSys.Instance.CallLuaFunc("CIAPSys.HandleBuyIAPCB", "0");
        }
    }


    #endregion

    #region Bugly

    public static void Bugly_SetUserID(int userID)
    {
        string str = userID.ToString();
        CBuglyPlugin.SetUserId(str);
    }
    #endregion

    #region TouchCheck
    public static bool TouchBegin()
    {
        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            return true;
        }
        return false;
    }

    public static bool TouchEnd()
    {
        if (Input.GetMouseButtonUp(0))
        {
            return true;
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            return true;
        }
        return false;
    }

    public static bool TouchIng()
    {
        if (Input.GetMouseButton(0))
        {
            return true;
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            return true;
        }
        return false;
    }
    #endregion

    public static Action ConversAction(LuaInterface.LuaFunction luaFunc,System.Object ob) 
    {
        Action ac =() =>
        {
            luaFunc.Call(ob);
        };
        return ac;
    }

    public static TimeValue GetTimeValue() 
    {
        return new TimeValue();
    }


    #region reflect 
    private static Assembly m_assembly = null;
    private static void CheckAssembly()
    {
        if(m_assembly == null)
        {
            Type testType = typeof(WrapSys);
            m_assembly = testType.Assembly;
        }
    }
    //静态函数或者传入实例对象来调用成员函数
    public static object Reflection_CallMethod(string methodName, string targetName = null, object obj = null, DwObject[] parameters = null)
    {
        CheckAssembly();
        if (m_assembly != null)
        {
            object obje = null;
            System.Reflection.MethodInfo method = null;
            if (obj == null)
            {
                var targetType = m_assembly.GetType(targetName);

                method = targetType.GetMethod(methodName);
            }
            else
            {
                obje = obj;
                method = obj.GetType().GetMethod(methodName);
            }

            if(method != null)
            {
                int len = parameters.Length;
                object[] objs = new object[len];
                for(int i = 0; i < len; i ++)
                {
                    objs[i] = parameters[i].value;
                }
                return method.Invoke(obje, objs);
            }
        }

        return null;
    }

    public static object Reflection_CallMethodByObjName(string methodName, string targetName = null, string objName = null, DwObject[] parameters = null)
    {
        CheckAssembly();
        if (m_assembly != null)
        {
            object obje = null;
            System.Reflection.MethodInfo method = null;
            var targetType = m_assembly.GetType(targetName);

            if (objName != null)
            {
                obje = Reflection_GetProperty(targetName,objName);
            }

            method = targetType.GetMethod(methodName);

            if (method != null)
            {
                int len = parameters.Length;
                object[] objs = new object[len];
                for (int i = 0; i < len; i++)
                {
                    objs[i] = parameters[i].value;
                }
                return method.Invoke(obje, objs);
            }
        }

        return null;
    }

    public static object Reflection_GetProperty(string targetName, string propertyName, object obj = null)
    {
        CheckAssembly();
        if (m_assembly != null)
        {
            var targetType = m_assembly.GetType(targetName);
            return targetType.GetProperty(propertyName).GetValue(obj, null);
        }
        return null;
    }

    public static object Reflection_GetField(string targetName, string fieldName, object obj = null)
    {
        CheckAssembly();
        if (m_assembly != null)
        {
            var targetType = m_assembly.GetType(targetName);
            return targetType.GetField(fieldName).GetValue(obj);
        }
        return null;
    }

    public static void Reflection_SetField(string targetName, string fieldName, object obj, object value)
    {
        CheckAssembly();
        if (m_assembly != null)
        {
            var targetType = m_assembly.GetType(targetName);
            targetType.GetField(fieldName).SetValue(obj, value);
        }
    }


    public static void TestReflect(string s)
    {
        Debug.LogError(s);
    }
    #endregion


    #region string tools

    public static string StringSafeConvert(LuaStringBuffer data)
    {
        string str = null;
        if (data != null)
        {
            str = System.Convert.ToBase64String(data.buffer);
        }
        return str;
    }
    #endregion
}
