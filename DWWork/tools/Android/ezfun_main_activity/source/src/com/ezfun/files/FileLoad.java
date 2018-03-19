package com.ezfun.files;

import android.content.res.AssetManager;

public class FileLoad {

	public native void  setAssetmanager(AssetManager assetMgr);
	
	static
	{
	  System.loadLibrary("XGame2File");
	}
}
