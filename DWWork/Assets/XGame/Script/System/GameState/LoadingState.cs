using UnityEngine;
using System.Collections;
using System.IO;
using ProtoBuf;

// loading system
public class LoadingState : BaseGameState
{
    private string m_loadSceneName = "";
    private static bool _m_loadSceneComplete = false;

    public static string m_configFileName = "11";
    public static bool IsNeedStopBg = false;

  

    public override void Enter()
    {
        m_loadSceneName = GameStateMgr.Instance.GetCurSceneName();
        EventSys.Instance.AddEventNow(EEventType.ChangeScene, m_loadSceneName);
        GameRoot.Instance.StartCoroutine(LoadSceneAsyn());
        //异步加载场景Loading时间太长，先改成同步加载。
        //if (GameStateMgr.Instance.GetWillStateType() != EGameStateType.LoginState)
        //    GameRoot.Instance.StartCoroutine(LoadSceneAsyn());
        //else
        //    GameRoot.Instance.StartCoroutine(LoadSceneDsync());
        m_changeProgress = false;
        if (IsNeedStopBg)
        {
            IsNeedStopBg = false;
            //SoundManager.Instance.StopBGMusic();
        }
        if (GameStateMgr.isLoaded)
        {
            base.Enter();
        }
    }



    public override EGameStateType GetStateType() { return EGameStateType.LoadingState; }


    public static bool m_loadSceneComplete
    {
        get
        {
            return _m_loadSceneComplete;
        }
        set
        {
            _m_loadSceneComplete = value;
        }
    }

    //private bool m_isChangeResolution = false;

    IEnumerator LoadSceneDsync()
    {
        m_progressValue = 0.5f;
        TimeProfiler.BeginTimer("LoadSceneDsync " + m_loadSceneName);
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    if (!m_isChangeResolution)
        //    {
        //        yield return new WaitForSeconds(0.1f);
        //        m_isChangeResolution = true;
        //    }
        //}
        while (!HandleLoadingWindow.m_inLoadingDone && GameStateMgr.Instance.GetWillStateType() != EGameStateType.LoginState)
        {
            yield return null;
        }
        if (!GameStateMgr.isLoaded)
        {
            GameStateMgr.Instance.StartLoadSys();
            while (!GameStateMgr.isLoaded)
            {
                yield return null;
            }
        }
        TimeProfiler.BeginTimer("Leave state");
        GameStateMgr.Instance.LeaveLastState();
        TimeProfiler.EndTimerAndLog("Leave state");

        m_progressValue = 1.0f;

        ResourceManager.Instance.LoadScene(m_loadSceneName, null);
        _m_loadSceneComplete = true;
        GameStateMgr.Instance.SetLoadSceneCompelete();
        TimeProfiler.EndTimerAndLog("LoadSceneDsync " + m_loadSceneName);
        m_progressValue = 0;
    }

	//用来过渡的场景，防止产生过高的内存峰值
	private const string EMPTY_SCENE_NAME = "Empty";
    IEnumerator LoadSceneAsyn()
    {
        TimeProfiler.BeginTimer("LoadSceneAsyn " + m_loadSceneName);
        while (!HandleLoadingWindow.m_inLoadingDone && GameStateMgr.Instance.GetWillStateType() != EGameStateType.LoginState)
        {
            yield return null;
        }

        AsyncOperation m_async = null;
		//过渡场景
        Debug.Log("Load Empty Scene" + EMPTY_SCENE_NAME);
        m_async = ResourceManager.Instance.LoadSceneAsync(EMPTY_SCENE_NAME, null);
		m_async.allowSceneActivation = true;

		while (!m_async.isDone)
		{
			yield return null;
		}
		yield return null;
        yield return null;
        m_async = null;
		Debug.Log("Load Target Scene" + m_loadSceneName);
        m_async = ResourceManager.Instance.LoadSceneAsync(m_loadSceneName, null);
        m_async.allowSceneActivation = true;

        GameStateMgr.Instance.LeaveLastState();

        while (!m_async.isDone)
        {
            m_progressValue = m_async.progress;
            yield return null;
        }
        if (!GameStateMgr.isLoaded)
        {
            GameStateMgr.Instance.StartLoadSys();
            while (!GameStateMgr.isLoaded)
            {
                yield return null;
            }
        }

        _m_loadSceneComplete = true;
        m_progressValue = 0;
        GameStateMgr.Instance.SetLoadSceneCompelete();
        TimeProfiler.EndTimerAndLog("LoadSceneAsyn " + m_loadSceneName);
    }

}