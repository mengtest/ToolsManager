/************************************************************
//     文件名      : ActorComponent.cs
//     功能描述    : 
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/07/11.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;

public interface IActorComponentEvent
{
}


public abstract class ActorComponent
{
    public enum UpdateType
    {
        Normal,     //正常每帧更新
        Visiable,   //可见更新
        NoUpdate    //不更新
    }

    //更新类型
    public abstract UpdateType m_UpdateType { get; }

    protected BaseSceneActor m_OwnerActor;

    //低频更新时间间隔
    public virtual float m_LowFreqDeltaTime
    {
        get { return -1f; }
    }

    //当前低频更新时间
    public float m_LowFreqCurrentTime = 0;

    public virtual void SetBaseOwner(BaseSceneActor actor)
    {
        m_OwnerActor = actor;
    }

    public BaseSceneActor GetBaseOwner()
    {
        return m_OwnerActor;
    }

    public virtual void Initialize()
    {
    }

    public virtual void UnInitialize()
    {
    }

    public virtual void InitData(object data1 = null)
    {
    }

    public virtual void Update(float deltaTime)
    {
    }

    public virtual void OnActorInst(BaseSceneActor actor)
    {
    }

    public virtual void OnActorUnInst(BaseSceneActor actor)
    {
    }

    public virtual void OnActorInSight(BaseSceneActor actor)
    {
    }

    public virtual void OnActorOutSight(BaseSceneActor actor)
    {
    }

    public virtual void OnActorDestory(BaseSceneActor actor)
    {

    }

    public void SendActorComponentEvent(IActorComponentEvent componentEvent)
    {
        GetBaseOwner().SendActorComponentEvent(componentEvent);
    }

    public T GetActorComponent<T>() where T : ActorComponent
    {
        return GetBaseOwner().GetActorComponent<T>();
    }
}
