using UnityEngine;
using System.Collections;

public class CMSDKData
{
	public int m_subPlatfrom;
	public string m_openID;
	public string m_openKey;
	public string m_payToken;
	public string m_pfKey;
	public string m_pf;
	public string m_PayChannelName;		// 支付渠道

	public CMSDKData()
	{
	}

	public CMSDKData(string cbStr)
	{
		string[] strArray = cbStr.Split(';');
		Init (strArray);
	}

	private void Init(string[] strArray)
	{
		string openID 			= strArray[0];
		string qqAccessToken 	= strArray[1];
		string qqPayToken 		= strArray[2];
		string wxAccessToken 	= strArray[3];
		string wxRefreshToken 	= strArray[4];
		string pf 				= strArray[5];
		string pfKey 			= strArray[6];
		string platform 		= strArray[7];
		string PayChannel = "";

		if(strArray.Length > 8)
		{
			PayChannel		= strArray[8];
		}
		
		m_subPlatfrom = int.Parse(platform);
		m_openID 			= openID;
		m_pfKey				= pfKey;
		m_pf 				= pf;
		m_PayChannelName    = PayChannel;
		switch(m_subPlatfrom)
		{
		case 1:
			//QQ
			m_openKey			= qqAccessToken;
			m_payToken			= qqPayToken;
			break;
		case 2:
			//wei xin
			m_openKey			= wxAccessToken;
			break;
		}
	}
	
	//处理支付回调
	public bool HandlePurchase(string cbstr)
	{
		string[] strArray = cbstr.Split(';');
		if(strArray[0] == "1")
		{
			string[] array = new string[strArray.Length];
			for(int i = 0; i < array.Length - 1; i ++)
			{
				array[i] = strArray[i + 1];
			}
			array[array.Length - 1] = m_subPlatfrom.ToString();
			Init(array);
			return true;
		}
		else
		{
			return false;
		}
	}
}

public class CSDKData
{

	// SDK登出
	public static void SDKLogout()
	{
		// 实现SDK的登出
		switch(Version.Instance.GetCurrentSdkPlatform())
		{
		//case ENUM_SDK_PLATFORM.ESP_YingyongBao:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_DFWS:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_byz:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_bayws:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_byzhan:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_dfzr:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_banyws:
		//	MSDKSys.MSDKLogout();
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_LJ:
		//	LJSDKSys.LJLogout();
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_UC:
		//	UCSDKSys.UCLogout();
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_BD:
		//	BDSDKSys.BDLogout();
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_OPPO:
		//	OppoSDKSys.oppoLogout();
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_vivo:
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_sogou:
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_qihoo360:
		//	Qihoo360SDKSys.QihooLogout();
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_jinli:
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_mzw:
		//	mzwSDKSys.mzwLogout();
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_m4399:
		//	m4399SDKSys.m4399Logout();
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_youku:
		//	youkuSDKSys.youkuLogout();
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_anzhi:
		//	anzhiSDKSys.anzhiLogout();
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_dangle:
		//	dangleSDKSys.dangleLogout();
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_xinlang:
		//	xinlangSDKSys.sinaLogout();
		//	break;
  //      case ENUM_SDK_PLATFORM.ESP_p1767ios:
  //          p1767SDKSys.InitSDK();
  //          break;
  //      case ENUM_SDK_PLATFORM.ESP_VTC_ios:
		//case ENUM_SDK_PLATFORM.ESP_VTC_android:
		//	VTCSDKSys.VTClogout();
	//		break;
		}
	}

