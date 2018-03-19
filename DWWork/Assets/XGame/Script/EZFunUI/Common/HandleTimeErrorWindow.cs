/************************************************************
     File      : HandleTimeErrorWindow.cs
     author    : lezen   lezenzeng@ezfun.cn
     fuction   : 定时通用弹窗
     version   : 1.0
     date      : 4/5/2017.
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/
using UnityEngine;
using System.Collections;

public class HandleTimeErrorWindow : WindowRoot {

    private int m_state;
    public delegate void CallBack(object p1 = null, object p2 = null);
    public static CallBack m_okCallBack = null;
    public static CallBack m_noCallBack = null;
    public static CallBack m_timePerSecCB = null;
    public static CallBack m_timeOverCB = null;

    public static string m_curWindowName = "";
    public static bool m_canClickCollider = false;
    public static int m_countDown = 0;   
    public static bool m_needGrayOkBtn = false;
    public static bool m_autoCallOKFunc = false;
    public static bool m_autoCallNoFunc = false;

    public static int m_contentID = -1;
    public static int m_yesTextID = (int)EnumText.ET_OK;
    public static int m_noTextID = (int)EnumText.ET_NO;
    public static string m_titleStr = null;
    public static string m_contentStr = null;
    public static string m_okBtnStr = null;
    public static string m_noBtnStr = null;


    Transform m_contentTypeTrans = null;
    Transform m_contentTrans = null;
    Transform m_centerTypeTrans = null;
    Transform m_okBtnTrans = null;
    Transform m_noBtnTrans = null;
    Transform m_yesBtnTrans = null;
    Transform m_titleTrans = null;

    protected override void CreateWindow()
    {
        base.CreateWindow();
        m_contentTypeTrans = GetTrans("topType");
        m_contentTrans = GetTrans("topCotent");
        m_centerTypeTrans = GetTrans("centerType");
        m_yesBtnTrans = GetTrans("YesBtn");
        m_noBtnTrans = GetTrans("NoBtn");
        m_okBtnTrans = GetTrans("OKBtn");
        m_titleTrans = GetTrans("title");
    }
    public override void InitWindow(bool open = true, int state = 0)
    {

        InitCamera(open, false);

        base.InitWindow(open, state);
        this.m_state = state;

        if (open)
        {
            if (m_contentTrans.GetComponent<UILabel>() != null)
            {
                m_contentTrans.GetComponent<UILabel>().alignment = NGUIText.Alignment.Center;
            }

            SetActive(m_contentTypeTrans, true);
            SetActive(m_centerTypeTrans, true);

            switch (state)
            {
                case 0:
                    SetActive(m_yesBtnTrans, false);
                    SetActive(m_noBtnTrans, false);
                    SetActive(m_okBtnTrans, true);
                    SetLabel(m_okBtnTrans, TextData.GetText((EnumText)m_yesTextID));
                    break;
                case 1:
                    SetActive(m_yesBtnTrans, true);
                    SetActive(m_noBtnTrans, true);
                    SetActive(m_okBtnTrans, false);
                    SetLabel(m_yesBtnTrans, TextData.GetText((EnumText)m_yesTextID));
                    SetLabel(m_noBtnTrans, TextData.GetText((EnumText)m_noTextID));

                    break;
                case 2:
                    SetActive(m_yesBtnTrans, false);
                    SetActive(m_noBtnTrans, false);
                    SetActive(m_okBtnTrans, false);
                    break;
                case 3: //content����
                    SetActive(m_yesBtnTrans, false);
                    SetActive(m_noBtnTrans, false);
                    SetActive(m_okBtnTrans, true);
                    SetLabel(m_okBtnTrans, TextData.GetText((EnumText)m_yesTextID));
                    m_contentTrans.GetComponent<UILabel>().alignment = NGUIText.Alignment.Left;
                    break;
            }

            SetActive(m_centerTypeTrans, false);
            SetActive(m_contentTypeTrans, true);

            if (m_contentID >= 0)
            {
                m_contentStr = TextData.GetText((EnumText)m_contentID);
                if (m_contentStr == null)
                {
                    m_contentStr = "ERROR " + m_contentID.ToString();
                }
            }
            SetLabel(m_contentTrans, m_contentStr == null ? "" : m_contentStr);
            SetLabel(m_titleTrans, m_titleStr == null ? "" : m_titleStr);
            SetGray(m_okBtnTrans, m_needGrayOkBtn, true);
        }
        else
        {
            ResetData();
            ResetCbk();
        }
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
        if (gb == null)
        {
            return;
        }
        switch (clickName)
        {
            case "YesBtn":
            case "OKBtn":
                HandleOK(gb);
                break;
            case "NoBtn":
                HandleNO(gb);
                break;
            case "collider":
                OnClickCollider();
                break;
            default:
                break;
        }

    }

    public void SetContent(string str)
    {
        SetLabel(m_contentTrans, str);
    }

    private void OnClickCollider()
    {
        if (m_canClickCollider)
        {
            ResetData();
        }
    }

    void HandleOK(GameObject gb)
    {
        if (m_okCallBack != null)
        {
            m_okCallBack(this, gb);
        }
    }

    void HandleNO(GameObject gb)
    {
        ResetData();
        if (m_noCallBack != null)
        {
            m_noCallBack(this, gb);
        }
        ResetCbk();
        InitWindow(false);
    }

    void ResetData()
    {
        m_yesTextID = (int)EnumText.ET_OK;
        m_noTextID = (int)EnumText.ET_NO;
        m_titleStr = null;
        m_contentStr = null;
        m_contentID = -1;
        m_canClickCollider = false;
        m_needGrayOkBtn = false;
        m_autoCallOKFunc = false;
        SetGray(m_okBtnTrans, m_needGrayOkBtn, true);
        m_curWindowName = "";
    }

    void ResetCbk()
    {
        m_okCallBack = null;
        m_noCallBack = null;
        m_timePerSecCB = null;
        m_timeOverCB = null;
    }

   protected override void UpdatePerSecond()
    {
        if (m_countDown > 0)
        {
            m_countDown -= 1;
            if (m_countDown <= 0)
            {
                m_timePerSecCB = null;

				if (m_timeOverCB != null)
				{
                    SetLabel(m_contentTrans, m_contentStr == null ? "" : m_contentStr);
                    if (m_needGrayOkBtn)
                    {
                        m_needGrayOkBtn = false;
                        SetGray(m_okBtnTrans, m_needGrayOkBtn, true);
                    }

                    m_timeOverCB();
                }
                if (m_autoCallOKFunc && m_okCallBack != null)
                {
                    m_okCallBack();
                }
                else if(m_noCallBack != null && m_autoCallNoFunc)
                {
                    m_noCallBack();
                }
            }
            else if (m_timePerSecCB != null)
			{
                m_timePerSecCB();
                SetLabel(m_contentTrans, m_contentStr == null ? "" : m_contentStr);
                SetLabel(m_okBtnTrans, m_okBtnStr == null ? TextData.GetText((EnumText)m_yesTextID) : m_okBtnStr);
                SetLabel(m_noBtnTrans, m_noBtnStr == null ? TextData.GetText((EnumText)m_noTextID) : m_noBtnStr);
            }
        }
    }
}
