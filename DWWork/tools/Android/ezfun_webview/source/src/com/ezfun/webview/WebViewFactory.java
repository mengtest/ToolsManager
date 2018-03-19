package com.ezfun.webview;


import android.app.Activity;
import android.content.Context;
import android.view.Gravity;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup.LayoutParams;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebSettings.PluginState;
import android.widget.FrameLayout;

public class WebViewFactory {
	
	public static WebView Create( Activity actvitiy, final String gameObject )
	{
		
		// Create WebView Instance
		WebView webview = new WebView( actvitiy );
		
		//set full screen
		final FrameLayout.LayoutParams params = new FrameLayout.LayoutParams(
				LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT,
					Gravity.NO_GRAVITY);
		params.setMargins(0, 0, 0, 0);
		webview.setLayoutParams(params);
		
		// Set WebView Optimize;
		webview.setVisibility( View.GONE );
		webview.setFocusable( true );
		webview.setFocusableInTouchMode( true );
		
		//set WebChormeClient
		MyWebChromeClient chorme_client = new MyWebChromeClient();
		chorme_client.actvitiy = actvitiy;
		webview.setWebChromeClient(chorme_client);
		
		// set Custom WebView Client
		UnityWebViewClient client = new UnityWebViewClient();
		client.setGameObject( gameObject );
		webview.setWebViewClient( client );
		
		// Init WebSettings
		WebSettings webSettings = webview.getSettings();
		webSettings.setSupportZoom(false);
		webSettings.setJavaScriptEnabled(true);
		
		//cache mode
		webSettings.setAppCacheEnabled(true);
		webSettings.setDomStorageEnabled(true);
		webSettings.setCacheMode(WebSettings.LOAD_DEFAULT);
		
		//js open html
		webSettings.setJavaScriptCanOpenWindowsAutomatically(true);
		
		//set WebViewInterfaceForJS
		//Context context = actvitiy.getApplicationContext();
		//webview.addJavascriptInterface(new WebViewInterfaceForJS(context, client), "JavaBridge");  
		
		webSettings.setPluginState( PluginState.ON );
		
		webview.setOnTouchListener(new View.OnTouchListener() {
			
			@Override
			public boolean onTouch(View v, MotionEvent event) {
	            switch (event.getAction()) {
	            case MotionEvent.ACTION_DOWN:
	            case MotionEvent.ACTION_UP:
	                if (!v.hasFocus()) {
	                    v.requestFocusFromTouch();
	                }
	                break;
	            }

				return false;
			}
		});

		return webview;
		
	}
	
	
}
