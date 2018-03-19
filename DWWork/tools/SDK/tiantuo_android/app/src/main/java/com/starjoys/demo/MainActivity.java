package com.starjoys.demo;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.telephony.TelephonyManager;
import android.util.Log;

import com.ezfun.files.FileLoad;
import com.tencent.android.tpush.XGIOperateCallback;
import com.tencent.android.tpush.XGPushConfig;
import com.tencent.android.tpush.XGPushManager;
import com.unity3d.player.UnityPlayerActivity;

public class MainActivity extends UnityPlayerActivity {

	private static PlatformSDK mPlatform = null;
	private static MainActivity mActivity = null;
	@Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.d("xgame2", "beginnnnn");
		FileLoad load = new FileLoad();
		Log.d("xgame2", "load dll finish");
		load.setAssetmanager(getResources().getAssets());
		Log.d("xgame2", "init dll finish");
        if(mPlatform == null)
        {
        	mPlatform = PlatformSDK.getInstance(this);
        }

		if(mActivity == null)
		{
			mActivity = this;
		}

		XGPushConfig.enableDebug(this, true);

		Context context = getApplicationContext();
		XGPushManager.registerPush(context,
				new XGIOperateCallback() {
					@Override
					public void onSuccess(Object data, int flag) {
						Log.e("TPush", "注册成功,Token值为：" + data);
					}
					@Override
					public void onFail(Object data, int errCode, String msg) {
						Log.e("TPush", "注册失败,错误码为：" + errCode + ",错误信息：" + msg);
					}
				});
	}
	
	@Override
	protected void onResume() {
		super.onResume();
		mPlatform.onResume();
	}
	
	@Override
	protected void onPause() {
		super.onPause();
		mPlatform.onPause();
	}
	
	 @Override
	 protected void onNewIntent(Intent intent)
	 {
		 super.onNewIntent(intent);;
		 mPlatform.onNewIntent(intent);
	 }
	 
	 @Override
	 protected void onDestroy()
	 {
		 mPlatform.onDestroy();
		 super.onDestroy();
	 }
	 
	 @Override
	 protected void onStart() 
	 {
		 super.onStart();
		 mPlatform.onStart();
	 }
	 
	 @Override
	 protected void onStop()
	 {
		super.onStop();
		mPlatform.onStop();
	 }
	
	public static void Login(String strLogin)
	{
		mPlatform.Login(strLogin);
	}

	public static void Restore(String strSandBox)
	{
		return;
	}

	public static void RestoreProductID(String strPID)
	{
		return;
	}

	public static void Pay(String strOrder, String strJson, String strCustom, String strParam, String strServerParam)
	{
		mPlatform.Pay(strOrder, strJson, strCustom, strParam, strServerParam);
	}

	public  static void PayFinishSuc(String strProductID, String strSerialnumber)
	{
		return;
	}

	public static String GetDeviceID()
	{
		TelephonyManager TelephonyMgr = (TelephonyManager) mActivity.getSystemService(TELEPHONY_SERVICE);
		String szImei = TelephonyMgr.getDeviceId();
		return szImei;
	}

	public static String GetAppConfig()
	{
		String strConfig = mPlatform.GetAppConfig();

		return strConfig;
	}
	
	public static void Logout()
	{
		mPlatform.Logout();
	}
	
	public static void CreatRole(String strJson)
	{
		mPlatform.CreatRole(strJson);
	}
	
	public static void EnterGame(String strJson)
	{
		mPlatform.EnterGame(strJson);
	}
	
	public static void GameUpdeteLevel(String strJson)
	{
		mPlatform.GameUpdeteLevel(strJson);
	}
	
	public static void GameUpdateRoleName(String strJson)
	{
		mPlatform.GameUpdateRoleName(strJson);
	}
	
	public static void ExitGame()
	{
		mPlatform.ExitGame();
	}
}
