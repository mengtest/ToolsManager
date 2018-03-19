using UnityEngine;
using System.Collections;

[LuaWrap]
public enum EGameStateType
{
	StateNone,
	LoginState,
	LoadingState,
	MainCityState,
    GameState,
}

public class BaseGameState
{
	protected static float m_progressValue = 0f;
	public static float ProgressValue{get{return m_progressValue;}}
    protected bool m_changeProgress = true;

	public virtual void Init() {}
    public void BaseReset() { Reset(); }
    public virtual void Reset() { }
	public virtual void Enter()
	{
        if (m_changeProgress)
        {
            m_progressValue = 1f;
        }
        ShowFPS.m_openLowFPSCheck = true;
        EventSys.Instance.AddEvent(EEventType.EnterStateFinish, 
            GetStateType());
	}

	public virtual void Leave()
	{
        LevelAgainLeave();
		
		Resources.UnloadUnusedAssets();
		//SoundManager.Instance.UnloadSounds();
		System.GC.Collect();
		Time.timeScale = 1;
		m_progressValue = 0f;
	}

    protected virtual void LevelAgainLeave()
    {
        EZFunWindowMgr.Instance.ClearWindow();
        
		//相当之坑，ResourceMgr竟然会武断的销毁正在使用的资源
		ResourceMgr.Clear();

        m_progressValue = 0f;
    }

	public virtual void GameUpdate() {}
	public virtual EGameStateType GetStateType() { return EGameStateType.StateNone; }
	public string GetName()
	{
		return GetType().Name;
	}
}
