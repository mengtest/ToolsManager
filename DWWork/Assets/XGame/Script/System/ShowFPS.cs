using UnityEngine;
using System.Collections;

public class ShowFPS : MonoBehaviour {
	
	public float f_UpdateInterval = 0.5F;
	
	private float f_LastInterval;
	
	private int i_Frames = 0;
	
	private float f_Fps;

	private bool m_showFps = false;

    //需要控制的帧下限 add by lezen 2016/10/22
    private const float CONTROLL_FRAME_LIMIT = 15;
    
    private float m_checkInterval = 0f;
    //检查间隔
    private const float CHECK_INTERVAL = 1f;
    //当前累计检查次数
    public static int m_totalTime = 0;
    //通知玩家画质切换 间隔
    private const int NOTIFY_INTERVAL = 10;
    //累计低于控制帧的次数
    public static int m_lowTime = 0;
    private const float LOW_LIMIT = 7;

    public static bool m_openLowFPSCheck = false;
    void Start() 
	{
	
		f_LastInterval = Time.realtimeSinceStartup;
		
		i_Frames = 0;

		StartCoroutine(WaitForResolution());
    }

	IEnumerator WaitForResolution()
	{
		yield return new WaitForSeconds(0.1f);
		m_showFps = true;
	}
#if UNITY_EDITOR
    void OnGUI() 
	{
		if(Constants.RELEASE)
		{
			return;
		}

		if(!m_showFps)
		{
			return;
		}

		GUI.color = Color.white;
		GUI.skin.label .fontSize = 20;


		int type = PlayerPrefs.GetInt("resolution");
		if(type == 2)
		{
			GUI.Label(new Rect(0, Screen.height * 0.90f  , 200, 200), f_Fps.ToString("f2"));
		}
		else
		{
			GUI.Label(new Rect(0, Screen.height * 0.95f  , 200, 200), f_Fps.ToString("f2"));
		}
	}
#endif

    void Update() 
	{
		//if(Constants.RELEASE)
		//{
		//	return;
		//}

		if(!m_showFps)
		{
			return;
		}

		++i_Frames;

        if (Time.realtimeSinceStartup > f_LastInterval + f_UpdateInterval) 
		{
			f_Fps = i_Frames / (Time.realtimeSinceStartup - f_LastInterval);
			
			i_Frames = 0;
			
			f_LastInterval = Time.realtimeSinceStartup;
        }
    }

    public static void Reset()
    {
        m_lowTime = 0;
        m_totalTime = 0;
    }
}