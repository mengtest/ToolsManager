using UnityEngine;
using System.Collections;

[RegisterSystemAttribute(typeof(UserIdleDetectSys))]
public class UserIdleDetectSys : TCoreSystem<UserIdleDetectSys>, IUpdateable {

	private bool m_isInited;

	#region IInitializeable implementation

	public void Init ()
	{
		if (!m_isInited) {
			m_isInited = true;
			BrightnessAdjuster.Setup ();
		}

		m_lastActiveSinceGameStart = Time.realtimeSinceStartup;
	}

	public void Release ()
	{
		
	}

	#endregion

	#region IUpdateable implementation

	private float m_lastActiveSinceGameStart = 0f;

	private bool m_isInIdleState;

	public bool IsInIdleState
	{
		get {
			return m_isInIdleState;
		}
	}

	private float m_originBrightness;

	//60s不动就认为进入闲置状态
	private const float TIME_TO_ENTER_IDLE_STATE = 600f;

	private const int IDLE_FRAME_RATE = 16;

	private const float IDLE_BRIGHTNESS = 0.1f;
	private const float NORMAL_BRIGHTNESS = 1f;

	private int m_originFrameRate;

	public void Update ()
	{
		//if (!m_isInited || Application.isEditor)
		if (!m_isInited)
        {
			return;
		}

		bool isActiveThisFrame = Input.GetMouseButton (0);

		if (m_isInIdleState) {

			if (isActiveThisFrame) {
				SetIdleState (false);
			}

		} else {

			if (Time.realtimeSinceStartup - m_lastActiveSinceGameStart > TIME_TO_ENTER_IDLE_STATE) {
				//enter idle state
				SetIdleState(true);
			}

		}

		if (isActiveThisFrame) {
			m_lastActiveSinceGameStart = Time.realtimeSinceStartup;
		}
	}

	private void SetIdleState(bool state)
	{
		m_isInIdleState = state;

		if (m_isInIdleState) {

			Debug.Log ("Enter User Idle State, set targetFrameRate to:" + IDLE_FRAME_RATE);
			m_originFrameRate = Application.targetFrameRate;
			Application.targetFrameRate = IDLE_FRAME_RATE;

			m_originBrightness = BrightnessAdjuster.Instance.GetCurrentBrightess ();

			BrightnessAdjuster.Instance.StopAutoBrightness ();
			BrightnessAdjuster.Instance.SetBrightness (IDLE_BRIGHTNESS);

		} else {

			Debug.Log ("Exit User Idle State, set targetFrameRate back to:" + m_originFrameRate);
			Application.targetFrameRate = m_originFrameRate;

			BrightnessAdjuster.Instance.StopAutoBrightness ();
			BrightnessAdjuster.Instance.SetBrightness (m_originBrightness);

            ShowFPS.Reset();
        }
	}

	#endregion

//	private void SetApplicationBrightnessTo(float Brightness)
//	{
//		AndroidJavaObject Activity = null;
//		Activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
//		Activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
//			AndroidJavaObject Window = null, Attributes = null;
//			Window = Activity.Call<AndroidJavaObject>("getWindow");
//			Attributes = Window.Call<AndroidJavaObject>("getAttributes"); 
//			Attributes.Set("screenBrightness", Brightness);
//
//			var originBrightness = Attributes.Get<int>("screenBrightness");
//
//
//			Window.Call("setAttributes", Attributes);
//		}));
//	}


}
