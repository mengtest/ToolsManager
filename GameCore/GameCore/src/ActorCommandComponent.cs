/************************************************************
//     文件名      : ActorActionComponent.cs
//     功能描述    : 
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/07/11.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class CommandEvent
{
}

public abstract class ActorCommand
{
    public ActorCommandComponent m_ActorCommandComponent = null;

    public virtual void Initialize()
    {
    }

    public virtual void OnActorInst()
    {
    }

    public virtual void OnActorUnInst()
    {
    }

    public virtual void OnActorInSight()
    {
    }

    public virtual void OnActorOutSight()
    {
    }

    public virtual void UnInitialize()
    {
    }

    public virtual void Update(float deltaTime)
    {
    }
}

public class ActorCommandComponent : ActorComponent
{
    public override UpdateType m_UpdateType { get { return UpdateType.Normal; } }
    private List<ActorCommand> m_ActorCommands = new List<ActorCommand>();
    private Dictionary<Type, FastEvent<Action<CommandEvent>>> m_CommandEventRouter = new Dictionary<Type, FastEvent<Action<CommandEvent>>>();

    public override void Initialize()
    {
    }

    public override void UnInitialize()
    {
        for (int i = 0; i < m_ActorCommands.Count; ++i)
        {
            m_ActorCommands[i].UnInitialize();
        }
        m_ActorCommands.Clear();
    }

    public override void OnActorInst(BaseSceneActor actor)
    {
        for (int i = 0; i < m_ActorCommands.Count; ++i)
        {
            m_ActorCommands[i].OnActorInst();
        }
    }

    public override void OnActorUnInst(BaseSceneActor actor)
    {
        for (int i = 0; i < m_ActorCommands.Count; ++i)
        {
            m_ActorCommands[i].OnActorUnInst();
        }
    }

    public override void OnActorInSight(BaseSceneActor actor)
    {
        for (int i = 0; i < m_ActorCommands.Count; ++i)
        {
            m_ActorCommands[i].OnActorInSight();
        }
    }

    public override void OnActorOutSight(BaseSceneActor actor)
    {
        for (int i = 0; i < m_ActorCommands.Count; ++i)
        {
            m_ActorCommands[i].OnActorOutSight();
        }
    }

    public void SendCommand(CommandEvent evt)
    {
        FastEvent<Action<CommandEvent>> eventList = null;
        m_CommandEventRouter.TryGetValue(evt.GetType(), out eventList);
        if (eventList != null)
        {
            eventList.Invoke(evt);
        }
    }

    public void AddActorCommand(ActorCommand cmd)
    {
        for (int i = 0; i < m_ActorCommands.Count; ++i)
        {
            if (m_ActorCommands[i].GetType() == cmd.GetType())
                return;
        }
        cmd.m_ActorCommandComponent = this;
        cmd.Initialize();
        m_ActorCommands.Add(cmd);
    }

    public override void Update(float deltaTime)
    {
        int len = m_ActorCommands.Count;
        for (int i = 0; i < len; ++i)
        {
            m_ActorCommands[i].Update(deltaTime);
        }
    }

    public void RegisterCommandEvent<T>(Action<CommandEvent> action) where T : CommandEvent
    {
        FastEvent<Action<CommandEvent>> eventList = null;
        Type type = typeof(T);
        m_CommandEventRouter.TryGetValue(type, out eventList);
        if (eventList == null)
        {
            eventList = new FastEvent<Action<CommandEvent>>();
            m_CommandEventRouter.Add(type, eventList);
        }
        eventList.RegisterHandler(action);
    }

    public void UnRegisterCommandEvent<T>(Action<CommandEvent> action) where T : CommandEvent
    {
        FastEvent<Action<CommandEvent>> eventList = null;
        Type type = typeof(T);
        m_CommandEventRouter.TryGetValue(type, out eventList);
        if (eventList != null)
        {
            eventList.UnRegisterHandler(action);
        }
    }
}

