package com.starjoys.demo;

import java.util.HashMap;

import org.json.JSONException;
import org.json.JSONObject;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

import com.starjoys.msdk.SJoyMSDK;
import com.starjoys.msdk.SJoyMsdkCallback;
import com.starjoys.msdk.model.constant.MsdkConstant;
import com.unity3d.player.UnityPlayer;

public class PlatformSDK {

	private static PlatformSDK mPlatform = null;
	private static MainActivity mActivity = null;
	
	private static String m_strAppkey="CiT3R2wS5aHnDmV";
	
	public static PlatformSDK getInstance(MainActivity activity)
	{
		mPlatform = new PlatformSDK(activity);
		return mPlatform;
	}
	
	public PlatformSDK(MainActivity activity)
	{
		mActivity = activity;
		InitSDK();
	}
	
	public void InitSDK()
	{
		SJoyMSDK.getInstance().doInit(mActivity, m_strAppkey, new SJoyMsdkCallback() {

			@Override
			public void onExitGameFail() {
				// TODO Auto-generated method stub
				
			}

			@Override
			public void onExitGameSuccess() {
				// TODO Auto-generated method stub
				System.exit(1);
			}

			@Override
			public void onInitFail(String arg0) {
				// TODO Auto-generated method stub
				
			}

			@Override
			public void onInitSuccess() {
				// TODO Auto-generated method stub
				
			}

			@Override
			public void onLoginFail(String arg0) {
				// TODO Auto-generated method stub
				UnityPlayer.UnitySendMessage("EzFunSDKRoot","LoginCallBack", "0");
			}

			@Override
			public void onLoginSuccess(Bundle bundle) {
				// TODO Auto-generated method stub
				String strLogin = "1;1" + ";" + "" + ";" + bundle.getString("token");
				UnityPlayer.UnitySendMessage("EzFunSDKRoot","LoginCallBack", strLogin);
			}

			@Override
			public void onLogoutFail(String arg0) {
				// TODO Auto-generated method stub
				
			}

			@Override
			public void onLogoutSuccess() {
				// TODO Auto-generated method stub
				UnityPlayer.UnitySendMessage("EzFunSDKRoot","OnSwitchAccount", "1");
			}

			@Override
			public void onPayFail(String arg0) {
				// TODO Auto-generated method stub
				UnityPlayer.UnitySendMessage("EzFunSDKRoot","PayCallBack", "0");
			}

			@Override
			public void onPaySuccess(Bundle arg0) {
				// TODO Auto-generated method stub
				UnityPlayer.UnitySendMessage("EzFunSDKRoot","PayCallBack", "1;2");
			}

			@Override
			public void onUserSwitchFail(String arg0) {
				// TODO Auto-generated method stub
				
			}

			@Override
			public void onUserSwitchSuccess(Bundle arg0) {
				// TODO Auto-generated method stub
				UnityPlayer.UnitySendMessage("EzFunSDKRoot","OnSwitchAccount", "1");
			}
		});
	}
	
	public void onStart()
	{
		SJoyMSDK.getInstance().onStart();
	}

	protected void onRestart() {
		SJoyMSDK.getInstance().onRestart();
	}

	protected void onResume() {
		SJoyMSDK.getInstance().onResume();
	}

	protected void onPause() {
	    SJoyMSDK.getInstance().onPause();
	}

	protected void onStop() {
	    SJoyMSDK.getInstance().onStop();
	}

