using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class BrightnessAdjuster : MonoBehaviour {

	private static BrightnessAdjuster m_ins;

	public static void Setup()
	{
		var o = new GameObject ("BrightnessAdjuster");
		m_ins = o.AddComponent<BrightnessAdjuster> ();

		GameObject.DontDestroyOnLoad (o);

		SetupNative ();
	}

	public static BrightnessAdjuster Instance
	{
		get {
			return m_ins;
		}
	}

	private static AndroidJavaClass m_andrPluginClass;
	private static void SetupNative()
	{
		if (Application.platform == RuntimePlatform.Android) {
			m_andrPluginClass = new AndroidJavaClass ("cn.ezfun.brightnessadjuster.BrightnessAdjuster");
		}
	}

	[DllImport ("__Internal")]
	private static extern void __IOS_SetBrightness (float brightness);

	[DllImport ("__Internal")]
	private static extern float __IOS_GetBrightness();

	public void StartAutoBrightness()
	{
		if (Application.platform == RuntimePlatform.Android) {
			//打开博主的博客  
			m_andrPluginClass.CallStatic("startAutoBrightness");
		}
	}

	public void StopAutoBrightness()
	{
		if (Application.platform == RuntimePlatform.Android) {
			//打开博主的博客  
			m_andrPluginClass.CallStatic("stopAutoBrightness");
		}
	}

	public void SetBrightness(float brightness)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			__IOS_SetBrightness (brightness);
		}

		if (Application.platform == RuntimePlatform.Android) {
			m_andrPluginClass.CallStatic("setBrightness", brightness);
		}
	}

	public float GetCurrentBrightess()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer) {

			return __IOS_GetBrightness();

		}

		if (Application.platform == RuntimePlatform.Android) {
			return m_andrPluginClass.CallStatic<int>("getScreenBrightness");
		}

		return 1.0f;
	}



}
