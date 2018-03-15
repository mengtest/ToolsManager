package com.ezfun.xgamenet;


import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.view.View;
import android.webkit.JsResult;
import android.webkit.ValueCallback;
import android.webkit.WebChromeClient;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebSettings.PluginState;



public class WebViewFactory {

	
	public static class MyWebChromeClient extends WebChromeClient
	{
		public UnityEzfunActivity actvitiy;
		public ValueCallback<Uri> mUploadMessage;
		//webView中支持input的file现选择 
		 // For Android 3.0+ 
	    @SuppressLint("NewApi") public void openFileChooser(ValueCallback<Uri> uploadMsg, String acceptType) 
	    {   
	        if (actvitiy.mUploadMessage != null) return; 
	        actvitiy.mUploadMessage = uploadMsg;
	        Intent i = new Intent(Intent.ACTION_GET_CONTENT); 
	        i.addCategory(Intent.CATEGORY_OPENABLE); 
	        i.setType("*/*"); 
//	        startActivityForResult();
	        actvitiy.startActivityForResult( Intent.createChooser( i, "File Chooser" ), 1 ); 
	    } 
	    // For Android < 3.0 
	    public void openFileChooser(ValueCallback<Uri> uploadMsg) 
	    { 
	        openFileChooser( uploadMsg, "" ); 
	    } 
	    // For Android  > 4.1.1 
	    public void openFileChooser(ValueCallback<Uri> uploadMsg, String acceptType, String capture) 
	    { 
	        openFileChooser(uploadMsg, acceptType); 
	    } 

	}

	
	public static WebView Create( Activity actvitiy, final String gameObject )
	{
		
		// Create WebView Instance
		WebView webview = new WebView( actvitiy );
		
		// Set WebView Optimize;
		webview.setVisibility( View.GONE );
		webview.setFocusable( true );
		webview.setFocusableInTouchMode( true );
		MyWebChromeClient chorme_client = new MyWebChromeClient();
		chorme_client.actvitiy = (UnityEzfunActivity)actvitiy;
		webview.setWebChromeClient(chorme_client);
		
		// set Custom WebView Client
		UnityWebViewClient client = new UnityWebViewClient();
		client.setGameObject( gameObject );
		webview.setWebViewClient( client );
		
		/*
		webview.addJavascriptInterface(
			new WebViewPluginInterface(gameObject), "Unity");
		*/

		// Init WebSettings
		WebSettings webSettings = webview.getSettings();
		webSettings.setSupportZoom(false);
		webSettings.setJavaScriptEnabled(true);
		webSettings.setPluginState( PluginState.ON );
		

		return webview;
		
	}
	
	
//	//webView中支持input的file现选择 
//	 // For Android 3.0+ 
//    public void openFileChooser(ValueCallback<Uri> uploadMsg, String acceptType) 
//    {   
//        if (mUploadMessage != null) return; 
//        mUploadMessage = uploadMsg;    
//        Intent i = new Intent(Intent.ACTION_GET_CONTENT); 
//        i.addCategory(Intent.CATEGORY_OPENABLE); 
//        i.setType("*/*"); 
//        startActivityForResult( Intent.createChooser( i, "File Chooser" ), Util.FILECHOOSER_RESULTCODE ); 
//    } 
//    // For Android < 3.0 
//    public void openFileChooser(ValueCallback<Uri> uploadMsg) 
//    { 
//        openFileChooser( uploadMsg, "" ); 
//    } 
//    // For Android  > 4.1.1 
//    public void openFileChooser(ValueCallback<Uri> uploadMsg, String acceptType, String capture) 
//    { 
//        openFileChooser(uploadMsg, acceptType); 
//    } 
//    @Override   
//    protected   void  onActivityResult( int  requestCode,  int  resultCode,Intent intent) 
//    {  
//        if (requestCode==FILECHOOSER_RESULTCODE)  
//        {  
//            if  ( null  == mUploadMessage)  return ;  
//            Uri result = intent ==  null  || resultCode != RESULT_OK ?  null : intent.getData();  
//            mUploadMessage.onReceiveValue(result);  
//            mUploadMessage =  null ;  
//        }  
//    }
}
