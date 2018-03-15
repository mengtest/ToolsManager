package com.ezfun.main_activity;

import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;

import com.ezfun.files.FileLoad;
import com.ezfun.webview.MyWebChromeClient;
import com.unity3d.player.UnityPlayerNativeActivity;

public class EZfunMainActivity extends UnityPlayerNativeActivity {
	
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		Log.d("xgame2", "beginnnnn");
		FileLoad load = new FileLoad();
		Log.d("xgame2", "load dll finish");
		load.setAssetmanager(getResources().getAssets());
		Log.d("xgame2", "init dll finish");
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
	}
}
