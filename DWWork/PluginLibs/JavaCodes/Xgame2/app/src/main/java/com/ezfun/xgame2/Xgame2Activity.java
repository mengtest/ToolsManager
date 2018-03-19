package com.ezfun.xgame2;

//import com.unity3d.player.UnityPlayerActivity;
import java.net.*;
import java.io.*;
import android.os.Bundle;
import android.util.Log;

public class Xgame2Activity {
    static public void TestData()
    {
        Log.d("OverrideActivity", "end copy");
    }
    static public void CallTest(String srcPath, String destPath, String folderName)
    {
        Log.d("OverrideActivity", "start copy");
        long time = System.currentTimeMillis();
        try {
            final URL url = new URL(srcPath);
            FileUtils.copyResourcesRecursively(url, destPath, folderName);
        }
        catch (MalformedURLException e) {
            e.printStackTrace();
        }

        Log.d("OverrideActivity", "end copy");

        double deltaTime = (double)(System.currentTimeMillis() - time);
        deltaTime /= 1000.0;
        Log.d("OverrideActivity", "delta time is " + Double.toString(deltaTime));
    }
}