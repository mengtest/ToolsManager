package com.ezfun.webview;

import com.unity3d.player.UnityPlayer;
import android.app.Activity;
import android.util.Log;
import android.view.Gravity;
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

	public void LoadURL(final String url)
	{
		final Activity a = UnityPlayer.currentActivity;
		
		a.runOnUiThread(new Runnable() {public void run() {
			Log.d("EZFun", " url:"+url);
			mWebView.loadUrl(url);

		}});
	}

	public void SetMargins(final int left, final int top, final int right, final int bottom)
	{
		final FrameLayout.LayoutParams params = new FrameLayout.LayoutParams(
			LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT,
				Gravity.NO_GRAVITY);
		params.setMargins(left, top, right, bottom);

		Activity a = UnityPlayer.currentActivity;
		a.runOnUiThread(new Runnable() {public void run() {
			Log.d("EZFun", " setMargins:"+left+","+top+","+right+","+bottom);
			mWebView.setLayoutParams(params);

		}});
	}

	public void SetVisibility(final boolean visibility)
	{
		Activity a = UnityPlayer.currentActivity;
		a.runOnUiThread(new Runnable() {public void run() {
			Log.d("EZFun", " SetVisibility:"+visibility);
			if (visibility) {
				mWebView.setVisibility(View.VISIBLE);
				layout.requestFocus();
				mWebView.requestFocus();
				//mWebView.loadUrl("javascript:self_adapt()");
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