	// 实现SDK数据上传
	public static void SDKExtDataInfo(string id = "")
	{
		//long roleId = CAccMgr.Instance.RID;
		//string roleName = CAccMgr.Instance.Name;
		//int roleLev = CAccMgr.Instance.Level;
		//int ZoneId = CNetSys.Instance.getCurZoneNode().sid;
		//string ZoneName = CNetSys.Instance.getCurZoneNode().name;
		//int balance = PackSys.CreateInstance().GetDiamond();
		//int vip = VipSys.Instance.GetVipLv();
		//ezfun.SCGuildInfoRsp info = GuildSys.Instance.getGuildInfo();
		//string partyName = "";
		//if(info == null)
		//{
		//	partyName = TextData.GetText(400501);
		//}
		//else
		//{
		//	partyName = info.name;
		//}
		switch(Version.Instance.GetCurrentSdkPlatform())
		{
		//case ENUM_SDK_PLATFORM.ESP_LJ:
		//	LJSDKSys.LJSetExtData(id, roleId.ToString(), roleName, roleLev.ToString(), ZoneId.ToString(), 
		//	                      ZoneName, balance.ToString(), vip.ToString(), partyName);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_UC:
		//	UCSDKSys.UCSetExtData(roleId.ToString(),roleName, roleLev.ToString(), ZoneName, ZoneId);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_OPPO:
		//	OppoSDKSys.oppoExtInfo(ZoneName, roleId.ToString(), roleLev.ToString());
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_vivo:
		//	vivoSDKSys.vivoExtInfo(roleId.ToString(), roleLev.ToString(), roleName, ZoneId.ToString(), ZoneName);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_huawei:
		//	huaweiSDKSys.huaweiExtInfo(roleLev.ToString(), roleName, ZoneName, partyName);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_qihoo360:
		//	string strQihooID = CNetSys.Instance.m_qihooUserID;
		//	Qihoo360SDKSys.QihooUploadScore(strQihooID, roleLev.ToString());
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_pps:
		//	ppsSDKSys.ppsEnterGame(ZoneId.ToString());
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_kugou:
		//	kugouSDKSys.kugouExtInfo(roleName, roleLev, ZoneId);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_m4399:
		//	m4399SDKSys.m4399SetServer(ZoneId.ToString());
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_anzhi:
		//	anzhiSDKSys.anzhiExtInfo(ZoneId.ToString(), roleLev.ToString(), "", roleId.ToString());
		//	break;
  //      case ENUM_SDK_PLATFORM.ESP_p1767ios:
  //          p1767SDKSys.p1767SetServerID(ZoneId.ToString());
  //          break;
		//case ENUM_SDK_PLATFORM.ESP_p1767:
		//case ENUM_SDK_PLATFORM.ESP_p1767_mc:
  //          p1767SDKSys.p1767SetServerID(ZoneId.ToString());
  //          break;
		}
	}

	public static void LoginFail(string strLogin)
	{
	}

