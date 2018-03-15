/************************************************************
//     文件名      : GameStateMgr.cs
//     功能描述    : 游戏状态管理
//     负责人      : blackzhou   blackzhou@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2016-08-16 10:30.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using UnityEngine.SceneManagement;
using System.IO;

[RegisterSystem(typeof(GameStateMgr), true)]
public class GameStateMgr : TCoreSystem<GameStateMgr>, IInitializeable, IUpdateable
{
    private List<BaseGameState> m_gameStateList = new List<BaseGameState>();
    public static bool isLoaded = false;
    private EGameStateType m_curGameState = EGameStateType.StateNone;
    private EGameStateType m_willGameState = EGameStateType.StateNone;
    private BaseGameState m_leaveState;
    public string m_sceneName = null;
    private bool m_isLoadSceneCompelete = false;
    private PreloadState m_preloadState;


    public void Init()
    {
        BaseGameState gameState = CreateGameState<LoadingState>();
        gameState.Init();
        m_gameStateList.Add(gameState);
    }

    public void Release()
    {
        m_gameStateList.Clear();
    }

    public void Update()
    {
        if (m_isLoadSceneCompelete)
        {
#if !UNITY_EDITOR
             try
            {
#endif
            GameRoot.Instance.StartCoroutine(ChangeStateForce(m_willGameState, true));
            m_isLoadSceneCompelete = false;
#if !UNITY_EDITOR
            }
             catch (System.Exception ex)
             {
                 NetLog.LogException(ex);
             }
#endif
        }

        if (m_curGameState != EGameStateType.StateNone)
        {
#if !UNITY_EDITOR
            try
            {
#endif
            BaseGameState gameState = GetState(m_curGameState);
            if (gameState != null)
                gameState.GameUpdate();
#if !UNITY_EDITOR
            }
            catch (System.Exception ex)
            {
                NetLog.LogException(ex);
            }
#endif
        }
    }

    public void SetLoadSceneCompelete()
    {
        m_isLoadSceneCompelete = true;
    }

    public bool GetIsLoadSceneComplete()
    {
        return m_isLoadSceneCompelete;
    }

    private static BaseGameState CreateGameState<T>() where T : BaseGameState, new()
    {
        return new T();
    }

    #region crash reprot
    void ReportIfCrashed()
    {
        //if (CrashReport.reports != null && CrashReport.reports.Length > 0)
        //{
        //    if (CrashReport.reports.Length > 1)
        //    {
        //        Debug.LogError("[CRASH REPROT] normally there should be only one reprot!!!");
        //    }

        //    if (CrashReport.reports.Length == 1)
        //    {
        //        Debug.LogError("[CRASH REPROT] " + CrashReport.reports[0].text);

        //        CSPackageBody body = new CSPackageBody();
        //        body.crashUpdateReq = new CSCrashUpdateReq();

        //        //server id
        //        body.crashUpdateReq.serverId = CNetSys.Instance.getCurZoneNode().sid;
        //        //crash time
        //        DateTime original_date = new DateTime(1970, 1, 1);
        //        TimeSpan time_span = CrashReport.reports[0].time - original_date;
        //        body.crashUpdateReq.time = (int)time_span.TotalSeconds;
        //        //game version
        //        body.crashUpdateReq.gameVersion = Version.Instance.GetVersion(VersionType.App);
        //        //source version
        //        body.crashUpdateReq.sourceVersion = Version.Instance.GetVersion(VersionType.Resource);
        //        //os version
        //        body.crashUpdateReq.OsVersion = "fix";
        //        //mobile brand
        //        body.crashUpdateReq.mobileBrand = "fix";
        //        //mobile model
        //        body.crashUpdateReq.mobileModel = "fix";
        //        //last stack
        //        body.crashUpdateReq.lastStack = CrashReport.reports[0].text.Split('\n')[0];
        //        //all stack
        //        body.crashUpdateReq.OsVersion = CrashReport.reports[0].text;

        //        CNetSys.Instance.SendNetMsg(ezfun.CS_CMD.CS_LOGIC_CMD_CRASH_UPDATE, body, (ezfun.SCPackage msg) =>
        //        {
        //            if (msg.head.errno == (int)ezfun.CS_ERRNO.COMMON_SUCCESS)
        //            {
        //                Debug.LogError("recv crash success!!!");
        //            }
        //        });
        //    }

        //    CrashReport.RemoveAll();
        //}
    }
    #endregion

    public void StartLoadSys()
    {
        GameRoot.StartMonoCoroutine(StartLoad());
    }

    IEnumerator StartLoad()
    {
        // start system init	

        //crash report
        ReportIfCrashed();
        //get system sample
        //EventSys es = EventSys.Instance;
        // end system init

        EventSys es = EventSys.Instance;
        es.AddHandler(EEventType.GamePause, HandleEvent);
        es.AddHandler(EEventType.ChangeGameState, HandleEvent);
        es.AddHandler(EEventType.ChangeGameStateForce, HandleEventForce);
        // end system init
        //------------------------------------------------


        //------------------------------------------------
        // game state init
        m_gameStateList.Clear();
        //        yield return null;

        BaseGameState gameState = CreateGameState<LoginState>();
        gameState.Init();
        m_gameStateList.Add(gameState);

        gameState = CreateGameState<LoadingState>();
        gameState.Init();
        m_gameStateList.Add(gameState);

        gameState = CreateGameState<MainCityState>();
        gameState.Init();
        m_gameStateList.Add(gameState);

        gameState = CreateGameState<GameState>();
        gameState.Init();
        m_gameStateList.Add(gameState);
        // end game state init
        //------------------------------------------------
        //        yield return null;
        //------------------------------------------------
        // don't delete this in all game life.
        //PhysicalPower.CreateInstance().LoadDate();
        //------------------------------------------------

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        isLoaded = true;

        yield break;
    }

    public void PreLoadScene(string sceneName)
    {
        if (m_preloadState != null)
        {
            SceneManager.UnloadScene(m_preloadState.m_sceneName);
        }
        ResourceManager.Instance.LoadScene(sceneName, null, false);
        m_preloadState = new PreloadState();
        m_preloadState.m_sceneName = sceneName;
    }

    /// <summary>
    /// 这里只有新手副本用到
    /// </summary>
    /// <param name="sceneName"></param>
    private bool ActiveLoadedScene(string sceneName)
    {
        //已经加载完了
        if (m_preloadState != null && m_preloadState.m_sceneName == sceneName)
        {
            SceneManager.UnloadScene(SceneManager.GetActiveScene());
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(m_preloadState.m_sceneName));
            m_preloadState = null;
            return true;
        }
        else //压根没有 或者不是需要的
        if (m_preloadState == null || m_preloadState.m_sceneName != sceneName)
        {
            ResourceManager.Instance.LoadScene(sceneName, null);
            return true;
        }
        return false;
    }



    /// <summary>
    /// 重新进入当前状态,add by Atin
    /// </summary>
    public void ResetCurState()
    {
        if (m_curGameState == EGameStateType.StateNone)
        {
            return;
        }
        m_leaveState = GetState(m_curGameState);
        LeaveLastState();
        BaseGameState enterState = GetState(m_curGameState);
        if (enterState != null)
        {
            enterState.Enter();
        }
    }

    public IEnumerator ChangeStateForce(EGameStateType state, bool loadingDone = false)
    {
        if (m_curGameState != EGameStateType.StateNone)
        {
            m_leaveState = GetState(m_curGameState);
        }

        if (m_curGameState == EGameStateType.LoadingState) //out loading
        {
            m_curGameState = m_willGameState;
        }
        
        else
        {
            Debug.Log("ChangeStateForce "+state.ToString());

            m_willGameState = state;
            this.m_sceneName = GetSceneNameByState(state,false);

            LoadingState.m_loadSceneComplete = false;
            m_curGameState = EGameStateType.LoadingState;
            yield return null;
            if (m_willGameState != EGameStateType.LoginState || CNetSys.Instance.GameNetStateConnect.m_isForceOut)
            {
                EZFunWindowMgr.Instance.InitWindowDic(EZFunWindowEnum.loading_ui_window, RessType.RT_LoadingUI, RessStorgeType.RST_Always);
                EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.loading_ui_window, RessType.RT_LoadingUI, true, 0);
				CNetSys.Instance.GameNetStateConnect.m_isForceOut = false;
            }
        }

        BaseGameState enterState = GetState(m_curGameState);
        if (enterState != null)
        {
            enterState.Enter();
        }
    }

    public BaseGameState GetState(EGameStateType state)
    {
        for (int i = 0; i < m_gameStateList.Count; i++)
        {
            if (m_gameStateList[i].GetStateType() == state)
            {
                return m_gameStateList[i];
            }
        }
        return null;
    }

   

    public EGameStateType GetCurStateType()
    {
        return m_curGameState;
    }

    public EGameStateType GetWillStateType()
    {
        return m_willGameState;
    }

    public void LeaveLastState()
    {
        if (m_leaveState != null)
            m_leaveState.Leave();
    }

    public void HandleEvent(EEventType eventId, object param1, object param2)
    {
        if (eventId == EEventType.ChangeGameState)
        {
            EGameStateType gameState = (EGameStateType)param1;
            if (param2 is string)
            {
                m_sceneName = (string)param2;
            }
            else
                m_sceneName = GetSceneNameByState(gameState,true);

           GameRoot.Instance.StartCoroutine( ChangeStateForce(gameState));
        }

    }

    public void HandleEventForce(EEventType eventId, object param1, object param2)
    {
        if (eventId == EEventType.ChangeGameStateForce)
        {
            EGameStateType gameState = (EGameStateType)param1;
            m_sceneName = (string)param2;
            GameRoot.Instance.StartCoroutine(ChangeStateForce(gameState));
        }
    }

    public string GetCurSceneName()
    {
        return m_sceneName;
    }

    private void ResetGameStates()
    {
        for (int i = 0; i < m_gameStateList.Count; ++i)
        {
            m_gameStateList[i].BaseReset();
        }
    }

    private void HandleCutSceneEnd()
    {
    }


    public bool CheckNeedNet()
    {
        return true;
    }
    public string GetSceneNameByState(EGameStateType willType,bool forceInit = false)
    {
        var sceneName = "";
        string curSceneName = null;
        if (!forceInit)
        {
            curSceneName = GetCurSceneName();
        }
        switch (willType)
        {
            case EGameStateType.LoginState:
                sceneName = "Login";
                break;
            case EGameStateType.MainCityState:
                sceneName = "MainScene";
                break;
            case EGameStateType.GameState:
                if (curSceneName != null)
                    sceneName = curSceneName;
                else
                    sceneName = "GameScene";
                break;
            default:
                Debug.LogError("[no handle GameStateType]" + willType);
                break;
        }
        return sceneName;
    }

    private class PreloadState
    {
        public string m_sceneName;
    }
}
