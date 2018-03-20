/************************************************************
//     文件名      : ActorState.cs
//     功能描述    : Actor状态基类
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/07/12.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/
using UnityEngine;
using System;
using System.Collections.Generic;

public delegate bool StateEnterTransitDelegate(ActorState preState, IStateEvent evt);
public delegate bool StateLeaveTransitDelegate(ActorState nextState, IStateEvent evt);

public abstract class IStateEvent
{
    public virtual int GetStateType() { return ActorState.UnknownStateType; }
}

public abstract class TStateEvent<T> : IStateEvent
    where T : class, new()
{
    private sealed class Singleton
    {
        private Singleton() { }
        internal static readonly T Value = new T();
    }

    public static T Default
    {
        get { return Singleton.Value; }
    }
}

public abstract class ActorState
{
    public const int UnknownStateType = 0;
    /// <summary>
    /// 改成非必要不要创建，减少不必要的内存
    /// </summary>
    private Dictionary<int, StateEnterTransitDelegate> m_StateEnterTransit = null;
    private Dictionary<int, StateLeaveTransitDelegate> m_StateLeaveTransit = null;

    public ActorStateMachine m_StateMachine = null;
    public BaseSceneActor m_Owner
    {
        get { return m_StateMachine.m_SceneActor; }
    }

    public virtual void Create()
    {
    }

    public virtual void Destroy()
    {
    }

    public virtual void EnterState(ActorState preState, IStateEvent evt)
    {
    }

    public virtual void LeaveState(ActorState nextState)
    {
    }

    public virtual bool IsStateEnded()
    {
        return true;
    }

    public virtual void Update(float deltaTime)
    {
    }

    public virtual void ReplicateState(object param)
    {
    }

    public abstract int GetStateType();

    public void StateEnterTransit(int stateFlags, StateEnterTransitDelegate transit)
    {
        if (transit != null && stateFlags != 0)
        {
            for (int i = 0; i < m_StateMachine.ActorStateTypeCount(); ++i)
            {
                int stateType = 1 << i;
                if ((stateFlags & stateType) != 0)
                {
                    if (m_StateEnterTransit == null)
                    {
                        m_StateEnterTransit = new Dictionary<int, StateEnterTransitDelegate>();
                    }
                    m_StateEnterTransit.Add(stateType, transit);
                }
            }
        }
    }

    public void StateLeaveTransit(int stateFlags, StateLeaveTransitDelegate transit)
    {
        if (transit != null && stateFlags != 0)
        {
            for (int i = 0; i < m_StateMachine.ActorStateTypeCount(); ++i)
            {
                int stateType = 1 << i;
                if ((stateFlags & stateType) != 0)
                {
                    if (m_StateLeaveTransit == null)
                    {
                        m_StateLeaveTransit = new Dictionary<int, StateLeaveTransitDelegate>();
                    }
                    m_StateLeaveTransit.Add(stateType, transit);
                }
            }
        }
    }

    public bool TryTransitState(ActorState nextState, IStateEvent evt)
    {
        bool ret = true;
        StateLeaveTransitDelegate leaveDel = null;
        int nextStateType = nextState.GetStateType();
        int preStateType = this.GetStateType();
        if (m_StateLeaveTransit != null && m_StateLeaveTransit.TryGetValue(nextStateType, out leaveDel))
        {
            if (leaveDel != null)
            {
                ret = leaveDel(nextState, evt);
            }
        }

        if (!ret)
        {
            return ret;
        }

        StateEnterTransitDelegate enterDel = null;
        if (nextState.m_StateEnterTransit != null)
        {
            nextState.m_StateEnterTransit.TryGetValue(preStateType, out enterDel);
        }
        if (enterDel != null)
        {
            ret &= enterDel(this, evt);
        }
        return ret;
    }

    public virtual void HandleEvent(IStateEvent evt)
    {
        if (evt.GetStateType() == ActorState.UnknownStateType)
        {
            return;
        }

        ActorState nextState = m_StateMachine.GetState(evt.GetStateType());
        if (nextState == null)
        {
            Debug.LogError("Relative state is null!");
            return;
        }
        if (TryTransitState(nextState, evt))
        {
            m_StateMachine.GotoState(this, nextState, evt);
        }
    }
}
