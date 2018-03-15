/************************************************************
    File      : HandleErrorWindow.cs
    brief     :   
    author    : JanusLiu   janusliu@ezfun.cn
    version   : 1.0
    date      : 2014/11/11 15:21:12
    copyright : Copyright 2014 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;
using System.Text;

public class HandleErrorWindow : WindowRoot
{

    private bool m_hasInitEvtHandle = false;
    private int m_state;


    public static int m_windowDepth = (int)SpecialWindowDepthEnum.SWE_ErrorWindow;

    public delegate void CallBack(object p1 = null, object p2 = null);
    public static bool m_isOpen = false;
    public static CallBack m_okCallBack = null;
    public static CallBack m_noCallBack = null;
    public static CallBack m_cancelCallBack = null;

    public static string m_titleStr = null;
    public static string m_contentStr = null;
    public static string m_contentSecStr = null;
    public static bool m_contentLeftAlign = false;

    public static bool m_needCloseWindow = true;

    public static int m_iOKTimeLeft;
    public static int m_iCancelTimeLeft;

    private StringBuilder m_btnLabelText = new StringBuilder();

    Transform m_contentTypeTrans = null;
    Transform m_contentTrans = null;
    Transform m_contentSecTrans = null;

    Transform m_OKBtn;
    Transform m_YesBtn;
    Transform m_NoBtn;

    protected override void CreateWindow()
    {
        base.CreateWindow();
        m_OKBtn = GetTrans("btn_ok");
        m_YesBtn = GetTrans("btn_yes");
        m_NoBtn = GetTrans("btn_no");
        m_contentTrans = GetTrans("label_content");
        m_contentSecTrans = GetTrans("label_content_sec");
    }

    public override void InitWindow(bool open = true, int state = 0)
    {
        InitCamera(open, false, m_windowDepth);

        base.InitWindow(open, state);
        this.m_state = state;

        if (open)
        {
            switch (state)
            {
                case 0:
                    SetActive(m_YesBtn, false);
                    SetActive(m_NoBtn, false);
                    SetActive(m_OKBtn, true);
                    break;
                case 1:
                    SetActive(m_OKBtn, false);
                    SetActive(m_YesBtn, true);
                    SetActive(m_NoBtn, true);
                    break;
            }

            SetLabel(m_contentTrans, m_contentStr == null ? "" : m_contentStr);
            SetLabel(m_contentSecTrans, m_contentSecStr == null ? "" : m_contentSecStr);
        }

        InitEvtHandle();
    }

	protected override void OnWindowDidClose ()
	{
		base.OnWindowDidClose ();

		m_isOpen = false;
		m_iOKTimeLeft = 0;
		m_iCancelTimeLeft = 0;
	}

    void OnEnable()
    {
        m_isOpen = true;
    }

    protected override void HandleWidgetClick(GameObject gb)
    {
        if (!CheckBtnCanClick(gb))
        {
            return;
        }

        string clickName = gb.name;
        if (m_state != 3)
        {
            base.HandleWidgetClick(gb);
        }

        switch (clickName)
        {
            case "btn_yes":
            case "btn_ok":
                HandleOK();
                break;
            case "btn_no":
                HandleNO();
                break;
            default:
                break;
        }

    }

    public void SetContent(string str)
    {
        SetLabel(m_contentTrans, str);
    }

    private string TextAndTime(string text, int time)
    {
        if (time > 0)
        {
            return EZFunString.LinkString(text, "(", time.ToString(), ")");
        }
        else
        {
            return text;
        }
    }

    void HandleOK()
    {
        CallBack temp = m_okCallBack;
        if (m_needCloseWindow) 
        {
            ResetData();
            ResetCbk();
        }
         //ResetData();
         //ResetCbk();

        if (temp != null)
        {
            temp();
        }
    }

    void HandleNO()
    {
        ResetData();
        CallBack temp = m_noCallBack;
        ResetCbk();
        if (temp != null)
        {
            temp();
        }
    }

    void HandleCancel()
    {
        ResetData();
        CallBack temp = m_cancelCallBack;
        ResetCbk();
        if (temp != null)
        {
            temp();
        }
    }

    void ResetData()
    {
        m_windowDepth = (int)SpecialWindowDepthEnum.SWE_ErrorWindow;
        m_contentLeftAlign = false;
        m_titleStr = null;
        m_contentStr = null;
        m_contentSecStr = null;
        m_needCloseWindow = true;
        CloseWindow();
    }

    void ResetCbk()
    {
        m_okCallBack = null;
        m_noCallBack = null;
    }

    private void InitEvtHandle()
    {
        if (!m_hasInitEvtHandle && EventSys.Instance != null)
        {
            EventSys.Instance.AddHandler(EEventType.UI_Msg_RefreshErrorContent, (EEventType eventid, object p1, object p2) =>
            {
                SetLabel(m_contentTrans, m_contentStr == null ? "" : m_contentStr);
            });

            m_hasInitEvtHandle = true;
        }
    }

    private void CloseWindow()
    {
        InitWindow(false);
    }

    public static void set_m_okCallBack(CallBack okCBK)
    {
        m_okCallBack = okCBK;
    }

    public static void set_m_noCallBack(CallBack noCBK)
    {
        m_noCallBack = noCBK;
    }

	public override void OnDestroy()
    {
        m_noCallBack = null;
    }
}
