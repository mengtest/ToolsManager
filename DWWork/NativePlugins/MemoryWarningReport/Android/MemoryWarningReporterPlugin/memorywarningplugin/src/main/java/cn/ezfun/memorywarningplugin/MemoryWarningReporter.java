package cn.ezfun.memorywarningplugin;

import android.app.Fragment;
import android.os.Bundle;

import com.unity3d.player.UnityPlayer;

/**
 * Created by xclouder on 2017/8/1.
 */

public class MemoryWarningReporter extends Fragment {

    // Constants.
    public static final String TAG = "Ezfun_MemoryWarningReporter";
    // Singleton instance.
    public static MemoryWarningReporter instance;

    // Unity context.
    String gameObjectName;
    public static void init(String gameObjectName)
    {
        // Instantiate and add to Unity Player Activity.
        instance = new MemoryWarningReporter();
        instance.gameObjectName = gameObjectName; // Store 'GameObject' reference
        UnityPlayer.currentActivity.getFragmentManager().beginTransaction().add(instance, MemoryWarningReporter.TAG).commit();
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setRetainInstance(true); // Retain between configuration changes (like device rotation)
    }

    @Override
    public void onTrimMemory(int level) {
        super.onTrimMemory(level);

        //report memory warning
        UnityPlayer.UnitySendMessage(gameObjectName, "_OnMemoryWarningAndroid", String.valueOf(level));
    }
}
