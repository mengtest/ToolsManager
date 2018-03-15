package com.ezfun.webview;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.content.Context;
import android.util.Log;
import android.webkit.JavascriptInterface;
import android.widget.Toast;

public class WebViewInterfaceForJS {

	private Context mContext;
	private UnityWebViewClient mWebViewClient;

	public WebViewInterfaceForJS(Context context, UnityWebViewClient webViewClient){
		this.mContext = context;
		this.mWebViewClient = webViewClient;
	}
	
	//webview�е���toastԭ�����
	public void showToast(String toast) {
		Toast.makeText(mContext, toast, Toast.LENGTH_LONG).show();
	}
	
	//��jsonʵ��webview��js֮������ݽ���
	@JavascriptInterface
	public void JSCallBack(String json_str){
		//showToast(json_str);
		mWebViewClient.JSCallBack(json_str);
	}
	
}

