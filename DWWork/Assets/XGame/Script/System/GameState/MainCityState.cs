using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class MainCityState : BaseGameState
{
    public EZFunWindowEnum m_backWindow = EZFunWindowEnum.None;

    public override void Enter()
    {
        m_progressValue = 0.51f;
        LuaRootSys.Instance.CallLuaFunc("MainCityState.Enter");
        m_progressValue = 1f;
        m_changeProgress = false;
        base.Enter();
    }

    public override void Leave()
    {
        LuaRootSys.Instance.CallLuaFunc("MainCityState.Leave");
        base.Leave();
    }


    public override EGameStateType GetStateType()
    {
        return EGameStateType.MainCityState;
    }

    void InitSounds()
    {

    }

}