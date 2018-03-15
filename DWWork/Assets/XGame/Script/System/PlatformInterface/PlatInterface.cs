using System.Collections;
using UnityEngine;
using System;

#if UNITY_IPHONE || UNITY_IOS
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
#endif

public class PlatInterface:MonoBehaviour
{
    public enum NetWorkType
    {
        NetWorkTypeNone = 0,
        NetWorkType2G = 1,
        NetWorkType3G = 2,
        NetWorkType4G = 3,
        NetWorkTypeWiFI = 5,
    }


    // Use this for initialization
    AndroidJavaClass unityPlayer;
    AndroidJavaObject currentActivity;

    AndroidJavaClass jc;
    AndroidJavaObject jo;
    public static PlatInterface _instance;
    public static PlatInterface Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gb = new GameObject("PlatInterface");
                GameObject.DontDestroyOnLoad(gb);
                _instance = EZFunTools.GetOrAddComponent<PlatInterface>(gb);

            }
            return _instance;
        }
    }

#if UNITY_IPHONE || UNITY_IOS
    [DllImport("__Internal")]
    public static extern void GetLocation_IOS();

    [DllImport("__Internal")]
    public static extern void GetRssi_IOS();

    [DllImport("__Internal")]
    public static extern void GetDeviceID_IOS();

    [DllImport("__Internal")]
    public static extern void GetBattery_IOS();

	[DllImport("__Internal")]
	public static extern void GetNetWorkType_IOS ();

    [DllImport("__Internal")]
    public static extern void CopyStr_IOS(string str);

//    [DllImport("__Internal")]
//    public static extern bool IsWeChatInstalled();
#endif

    public void Init()
    {
        Debug.Log("===================start");
        if (Application.platform == RuntimePlatform.Android)
        {
            unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            jc = new AndroidJavaClass("com.dawang.jxqp.platformUtils");
            jc.CallStatic("init", currentActivity);
            if (jc == null)
            {
                Debug.Log("=================init jc failed");
            }
            else
            {
                Debug.Log("=================init jc success");
            }
        }

        getBattery();
        getLocation();
        getWifiRssi();
        getDeviceID();
  
        Debug.Log("===================start end");
    }

    // Update is called once per frame
    void Update()
    {
    }


    //获取设备ID
    public string getDeviceID()
    {
        Debug.Log("===============================getDeviceID");
        string id = "";
        RuntimePlatform platform = Application.platform;
        if (platform == RuntimePlatform.Android)
        {
            jc.CallStatic<string>("getDeviceID");
        }
        else if (platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE || UNITY_IOS
            GetDeviceID_IOS();
#endif
        }

        return id;
    }

    //获取wifi信号强度，0到-100，值越大信号越好
    public void getWifiRssi()
    {
        Debug.Log("===============================getWifiRssi");
        RuntimePlatform platform = Application.platform;
        if (platform == RuntimePlatform.Android)
        {
            jc.CallStatic<string>("getNetworkType");
            jc.CallStatic<string>("getWifiRssi");
        }
        else if (platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE || UNITY_IOS
            GetNetWorkType_IOS();
            GetRssi_IOS();
#endif
        }
    }
    
    //获取经纬度 字符串，由两个double组成中间以#分割，前部表示latitude，后部表示longitude
    public void getLocation()
    {
        Debug.Log("===============================getLocation");
        RuntimePlatform platform = Application.platform;
        if (platform == RuntimePlatform.Android)
        {
            Debug.Log("=============================== Android getLocation");
            jc.CallStatic<string>("getLocation");
        }
        else if (platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE || UNITY_IOS
            GetLocation_IOS();
#endif
        }
    }

    public void getBattery()
    {
        Debug.Log("========================getBattery");
        RuntimePlatform platform = Application.platform;
        if (platform == RuntimePlatform.Android)
        {
            jc.CallStatic<string>("getBattery");
        }
        else if (platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE || UNITY_IOS
            GetBattery_IOS();
#endif
        }
    }

    public void systemCopyStr(string str)
    {
        RuntimePlatform platform = Application.platform;
        if (platform == RuntimePlatform.Android)
        {
            jc.CallStatic("SystemCopyStr", str);
        }
        else if (platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE || UNITY_IOS
            CopyStr_IOS(str);
#endif
        }
        else if (platform == RuntimePlatform.WindowsEditor)
        {
            Debug.LogWarning("Copy str is : " +str);
        }
    }

    public bool isWeChatInstalled()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE || UNIYT_IOS
            //return IsWeChatInstalled();
            return false;
#else
            return false;
