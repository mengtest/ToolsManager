package cn.ezfun.brightnessadjuster;

import android.app.Activity;
import android.view.WindowManager;
import android.provider.Settings;
import android.util.Log;
import com.unity3d.player.UnityPlayer;
import android.content.ContentResolver;

/**
 * Created by xclouder on 2017/8/1.
 */

public class BrightnessAdjuster {

    public static void startAutoBrightness()
    {
        Activity activity = UnityPlayer.currentActivity;

        Settings.System.putInt(activity.getContentResolver(),
                Settings.System.SCREEN_BRIGHTNESS_MODE,
                Settings.System.SCREEN_BRIGHTNESS_MODE_AUTOMATIC);

    }

    public static void stopAutoBrightness()
    {
        Activity activity = UnityPlayer.currentActivity;

        Settings.System.putInt(activity.getContentResolver(),
                Settings.System.SCREEN_BRIGHTNESS_MODE,
                Settings.System.SCREEN_BRIGHTNESS_MODE_MANUAL);

    }

    public static void setBrightness(float brightness) {

        final float b = brightness;

        final Activity activity = UnityPlayer.currentActivity;

        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {

                WindowManager.LayoutParams lp = activity.getWindow().getAttributes();
                lp.screenBrightness = b;

                Log.d("brightnessadjust", "set lp.screenBrightness == " + lp.screenBrightness);
                activity.getWindow().setAttributes(lp);

            }
        });


    }

    public static int getScreenBrightness() {

        Activity activity = UnityPlayer.currentActivity;

        int nowBrightnessValue = 0;
        ContentResolver resolver = activity.getContentResolver();
        try{
            nowBrightnessValue = android.provider.Settings.System.getInt(resolver,Settings.System.SCREEN_BRIGHTNESS);
        } catch(Exception e) {
            e.printStackTrace();
        }
        return nowBrightnessValue;
    }

}
