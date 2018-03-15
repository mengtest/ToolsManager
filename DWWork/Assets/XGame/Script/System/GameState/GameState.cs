using UnityEngine;
using System.Collections;

public class GameState : BaseGameState {

    public override void Enter()
    {
        m_progressValue = 0.51f;
        LuaRootSys.Instance.CallLuaFunc("GameState.Enter");
        m_changeProgress = false;
        m_progressValue = 1f;
        base.Enter();
    }

    public override void Leave()
    {
        LuaRootSys.Instance.CallLuaFunc("GameState.Leave");
        base.Leave();
    }


    public override EGameStateType GetStateType()
    {
        return EGameStateType.GameState;
    }
}
