/************************************************************
//     文件名      : InstancePool.cs
//     功能描述    : 场景GameObject对象池
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/07/01.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

//开启 InstancePool 调试信息
//#define DEBUG_INSTANCE_POOL

//通过时间释放资源
//#define FREE_RESOURCE_BY_TIME

using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 初始化实例对象回调函数
/// </summary>
/// <param name="instanceObject">创建完成的实例对象</param>
/// <param name="param">自定义初始化参数</param>
public delegate void InitializeInstanceCallback(InstanceObject instanceObject, string error, string resourcePath, System.Object param);

/// <summary>
/// 复用实例对象回调函数
/// </summary>
/// <param name="instanceObject">复用的实例对象</param>
/// <param name="param">自定义复用参数</param>
public delegate void ReuseInstanceCallback(InstanceObject instanceObject, string error, string resourcePath, System.Object param);

/// <summary>
/// 回收实例对象回调函数
/// </summary>
/// <param name="instanceObject">将被回收的实例对象</param>
/// <param name="param">自定义回收参数</param>
public delegate void RecycleInstanceCallback(InstanceObject instanceObject, System.Object param);
/// <summary>
/// 初始化实例失败回调函数
/// </summary>
/// <param name="error"></param>
/// <param name="resourcePath"></param>
/// <param name="param"></param>
public delegate void InitializeInstanceFailCallback(string error, string resourcePath, System.Object param);
//实例对象
public class InstanceObject
{
    //实例状态
    public enum State
    {
        OnResourceLoading,  //资源异步加载中
        Instantiated,       //已被实例化，正常使用中。
        Recycled,           //实例已被回收
        Destroyed,          //实例已被销毁
    }

    public string m_ResourcePrefabPath;     //实例资源 Prefab 路径
    public GameObject m_GameObject;         //实例 GameObject 对象
    public State m_eState;                  //实例状态（以防止在资源异步加载完成前实例就被回收或销毁时产生逻辑错误）

    public IInstancePool m_InstancePool;

    public void Destroy()
    {
        if (m_InstancePool != null)
        {
            if (m_eState == State.Instantiated)
            {
                m_InstancePool.DestroyInstanceObject(this);
            }
        }
    }
}


public interface IInstancePool
{
    void Initialize(string cachedRootNodeName, int nMaxCachedInstance = 20, float fInstanceMaxCacheTime = 30.0f, float fResourceCheckInterval = 1.0f, float fResourceMaxCacheTime = 30.0f, bool isDestroybyTime = false);
    void LateUpdate();
    bool PrefetchResource(string resourcePrefabPath, bool bSyncLoad = true, ResourceLoadedCallback callBack = null);
    bool IsResourceLoaded(string resourcePrefabPath);
    void DestroyInstanceObject(object instanceObject, System.Object param = null);
}


/// <summary>
/// 实例对象池
/// 
/// 实例对象池在实例 GameObject 层面和资源层面分别进行了缓存管理。当需要创建一个时会首先异步加载其资源，再创建其实例。
/// 
/// 每个资源都会记录它同时被多少个实例所引用。当一个资源没有被任何实例所引用时，它就会被加入待删除检测队列，并在超过一定时间后被卸载。
/// 
/// 当一个实例被删除时也不会被立即销毁，而是先被反激活，然后回收到了实例对象池中。如果一定时间内又需要创建同类的实例，
/// 则会从池中直接取出先前被回收的实例对象，并在重新设置参数后激活使用。长时间未被使用的实例依然会被实际销毁。
/// 
/// 由于资源是异步加载的，调用 CreateInstance 函数首次创建实例时会有一定的延迟。如果要避免延迟应在实际创建时提前调用 PrefetchResource 函数预加载实例的资源。
/// </summary>
public class InstancePool<T> : IInstancePool
    where T : InstanceObject, new()
{
    public int m_nMaxCachedInstance = 20;              //最大缓存实例数量
    public float m_fInstanceMaxCacheTime = 30.0f;      //实例最大缓存时间
    public float m_fResourceCheckInterval = 1.0f;      //资源检查间隔时间
    public float m_fResourceMaxCacheTime = 30.0f;      //资源最大缓存时间
    private float m_fResourceLazyUpdateTime = 0.0f;
    public bool m_destroyByTime = false;
    private List<string> m_ToUnloadResourceList = new List<string>();

    private bool m_IsPrintDebugInfo = false;
    //实例资源
    protected class InstanceResource
    {
        public GameObject m_Prefab;     //资源 Prefab 原始对象
        public int m_nRefCount;         //资源引用计数
        public float m_fUnusedTime;     //未使用计时
        public bool m_bManuallyUnload;  //是否手动卸载资源
    }

    //创建实例时使用的加载资源回调函数参数
    protected class ResourceLoadedParam
    {
        public T m_InstanceObject;  //实例对象
        public System.Object m_InitParam;       //实例对象创建参数
    }

    //实例初始化参数
    protected class InstanceInitParam
    {
        public InitializeInstanceCallback m_InitCallback;
        public ReuseInstanceCallback m_ReuseCallback;
        public InitializeInstanceFailCallback m_FailCallBack;
        public System.Object m_Param;
        public bool m_bSyncLoad;
    }

    //实例反初始化参数
    protected class InstanceUninitParam
    {
        public RecycleInstanceCallback m_RecycleCallback;
        public System.Object m_Param;
    }

    /// <summary>
    /// 初始化实例对象池
    /// </summary>
    /// <param name="cachedRootNodeName">缓存实例的根节点名称。</param>
    /// <param name="nMaxCachedInstance">最大缓存实例数量。当同时回收的实例数量超过该值后将不再回收实例对象。</param>
    /// <param name="fInstanceMaxCacheTime">实例最大缓存时间。超过该时间且未被复用的实例将被销毁。</param>
    /// <param name="fResourceCheckInterval">资源检查间隔时间。每隔多长时间遍历一遍未使用的资源。</param>
    /// <param name="fResourceMaxCacheTime">资源最大缓存时间。超过该时间且未被使用的资源将被卸载。</param>
    public void Initialize(string cachedRootNodeName, int nMaxCachedInstance = 20, float fInstanceMaxCacheTime = 30.0f,
        float fResourceCheckInterval = 1.0f, float fResourceMaxCacheTime = 30.0f,
        bool isDestroybyTime = false)
    {
        m_CachedRootGameObj = new GameObject(cachedRootNodeName);
        GameObject.DontDestroyOnLoad(m_CachedRootGameObj);
        m_destroyByTime = isDestroybyTime;
        m_nMaxCachedInstance = nMaxCachedInstance;
        m_fInstanceMaxCacheTime = fInstanceMaxCacheTime;
        m_fResourceCheckInterval = fResourceCheckInterval;
        m_fResourceMaxCacheTime = fResourceMaxCacheTime;

        m_InstanceObjectPool.Initialize(nMaxCachedInstance, _CreateInstanceCallback, _DestroyInstanceCallback, _ReuseInstanceCallback, _RecycleInstanceCallback);
    }

    /// <summary>
    /// 获取资源数量
    /// </summary>
    /// <returns>资源数量</returns>
    public int GetResourceCount()
    {
        return m_MapResources.Count;
    }

    /// <summary>
    /// 获取已缓存实例数量
    /// </summary>
    /// <returns>已缓存实例数量</returns>
    public int GetCachedInstanceCount()
    {
        return m_InstanceObjectPool.GetObjectCount();
    }

    private static int m_destroyNum = 0;

    /// <summary>
    /// 遍历缓存的实例和资源。必须每帧被调用。
    /// </summary>
    public void LateUpdate()
    {
        if (m_destroyByTime)
        {
            //销毁长时间未使用的实例
            m_InstanceObjectPool.DestroyObjectsByTime(Time.deltaTime, m_fInstanceMaxCacheTime);

            //定时删除缓存的资源
            m_fResourceLazyUpdateTime += Time.deltaTime;

            //每间隔一段时间才检查一次
            if (m_fResourceLazyUpdateTime > m_fResourceCheckInterval)
            {
                m_ToUnloadResourceList.Clear();

                var resourceEnumerator = m_MapResources.GetEnumerator();
                while (resourceEnumerator.MoveNext())
                {
                    var entry = resourceEnumerator.Current;

                    //对于没有在使用中的资源累计未使用时间
                    if (entry.Value.m_nRefCount <= 0)
                    {
                        entry.Value.m_fUnusedTime += m_fResourceLazyUpdateTime;

                        //如果不是手动卸载的资源
                        if (!entry.Value.m_bManuallyUnload)
                        {
                            //超过最大缓存时间则加入删除列表
                            if (entry.Value.m_fUnusedTime >= m_fResourceMaxCacheTime)
                            {
                                m_ToUnloadResourceList.Add(entry.Key);
                            }
                        }
                    }
                }

                m_fResourceLazyUpdateTime = 0.0f;

                //从缓存列表中删除资源
                for (int nEntry = 0; nEntry < m_ToUnloadResourceList.Count; ++nEntry)
                {
                    string resourcePath = m_ToUnloadResourceList[nEntry];
                    InstanceResource instanceRes = m_MapResources[resourcePath];
                    m_MapResources.Remove(resourcePath);
                    instanceRes.m_Prefab = null;
                    instanceRes = null;
                    m_destroyNum++;
#if DEBUG_INSTANCE_POOL
                Debug.Log("InstancePool : Unload the instance resource of '" + resourcePath + "'.");
#endif
                }

                m_ToUnloadResourceList.Clear();
            }

        }
    }


    private static void ClearUnusedAssets()
    {
        if (m_destroyNum > 40)
        {
            m_destroyNum = 0;
            Resources.UnloadUnusedAssets();
        }
    }

    /// <summary>
    /// 预取指定的资源。提前缓存资源的可以在调用 CreateInstance 函数后被立即创建，无需等待异步加载完成。（如果预缓存后长时间未调用创建函数资源依然会被卸载。）
    /// </summary>
    /// <param name="m_ResourcePrefabPath">指定加载的资源路径</param>
    /// <param name="bSyncLoad">true 为同步加载，false 为异步加载。</param>
    /// <returns>如实际进行了资源预加载则返回 true，否则返回 false。</returns>
    public bool PrefetchResource(string resourcePrefabPath, bool bSyncLoad = true, ResourceLoadedCallback callBack = null)
    {
        if (string.IsNullOrEmpty(resourcePrefabPath))
        {
            Debug.LogError("指定预取的资源路径为空。");
            if (callBack != null)
            {
                callBack(null, "path is null or empty", resourcePrefabPath, null);
            }
            return false;
        }
        //如果资源已在缓存中则直接返回
        InstanceResource instanceResource = null;
        if (m_MapResources.TryGetValue(resourcePrefabPath, out instanceResource))
        {
            if (callBack != null)
            {
                callBack(instanceResource.m_Prefab, "success", resourcePrefabPath, null);
            }
            return false;
        }
        if (bSyncLoad)
        {
            ResourceManager.Instance.LoadResource(resourcePrefabPath, typeof(GameObject),
                (UnityEngine.Object resource, string error, string resourcePath, System.Object customParam) =>
                {
                    _PrefetchInstanceResourceLoadedCallback(resource, error, resourcePath, customParam);
                    if (callBack != null)
                    {
                        callBack(resource, error, resourcePath, customParam);
                    }
                }
                , null);
        }
        else
        {
            //异步加载资源
            ResourceManager.Instance.LoadResourceAsync(resourcePrefabPath, typeof(GameObject), 
                (UnityEngine.Object resource, string error, string resourcePath, System.Object customParam) =>
                {
                    _PrefetchInstanceResourceLoadedCallback(resource, error, resourcePath, customParam);
                    if (callBack != null)
                    {
                        callBack(resource, error, resourcePath, customParam);
                    }
                }, null);
        }
        return true;
    }

    /// <summary>
    /// 判断指定的资源是否已经加载完毕
    /// </summary>
    /// <param name="m_ResourcePrefabPath"></param>
    /// <returns>指定的资源已加载完毕则返回 true</returns>
    public bool IsResourceLoaded(string resourcePrefabPath)
    {
        InstanceResource instanceResource = null;
        if (m_MapResources.TryGetValue(resourcePrefabPath, out instanceResource))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 设置资源是否为手动卸载（先前被标记为手动卸载的资源必须通过该函数取消手动卸载设置后才能被正常卸载）
    /// </summary>
    /// <param name="m_ResourcePrefabPath">指定的资源路径</param>
    /// <param name="bIsManuallyUnload">设置资源是否为手动卸载</param>
    public void SetResourceManuallyUnload(string resourcePrefabPath, bool bIsManuallyUnload)
    {
        InstanceResource instanceResource = null;
        if (m_MapResources.TryGetValue(resourcePrefabPath, out instanceResource))
        {
            instanceResource.m_bManuallyUnload = bIsManuallyUnload;
        }
    }

    /// <summary>
    /// 创建实例对象。如实例的资源尚未加载，该函数会首先异步加载资源，并在加载完成后自动创建实例。此时立即返回的 InstanceObject 对象无法获取的 GameObject。
    /// </summary>
    /// <param name="resourcePath">实例对象资源路径</param>
    /// <param name="initCallback">实例初始化回调函数。当创建全新实例时会调用该函数。</param>
    /// <param name="reuseCallback">实例复用回调函数。当复用先前被回收的实例时会调用该函数。</param>
    /// <param name="param">传给实例初始化和复用回调函数的自定义参数</param>
    /// <param name="bSyncLoad">true 为同步加载，false 为异步加载。</param>
    /// <returns>创建的对象。如异步加载尚未完成其 gameObject 属性为空。</returns>
    public T CreateInstance(string resourcePath, InitializeInstanceCallback initCallback, ReuseInstanceCallback reuseCallback, System.Object param, bool bSyncLoad = false, InitializeInstanceFailCallback failCallBack = null)
    {
        //通过对象池加载实例（如有回收的可复用对象实例则不重新创建）
#if DEBUG_INSTANCE_POOL
        Debug.Log("InstancePool : Create the instance object of resource '" + resourcePath + "'.");
#endif

        InstanceInitParam initParam = new InstanceInitParam();
        initParam.m_InitCallback = initCallback;
        initParam.m_ReuseCallback = reuseCallback;
        initParam.m_FailCallBack = failCallBack;
        initParam.m_Param = param;
        initParam.m_bSyncLoad = bSyncLoad;

        T instanceObject = m_InstanceObjectPool.LoadObject(resourcePath, initParam);
        return instanceObject;
    }

    /// <summary>
    /// 销毁实例对象。如未超过对象池的实例最大缓存数量，并不会被实际删除，而是从父节点断开，并被设置为非激活状态回收到对象池中。
    /// </summary>
    /// <param name="instanceObject">指定销毁的对象</param>
    /// <param name="recycleCallback">实例回收回调函数</param>
    /// <param name="param">传给实例回收回调函数的自定义参数</param>
    public void DestroyInstance(T instanceObject,/* RecycleInstanceCallback recycleCallback = null, */System.Object param = null)
    {
#if DEBUG_INSTANCE_POOL
        Debug.Log("InstancePool : Destroy the instance object of resource '" + instanceObject.m_ResourcePrefabPath + "'.");
#endif
        if (instanceObject == null)
        {
            return;
        }

        //如果实例在删除时资源尚未加载完成则不进行回收
        if (instanceObject.m_GameObject == null)
        {
            instanceObject.m_eState = InstanceObject.State.Destroyed;

#if DEBUG_INSTANCE_POOL
        Debug.Log("InstancePool : Marks the instance object of '" + instanceObject.m_ResourcePrefabPath + "' with null GameObject was destroyed.");
#endif
            return;
        }

        InstanceUninitParam uninitParam = new InstanceUninitParam();
        //uninitParam.m_RecycleCallback = recycleCallback;
        uninitParam.m_Param = param;

        m_InstanceObjectPool.UnloadObject(instanceObject.m_ResourcePrefabPath, instanceObject, uninitParam);
    }

    public void DestroyInstanceObject(object instanceObject, System.Object param = null)
    {
        DestroyInstance(instanceObject as T, param);
    }

    /// <summary>
    /// 清空未使用的实例及资源缓存
    /// </summary>
    /// <param name="bClearManuallyUnloadResources">使用清空被标记为手动卸载的资源</param>
    public void Clear(bool bClearManuallyUnloadResources)
    {
#if DEBUG_INSTANCE_POOL
        Debug.Log("InstancePool : Clear all instance resources.");
#endif

        //清空实例对象池
        m_InstanceObjectPool.Clear();

        //清空缓存的未使用资源
        List<string> deleteResourceList = new List<string>();

        foreach (var entry in m_MapResources)
        {
            if (entry.Value.m_nRefCount <= 0)
            {
                if (bClearManuallyUnloadResources || !entry.Value.m_bManuallyUnload)
                {
                    deleteResourceList.Add(entry.Key);
                }
            }
        }

        for (int nEntry = 0; nEntry < deleteResourceList.Count; ++nEntry)
        {
            string resourcePath = deleteResourceList[nEntry];
            //InstanceResource instanceRes = m_MapResources[resourcePath];

            m_MapResources.Remove(resourcePath);

            //TODO: 如果将来能修改 Unity 引擎提供卸载 GameObject 资源的接口则在这里进行卸载
            //resourceManager.UnloadResource(instanceRes.m_Prefab);
        }
    }

    //创建实例对象回调函数
    protected T _CreateInstanceCallback(string resourcePrefabPath, System.Object param)
    {
        T instanceObject = new T();
        instanceObject.m_InstancePool = this;
        instanceObject.m_ResourcePrefabPath = resourcePrefabPath;
        instanceObject.m_GameObject = null;
        instanceObject.m_eState = InstanceObject.State.OnResourceLoading;

        var initParam = param as InstanceInitParam;

        //如果资源已缓存
        InstanceResource instanceResource = null;
        if (m_MapResources.TryGetValue(resourcePrefabPath, out instanceResource))
        {
#if DEBUG_INSTANCE_POOL
            Debug.Log("InstancePool : Create a new instance from loaded resource '" + instanceObject.m_ResourcePrefabPath + "'.");
#endif

            _InstantiateObject(instanceObject, instanceResource, null, initParam);
        }
        //如果资源未加载
        else
        {
#if DEBUG_INSTANCE_POOL
            Debug.Log("InstancePool : Create a new instance by loading resource '" + instanceObject.m_ResourcePrefabPath + "'.");
#endif

            //异步加载资源并指定加载完成回调函数
            ResourceLoadedParam resourceLoadedParam = new ResourceLoadedParam();
            resourceLoadedParam.m_InstanceObject = instanceObject;
            resourceLoadedParam.m_InitParam = initParam;

            if (initParam.m_bSyncLoad)
            {
                ResourceManager.Instance.LoadResource(resourcePrefabPath, typeof(GameObject), _CreateInstanceResourceLoadedCallback, resourceLoadedParam);
            }
            else
            {
                ResourceManager.Instance.LoadResourceAsync(resourcePrefabPath, typeof(GameObject), _CreateInstanceResourceLoadedCallback, resourceLoadedParam);
            }
        }

        return instanceObject;
    }

    //销毁实例对象回调函数
    protected void _DestroyInstanceCallback(T instanceObject)
    {
#if DEBUG_INSTANCE_POOL
        Debug.Log("InstancePool : Destroy the instance (GameObjID = {0}) of resource '{1}'.",
            instanceObject.m_GameObject.GetInstanceID(), instanceObject.m_ResourcePrefabPath);
#endif

        //销毁 GameObject 实例
        if (instanceObject.m_GameObject != null)
        {
            GameObject.Destroy(instanceObject.m_GameObject);
            instanceObject.m_GameObject = null;
        }

        instanceObject.m_eState = InstanceObject.State.Destroyed;

        //减少资源引用
        InstanceResource instanceResource = null;
        if (m_MapResources.TryGetValue(instanceObject.m_ResourcePrefabPath, out instanceResource))
        {
            if (instanceResource.m_nRefCount <= 0)
            {
                Debug.LogError("InstancePool : Decreases the reference count of instance resource '" + instanceObject.m_ResourcePrefabPath +
                    "' while destroying the instance, but the reference count is already zero.");
            }
            --instanceResource.m_nRefCount;
            if (!m_destroyByTime)
            {
                if (instanceResource.m_nRefCount <= 0)
                {
                    m_MapResources.Remove(instanceObject.m_ResourcePrefabPath);
                }

#if DEBUG_INSTANCE_POOL
            Debug.Log("InstancePool : Unload the instance resource of '" + instanceObject.m_ResourcePrefabPath + "'.");
#endif

                //TODO: 如果将来能修改 Unity 引擎提供卸载 GameObject 资源的接口则在这里进行卸载
                //resourceManager.UnloadResource(instanceResource.m_Prefab);

            }
        }
    }

    //重用实例对象回调函数
    protected void _ReuseInstanceCallback(T instanceObject, System.Object param)
    {
#if DEBUG_INSTANCE_POOL
        Debug.Log("InstancePool : Reuse the instance (GameObjID = {0}) of resource '{1}'.",
            instanceObject.m_GameObject.GetInstanceID(), instanceObject.m_ResourcePrefabPath);
#endif

        //如果资源已载入完成并且被实例化（如果此时未加载完成，当加载完成后回调函数会使用上面赋值的新参数值正常地设置实例属性。）
        if (instanceObject.m_GameObject != null)
        {
            instanceObject.m_eState = InstanceObject.State.Instantiated;

#if DEBUG_INSTANCE_POOL
            Debug.Log("InstancePool : Initialize the instance of resource '" + instanceObject.m_ResourcePrefabPath + "'.");
#endif
            //激活对象
            instanceObject.m_GameObject.SetActive(true);

            //调用复用实例回调
            InstanceInitParam initParam = param as InstanceInitParam;
            if (initParam.m_ReuseCallback != null)
            {
                initParam.m_ReuseCallback(instanceObject, null, instanceObject.m_ResourcePrefabPath, initParam.m_Param);
            }
        }
    }

    //回收实例对象回调函数
    protected void _RecycleInstanceCallback(T instanceObject, System.Object param)
    {
#if DEBUG_INSTANCE_POOL
        Debug.Log("InstancePool : Recycle the instance (GameObjID = {0}) of resource '{1}'.",
            instanceObject.m_GameObject.GetInstanceID(), instanceObject.m_ResourcePrefabPath);
#endif

        instanceObject.m_eState = InstanceObject.State.Recycled;

        //如果实例对象已被创建
        if (instanceObject.m_GameObject != null)
        {
            //断开实例父节点并设置为非激活状态
            instanceObject.m_GameObject.transform.SetParent(m_CachedRootGameObj.transform, false);
            instanceObject.m_GameObject.SetActive(false);

            ////调用回收实例回调
            //InstanceUninitParam uninitParam = param as InstanceUninitParam;
            //if (uninitParam.m_RecycleCallback != null)
            //{
            //    uninitParam.m_RecycleCallback(instanceObject, uninitParam.m_Param);
            //}
        }
    }

    //处理加载完成的资源
    protected InstanceResource _OnResourceLoaded(UnityEngine.Object resource, string error, string resourcePath, bool bManuallyUnload)
    {
        //检查载入结果
        if (resource == null)
        {
            if(m_IsPrintDebugInfo)
                Debug.LogError("InstancePool : Failed to load instance resource from path '" + resourcePath + "'. ResourceManager Error: " + error);
            return null;
        }

        GameObject instancePrefab = resource as GameObject;
        if (instancePrefab == null)
        {
            if (m_IsPrintDebugInfo)
                Debug.LogError("InstancePool : The loaded instance resource '" + resourcePath + "' is not a GameObject.");
            return null;
        }

        //如果资源尚未缓存
        InstanceResource instanceResource = null;
        if (!m_MapResources.TryGetValue(resourcePath, out instanceResource))
        {
            //将资源加入缓存列表
            instanceResource = new InstanceResource();
            instanceResource.m_Prefab = instancePrefab;
            instanceResource.m_nRefCount = 0;
            instanceResource.m_fUnusedTime = 0.0f;
            instanceResource.m_bManuallyUnload = bManuallyUnload;
            m_MapResources.Add(resourcePath, instanceResource);
        }

        return instanceResource;
    }

    //预取资源时的加载完成回调函数
    protected void _PrefetchInstanceResourceLoadedCallback(UnityEngine.Object resource, string error, string resourcePath, System.Object customParam)
    {
        bool bManuallyUnload = (customParam != null) ? (bool)customParam : false;
        InstanceResource instanceResource = _OnResourceLoaded(resource, error, resourcePath, bManuallyUnload);
        if (instanceResource == null)
        {
            return;
        }

#if DEBUG_INSTANCE_POOL
        Debug.Log("InstancePool : Prefetched resource '" + resourcePath + "'.");
#endif
    }

    //创建时的资源加载完成回调函数
    protected void _CreateInstanceResourceLoadedCallback(UnityEngine.Object resource, string error, string resourcePath, System.Object customParam)
    {
        ResourceLoadedParam resourceLoadedParam = customParam as ResourceLoadedParam;
        InstanceInitParam initParam = resourceLoadedParam.m_InitParam as InstanceInitParam;

        InstanceResource instanceResource = _OnResourceLoaded(resource, error, resourcePath, false);
        if (instanceResource == null)
        {
            if (initParam.m_FailCallBack != null)
                initParam.m_FailCallBack(error, resourcePath, initParam.m_Param);
            else if (initParam.m_InitCallback != null)
                initParam.m_InitCallback(null, error, resourcePath, initParam.m_Param);
            return;
        }

#if DEBUG_INSTANCE_POOL
        Debug.Log("InstancePool : Loaded resource '" + resourcePath + "'.");
#endif

        //实例化对象
        _InstantiateObject(resourceLoadedParam.m_InstanceObject, instanceResource, error, initParam);
    }

    //使用资源实例化对象
    protected void _InstantiateObject(T instanceObject, InstanceResource instanceResource, string error, InstanceInitParam initParam)
    {
#if DEBUG_INSTANCE_POOL
        Debug.Log("InstancePool : Instantiate the game object of resource '{0}'.", instanceObject.m_ResourcePrefabPath);
#endif

        switch (instanceObject.m_eState)
        {
            //资源正常加载完成
            case InstanceObject.State.OnResourceLoading:
                {
                    //增加资源引用计数
                    ++instanceResource.m_nRefCount;
                    instanceResource.m_fUnusedTime = 0.0f;

                    //实例化资源
                    instanceObject.m_GameObject = GameObject.Instantiate(instanceResource.m_Prefab) as GameObject;

                    //设置特效对象在切换场景时不自动销毁
                    //UnityEngine.Object.DontDestroyOnLoad(instanceObject.m_GameObject);

                    instanceObject.m_eState = InstanceObject.State.Instantiated;

#if DEBUG_INSTANCE_POOL
                    Debug.Log("InstancePool : Initialize the instance game object (GameObjID = {0}) of resource '{1}'.",
                        instanceObject.m_GameObject.GetInstanceID(), instanceObject.m_ResourcePrefabPath);
#endif
                    if (initParam.m_InitCallback != null)
                    {
                        initParam.m_InitCallback(instanceObject, error, instanceObject.m_ResourcePrefabPath, initParam.m_Param);
                    }
                    break;
                }
            //实例已被创建
            case InstanceObject.State.Instantiated:
                {
                    Debug.LogError("InstancePool : Couldn't initialize an instance of resource '" + instanceObject.m_ResourcePrefabPath + "' which has been initialized already.");
                    break;
                }
            //如果已被回收
            case InstanceObject.State.Recycled:
                {
                    //如果该实例在回收前资源已加载完成并被实例化
                    if (instanceObject.m_GameObject != null)
                    {
                        //断开实例父节点并设置为非激活状态
                        instanceObject.m_GameObject.transform.SetParent(null, false);
                        instanceObject.m_GameObject.SetActive(false);
                    }
                    //如果该实例在被回收前资源尚未加载完成则不做处理
                    else
                    {
                        Debug.LogError("InstancePool : Try to initialize an instance of resource '" + instanceObject.m_ResourcePrefabPath + "' which has been recycled but no game object.");
                    }
                    break;
                }
            //如果已被销毁
            case InstanceObject.State.Destroyed:
                {
                    break;
                }
        }
    }

    //资源表
    protected Dictionary<string, InstanceResource> m_MapResources = new Dictionary<string, InstanceResource>();

    //实例对象池
    protected KeyObjectPool<string, T> m_InstanceObjectPool = new KeyObjectPool<string, T>();

    //已回收实例的根节点
    protected GameObject m_CachedRootGameObj;

#if FREE_RESOURCE_BY_TIME

    //资源延时更新累计时间
    protected float m_fResourceLazyUpdateTime = 0.0f;

    //超时需要卸载的资源列表
    protected List<string> m_ToUnloadResourceList = new List<string>();

#endif // #if FREE_RESOURCE_BY_TIME
}

