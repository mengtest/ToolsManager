/************************************************************
//     文件名      : ActorStateMachine.cs
//     功能描述    : Actor状态机基类
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/07/12.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/
using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class ActorStateMachine
{
    protected Dictionary<int, ActorState> m_States = new Dictionary<int, ActorState>();
    protected ActorState m_DefualtState = null;
    protected IStateEvent m_DefualtStateEvent = null;
    protected ActorState m_ActiveState = null;
    public ActorStateMachineComponent m_StateMachineComponent = null;
    public BaseSceneActor m_SceneActor = null;
    public abstract int ActorStateTypeCount();

    public virtual void Initialize()
    {
        var enm = m_States.GetEnumerator();
        while (enm.MoveNext())
        {
            enm.Current.Value.Create();
        }
    }

    public virtual void UnInitialize()
    {
        var enm = m_States.GetEnumerator();
        while (enm.MoveNext())
        {
            enm.Current.Value.Destroy();
        }
        m_States.Clear();
    }

    public void Update(float deltaTime)
    {
        if (m_ActiveState == null)
        {
            m_ActiveState = m_DefualtState;
            m_DefualtState.EnterState(null, m_DefualtStateEvent);
        }
        else
        {
            if (m_ActiveState.IsStateEnded())
            {
                m_StateMachineComponent.m_OnEndStateEvent.Invoke(m_ActiveState);
                GotoState(m_ActiveState, m_DefualtState, m_DefualtStateEvent);
            }
            else
            {
                m_ActiveState.Update(deltaTime);
                m_StateMachineComponent.m_OnUpdateStateEvent.Invoke(m_ActiveState);
            }
        }
    }

    public virtual void Begin()
    {
        m_ActiveState = m_DefualtState;
        m_StateMachineComponent.m_OnPreEnterStateEvent.Invoke(m_DefualtState, (ActorState)null, m_DefualtStateEvent);
        m_DefualtState.EnterState(null, m_DefualtStateEvent);
        m_StateMachineComponent.m_OnPostEnterStateEvent.Invoke(m_DefualtState, (ActorState)null, m_DefualtStateEvent);
    }

    public virtual void End()
    {
        if (m_ActiveState != null)
        {
            m_StateMachineComponent.m_OnPreLeaveStateEvent.Invoke(m_ActiveState, (ActorState)null);
            m_ActiveState.LeaveState(null);
            m_StateMachineComponent.m_OnPostLeaveStateEvent.Invoke(m_ActiveState, (ActorState)null);
            m_ActiveState = null;
        }
    }

    protected void AddState(ActorState state)
    {
        if (m_States.ContainsKey((int)state.GetStateType()))
        {
            Debug.LogError("Register state failed!");
            return;
        }
        state.m_StateMachine = this;
        m_States.Add((int)state.GetStateType(), state);
    }

    public void GotoState(ActorState preState, ActorState nextState, IStateEvent evt)
    {
        if (preState != nextState)
        {
            if (preState != null)
            {
                m_StateMachineComponent.m_OnPreLeaveStateEvent.Invoke(preState, nextState);
                preState.LeaveState(nextState);
                m_StateMachineComponent.m_OnPostLeaveStateEvent.Invoke(preState, nextState);
            }
            if (nextState == null)
            {
                nextState = m_DefualtState;
            }
        }
        m_ActiveState = nextState;
        m_StateMachineComponent.m_OnPreEnterStateEvent.Invoke(nextState, preState, evt);
        nextState.EnterState(preState, evt);
        m_StateMachineComponent.m_OnPostEnterStateEvent.Invoke(nextState, preState, evt);
    }

    public void SetState(ActorState nextState, IStateEvent evt)
    {
        if (nextState == null)
        {
            return;
        }
        if (m_ActiveState != null)
        {
            m_StateMachineComponent.m_OnPreLeaveStateEvent.Invoke(m_ActiveState, nextState);
            m_ActiveState.LeaveState(nextState);
            m_StateMachineComponent.m_OnPostLeaveStateEvent.Invoke(m_ActiveState, nextState);
        }
        m_StateMachineComponent.m_OnPreEnterStateEvent.Invoke(nextState, m_ActiveState, evt);
        nextState.EnterState(m_ActiveState, evt);
        m_StateMachineComponent.m_OnPostEnterStateEvent.Invoke(nextState, m_ActiveState, evt);
        m_ActiveState = nextState;
    }

    public void ReplicateState(int type, object param)
    {
        ActorState actorState = GetState(type);
        if (actorState == null)
        {
            return;
        }

        if (m_ActiveState != null)
        {
            m_StateMachineComponent.m_OnPreLeaveStateEvent.Invoke(m_ActiveState, actorState);
            m_ActiveState.LeaveState(actorState);
            m_StateMachineComponent.m_OnPostLeaveStateEvent.Invoke(m_ActiveState, actorState);
        }

        actorState.ReplicateState(param);
        m_StateMachineComponent.m_OnReplicateStateEvent.Invoke(actorState);
        m_ActiveState = actorState;
    }

    public void SendEvent(IStateEvent evt)
    {
        if (m_ActiveState != null)
        {
            m_ActiveState.HandleEvent(evt);
        }
    }

    public ActorState GetState(int type)
    {
        ActorState ret = null;
        m_States.TryGetValue(type, out ret);
        return ret;
    }

    public void SetDefaultState(int type, IStateEvent evt)
    {
        ActorState state = GetState(type);
        if (state != null)
        {
            m_DefualtState = state;
            m_DefualtStateEvent = evt;
        }
    }

    public ActorState GetDefaultState()
    {
        return m_DefualtState;
    }

    public ActorState GetActiveState()
    {
        return m_ActiveState;
    }

    public bool IsInState<T>() where T : ActorState
    {
        if (m_ActiveState == null)
        {
            return false;
        }
        return m_ActiveState.GetType() == typeof(T);
    }
}
