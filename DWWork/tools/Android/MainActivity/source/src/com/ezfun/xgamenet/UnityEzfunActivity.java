package com.ezfun.xgamenet;

import com.unity3d.player.UnityPlayerNativeActivity;

import android.content.Intent;
import android.net.Uri;
import android.webkit.ValueCallback;

public class UnityEzfunActivity extends UnityPlayerNativeActivity
{
	public ValueCallback<Uri> mUploadMessage;
	@Override   
    protected   void  onActivityResult( int  requestCode,  int  resultCode,Intent intent) 
    {  
        if (requestCode==1)  
        {  
            if  ( null  == mUploadMessage)  return ;  
            Uri result = intent ==  null  || resultCode != RESULT_OK ?  null : intent.getData();  
            mUploadMessage.onReceiveValue(result);  
            mUploadMessage =  null ;  
        }  
    }
}
