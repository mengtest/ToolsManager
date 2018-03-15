using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandleCoverWindow : WindowRoot 
{
	public delegate void CB();
	public static bool m_hasInitDone = true;
	private static Dictionary<int, CB> m_cbDic = new Dictionary<int, CB>();
	private static int m_currentSeq = 0;
    public CB closeWindowCB;

	public static CB m_openCB
	{
		set
		{
			if(value == null)
			{
				if(m_cbDic.ContainsKey(m_currentSeq))
				{
					m_cbDic.Remove(m_currentSeq);
				}
			}
			else
			{
				m_currentSeq ++;
				m_cbDic.Add(m_currentSeq, value);
			}	
		}
		get
		{
			if(m_cbDic.ContainsKey(m_currentSeq))
			{
				return m_cbDic[m_currentSeq];
			}
			else
			{
				return null;
			}
		}
	}


	TweenAlpha m_tweenAlpha;
	public override void InitWindow (bool open = true, int state = 0)
	{
		InitCamera(open, false, (int)SpecialWindowDepthEnum.SWE_LoadingWindow);
		base.InitWindow (open, state);

        closeWindowCB = CloseWindow;

        if (open)
        {
            OpenWindow();
        }
	}

	protected override void CreateWindow ()
	{
		base.CreateWindow ();

		m_tweenAlpha = GetTrans("background").GetComponent<TweenAlpha>();
	}

	//检测是否还有没有调用的回调
	private void CheckHasNeedCallCB()
	{
		int findKey = -1;
		foreach(var kv in m_cbDic)
		{
			if(kv.Key != m_currentSeq)
			{
				findKey = kv.Key;
				break;
			}
		}

		if(findKey != -1)
		{
			CB cb = m_cbDic[findKey];
			m_cbDic.Remove(findKey);
			try
			{
				cb();
			}
			catch (System.Exception e)
			{
				CNetSys.Instance.SendImportantLog("CoverErroer:" + e.ToString());
			}
			CheckHasNeedCallCB();
		}
	}

    void AlwaysOpenWindow()
    {
        CheckHasNeedCallCB();
        m_tweenAlpha.PlayForward();


        TimerSys.Instance.AddFrameEvtByLeftFrame(() =>
        {
            if (m_openCB != null)
            {
                try
                {
                    m_openCB();
                }
                catch (System.Exception e)
                {
                    CNetSys.Instance.SendImportantLog("CoverErroer:" + e.ToString());
                }
            }
            m_openCB = null;
        }, 10);
    }

	void OpenWindow()
	{
		CheckHasNeedCallCB();
		m_tweenAlpha.PlayForward();

		TimerSys.Instance.AddFrameEvtByLeftFrame(()=>{
			if(m_openCB != null)
			{
				try
				{
					m_openCB();
				}
				catch (System.Exception e)
				{
					CNetSys.Instance.SendImportantLog("CoverErroer:" + e.ToString());
				}
			}
			m_openCB = null;
            if(CheckCanClose())
            {
                //CloseWindow();
                closeWindowCB();
				//TimerSys.Instance.AddTimerEventByLeftTime(()=>{
				//	EventSys.Instance.AddEventNow(EEventType.UI_Msg_HideJoysticButton,true);
				//},0.1f);
            }
            else
            {
                StartCoroutine(CloseWindowCoroutine());
            }
		}, 10);
	}

	private bool CheckCanClose()
	{
		return m_hasInitDone;
	}

	IEnumerator CloseWindowCoroutine()
	{
		while(!m_hasInitDone)
		{
			yield return null;
		}

		//CloseWindow();
        closeWindowCB();
	}

	void CloseWindow()
	{
		m_tweenAlpha.PlayReverse();

		TimerSys.Instance.AddFrameEvtByLeftFrame(()=>{
			InitWindow(false);
		},10);
	}

    protected override void HandleWidgetClick(GameObject gb)
    {
        if (!CheckBtnCanClick(gb))
        {
            return;
        }
        base.HandleWidgetClick(gb);
        switch (gb.name)
        {
            case "cover":
                OnCoverClicked();
                break;
            default:
                break;
        }
    }

    private void OnCoverClicked()
    {
        var bg = GetTrans("background");
        var txt = bg.GetComponent<UITexture>();
        if (txt.color.a <= 0.0001F)
        {
            InitWindow(false);
        }
    }
}
