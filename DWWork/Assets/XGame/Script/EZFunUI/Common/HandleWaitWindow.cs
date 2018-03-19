/************************************************************
     File      : HandleWaitWindow.cs
     brief     :   
     author    : JanusLiu   janusliu@ezfun.cn
     version   : 1.0
     date      : 2014/11/10 14:32:16
     copyright : Copyright 2014 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;

public class HandleWaitWindow : WindowRoot {

	private float m_openTime = 0;

	public override void InitWindow (bool open=true, int state=0)
	{
		InitCamera(open, false, (int)SpecialWindowDepthEnum.SWE_WaitingWindow);
		base.InitWindow (open, state);
        Debug.Log("wait_ui_window state = " + state);
		if(open)
		{
			m_openTime = Time.realtimeSinceStartup;
            SetActive(GetTrans(GetTrans("label"), "label3"), false);
            switch (state)
			{
			    case 1:
                    SetActive(GetTrans("collider"), true);
                    SetActive(GetTrans("cover"), false);
                    ShowChild(GetTrans("label"), "label1",false);
                    break;
			    case 2:
                    SetActive(GetTrans("collider"), true);
                    SetActive(GetTrans("cover"), true);
                    SetActive(GetTrans("background"), true);
                    ShowChild(GetTrans("label"), "label1");
				    break;
			    case 3:
                    SetActive(GetTrans("collider"), true);
                    SetActive(GetTrans("cover"), true);
                    SetActive(GetTrans("background"), false);
				    ShowChild(GetTrans("label"), "label1");
				    break;
			    case 4:
                    SetActive(GetTrans("collider"), true);
                    SetActive(GetTrans("cover"), true);
                    SetActive(GetTrans("background"), true);
				    ShowChild(GetTrans("label"), "label2");
				    break;
                case 5:
                    SetActive(GetTrans("collider"), true);
                    SetActive(GetTrans("cover"), true);
                    SetActive(GetTrans("background"), true);
                    ShowChild(GetTrans("label"), "label3");
                    break;
            }
		}
	}

	protected override void UpdatePerSecond ()
	{
		base.UpdatePerSecond ();
	}
}