#endif
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            return jc.CallStatic<bool>("IsWeChatInstalled");
        }
        else
        {
            return false;
        }
    }

    public void onLocationBack_IOS(string location)
    {
        Debug.Log("==========================location back IOS= " + location);
        //TODO

        UserLocation ul = JsonUtility.FromJson<UserLocation>(location);
        DWPlatformInfo.country = ul.Country;
        DWPlatformInfo.countrycode = ul.CountryCode;
        DWPlatformInfo.formattedAddressLine = ul.FormattedAddressLine;
        DWPlatformInfo.latitude = ul.Latitude;
        DWPlatformInfo.longitude = ul.Longitude;
        DWPlatformInfo.name = ul.Name;
        DWPlatformInfo.state = ul.State;
        DWPlatformInfo.street = ul.Street;
        DWPlatformInfo.city = ul.City;
        DWPlatformInfo.subLocality = ul.SubLocality;
        DWPlatformInfo.subThoroughfare = ul.SubThoroughfare;
        DWPlatformInfo.thoroughfare = ul.Thoroughfare;
        EventSys.Instance.AddEvent(EEventType.LocationRefresh);
    }

    public void onLocationBack_Android(string location)
    {
        Debug.Log("==========================location back Android= " + location);
        string sp1 = "#";
        string sp2 = ",";
        string[] addArray = location.Split(sp1.ToCharArray());
        DWPlatformInfo.latitude = addArray[1];
        DWPlatformInfo.longitude = addArray[0];
        string preaddr = addArray[2];
        DWPlatformInfo.name = addArray[3];
        DWPlatformInfo.subThoroughfare = addArray[4];
        string[] preaddrArray = preaddr.Split(sp2.ToCharArray());
        Debug.Log("preaddrArray len =" + preaddrArray.Length);
        DWPlatformInfo.state = preaddrArray[0];
        DWPlatformInfo.city = preaddrArray[1];
        if (preaddrArray.Length >= 3)
        {
            Debug.Log(preaddrArray[2]);
            DWPlatformInfo.subLocality = preaddrArray[2];
        }
        EventSys.Instance.AddEvent(EEventType.LocationRefresh);
    }
    
    public void onRssiBack(string rssi)
    {
		Debug.Log ("==========================rssi back= " + rssi);
        //TODO
        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if(Convert.ToInt32(rssi) > -20 && Convert.ToInt32(rssi) < 0)
            {
                rssi = Convert.ToString(5);
            }else if (Convert.ToInt32(rssi) <= -20 && Convert.ToInt32(rssi) > -40)
            {
                rssi = Convert.ToString(4);
            }else if (Convert.ToInt32(rssi) <= -40 && Convert.ToInt32(rssi) > -60)
            {
                rssi = Convert.ToString(3);
            }else if (Convert.ToInt32(rssi) <= -60 && Convert.ToInt32(rssi) > -80)
            {
                rssi = Convert.ToString(2);
            }
            else if (Convert.ToInt32(rssi) <= -80 && Convert.ToInt32(rssi) > -100)
            {
                rssi = Convert.ToString(1);
            }
            else
            {
                rssi = Convert.ToString(0);
            }
        }
        DWPlatformInfo.rssi = Convert.ToInt32(rssi);
        if(DWPlatformInfo.netType != (int)NetWorkType.NetWorkTypeWiFI)
        {
            DWPlatformInfo.rssi = 4;
        }
        EventSys.Instance.AddEvent(EEventType.NetWorkStrengthRefresh);
    }

	public void onNetWorkTypeBack(string type)
	{
		Debug.Log ("==========================network type back= " + type);
		//TODO
		DWPlatformInfo.netType = Convert.ToInt32(type);
        EventSys.Instance.AddEvent(EEventType.NetWorkTypeRefresh);
    }

    public void onDeviceIDBack(string deviceid)
    {
		Debug.Log ("==========================deviceid back= " + deviceid);
        //TODO
        DWPlatformInfo.deviceid = deviceid;
        EventSys.Instance.AddEvent(EEventType.DeviceIDRefresh);
    }

    public void onBatteryBack(string battery)
    {
		Debug.Log ("==========================battery back = " + battery);
        //TODO

        DWPlatformInfo.battery = Convert.ToDouble(battery);
#if UNITY_IPHONE
        DWPlatformInfo.battery = Convert.ToDouble(battery)*100;
#endif
        EventSys.Instance.AddEvent(EEventType.BatteryRefreh);
    }

    public void OnApplicationQuit()
    {
        Debug.Log("quit=======================================");
        //DWChatTeam.getInstance().DW_dismissTeam("");
        //LocalData.DeleteKey("TeamID");
        //DWChatTalk.getInstance().ClearAudio();
        //DWChatLogin.getInstance().DW_logout();
        //NIM.ClientAPI.Cleanup();
    }

}