	protected void onDestroy() {
	    SJoyMSDK.getInstance().onDestroy();
	}

	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
	    SJoyMSDK.getInstance().onActivityResult(requestCode, resultCode, data);
	}

	protected void onNewIntent(Intent intent) {
	    SJoyMSDK.getInstance().onNewIntent(intent);
	}
	
	public void Login(String strLogin)
	{
		SJoyMSDK.getInstance().userLogin(mActivity);
	}
	
	public void Pay(String strOrderID, String PayJson, String strExt, String strParam, String strServerParam)
	{
		String strZoneID = ""; 
		String strZoneName = "";  
		String strRoleName = "";  
		String strRoleLev = ""; 
		String strRoleID = "";  
		String strProductName = ""; 
		String strMoney = "";
		
		try {
			JSONObject json = new JSONObject(PayJson);
			strZoneID = json.optString("ZoneID");
			strZoneName = json.optString("ZoneName");
			strRoleName = json.optString("RoleName");
			strRoleLev = json.optString("RoleLev");
			strRoleID = json.optString("RoleID");
			strProductName = json.optString("ProductName");
			strMoney = json.optString("MoneyAmount");
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			Log.d("Pay", e.toString());
			e.printStackTrace();
		}
		
		String sMoneyYuan = Integer.parseInt(strMoney) / 100 + "";
		HashMap<String, String> payinfo = new HashMap<String, String>();
		payinfo.put(MsdkConstant.PAY_MONEY, sMoneyYuan);			//整数，充值金额，单位：元
		payinfo.put(MsdkConstant.PAY_ORDER_NO, strOrderID);			//CP订单号（不能超过64位），全局唯一，不可重复
		payinfo.put(MsdkConstant.PAY_ORDER_NAME, strProductName);	//商品名称
		payinfo.put(MsdkConstant.PAY_ORDER_EXTRA, strExt);			//商品拓展数据
		payinfo.put(MsdkConstant.PAY_ROLE_ID, strRoleID);			//角色ID，不得超过32个字符
		payinfo.put(MsdkConstant.PAY_ROLE_NAME, strRoleName);		//角色名称
		payinfo.put(MsdkConstant.PAY_ROLE_LEVEL, strRoleLev);		//数字，角色等级
		payinfo.put(MsdkConstant.PAY_SERVER_ID, strZoneID);			//数字，服务器ID
		payinfo.put(MsdkConstant.PAY_SERVER_NAME, strZoneName);		//服务器名称
					
		SJoyMSDK.getInstance().userPay(mActivity, payinfo);
	}
	
	public void ExitGame()
	{
		SJoyMSDK.getInstance().doExitGame(mActivity);
	}
	
	public void CreatRole(String RoleJson)
	{
		String strZoneID = ""; 
		String strZoneName = "";  
		String strRoleName = "";  
		String strRoleLev = ""; 
		String strRoleID = "";  
		String strVip = ""; 
		String strBalance = "";
		String strParty = "";
		String strExt = "";
		
		try {
			JSONObject json = new JSONObject(RoleJson);
			strZoneID = json.optString("ZoneID");
			strZoneName = json.optString("ZoneName");
			strRoleName = json.optString("RoleName");
			strRoleLev = json.optString("RoleLev");
			strRoleID = json.optString("RoleID");
			strVip = json.optString("VipLev");
			strBalance = json.optString("Balance");
			strParty = json.optString("PartyName");
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		HashMap<String, String> info = new HashMap<String, String>();
		info.put(MsdkConstant.SUBMIT_ROLE_ID, strRoleID);		//角色ID，不得超过32个字符
		info.put(MsdkConstant.SUBMIT_ROLE_NAME, strRoleName);	//角色名称
		info.put(MsdkConstant.SUBMIT_ROLE_LEVEL, strRoleLev);	//角色等级
		info.put(MsdkConstant.SUBMIT_SERVER_ID, strZoneID);		//服务器ID，数字
		info.put(MsdkConstant.SUBMIT_SERVER_NAME, strZoneName);	//服务器名称
		info.put(MsdkConstant.SUBMIT_BALANCE, strBalance);		//玩家余额，默认0
		info.put(MsdkConstant.SUBMIT_VIP, strVip);				//玩家VIP等级，默认0
		info.put(MsdkConstant.SUBMIT_PARTYNAME, strParty);		//玩家帮派，没有传“无”
		info.put(MsdkConstant.SUBMIT_EXTRA, strExt);			//拓展字段
		
        SJoyMSDK.getInstance().roleCreate(info);
	}
	
	public void EnterGame(String RoleJson)
	{
		String strZoneID = ""; 
		String strZoneName = "";  
		String strRoleName = "";  
		String strRoleLev = ""; 
		String strRoleID = "";  
		String strVip = ""; 
		String strBalance = "";
		String strParty = "";
		String strExt = "";
		
		try {
			JSONObject json = new JSONObject(RoleJson);
			strZoneID = json.optString("ZoneID");
			strZoneName = json.optString("ZoneName");
			strRoleName = json.optString("RoleName");
			strRoleLev = json.optString("RoleLev");
			strRoleID = json.optString("RoleID");
			strVip = json.optString("VipLev");
			strBalance = json.optString("Balance");
			strParty = json.optString("PartyName");
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		HashMap<String, String> info = new HashMap<String, String>();
		info.put(MsdkConstant.SUBMIT_ROLE_ID, strRoleID);		//角色ID，不得超过32个字符
		info.put(MsdkConstant.SUBMIT_ROLE_NAME, strRoleName);	//角色名称
		info.put(MsdkConstant.SUBMIT_ROLE_LEVEL, strRoleLev);	//角色等级
		info.put(MsdkConstant.SUBMIT_SERVER_ID, strZoneID);		//服务器ID，数字
		info.put(MsdkConstant.SUBMIT_SERVER_NAME, strZoneName);	//服务器名称
		info.put(MsdkConstant.SUBMIT_BALANCE, strBalance);		//玩家余额，默认0
		info.put(MsdkConstant.SUBMIT_VIP, strVip);				//玩家VIP等级，默认0
		info.put(MsdkConstant.SUBMIT_PARTYNAME, strParty);		//玩家帮派，没有传“无”
		info.put(MsdkConstant.SUBMIT_EXTRA, strExt);			//拓展字段
		
        SJoyMSDK.getInstance().roleEnterGame(info);
	}
	
	public void GameUpdeteLevel(String RoleJson)
	{
		String strZoneID = ""; 
		String strZoneName = "";  
		String strRoleName = "";  
		String strRoleLev = ""; 
		String strRoleID = "";  
		String strVip = ""; 
		String strBalance = "";
		String strParty = "";
		String strExt = "";
		
		try {
			JSONObject json = new JSONObject(RoleJson);
			strZoneID = json.optString("ZoneID");
			strZoneName = json.optString("ZoneName");
			strRoleName = json.optString("RoleName");
			strRoleLev = json.optString("RoleLev");
			strRoleID = json.optString("RoleID");
			strVip = json.optString("VipLev");
			strBalance = json.optString("Balance");
			strParty = json.optString("PartyName");
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		HashMap<String, String> info = new HashMap<String, String>();
		info.put(MsdkConstant.SUBMIT_ROLE_ID, strRoleID);		//角色ID，不得超过32个字符
		info.put(MsdkConstant.SUBMIT_ROLE_NAME, strRoleName);	//角色名称
		info.put(MsdkConstant.SUBMIT_ROLE_LEVEL, strRoleLev);	//角色等级
		info.put(MsdkConstant.SUBMIT_SERVER_ID, strZoneID);		//服务器ID，数字
		info.put(MsdkConstant.SUBMIT_SERVER_NAME, strZoneName);	//服务器名称
		info.put(MsdkConstant.SUBMIT_BALANCE, strBalance);		//玩家余额，默认0
		info.put(MsdkConstant.SUBMIT_VIP, strVip);				//玩家VIP等级，默认0
		info.put(MsdkConstant.SUBMIT_PARTYNAME, strParty);		//玩家帮派，没有传“无”
		info.put(MsdkConstant.SUBMIT_EXTRA, strExt);			//拓展字段
		
        SJoyMSDK.getInstance().roleUpgrade(info);
	}
	
	public void GameUpdateRoleName(String RoleJson)
	{
		String strZoneID = ""; 
		String strZoneName = "";  
		String strRoleName = "";  
		String strRoleLev = ""; 
		String strRoleID = "";  
		String strVip = ""; 
		String strBalance = "";
		String strParty = "";
		String strExt = "";
		
		try {
			JSONObject json = new JSONObject(RoleJson);
			strZoneID = json.optString("ZoneID");
			strZoneName = json.optString("ZoneName");
			strRoleName = json.optString("RoleName");
			strRoleLev = json.optString("RoleLev");
			strRoleID = json.optString("RoleID");
			strVip = json.optString("VipLev");
			strBalance = json.optString("Balance");
			strParty = json.optString("PartyName");
			strExt = json.optString("RoleOldName");
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		HashMap<String, String> info = new HashMap<String, String>();
		info.put(MsdkConstant.SUBMIT_ROLE_ID, strRoleID);		//角色ID，不得超过32个字符
		info.put(MsdkConstant.SUBMIT_ROLE_NAME, strRoleName);	//角色名称
		info.put(MsdkConstant.SUBMIT_ROLE_LEVEL, strRoleLev);	//角色等级
		info.put(MsdkConstant.SUBMIT_SERVER_ID, strZoneID);		//服务器ID，数字
		info.put(MsdkConstant.SUBMIT_SERVER_NAME, strZoneName);	//服务器名称
		info.put(MsdkConstant.SUBMIT_BALANCE, strBalance);		//玩家余额，默认0
		info.put(MsdkConstant.SUBMIT_VIP, strVip);				//玩家VIP等级，默认0
		info.put(MsdkConstant.SUBMIT_PARTYNAME, strParty);		//玩家帮派，没有传“无”
		info.put(MsdkConstant.SUBMIT_EXTRA, strExt);			//拓展字段
		
        SJoyMSDK.getInstance().roleUpdate(info);
	}
	
	public void Logout()
	{
		SJoyMSDK.getInstance().userSwitch(mActivity);
	}
	
	public String GetAppConfig()
	{
		String appID = SJoyMSDK.getInstance().getAppConfig(mActivity).getApp_id();
		String cchID = SJoyMSDK.getInstance().getAppConfig(mActivity).getCch_id();
		String mdID = SJoyMSDK.getInstance().getAppConfig(mActivity).getMd_id();
		String sdkverion = SJoyMSDK.getInstance().getAppConfig(mActivity).getSdk_version();

		String strConfig = appID + ";" + cchID + ";" + mdID+  ";" + sdkverion;

		return strConfig;
	}
}