	public static void XgRegisterPush()
	{
		//long roleId = CAccMgr.Instance.RID;
		//string account = "ezfun" + roleId;
		switch(Version.Instance.GetCurrentSdkPlatform())
		{
		//case ENUM_SDK_PLATFORM.ESP_YingyongBao:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_DFWS:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_byz:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_bayws:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_byzhan:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_dfzr:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_banyws:
		//	MSDKSys.MSDKPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_LJ:
		//	LJSDKSys.LJPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_UC:
		//	UCSDKSys.UCPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_BD:
		//	BDSDKSys.BDPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_MI:
		//	MiSDKSys.MiPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_IPAY:
		//case ENUM_SDK_PLATFORM.ESP_kaopu:
		//case ENUM_SDK_PLATFORM.ESP_XiaoJiShouBing:
		//	IAppPaySDKSys.IAppPayPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_OPPO:
		//	OppoSDKSys.oppoXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_vivo:
		//	vivoSDKSys.vivoXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_lenovo:
		//	lenovoSDKSys.lenovoXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_TianTuo:
		//case ENUM_SDK_PLATFORM.ESP_TianTuoHD:
  //      case ENUM_SDK_PLATFORM.ESP_p1767ios:
  //      case ENUM_SDK_PLATFORM.ESP_VTC_ios:
		//	XgPush.xgSetAccount(account);
		//	XgPush.xgRegDevToken();
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_huawei:
		//	huaweiSDKSys.huaweiXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_sogou:
		//	sogouSDKSys.sogouXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_qihoo360:
		//	Qihoo360SDKSys.QihooXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_jinli:
		//	jinliSDKSys.jinliXgRegister(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_mzw:
		//	mzwSDKSys.mzwRegisterXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_yyh:
		//	yyhSDKSys.yyhXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_pps:
		//	ppsSDKSys.ppsXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_htc:
		//	HTCSDKSys.HTCXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_kugou:
		//	kugouSDKSys.kugouXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_coolpad:
		//	coolpadSDKSys.CoolPadXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_m4399:
		//	m4399SDKSys.m4399XgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_youku:
		//	youkuSDKSys.youkuXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_wdj:
		//	wdjSDKsys.wdjXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_anzhi:
		//	anzhiSDKSys.anzhiXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_meizu:
		//	meizuSDKsys.meizuXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_dangle:
		//	dangleSDKSys.dangleXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_dianxin:
		//	dianxinSDKSys.dianxinSetTag(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_xinlang:
		//	xinlangSDKSys.sinaXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_pptv:
		//	pptvSDKSys.pptvXgPush(account);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_p1767_mc:
		//case ENUM_SDK_PLATFORM.ESP_p1767:
		//	p1767SDKSys.p1767XgPush(account);
		//	break;
		}

		//string strChanelName = DataEyeStaticSys.GetSDKPlatformName();
		//SetXgPushTag(strChanelName);			// 设置渠道推送标签
	}

	// 设置推送的标签
	public static void SetXgPushTag(string strTag)
	{
		switch(Version.Instance.GetCurrentSdkPlatform())
		{
		//case ENUM_SDK_PLATFORM.ESP_YingyongBao:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_DFWS:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_byz:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_bayws:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_byzhan:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_dfzr:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_banyws:
		//	MSDKSys.MSDKSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_LJ:
		//	LJSDKSys.LJXgSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_UC:
		//	UCSDKSys.UCSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_BD:
		//	BDSDKSys.BDXgSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_MI:
		//	MiSDKSys.MiXgSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_IPAY:
		//case ENUM_SDK_PLATFORM.ESP_kaopu:
		//case ENUM_SDK_PLATFORM.ESP_XiaoJiShouBing:
		//	IAppPaySDKSys.IAppPaySetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_OPPO:
		//	OppoSDKSys.oppoXgSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_vivo:
		//	vivoSDKSys.vivoXgSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_lenovo:
		//	lenovoSDKSys.lenovoXgSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_TianTuo:
		//case ENUM_SDK_PLATFORM.ESP_TianTuoHD:
  //      case ENUM_SDK_PLATFORM.ESP_VTC_ios:
  //      case ENUM_SDK_PLATFORM.ESP_p1767ios:
		//	XgPush.XgSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_huawei:
		//	huaweiSDKSys.huaweiXgSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_sogou:
		//	sogouSDKSys.sogouXgSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_qihoo360:
		//	Qihoo360SDKSys.QihooSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_jinli:
		//	jinliSDKSys.jinliXgSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_mzw:
		//	mzwSDKSys.mzwXgSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_yyh:
		//	yyhSDKSys.yyhXgSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_pps:
		//	ppsSDKSys.ppsSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_htc:
		//	HTCSDKSys.HTCXgSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_kugou:
		//	kugouSDKSys.kugouXgSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_coolpad:
		//	coolpadSDKSys.CoolPadSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_m4399:
		//	m4399SDKSys.m4399SetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_youku:
		//	youkuSDKSys.youkuSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_wdj:
		//	wdjSDKsys.wdjSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_anzhi:
		//	anzhiSDKSys.anzhiSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_meizu:
		//	meizuSDKsys.meizuSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_dangle:
		//	dangleSDKSys.dangleSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_xinlang:
		//	xinlangSDKSys.sinaXgSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_pptv:
		//	pptvSDKSys.pptvXgSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_p1767_mc:
		//case ENUM_SDK_PLATFORM.ESP_p1767:
		//	p1767SDKSys.p1767XgSetTag(strTag);
		//	break;
		}
	}

