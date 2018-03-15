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
using System.Collections.Generic;

public class HandleNetTimeOutWindow : WindowRoot {

	public delegate void CallBack();
	public static EZFunDictionary<string,CallBack> m_okCallBack = new EZFunDictionary<string,CallBack>();
    public static void AddOkCallBack(string key,CallBack cb)
    {
        if (!m_okCallBack.ContainsKey(key))
            m_okCallBack.Add(key,cb);
    }
    
    public static CallBack m_noCallBack = null;

	public override void InitWindow (bool open=true, int state=0)
	{
        InitCamera(open, false, (int)SpecialWindowDepthEnum.SWE_NetWorkError);

		base.InitWindow (open, state);	
		if(open)
		{
			InitWindowDetail(state);
		}
	}

	protected override void CreateWindow()
	{
		base.CreateWindow();
		
		//SetLabel(GetTrans("title"), TextData.GetText(120001));
		//SetLabel(GetTrans("centerCotent"), TextData.GetText(120002));
		//SetLabel(GetTrans("okbtn"), TextData.GetText(120003));
		//SetLabel(GetTrans("yesbtn"), TextData.GetText(120003));
		//SetLabel(GetTrans("nobtn"), TextData.GetText(120004));
	}

	protected override void HandleWidgetClick (GameObject gb)
	{
		if(!CheckBtnCanClick(gb))
		{
			return;
		}

		string clickName = gb.name;

		switch(clickName)
		{
		case "okbtn":
		case "yesbtn":
			HandleOK();
			break;
		case "nobtn":
			HandleON();
			break;
        case "confirmBtn":
            InitWindow(false);
            break;
		}

        
	}



	private void HandleOK()
	{
        for (int i = 0, len = m_okCallBack.Count; i < len; i++)
            m_okCallBack.GetValue(i)();

        m_okCallBack.Clear();

        InitWindow(false);
	}

	private void HandleON()
	{
		if(m_noCallBack != null)
		{
			m_noCallBack();
			m_noCallBack = null;
		}
		InitWindow(false);
	}

    void InitWindowDetail(int state)
    {
        switch(state)
        {
        case 0:            
			SetActive(GetTrans("okbtn"), true);
			SetActive(GetTrans("yesbtn"), false);
			SetActive(GetTrans("nobtn"), false);
            SetActive(GetTrans("confirmBtn"), false);
            break;
		case 1:
			SetActive(GetTrans("okbtn"), false);
			SetActive(GetTrans("yesbtn"), true);
			SetActive(GetTrans("nobtn"), true);
            SetActive(GetTrans("confirmBtn"), false);
			break;
        case 2:
            SetActive(GetTrans("okbtn"), false);
			SetActive(GetTrans("yesbtn"), false);
			SetActive(GetTrans("nobtn"), false);
            SetActive(GetTrans("confirmBtn"), true);
            break;
        }
    }
}
