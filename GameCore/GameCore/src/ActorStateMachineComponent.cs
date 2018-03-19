/************************************************************
//     文件名      : ActorStateMachineComponent.cs
//     功能描述    : 角色状态机组件
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/07/27.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/
using UnityEngine;
using System;
using System.Collections.Generic;

public class ActorStateMachineComponent : ActorComponent
{
    public FastEvent<Action<ActorState>> m_OnCreateStateEvent = new FastEvent<Action<ActorState>>();
    public FastEvent<Action<ActorState>> m_OnDestroyStateEvent = new FastEvent<Action<ActorState>>();
    public FastEvent<Action<ActorState>> m_OnEndStateEvent = new FastEvent<Action<ActorState>>();
    public FastEvent<Action<ActorState, ActorState, IStateEvent>> m_OnPreEnterStateEvent = new FastEvent<Action<ActorState, ActorState, IStateEvent>>();
    public FastEvent<Action<ActorState, ActorState, IStateEvent>> m_OnPostEnterStateEvent = new FastEvent<Action<ActorState, ActorState, IStateEvent>>();
    public FastEvent<Action<ActorState, ActorState>> m_OnPreLeaveStateEvent = new FastEvent<Action<ActorState, ActorState>>();
    public FastEvent<Action<ActorState, ActorState>> m_OnPostLeaveStateEvent = new FastEvent<Action<ActorState, ActorState>>();
    public FastEvent<Action<ActorState>> m_OnUpdateStateEvent = new FastEvent<Action<ActorState>>();
    public FastEvent<Action<ActorState>> m_OnReplicateStateEvent = new FastEvent<Action<ActorState>>();

    private List<ActorStateMachine> m_StateMachines = new List<ActorStateMachine>();
    private ActorStateMachine m_ActiveStateMachine = null;
    public FastEvent<Action<ActorStateMachine, bool>> m_OnStateMachineChanged = new FastEvent<Action<ActorStateMachine, bool>>();

    public override UpdateType m_UpdateType { get { return UpdateType.Normal; } }

    public void AddStateMachine(ActorStateMachine stateMachine)
    {
        stateMachine.m_StateMachineComponent = this;
        m_StateMachines.Add(stateMachine);
    }

    public override void Initialize()
    {
        for (int i = 0; i < m_StateMachines.Count; ++i)
        {
            m_StateMachines[i].Initialize();
        }
        m_OwnerActor.RegisterComponentEvent<ActorComponentToStateEvent>(HandleComponentToState);
    }

    public override void UnInitialize()
    {
        for (int i = 0; i < m_StateMachines.Count; ++i)
        {
            m_StateMachines[i].UnInitialize();
        }
        m_OwnerActor.UnRegisterComponentEvent<ActorComponentToStateEvent>(HandleComponentToState);
    }

    public override void Update(float deltaTime)
    {
        if (m_ActiveStateMachine != null)
        {
            m_ActiveStateMachine.Update(deltaTime);
        }
    }

    public void SetActiveStateMachine(Type type)
    {
        if (m_ActiveStateMachine != null && m_ActiveStateMachine.GetType() == type)
        {
            return;
        }
        if (type == null)
        {
            if (m_ActiveStateMachine != null)
            {
                m_ActiveStateMachine.End();
                m_OnStateMachineChanged.Invoke(m_ActiveStateMachine, false);
                m_ActiveStateMachine = null;
            }
        }
        else
        {
            for (int i = 0; i < m_StateMachines.Count; ++i)
            {
                if (m_StateMachines[i].GetType() == type)
                {
                    if (m_ActiveStateMachine != null)
                    {
                        m_ActiveStateMachine.End();
                        m_OnStateMachineChanged.Invoke(m_ActiveStateMachine, false);
                    }
                    m_ActiveStateMachine = m_StateMachines[i];
                    m_ActiveStateMachine.Begin();
                    m_OnStateMachineChanged.Invoke(m_ActiveStateMachine, true);
                    return;
                }
            }
        }
    }

    public ActorStateMachine GetActiveStateMachine()
    {
        return m_ActiveStateMachine;
    }

    public ActorStateMachine GetStateMachine<T>()
    {
        Type type = typeof(T);
        for (int i = 0; i < m_StateMachines.Count; ++i)
        {
            if (m_StateMachines[i].GetType() == type)
            {
                return m_StateMachines[i];
            }
        }
        return null;
    }

    public override void SetBaseOwner(BaseSceneActor actor)
    {
        base.SetBaseOwner(actor);
        for (int i = 0; i < m_StateMachines.Count; ++i)
        {
            m_StateMachines[i].m_SceneActor = actor;
        }
    }

    public override void OnActorInst(BaseSceneActor actor)
    {
    }

    public override void OnActorUnInst(BaseSceneActor actor)
    {
    }


    public int StateType
    {
        get
        {
            if (m_ActiveStateMachine != null && m_ActiveStateMachine.GetActiveState() != null)
            {
                return m_ActiveStateMachine.GetActiveState().GetStateType();
            }
            return ActorState.UnknownStateType;
        }
    }

    public ActorState ActiveState
    {
        get
        {
            if(m_ActiveStateMachine != null && m_ActiveStateMachine.GetActiveState() != null)
            {
                return m_ActiveStateMachine.GetActiveState();
            }
            return null;
        }
    }

    private void HandleComponentToState(IActorComponentEvent evt)
    {
        ActorComponentToStateEvent newEvt = (ActorComponentToStateEvent)evt;
        if (m_ActiveStateMachine != null && newEvt.m_stateEvent != null)
        {
            m_ActiveStateMachine.SendEvent(newEvt.m_stateEvent);
        }
    }
}
