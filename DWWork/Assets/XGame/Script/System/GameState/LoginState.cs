using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
public class LoginState : BaseGameState
{
	public override void Init()
	{
	}

	public override void Enter()
    {
        GameRoot.Instance.StartCoroutine(EnterCoroutine());
	}

	IEnumerator EnterCoroutine()
	{
		while(!ResourceManager.Instance.m_initLoginDone)
		{
			yield return null;
		}

		EZFunWindowMgr.Instance.InitWaitWindow();

		m_progressValue = 0;
		
		InitSounds ();	
		m_progressValue = 1;
		
		base.Enter();

		EZFunWindowMgr.Instance.DestoryUpdateUI();

        LuaRootSys.Instance.CallLuaFunc("LoginState.Enter");
    }
	
	public override void Leave()
	{
		base.Leave();
        LuaRootSys.Instance.CallLuaFunc("LoginState.Leave");
    }


	public override EGameStateType GetStateType()
	{
		return EGameStateType.LoginState;
	}


	void InitSounds()
	{
		
	}

}
