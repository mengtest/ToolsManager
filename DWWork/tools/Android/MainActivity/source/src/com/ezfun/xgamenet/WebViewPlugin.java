package com.ezfun.xgamenet;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;

import com.unity3d.player.UnityPlayer;
import android.app.Activity;
import android.util.Log;
import android.view.Gravity;
import android.view.KeyEvent;
import android.view.View;
import android.view.ViewGroup.LayoutParams;
import android.webkit.WebView;
import android.widget.FrameLayout;

public class WebViewPlugin
{
	private static FrameLayout layout = null;
	private WebView mWebView;

	public WebViewPlugin(){}

	public void Init(final String gameObject)
	{
		final Activity a = UnityPlayer.currentActivity;
		Log.d("EZFUN", a.toString() + a.getClass().toString());
		a.runOnUiThread(new Runnable() {public void run() {
			
			mWebView = WebViewFactory.Create( a, gameObject );
			
			if ( layout == null ) {
				layout = new FrameLayout(a);
				a.addContentView(layout, new LayoutParams(
					LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT));
				layout.setFocusable(true);
				layout.setFocusableInTouchMode(true);
			}

			layout.addView(mWebView, new FrameLayout.LayoutParams(
				LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT,
				Gravity.NO_GRAVITY));
			
		}});
	}

	public void Destroy()
	{
		Activity a = UnityPlayer.currentActivity;
		a.runOnUiThread(new Runnable() {public void run() {

			if (mWebView != null) {
				layout.removeView(mWebView);
				mWebView = null;
			}

		}});
	}

//	public static String toURLEncoded(String paramString) {
//		if (paramString == null || paramString.equals("")) {
//			Log.d("EZFun", "toURLEncoded error:"+paramString);
//			return "";
//		}
//		
//		try
//		{
//			//String str = new String(paramString.getBytes(), "UTF-8");
//			String str = URLEncoder.encode(paramString, "UTF-8");
//			return str;
//		}
//		catch (Exception localException)
//		{
//			Log.e("EZFun", "toURLEncoded error:"+paramString, localException);
//		}
//		
//		return "";
//	}
//	
	public void LoadURL(final String url)
	{
		final Activity a = UnityPlayer.currentActivity;
//		Log.d("EZFun", " url:"+url);
		
		a.runOnUiThread(new Runnable() {public void run() {

			mWebView.loadUrl(url);

		}});
	}

	public void SetMargins(int left, int top, int right, int bottom)
	{
		final FrameLayout.LayoutParams params = new FrameLayout.LayoutParams(
			LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT,
				Gravity.NO_GRAVITY);
		params.setMargins(left, top, right, bottom);

		Activity a = UnityPlayer.currentActivity;
		a.runOnUiThread(new Runnable() {public void run() {

			mWebView.setLayoutParams(params);

		}});
	}

	public void SetVisibility(final boolean visibility)
	{
		Activity a = UnityPlayer.currentActivity;
		a.runOnUiThread(new Runnable() {public void run() {

			if (visibility) {
				mWebView.setVisibility(View.VISIBLE);
				layout.requestFocus();
				mWebView.requestFocus();
			} else {
				mWebView.setVisibility(View.GONE);
			}

		}});
	}
	
	/*
	public void DispatchKeyEvent( int keyCode )
	{
		// とりあえずはDELキーだけ
		if( keyCode == 8 )
		{
			keyCode = KeyEvent.KEYCODE_DEL;
		}
		else return;

		final int _keyCode = keyCode;

		Activity a = UnityPlayer.currentActivity;
		a.runOnUiThread(new Runnable() {public void run() {

			KeyEvent _event = new KeyEvent( KeyEvent.ACTION_DOWN, _keyCode );
			mWebView.dispatchKeyEvent( _event );

		}});

	}*/
	
}
