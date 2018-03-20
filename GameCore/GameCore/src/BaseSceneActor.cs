/************************************************************
//     文件名      : BaseSceneActor.cs
//     功能描述    : 场景actor，游戏中的角色，怪物，物件等。
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/07/12.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class ActorType
{
    public const int None = -1;
    public const int Player = 0;//继承于 PlayerActor 类的玩家角色（包括自身和其他玩家）
    public const int NPC = 1;//继承于 NPCActor 类的服务器控制 NPC 角色（怪物、Boss）
    public const int Count = 2;
}

public class ActorInstance : InstanceObject
{
}

public interface IActorBuilder
{
    void PreBuild(BaseSceneActor actor, object param);
    void Build(BaseSceneActor actor);

    void Destroy(BaseSceneActor actor);
}

public class SceneActorPrefabInfo
{
    public string m_ResourcePath;
}

public abstract class BaseSceneActor
{
    ///角色进入视野
    public FastEvent<Action> ActorInSightEvent = new FastEvent<Action>();
    ///角色进出视野
    public FastEvent<Action> ActorOutSightEvent = new FastEvent<Action>();
    ///角色实例化, 获得GameObejct.
    public FastEvent<Action> ActorInstEvent = new FastEvent<Action>();
    ///角色实例删除， 失去 GameObject.
    public FastEvent<Action> ActorUnInstEvent = new FastEvent<Action>();
    ///逻辑层角色SceneActor创建.
    public FastEvent<Action> ActorCreateEvent = new FastEvent<Action>();
    ///逻辑层角色SceneActor删除，超出服务器视野.
    public FastEvent<Action> ActorDestroyEvent = new FastEvent<Action>();
    ///角色更换prefab前的事件
    public FastEvent<Action> BeforeActorChangePrefabEvent = new FastEvent<Action>();
    ///角色更换prefab后的事件
    public FastEvent<Action> AfterActorChangePrefabEvent = new FastEvent<Action>();
    ///角色acompoent事件字典
    private Dictionary<Type, FastEvent<Action<IActorComponentEvent>>> m_ActorComponentEventRouter = new Dictionary<Type, FastEvent<Action<IActorComponentEvent>>>();

    private string m_name;
    public string name
    {
        get { return m_name; }
        set { m_name = value; }
    }

    //序列号
    public long m_SerialID = 0;
    //是否是当前主角
    public bool m_IsHero = false;
    //是否隐藏
    public bool m_IsHide = false;

    protected bool m_IsAuthorizedActor = true;

    public bool m_NeedRefreshScene = true;

    public Action<BaseSceneActor> m_createInstanceCallBack = null;
    public Action<BaseSceneActor> m_createInstanceEndCallBack = null;

    /// <summary>
    /// 本地客户端是否有权限控制其行为（移动，攻击，造成伤害等）
    /// </summary>
    public virtual bool IsAuthorizedActor
    {
        get
        {
            if (GetSceneActorType() == ActorType.Player)
            {
                return m_IsHero;
            }
            else
            {
                return m_IsAuthorizedActor;
            }
        }
    }

    //角色绑定的GameObject是否显示
    public bool m_IsActive { get { return gameObject != null && gameObject.activeSelf; } }

    public float m_DistHeroSq = 0;

    public int m_DistHeroSign { get; set; }

    public bool mIsInSight { get; set; }

    // 上一次更新角色的时间RefreshScene
    public float m_LastRefreshSceneTime = 0.0f;

    //当前使用的骨骼类型（角色逻辑骨骼状态，在隐藏角色等情况下，只要角色逻辑骨骼类型不改变，删除角色实例时不赋0。）
    public uint m_CurrentPrefabID = 0;

    //正在异步加载中的骨骼类型（删除角色实例时需要将此值赋为0）
    public uint m_LoadingPrefabID = 0;

    //先前已加载资源的骨骼类型（删除角色实例时需要将此值赋为0）
    public uint m_LoadedPrefabID = 0;

    //逐帧更新 ActorComponent List
    protected List<ActorComponent> m_ActorComponentList = new List<ActorComponent>();

    //人物 GameObject 实例
    public ActorInstance m_ActorInstance = null;

    //禁止直接用这个属性，所有操作对视图的操作必须走ViewComponent
    public GameObject gameObject
    {
        get
        {
            return (m_ActorInstance != null) ? m_ActorInstance.m_GameObject : null;
        }
    }

    public abstract int GetSceneActorType();
    public abstract Type ControllerType { get; }

    public virtual int RefreshDistSign(Vector3 heroPos) { return 0; }

    /// <summary>
    /// -1 表示已经删除 0 创建,表示 1 init完成 2 表示实例化完毕, set方法禁止调用，只在ActorManager中调用
    /// </summary>
    public int State { get; set; }

    public void SendActorComponentEvent(IActorComponentEvent evt)
    {
        FastEvent<Action<IActorComponentEvent>> eventList = null;
        m_ActorComponentEventRouter.TryGetValue(evt.GetType(), out eventList);
        if (eventList != null)
        {
            eventList.Invoke(evt);
        }
    }

    public void RegisterComponentEvent(Action<IActorComponentEvent> action, Type type)
    {
        FastEvent<Action<IActorComponentEvent>> eventList = null;
        m_ActorComponentEventRouter.TryGetValue(type, out eventList);
        if (eventList == null)
        {
            eventList = new FastEvent<Action<IActorComponentEvent>>();
            m_ActorComponentEventRouter.Add(type, eventList);
        }
        eventList.RegisterHandler(action);
    }

    public void RegisterComponentEvent<T>(Action<IActorComponentEvent> action) where T : IActorComponentEvent
    {
        Type type = typeof(T);
        RegisterComponentEvent(action, type);
    }

    public void UnRegisterComponentEvent(Action<IActorComponentEvent> action, Type type)
    {
        FastEvent<Action<IActorComponentEvent>> eventList = null;
        m_ActorComponentEventRouter.TryGetValue(type, out eventList);
        if (eventList != null)
        {
            eventList.UnRegisterHandler(action);
        }
    }

    public void UnRegisterComponentEvent<T>(Action<IActorComponentEvent> action)
    {
        UnRegisterComponentEvent(action, typeof(T));
    }


    //获取指定的ActorComponent
    public T GetActorComponent<T>() where T : ActorComponent
    {
        var type = typeof(T);
        for (int i = 0; i < m_ActorComponentList.Count; i++)
        {
            if (m_ActorComponentList[i] is T)
            {
                return m_ActorComponentList[i] as T;
            }
        }
        return null;
    }

    public void InitializeActorComponents()
    {
        for (int i = 0; i < m_ActorComponentList.Count; ++i)
        {
            m_ActorComponentList[i].Initialize();
        }
    }

    public void UnInitializeActorComponents()
    {
        for (int i = 0; i < m_ActorComponentList.Count; i++)
        {
            m_ActorComponentList[i].UnInitialize();
        }
    }

    //添加一个指定的ActorComponent
    public void AddActorComponent(ActorComponent actorComponent)
    {
        actorComponent.SetBaseOwner(this);
        m_ActorComponentList.Add(actorComponent);
    }

    public void RemoveActorComponent<T>() where T : ActorComponent
    {
        for (int i = m_ActorComponentList.Count - 1; i >= 0; i--)
        {
            if (m_ActorComponentList[i] is T)
            {
                m_ActorComponentList.RemoveAt(i);
                return;
            }
        }
    }

    /// 仅用来更新各个Component
    public virtual void Update()
    {
        var componentLength = m_ActorComponentList.Count;
        for (int i = 0; i < componentLength; ++i)
        {
            var actorComponent = m_ActorComponentList[i];
            if (actorComponent.m_UpdateType == ActorComponent.UpdateType.Normal || (actorComponent.m_UpdateType == ActorComponent.UpdateType.Visiable && m_IsActive))
            {
                bool isLowFreqUpdate = actorComponent.m_LowFreqDeltaTime > 0;
                if (m_IsHero || !isLowFreqUpdate)
                {
                    actorComponent.Update(Time.deltaTime);
                }
                else
                {
                    actorComponent.m_LowFreqCurrentTime += Time.deltaTime;
                    if (actorComponent.m_LowFreqCurrentTime > actorComponent.m_LowFreqDeltaTime)
                    {
                        actorComponent.Update(actorComponent.m_LowFreqCurrentTime);
                        actorComponent.m_LowFreqCurrentTime -= (float)System.Math.Floor((double)(actorComponent.m_LowFreqCurrentTime / actorComponent.m_LowFreqDeltaTime)) * actorComponent.m_LowFreqDeltaTime;
                    }
                }
            }
        }
    }

    public virtual void Init()
    {
    }

    public virtual void InitData(object param)
    {
        for (int i = 0; i < m_ActorComponentList.Count; i++)
        {
            m_ActorComponentList[i].m_LowFreqCurrentTime = 0.0f;
            m_ActorComponentList[i].InitData(param);
        }
    }

    public abstract SceneActorPrefabInfo GetSceneActorPrefabInfo(uint id, object param = null, bool needCheckQualtiySet = false);

    public SceneActorPrefabInfo GetCurrentSceneActorPrefabInfo()
    {
        return GetSceneActorPrefabInfo(m_CurrentPrefabID,null,false);
    }
    public SceneActorPrefabInfo GetCurrentSceneActorPrefabInfoInQualtiySet()
    {
        return GetSceneActorPrefabInfo(m_CurrentPrefabID, null, true);
    }

    ///禁止手动调用
    public virtual void NotifyInst()
    {
        State = 2;
      
        for (int i = 0; i < m_ActorComponentList.Count; i++)
        {
            m_ActorComponentList[i].OnActorInst(this);
        }

        if (ActorInstEvent != null)
        {
            ActorInstEvent.Invoke();
        }
        if (ActorManager.Instance.ActorInstEvent != null)
        {
            ActorManager.Instance.ActorInstEvent.Invoke(this);
        }
    }

    ///禁止手动调用
    public virtual void NotifyUnInst()
    {
        State = 1;
        if (m_ActorInstance != null)
        {
            if (ActorUnInstEvent != null)
            {
                ActorUnInstEvent.Invoke();
            }
            if (ActorManager.Instance.ActorUnInstEvent != null)
            {
                ActorManager.Instance.ActorUnInstEvent.Invoke(this);
            }
        }
        for (int i = 0; i < m_ActorComponentList.Count; i++)
        {
            m_ActorComponentList[i].OnActorUnInst(this);
        }
    }
    /// <summary>
    ///禁止手动调用
    /// </summary>
    public virtual void NotifyInSight()
    {
        mIsInSight = true;

        if (gameObject != null/* && !gameObject.activeSelf*/)
        {
            gameObject.SetActive(true);

            if (ActorInSightEvent != null)
            {
                ActorInSightEvent.Invoke();
            }
            if (ActorManager.Instance.ActorInSightEvent != null)
            {
                ActorManager.Instance.ActorInSightEvent.Invoke(this);
            }
        }
        for (int i = 0; i < m_ActorComponentList.Count; i++)
        {
            m_ActorComponentList[i].OnActorInSight(this);
        }
    }

    /// <summary>
    /// 禁止手动调用
    /// </summary>
    public virtual void NotifyOutSight()
    {
        mIsInSight = false;

        for (int i = 0; i < m_ActorComponentList.Count; i++)
        {
            m_ActorComponentList[i].OnActorOutSight(this);
        }

        if (gameObject != null/* && gameObject.activeSelf*/)
        {
            if (ActorOutSightEvent != null)
            {
                ActorOutSightEvent.Invoke();
            }
            if (ActorManager.Instance.ActorOutSightEvent != null)
            {
                ActorManager.Instance.ActorOutSightEvent.Invoke(this);
            }

            gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 禁止手动调用
    /// </summary>
    public virtual void NotifyDestory()
    {
        //直接删除之前，调用NotifyOutSight、NotifyUnInst
        NotifyOutSight();
        NotifyUnInst();
        m_LoadedPrefabID = 0;
        m_LoadingPrefabID = 0;
        if (ActorDestroyEvent != null)
        {
            ActorDestroyEvent.Invoke();
        }
        if (ActorManager.Instance.ActorDestroyEvent != null)
        {
            ActorManager.Instance.ActorDestroyEvent.Invoke(this);
        }

        for (int i = 0; i < m_ActorComponentList.Count; i++)
        {
            m_ActorComponentList[i].OnActorDestory(this);
        }

        State = 0;
    }
    /// <summary>
    /// 禁止手动调用
    /// </summary>
    public virtual void NotifyCreate()
    {
        State = 1;

        if (ActorCreateEvent != null)
        {
            ActorCreateEvent.Invoke();
        }
        if (ActorManager.Instance.ActorCreateEvent != null)
        {
            ActorManager.Instance.ActorCreateEvent.Invoke(this);
        }
    }

    /// 角色更换模型前的通知
    public virtual void NotifyBeforeChangePrefab()
    {
        if (BeforeActorChangePrefabEvent != null)
        {
            BeforeActorChangePrefabEvent.Invoke();
        }
        if (ActorManager.Instance.BeforeActorChangePrefabEvent != null)
        {
            ActorManager.Instance.BeforeActorChangePrefabEvent.Invoke(this);
        }
    }

    /// 角色更换模型后的通知
    public virtual void NotifyAfterChangePrefab()
    {
        if (AfterActorChangePrefabEvent != null)
        {
            AfterActorChangePrefabEvent.Invoke();
        }
        if (ActorManager.Instance.AfterActorChangePrefabEvent != null)
        {
            ActorManager.Instance.AfterActorChangePrefabEvent.Invoke(this);
        }
    }
}
