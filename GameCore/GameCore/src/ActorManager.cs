/************************************************************
//     文件名      : ActorManager.cs
//     功能描述    : 用于管理场景Actor类
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/07/12.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ActorManagePolicy
{
    public float m_AssetPrepareRadius;
    public float m_InstantiateRadius;
    public float m_SightRadius;
    public float m_AssetPrepareRadiusSQ;
    public float m_InstantiateRadiusSQ;
    public float m_SightRadiusSQ;
}

public class ActorManager : TCoreSystem<ActorManager>, IUpdateable, IInitializeable
{
    protected delegate bool ActorFilterCallback(BaseSceneActor sceneActor, object param);
    private bool m_IsInited = false;
    public bool m_EnableStreamLoading = false;
    ///角色进入视野
    public FastEvent<Action<BaseSceneActor>> ActorInSightEvent = new FastEvent<Action<BaseSceneActor>>();
    ///角色进出视野
    public FastEvent<Action<BaseSceneActor>> ActorOutSightEvent = new FastEvent<Action<BaseSceneActor>>();
    ///角色实例化, 获得GameObejct.
    public FastEvent<Action<BaseSceneActor>> ActorInstEvent = new FastEvent<Action<BaseSceneActor>>();
    ///角色实例删除， 失去 GameObject.
    public FastEvent<Action<BaseSceneActor>> ActorUnInstEvent = new FastEvent<Action<BaseSceneActor>>();
    ///逻辑层角色SceneActor创建.
    public FastEvent<Action<BaseSceneActor>> ActorCreateEvent = new FastEvent<Action<BaseSceneActor>>();
    ///逻辑层角色SceneActor删除，超出服务器视野.
    public FastEvent<Action<BaseSceneActor>> ActorDestroyEvent = new FastEvent<Action<BaseSceneActor>>();
    ///角色更换prefab前的事件
    public FastEvent<Action<BaseSceneActor>> BeforeActorChangePrefabEvent = new FastEvent<Action<BaseSceneActor>>();
    ///角色更换prefab后的事件
    public FastEvent<Action<BaseSceneActor>> AfterActorChangePrefabEvent = new FastEvent<Action<BaseSceneActor>>();

    static int[] m_ActorTypes = new int[]
    {
                ActorType.Player,
                ActorType.NPC,
    };
    protected List<BaseSceneActor>[] m_TypedActors = new List<BaseSceneActor>[(int)ActorType.Count];
    protected ObjectPool<BaseSceneActor>[] m_TypedActorPools = new ObjectPool<BaseSceneActor>[(int)ActorType.Count];
    protected Dictionary<long, BaseSceneActor> m_DictActors = new Dictionary<long, BaseSceneActor>();
    protected ActorManagePolicy[] m_Policies = new ActorManagePolicy[(int)ActorType.Count];
    //private ActorManagePolicy m_FarDistancePloicies = new ActorManagePolicy();

    private float[] m_ActorUpdateTime = new float[(int)ActorType.Count];
    protected GameObject[] m_ActorRoot = new GameObject[(int)ActorType.Count];
    private List<long> m_willDestroyList = new List<long>();

    //角色实例对象池
    protected InstancePool<ActorInstance> m_ActorInstancePool = new InstancePool<ActorInstance>();

    //角色实例处理回调参数
    public class ActorInstanceCallbackParam
    {
        public long m_ActorSerialID;
        public GameObject m_ActorRootGameObj;
        public IActorBuilder m_ActorBuilder;
        public bool m_IsChangePrefab;          //当前创建角色是否是更换prefab导致的
        public SceneActorPrefabInfo m_prefabInfo;
        public Action<BaseSceneActor> m_createInstanceCallBack;
        public Action<BaseSceneActor> m_createInstanceEndCallBack;
    }

    protected IActorBuilder[] m_Builders = new IActorBuilder[(int)ActorType.Count];

    public virtual BaseSceneActor GetHeroActor() { return null; }
    public virtual Vector3 GetHeroPos() { return Vector3.zero; }

    protected virtual void OnInitialize()
    {
    }

    public void Init()
    {
        if (m_IsInited == true)
            return;
        m_IsInited = true;


        //设置远距离更新角色距离
        //SetFarDistancePolicy(90.0f, 95.0f, 100.0f);

        OnInitialize();
    }

    protected virtual void OnUnInitialize()
    {
    }

    public void Release()
    {
        OnUnInitialize();
        m_IsInited = false;
        m_ActorInstancePool.Clear(true);
    }

    protected virtual void OnUpdate() { }

    public void Clear(int type)
    {
        if (type == ActorType.None)
        {
            Debug.LogError("Must specify the actor type to clear.");
            return;
        }
        List<BaseSceneActor> actorList = m_TypedActors[type];
        if (actorList.Count == 0)
            return;
        long[] serialIDList = new long[actorList.Count];
        for (int i = 0; i < serialIDList.Length; ++i)
        {
            serialIDList[i] = actorList[i].m_SerialID;
        }

        var hero = GetHeroActor();
        for (int j = 0; j < serialIDList.Length; ++j)
        {
            if (hero != null && serialIDList[j] == hero.m_SerialID)
            {
                continue;
            }
            Destroy(serialIDList[j], true);
        }
    }

    public GameObject GetActorRoot(int type)
    {
        if ((int)type >= 0 && type < ActorType.Count)
        {
            return m_ActorRoot[(int)type];
        }
        return null;
    }

    public virtual void ClearAll(bool includeHero = false)
    {
        for (int i = 0; i < m_ActorTypes.Length; ++i)
        {
            var actorType = m_ActorTypes[i];
            Clear(actorType);
        }
        if (includeHero)
        {
            DestroyHero();
        }

        m_ActorInstancePool.Clear(false);
    }

    protected string GetCurrentTypedActorIdList(int actorType, ref int count, ActorFilterCallback filterCallback = null, object filterParam = null)
    {
        count = 0;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        List<BaseSceneActor> sceneActors;
        sceneActors = m_TypedActors[actorType];
        if (sceneActors != null)
        {
            for (int i = 0; i < sceneActors.Count; ++i)
            {
                BaseSceneActor actor = sceneActors[i];

                if (actor != null && (filterCallback == null || filterCallback(actor, filterParam)))
                {
                    count += 1;
                    sb.Append(actor.m_SerialID).Append(",");
                }
            }
        }

        return sb.ToString();
    }

    protected static BaseSceneActor CreateActorCallback<T>() where T : BaseSceneActor, new()
    {
        T actor = new T();
        actor.Init();
        return actor;
    }

    public BaseSceneActor FetchFromPool(int actorType, object data)
    {
        BaseSceneActor sceneActor = m_TypedActorPools[actorType].Fetch() as BaseSceneActor;
        var builder = m_Builders[actorType];
        builder.PreBuild(sceneActor, data);
        return sceneActor;
    }

    //初始化角色 GameObject 实例回调函数
    public void _CreateActorInstanceCallback(InstanceObject instanceObject, string error, string resourcePath, System.Object param)
    {
        ActorInstance actorInstance = instanceObject as ActorInstance;
        ActorInstanceCallbackParam actorParam = param as ActorInstanceCallbackParam;

        BaseSceneActor actor = GetActorByID(actorParam.m_ActorSerialID);

        //如果创建实例失败
        if (actorInstance == null)
        {
            Debug.LogError("Failed to create the actor instance.:" + resourcePath);
            return;
        }

        //如果实例创建完成时 SceneActor 已被销毁
        if (actor == null)
        {
            m_ActorInstancePool.DestroyInstance(actorInstance);
            return;
        }

        if (actor.m_ActorInstance != null)
        {
            actor.NotifyUnInst();

            m_ActorInstancePool.DestroyInstance(actor.m_ActorInstance);
            actor.m_LoadedPrefabID = 0;
            actor.m_ActorInstance = null;
        }
        actor.m_ActorInstance = actorInstance;
        actor.m_LoadedPrefabID = actor.m_LoadingPrefabID;
        actorInstance.m_GameObject.name = actor.name + "_" + actor.m_SerialID;
        actorParam.m_ActorBuilder.Build(actor);

        if(actorParam.m_createInstanceCallBack != null)
        {
            actorParam.m_createInstanceCallBack(actor);
        }

        var actorInstanceGameObj = actor.gameObject;
        //actor.gameObject.layer = actorParam.m_layer;
        //actorInstanceGameObj.transform.position = actor.viewComponent.position;
        //actorInstanceGameObj.transform.rotation = actor.viewComponent.rotation;
        actorInstanceGameObj.transform.SetParent(actorParam.m_ActorRootGameObj.transform, false);

        actor.NotifyInst();
        RefreshCreateSight(actor);

        //增加更换模型后的通知
        if (actorParam.m_IsChangePrefab)
        {
            actor.NotifyAfterChangePrefab();
        }
        if (actorParam.m_createInstanceEndCallBack != null)
        {
            actorParam.m_createInstanceEndCallBack(actor);
        }
    }

    /// <summary>
    /// 创建低配画质下的实例失败后，尝试用创建高配画质下的实例
    /// </summary>
    /// <param name="error"></param>
    /// <param name="resourcePath"></param>
    /// <param name="param"></param>
    protected void _CreateInstanceFailCallBack(string error, string resourcePath, System.Object param)
    {
        ActorInstanceCallbackParam actorParam = param as ActorInstanceCallbackParam;
        if (actorParam.m_prefabInfo != null)
        {
            m_ActorInstancePool.CreateInstance(actorParam.m_prefabInfo.m_ResourcePath, _CreateActorInstanceCallback, _CreateActorInstanceCallback, actorParam, false);
        }
    }

    //刷新创建对象时候的事业
    public void RefreshCreateSight(BaseSceneActor actor)
    {
        //非StreamLoading或者不需要RefreshScene actor需要刷新视野
        if (!m_EnableStreamLoading || !actor.m_NeedRefreshScene)
        {
            actor.NotifyInSight();
            return;
        }
    }

    public void RefreshUpdateSight(BaseSceneActor actor)
    {
        actor.RefreshDistSign(GetHeroPos());
        ActorManagePolicy curPolicy = m_Policies[actor.GetSceneActorType()];
        //处理进出视野圈事件
        if (actor.gameObject != null && !actor.m_IsHide)
        {
            float actorSquaredDist = actor.m_DistHeroSq;
            //离开视野圈
            if (actorSquaredDist >= curPolicy.m_SightRadiusSQ && actor.mIsInSight)
            {
                actor.NotifyOutSight();
            }
            //进入视野圈
            else if (actorSquaredDist < curPolicy.m_SightRadiusSQ && !actor.mIsInSight)
            {
                actor.NotifyInSight();
            }

            if (actor.gameObject.activeSelf && !actor.mIsInSight)
            {
                actor.gameObject.SetActive(false);
            }
            else if (!actor.gameObject.activeSelf && actor.mIsInSight)
            {
                actor.gameObject.SetActive(true);
            }
        }
    }

    public void OnCreate(BaseSceneActor actor, object param)
    {
        m_TypedActors[(int)actor.GetSceneActorType()].Add(actor);
        if (actor.m_SerialID != 0)
        {
            if (m_DictActors.ContainsKey(actor.m_SerialID))
            {
                Debug.LogError("Actor Dictionary already contains key :" + actor.m_SerialID);
                return;
            }
            m_DictActors.Add(actor.m_SerialID, actor);
        }
        BaseSceneActor hero = GetHeroActor();
        actor.InitializeActorComponents();
        actor.InitData(param);
        actor.NotifyCreate();

        if (ActorCreateEvent != null)
        {
            ActorCreateEvent.Invoke(actor);
        }
    }

    public void SetPolicy(int type, float sightR, float instR, float prepareR)
    {
        if (m_Policies != null && type >= 0 && type < m_Policies.Length)
        {
            if (m_Policies[type] != null)
            {
                m_Policies[type].m_AssetPrepareRadius = prepareR;
                m_Policies[type].m_AssetPrepareRadiusSQ = prepareR * prepareR;
                m_Policies[type].m_InstantiateRadius = instR;
                m_Policies[type].m_InstantiateRadiusSQ = instR * instR;
                m_Policies[type].m_SightRadius = sightR;
                m_Policies[type].m_SightRadiusSQ = sightR * sightR;
            }
        }
    }

    public void GetPolicy(int type, out float sightR, out float instR, out float prepareR)
    {
        prepareR = m_Policies[type].m_AssetPrepareRadius;
        instR = m_Policies[type].m_InstantiateRadius;
        sightR = m_Policies[type].m_SightRadius;
    }

    //void SetFarDistancePolicy(float sightR, float instR, float prepareR)
    //{
    //    m_FarDistancePloicies.m_AssetPrepareRadius = prepareR;
    //    m_FarDistancePloicies.m_AssetPrepareRadiusSQ = prepareR * prepareR;
    //    m_FarDistancePloicies.m_InstantiateRadius = instR;
    //    m_FarDistancePloicies.m_InstantiateRadiusSQ = instR * instR;
    //    m_FarDistancePloicies.m_SightRadius = sightR;
    //    m_FarDistancePloicies.m_SightRadiusSQ = sightR * sightR;
    //}

    //设置角色类型，RefreshScene的频率
    public void SetActorUpdateTime(int type, float fUpdateTime)
    {
        if (type >= ActorType.Player && type < ActorType.Count)
        {
            m_ActorUpdateTime[type] = fUpdateTime;
        }
    }

    public void Update()
    {
        if (m_IsInited == false)
            return;

        OnUpdate();

        //刷新场景
        if (m_EnableStreamLoading)
        {
            RefreshScene();
        }

        //更新 Actors
        for (int typeIndex = 0; typeIndex < m_TypedActors.Length; ++typeIndex)
        {
            List<BaseSceneActor> actorList = m_TypedActors[typeIndex];
            if (actorList != null)
            {
                for (int actorIndex = 0; actorIndex < actorList.Count; ++actorIndex)
                {
                    BaseSceneActor actor = actorList[actorIndex];
                    if (actor != null)
                    {
                        actor.Update();
                    }
                }
            }
        }

        if (m_willDestroyList.Count > 0)
        {
            for (int i = 0; i < m_willDestroyList.Count; i++)
            {
                Destroy(m_willDestroyList[i], true);
            }
            m_willDestroyList.Clear();
        }
    }

    private int m_LastTraversEnumIndex = 0;
    private int m_LastCurTypeActorIndex = -1;

    void RefreshScene()
    {
        BaseSceneActor hero = GetHeroActor();
        if (hero == null)
        {
            return;
        }
        //继续上次的 m_LastTraversEnumIndex 索引值遍历不同的 Actor 类型
        int iActorTypesLength = m_ActorTypes.Length;
        Vector3 heroPos = GetHeroPos();
        for (int actorTypeIndex = m_LastTraversEnumIndex; actorTypeIndex < iActorTypesLength; ++actorTypeIndex)
        {
            m_LastTraversEnumIndex = actorTypeIndex;

            int curType = (int)m_ActorTypes[actorTypeIndex];
            List<BaseSceneActor> curTypeActors = m_TypedActors[curType];
            float curTypeUpdateTime = m_ActorUpdateTime[curType];

            //继续上次的 ActorIndex 索引值遍历该类型下的 Actor
            for (int actorIndex = m_LastCurTypeActorIndex + 1; actorIndex < curTypeActors.Count; ++actorIndex)
            {
                m_LastCurTypeActorIndex = actorIndex;

                //处理完当前 Actor 后是否需要跳转到下一帧
                bool isGotoNextFrame = false;

                BaseSceneActor actor = curTypeActors[actorIndex];
                //计算 Actor 与玩家的距离
                float actorSquaredDist = 0.0f;

                //英雄自己不要做多余的计算
                if (!actor.m_IsHero && actor.m_NeedRefreshScene)
                {
                    //当前角色（非主角）到达更新的时间再做更新
                    actor.m_LastRefreshSceneTime += Time.deltaTime;
                    if (actor.m_LastRefreshSceneTime > curTypeUpdateTime)
                    {
                        actor.m_LastRefreshSceneTime = actor.m_LastRefreshSceneTime % curTypeUpdateTime;
                    }
                    else
                    {
                        continue;
                    }
                    actor.RefreshDistSign(heroPos);
                    actorSquaredDist = actor.m_DistHeroSq;
                }
                else
                {
                    //英雄玩家始终显示
                    continue;
                }

                ActorManagePolicy curPolicy = m_Policies[curType];

                //如果角色进资源准备圈
                if (actorSquaredDist < curPolicy.m_AssetPrepareRadiusSQ)
                {
                    //获取骨骼资源信息
                    var actorPrefabInfo = actor.GetCurrentSceneActorPrefabInfoInQualtiySet();
                    if (actorPrefabInfo == null)
                    {
                        continue;
                    }
                    //判断是否需要重新创建角色实例（角色实例未创建，或需要替换骨骼资源。）
                    bool actorPrefabIDChanged = (actor.m_CurrentPrefabID != actor.m_LoadingPrefabID);

                    //英雄相关资源使用同步加载
                    bool useSyncLoad = false;

                    //如果角色未被设置为隐藏且需要创建新的角色实例
                    if (!actor.m_IsHide && actorPrefabIDChanged)
                    {

                        //通知资源管理器开始异步加载角色资源
                        {
                            if (m_ActorInstancePool.PrefetchResource(actorPrefabInfo.m_ResourcePath, useSyncLoad))
                            {
                                //如果当帧进行过资源预加载操作则跳转到下一帧
                                isGotoNextFrame = true;
                            }
                        }
                    }

                    //进入实例化圈
                    if (actorSquaredDist < curPolicy.m_InstantiateRadiusSQ)
                    {
                        //如果角色骨骼已变更，需要创建新的角色实例。
                        if (actorPrefabIDChanged)
                        {
                            //如果角色已被隐藏
                            if (!actor.m_IsHide)
                            {
                                //更换骨骼前通知
                                actor.NotifyBeforeChangePrefab();

                                ActorInstanceCallbackParam actorParam = new ActorInstanceCallbackParam();
                                actorParam.m_ActorSerialID = actor.m_SerialID;

                                actorParam.m_ActorRootGameObj = m_ActorRoot[curType];
                                actorParam.m_ActorBuilder = m_Builders[actorTypeIndex];
                                actorParam.m_IsChangePrefab = actorPrefabIDChanged;
                                actorParam.m_prefabInfo = actor.GetCurrentSceneActorPrefabInfo();
                                actorParam.m_createInstanceCallBack = actor.m_createInstanceCallBack;
                                actorParam.m_createInstanceEndCallBack = actor.m_createInstanceEndCallBack;
                                actor.m_LoadingPrefabID = actor.m_CurrentPrefabID;

                                m_ActorInstancePool.CreateInstance(actorPrefabInfo.m_ResourcePath, _CreateActorInstanceCallback, _CreateActorInstanceCallback,
                                    actorParam, useSyncLoad,_CreateInstanceFailCallBack);
                            }
                            //如果当帧进行过资源实例化操作则跳转到下一帧
                            isGotoNextFrame = true;
                        }
                        //如果角色骨骼未变更，且角色实例已创建。
                        else if (actor.m_ActorInstance != null)
                        {
                            //如果角色需要被隐藏
                            if (actor.m_IsHide)
                            {
                                //删除角色实例
                                DestroyOnlyGameObject(actor);
                            }
                        }

                        //处理进出视野圈事件
                        RefreshUpdateSight(actor);
                    }
                }
                //角色出资源准备圈
                else
                {
                    //角色实例已被创建且未被隐藏
                    if (actor.m_ActorInstance != null && !actor.m_IsHide)
                    {
                        //删除角色实例
                        DestroyOnlyGameObject(actor);
                    }
                }

                //跳转到下一帧再继续处理
                if (isGotoNextFrame)
                {
                    return;
                }
            }
            m_LastCurTypeActorIndex = -1;
        }
        m_LastTraversEnumIndex = 0;
    }

    public void DestroyOnlyGameObject(BaseSceneActor actor)
    {
        actor.NotifyUnInst();

        actor.m_LoadingPrefabID = 0;
        actor.m_LoadedPrefabID = 0;

        if (actor.m_ActorInstance != null)
        {
            m_ActorInstancePool.DestroyInstance(actor.m_ActorInstance);
            actor.m_ActorInstance = null;
        }
    }

    /// <summary>
    /// 加一个参数做及时删除，但是及时删除明显不能再SceneActor.Update()中调用，所以必须在所有的sceneActor已经update之后调用
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="isImmediate"></param>
    public void Destroy(long playerID, bool isImmediate = false)
    {
        BaseSceneActor actor = GetActorByID(playerID);
        if (actor != null)
        {
            actor.State = -1;
        }
        if (isImmediate)
        {
            if (actor != null)
            {
                OnDestroy(actor);
            }
        }
        else
        {
            m_willDestroyList.Add(playerID);
        }
    }

    private void OnDestroy(BaseSceneActor actor)
    {
        if (actor == null)
        {
            return;
        }

        if (ActorDestroyEvent != null)
        {
            ActorDestroyEvent.Invoke(actor);
        }
        actor.NotifyDestory();
        actor.UnInitializeActorComponents();

        m_Builders[(int)actor.GetSceneActorType()].Destroy(actor);
        if (actor.gameObject != null)
        {
            m_ActorInstancePool.DestroyInstance(actor.m_ActorInstance);
            actor.m_ActorInstance = null;
        }
        var actorList = m_TypedActors[(int)actor.GetSceneActorType()];
        for (int actorIndex = actorList.Count - 1; actorIndex >= 0; --actorIndex)
        {
            BaseSceneActor actorInList = actorList[actorIndex];
            if (actor.m_SerialID == actorInList.m_SerialID)
            {
                actorList.RemoveAt(actorIndex);
                break;
            }
        }

        m_DictActors.Remove(actor.m_SerialID);
        m_TypedActorPools[(int)actor.GetSceneActorType()].Store(actor);
    }

    public BaseSceneActor GetActorByID(long id)
    {
        BaseSceneActor hero = GetHeroActor();
        if (hero != null && hero.m_SerialID == id)
        {
            return hero;
        }

        BaseSceneActor foundActor = null;
        m_DictActors.TryGetValue(id, out foundActor);
        return foundActor;
    }

    public List<BaseSceneActor> GetActorByType(int type)
    {
        if (type >= 0 && type < m_TypedActors.Length)
        {
            return m_TypedActors[type];
        }

        return null;
    }

    public string GetCurrentPlayerIdList(ref int count)
    {
        return GetCurrentTypedActorIdList(ActorType.Player, ref count);
    }

    public int GetActorActiveInstanceCountInSight()
    {
        int nActorInstanceCount = 0;

        List<BaseSceneActor> listSceneActor = null;
        for (int i = (int)ActorType.Player; i <= (int)ActorType.NPC; ++i)
        {
            listSceneActor = m_TypedActors[i];
            if (listSceneActor != null)
            {
                for (int j = 0; j < listSceneActor.Count; ++j)
                {
                    BaseSceneActor sceneActor = listSceneActor[j];
                    if (sceneActor != null && sceneActor.gameObject != null)
                    {
                        nActorInstanceCount++;
                    }
                }
            }
        }

        return nActorInstanceCount;
    }

    //根据GameObject 反查SceneActor，
    //actorType = None查询所有类型，填写已知类型查询对应类型效率更高
    public BaseSceneActor GetActorByGameObject(GameObject gobj, int actorType = ActorType.None)
    {
        if (gobj == null)
        {
            return null;
        }

        if (actorType == ActorType.None)
        {
            for (int i = 0; i < m_TypedActors.Length; ++i)
            {
                List<BaseSceneActor> lst = m_TypedActors[i];
                if (lst != null)
                {
                    for (int j = 0; j < lst.Count; ++j)
                    {
                        BaseSceneActor actor = lst[j];
                        if (actor.gameObject != null && actor.gameObject.GetInstanceID() == gobj.GetInstanceID())
                        {
                            return actor;
                        }
                    }
                }
            }
        }
        else
        {
            List<BaseSceneActor> lst = m_TypedActors[(int)actorType];
            if (lst != null)
            {
                for (int j = 0; j < lst.Count; ++j)
                {
                    BaseSceneActor actor = lst[j];
                    if (actor.gameObject != null && actor.gameObject.GetInstanceID() == gobj.GetInstanceID())
                    {
                        return actor;
                    }
                }
            }
        }

        return null;
    }

    public void CreateActorRoot(int type, string name)
    {
        GameObject actorRoot = new GameObject();
        actorRoot.name = name;
        m_ActorRoot[type] = actorRoot;
        GameObject.DontDestroyOnLoad(actorRoot);
        OnCreateActorRoot(actorRoot);
    }

    protected virtual void OnCreateActorRoot(GameObject actorRoot)
    {

    }

    /// <summary>
    /// 删除除自己之外的其他玩家
    /// </summary>
    public void DestroyOtherPlayer()
    {
        for (int i = 0; i < m_TypedActors[(int)ActorType.Player].Count; i++)
        {
            if (m_TypedActors[(int)ActorType.Player][i] == GetHeroActor())
                continue;
            ActorManager.Instance.Destroy(m_TypedActors[(int)ActorType.Player][i].m_SerialID);
        }
    }

    /// <summary>
    /// 删除一个类型的生物
    /// </summary>
    /// <param name="type">生物的类型</param>
    public void DestroyAllActorByType(int type)
    {
        if (type >= 0 && type < m_TypedActors.Length)
        {
            int count = m_TypedActors[type].Count;
            for (int i = 0; i < count; i++)
            {
                ActorManager.Instance.Destroy(m_TypedActors[type][0].m_SerialID, true);
            }
        }
    }

    /// <summary>
    /// 删除英雄
    /// </summary>
    private void DestroyHero()
    {
        if (GetHeroActor() != null)
            Destroy(GetHeroActor().m_SerialID, true);
    }
    }
