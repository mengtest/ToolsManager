package com.ezfun.main_activity;

import android.Manifest;
import android.app.Activity;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.res.Resources;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.util.Log;
import android.widget.ImageView;
import android.widget.ImageView.ScaleType;

import com.ezfun.files.FileLoad;
import com.ezfun.webview.MyWebChromeClient;
import com.tencent.bugly.crashreport.CrashReport;
import com.umeng.socialize.UMShareAPI;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerNativeActivity;

public class EZfunMainActivity extends UnityPlayerNativeActivity {
	
	private boolean isFirstStart = false;
	
	private ImageView bgView = null;  
	public static Activity mActivity = null;
	
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		Log.d("xgame2", "beginnnnn");
		FileLoad load = new FileLoad();
		Log.d("xgame2", "load dll finish");
		load.setAssetmanager(getResources().getAssets());
		Log.d("xgame2", "init dll finish");
		//checkPermission();
		
		CrashReport.initCrashReport(getApplicationContext(), "d0b5b1ce3c", false);
		Log.d("xgame2", "CrashReport initCrashReport finish");
		isFirstStart = false;
		
		mActivity = this;
		onShowSplash();
	}
	
	 public void onShowSplash() {
         if (bgView != null)  
                 return;  

         try {
        	 	 Resources r = mActivity.getResources();  
                 bgView = new ImageView(mActivity);  
                 int splash_bg = r.getIdentifier("splash", "drawable",  
                                                 UnityPlayer.currentActivity.getPackageName());  

                 bgView.setBackgroundResource(splash_bg);  
                 bgView.setScaleType(ScaleType.FIT_CENTER);  
                 mUnityPlayer.addView(bgView,r.getDisplayMetrics().widthPixels,  
                                 r.getDisplayMetrics().heightPixels);  

         } catch (Exception e) {  
        	 Log.e("xgame2", "onShowSplash error : " + e.toString());
         }  
	 }  
	 
	 public void onHideSplash() {  
         try {  
                 if(bgView == null)  
                         return;  

                 UnityPlayer.currentActivity.runOnUiThread(new Runnable() {  
                         public void run() {  
                                 mUnityPlayer.removeView(bgView);  
                                 bgView = null;  
                         }
                 });  

         } catch (Exception e) {  
        	 Log.e("xgame2", "onHideSplash error : " + e.toString());
         }  
	 }
	
	protected void onStart()
	{
		super.onStart();
		if(!isFirstStart)
		{
//			checkPermission();
			isFirstStart = true;
		}
	}
	
	//动态授权
	String[] permissions = new String[]{Manifest.permission.READ_PHONE_STATE,Manifest.permission.ACCESS_COARSE_LOCATION,Manifest.permission.ACCESS_FINE_LOCATION};
	public void checkPermission()
    {
		if (Build.VERSION.SDK_INT < 23)
		{
			return;
		}
		if(ContextCompat.checkSelfPermission(this, permissions[0]) == PackageManager.PERMISSION_GRANTED)
		{
			return;
		}
		ActivityCompat.requestPermissions(this, permissions, 1);
    }
	
	@Override
	protected void onActivityResult(int requestCode, int resultCode,
			Intent intent) {
		if (requestCode == 1) {
			if (null == MyWebChromeClient.mUploadMessage)
				return;
			Uri result = intent == null || resultCode != RESULT_OK ? null
					: intent.getData();
			MyWebChromeClient.mUploadMessage.onReceiveValue(result);
			MyWebChromeClient.mUploadMessage = null;
		}
		
	    super.onActivityResult(requestCode, resultCode, intent);
	    UMShareAPI.get(this).onActivityResult(requestCode, resultCode, intent);
	}
}
