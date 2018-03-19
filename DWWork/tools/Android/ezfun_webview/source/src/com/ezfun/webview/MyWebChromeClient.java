package com.ezfun.webview;


import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.webkit.ValueCallback;
import android.webkit.WebChromeClient;

public class MyWebChromeClient extends WebChromeClient {
	public Activity actvitiy;
	public static ValueCallback<Uri> mUploadMessage;
	//webView中支持input的file现选择 
	 // For Android 3.0+ 
    @SuppressLint("NewApi") public void openFileChooser(ValueCallback<Uri> uploadMsg, String acceptType) 
    {   
        if (mUploadMessage != null) return; 
        mUploadMessage = uploadMsg;
        Intent i = new Intent(Intent.ACTION_GET_CONTENT); 
        i.addCategory(Intent.CATEGORY_OPENABLE); 
        i.setType("*/*"); 
//        startActivityForResult();
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
