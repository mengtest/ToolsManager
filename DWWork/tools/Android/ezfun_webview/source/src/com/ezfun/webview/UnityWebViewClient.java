package com.ezfun.webview;

import com.unity3d.player.UnityPlayer;

import android.graphics.Bitmap;
import android.util.Log;
import android.webkit.WebView;
import android.webkit.WebViewClient;

import java.io.UnsupportedEncodingException;
import java.net.URLDecoder;

public class UnityWebViewClient extends WebViewClient
{

	private String mGameObject;
	
	public void setGameObject( String gameObject )
	{
		mGameObject = gameObject;
	}
	
	@Override
	public boolean shouldOverrideUrlLoading(WebView view, String url)
	{
		Log.d("EZFun", " shouldOverrideUrlLoading url:"+url);
		String url_str = "";
		try {
			url_str = URLDecoder.decode(url, "UTF-8");
		} catch (UnsupportedEncodingException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		Log.d("EZFun", " shouldOverrideUrlLoading url_str:"+url_str);
		if (url_str.contains("\"to\""))
		{
			Log.d("EZFun", " shouldOverrideUrlLoading not load");
			UnityPlayer.UnitySendMessage( mGameObject, "JSCallBack", url_str );
			
			return true;
		}
		
		Log.d("EZFun", " shouldOverrideUrlLoading load");
		view.loadUrl(url);
		return true;
	}

	@Override
	public void onPageStarted( WebView view, String url, Bitmap favicon)
	{
		Log.d("EZFun", " onPageStarted url:"+url);
		UnityPlayer.UnitySendMessage( mGameObject , "onLoadStart",url );
	}

	@Override
	public void onPageFinished( WebView view, String url )
	{
		Log.d("EZFun", " onPageFinished url:"+url);
		UnityPlayer.UnitySendMessage( mGameObject, "onLoadFinish", url );
	}

	@Override
	public void onReceivedError( WebView view, int errorCode, String desc, String url )
	{
		Log.d("EZFun", " onReceivedError url:"+url);
		UnityPlayer.UnitySendMessage( mGameObject, "onLoadFail", url );
	}
	
	public void JSCallBack(String json_str)
	{
		UnityPlayer.UnitySendMessage( mGameObject, "JSCallBack", json_str );
	}
}
