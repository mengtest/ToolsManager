using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void OnMemoryWarningHandler();

public class MemoryWarnReporter : MonoBehaviour {

	private const string GAMEOBJ_NAME = "MemoryWarnReporter";

	private static MemoryWarnReporter m_ins;
	public delegate void OnMemoryWarning ();
	private static bool m_inited;
	private static HashSet<OnMemoryWarningHandler> m_set;

	public static void Setup()
	{
		var obj = new GameObject (GAMEOBJ_NAME);
		GameObject.DontDestroyOnLoad (obj);

		m_ins = obj.AddComponent<MemoryWarnReporter> ();
		m_set = new HashSet<OnMemoryWarningHandler> ();

		if (Application.platform == RuntimePlatform.Android) {
			SetupAndroid();
		}

	}

	#region Android Related
	private static AndroidJavaClass _class;
	AndroidJavaObject andrObjInstance { get { return _class.GetStatic<AndroidJavaObject>("instance"); } }
	private static void SetupAndroid()
	{
		_class = new AndroidJavaClass("cn.ezfun.memorywarningplugin.MemoryWarningReporter");

		_class.CallStatic("init", GAMEOBJ_NAME);
	}
	#endregion

	public static void Register(OnMemoryWarningHandler handler)
	{
		if (m_set == null) {
			Debug.LogError ("m_set is null!");
			return;
		}

		if (!m_set.Contains (handler)) {
			m_set.Add (handler);
		}

	}

	public static void UnRegister(OnMemoryWarningHandler handler)
	{
		if (m_set == null) {
			Debug.LogError ("m_set is null!");
			return;
		}

		if (m_set.Contains(handler))
		{
			m_set.Remove (handler);
		}
	}

	private void OnReceiveMemoryWarning()
	{
		Debug.Log("OnReceiveMemoryWarning");

		if (m_set != null && m_set.Count > 0) {

			var ite = m_set.GetEnumerator ();
			while (ite.MoveNext ()) {
				ite.Current ();
			}

		}

		UnloadResourcesAndGC ();
	}

	private float m_previousCleanTime;
	private const float MIN_CLEAN_INTERVAL = 15f;

	private void UnloadResourcesAndGC()
	{
		if (Time.realtimeSinceStartup - m_previousCleanTime > MIN_CLEAN_INTERVAL) {
			m_previousCleanTime = Time.realtimeSinceStartup;

			Resources.UnloadUnusedAssets ();
			System.GC.Collect ();
		}
	}

	public static void SimulateMemoryWarning()
	{
		Debug.Log ("Simulate Memory Warning");
		m_ins.OnReceiveMemoryWarning();
	}

	public void _OnMemoryWarningIOS(string level)
	{
		OnReceiveMemoryWarning ();
	}

	public void _OnMemoryWarningAndroid(string level)
	{
		OnReceiveMemoryWarning ();
	}

}