	// 删除推送的标签
	public static void DeleteXgPushTag(string strTag)
	{
		switch(Version.Instance.GetCurrentSdkPlatform())
		{
		//case ENUM_SDK_PLATFORM.ESP_YingyongBao:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_DFWS:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_byz:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_bayws:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_byzhan:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_dfzr:
		//case ENUM_SDK_PLATFORM.ESP_MSDK_banyws:
		//	MSDKSys.MSDKDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_LJ:
		//	LJSDKSys.LJXgDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_UC:
		//	UCSDKSys.UCDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_BD:
		//	BDSDKSys.BDXgSetTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_MI:
		//	MiSDKSys.MiXgDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_IPAY:
		//case ENUM_SDK_PLATFORM.ESP_kaopu:
		//case ENUM_SDK_PLATFORM.ESP_XiaoJiShouBing:
		//	IAppPaySDKSys.IAppPayDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_OPPO:
		//	OppoSDKSys.oppoXgDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_vivo:
		//	vivoSDKSys.vivoXgDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_lenovo:
		//	lenovoSDKSys.lenovoXgDelTag(strTag);
		//	break;
        //case ENUM_SDK_PLATFORM.ESP_TianTuo:
        //case ENUM_SDK_PLATFORM.ESP_TianTuoHD:
        //case ENUM_SDK_PLATFORM.ESP_p1767ios:
        //case ENUM_SDK_PLATFORM.ESP_VTC_ios:
//			XgPush.XgDeleteTag(strTag);
            //break;
		//case ENUM_SDK_PLATFORM.ESP_huawei:
		//	huaweiSDKSys.huaweiXgSetDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_sogou:
		//	sogouSDKSys.sogouXgDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_qihoo360:
		//	Qihoo360SDKSys.QihooDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_jinli:
		//	jinliSDKSys.jinliXgDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_mzw:
		//	mzwSDKSys.mzwXgDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_yyh:
		//	yyhSDKSys.yyhXgDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_pps:
		//	ppsSDKSys.ppsDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_htc:
		//	HTCSDKSys.HTCXgDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_kugou:
		//	kugouSDKSys.kugouXgDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_coolpad:
		//	coolpadSDKSys.CoolPadDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_m4399:
		//	m4399SDKSys.m4399DelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_youku:
		//	youkuSDKSys.youkuDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_wdj:
		//	wdjSDKsys.wdjDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_anzhi:
		//	anzhiSDKSys.anzhiDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_meizu:
		//	meizuSDKsys.meizuDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_dangle:
		//	dangleSDKSys.dangleDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_dianxin:
		//	dianxinSDKSys.dianxinDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_xinlang:
		//	xinlangSDKSys.sinaXgDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_pptv:
		//	pptvSDKSys.pptvXgDelTag(strTag);
		//	break;
		//case ENUM_SDK_PLATFORM.ESP_p1767_mc:
		//case ENUM_SDK_PLATFORM.ESP_p1767:
		//	p1767SDKSys.p1767XgDelTag(strTag);
		//	break;
		}
	}

	// 是否是游客登陆
	public static bool IsGuestLogin()
	{
		bool isGuest = false;
        //switch(Version.Instance.GetCurrentSdkPlatform())
        //{
        ////case ENUM_SDK_PLATFORM.ESP_TianTuo:
        ////case ENUM_SDK_PLATFORM.ESP_IPAY:
        ////case ENUM_SDK_PLATFORM.ESP_kaopu:
        ////case ENUM_SDK_PLATFORM.ESP_TianTuoHD:
        ////case ENUM_SDK_PLATFORM.ESP_XiaoJiShouBing:
        ////	isGuest = m_TianTuoSDKData.m_isGuest;
        //    break;
        //}

		return isGuest;
	}
}
