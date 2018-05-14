/************************************************************
//     文件名      : ResourceManager.cs
//     功能描述    : 资源管理类
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/07/11.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/
//#define RES_LOG

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Core.CAssetBundleData;
using LitJson;

public delegate void ResourceLoadedCallback(UnityEngine.Object resource, string error, string resourcePath, System.Object customParam);
public delegate void SceneLoadingCallback(string sceneName, string error, float progress, bool isDone);

public delegate byte[] ReadFileCallback(string path);

public class Bundle
{
    public AssetBundle m_AssetBundle = null;   //使用 LoadFromFileAsync 477加载时存储变量
}

public class ResourceBundle
{
    public string m_Name;       // AssetBundle 名称
    public Bundle m_Bundle = null;     // AssetBundle 封装对象
    public int m_nRefCount = 0;     //引用计数
}

public class ResourceBundleSet
{
    public ResourceBundle m_MainAssetBundle;                    //直接存储该资源的 AssetBundle
    public ResourceBundle[] m_ArrayDependentAssetBundles;       //资源间接依赖的其它 AssetBundle
}

public class CheckSceneLoadingStatusParam
{
    public string m_SceneName;
    public SceneLoadingCallback m_SceneLoadingCallback;
    public AsyncOperation m_AsyncOperation;
    public ResourceBundleSet m_ResourceBundles = null;
}

public class CheckResourceLoadingStatusParam
{
    public ResourceLoader m_ResourceLoader;
    public ResourceLoadedCallback m_ResourceLoadedCallback;
    public System.Object m_CallbackCustomParam;
}
public enum EnumAB
{
    shaders_ab,
    uialtas_ab,
    ui_ab,
    model_ab,
    scenemodle_ab,
    scene_ab,
    play_ab,
}

public class ResourceLoader
{
    //通过 AssetBundle.LoadAsync 异步加载资源使用此构造函数
    public ResourceLoader(string resourcePath, Type resourceType, string resourceLoadPath, ResourceBundleSet resourceBundle)
    {
        m_ResourcePath = resourcePath;
        m_ResourceType = resourceType;
        m_LoadedObject = null;
        m_ResourceLoadPath = resourceLoadPath;
        m_ResourceBundles = resourceBundle;
        m_ResourceRequest = null;
    }

    //通过 Resources.LoadAsync 异步加载资源使用此构造函数
    public ResourceLoader(string resourcePath, Type resourceType, string resourceLoadPath, UnityEngine.ResourceRequest resourceRequest)
    {
        m_ResourcePath = resourcePath;
        m_ResourceType = resourceType;
        m_LoadedObject = null;
        m_ResourceLoadPath = resourceLoadPath;
        m_ResourceBundles = null;
        m_ResourceRequest = resourceRequest;
    }

    //是否完成加载（完成时有可能发生错误）
    public bool isDone
    {
        get
        {
            //如果已经加载资源
            if (m_LoadedObject != null)
            {
                return true;
            }

            //如果是通过 AssetBundle.LoadAsync 异步加载资源
            if (m_ResourceBundles != null)
            {
                //是否已经开始加载 AssetBundle 中的资源
                if (m_AssetBundleRequest == null)
                {
                    bool bHasError = false;

                    if (m_ResourceBundles.m_ArrayDependentAssetBundles != null)
                    {
                        //检查资源依赖的 AssetBundle 是否加载完成
                        for (int nDependency = 0; nDependency < m_ResourceBundles.m_ArrayDependentAssetBundles.Length; ++nDependency)
                        {
                            Bundle dependentBundle = m_ResourceBundles.m_ArrayDependentAssetBundles[nDependency].m_Bundle;

                            if (dependentBundle.m_AssetBundle == null)
                            {
                                bHasError = true;
                            }
                        }
                    }

                    //检查直接保存资源的 AssetBundle 是否加载完成
                    Bundle mainBundle = m_ResourceBundles.m_MainAssetBundle.m_Bundle;

                    AssetBundle mainAssetBundle = null;

                    if (mainBundle.m_AssetBundle == null)
                    {
                        bHasError = true;
                    }
                    else
                    {
                        mainAssetBundle = mainBundle.m_AssetBundle;
                    }

                    //如果加载 AssetBundle 没有发生错误
                    if (!bHasError && mainAssetBundle != null)
                    {
                        //开始从 AssetBundle 异步加载资源对象
                        m_AssetBundleRequest = mainAssetBundle.LoadAssetAsync(m_ResourceLoadPath, m_ResourceType);
                    }
                    // AssetBundle 都已载入完成但发生错误
                    else
                    {
                        return true;
                    }
                }

                if (m_AssetBundleRequest != null)
                {
                    return m_AssetBundleRequest.isDone;
                }

                return false;
            }
            //如果是通过 Resources.LoadAsync 异步加载资源
            else
            {
                return m_ResourceRequest.isDone;
            }
        }
    }

    //错误信息（为 null 则没有错误）
    public string error
    {
        get
        {
            if (m_LoadedObject != null)
            {
                return null;
            }

            //如果是通过 AssetBundle.LoadAsync 异步加载资源
            if (m_ResourceBundles != null)
            {
                if (m_AssetBundleRequest == null)
                {
                    //获取依赖 AssetBundle 错误
                    List<string> bundleErrors = new List<string>();

                    for (int nDependency = 0; nDependency < m_ResourceBundles.m_ArrayDependentAssetBundles.Length; ++nDependency)
                    {
                        ResourceBundle dependentResourceBundle = m_ResourceBundles.m_ArrayDependentAssetBundles[nDependency];
                        Bundle dependentBundle = dependentResourceBundle.m_Bundle;
                        if (dependentBundle.m_AssetBundle == null)
                        {
                            return string.Format("AssetBundle.LoadAsync error: loading dependent asset bundle from CreateFromFile({0}) failed!", dependentResourceBundle.m_Name);
                        }
                    }

                    //获取主 AssetBundle 错误
                    Bundle mainBundle = m_ResourceBundles.m_MainAssetBundle.m_Bundle;

                    if (mainBundle.m_AssetBundle == null)
                    {
                        return string.Format("AssetBundle.LoadAsync error: loading main bundle from CreateFromFile({0}) failed!", m_ResourceBundles.m_MainAssetBundle.m_Name);
                    }
                }
                else if (m_AssetBundleRequest.isDone && m_AssetBundleRequest.asset == null)
                {
                    return string.Format("AssetBundle.LoadAsync error:{0} loading request is load but asset is null!", m_ResourceBundles.m_MainAssetBundle.m_Name);
                }

                return null;
            }
            //如果是通过 Resources.LoadAsync 异步加载资源
            else
            {
                if (m_ResourceRequest == null)
                {
                    return string.Format("Resources.LoadAsync error: Resource loader is null!");
                }
                else if (m_ResourceRequest.isDone && m_ResourceRequest.asset == null)
                {
                    return string.Format("Resources.LoadAsync error: asset is null !");
                }

                return null;
            }
        }
    }

    public UnityEngine.Object resource
    {
        get
        {
            if (m_LoadedObject != null)
            {
                return m_LoadedObject;
            }

            //如果是通过 AssetBundle.LoadAsync 异步加载资源
            if (m_ResourceBundles != null)
            {
                if (m_AssetBundleRequest != null)
                {
                    return m_AssetBundleRequest.asset;
                }
            }
            //如果是通过 Resources.LoadAsync 异步加载资源
            else
            {
                if (m_ResourceRequest != null)
                {
                    return m_ResourceRequest.asset;
                }
            }

            return null;
        }
    }

    public readonly string m_ResourcePath;                      //资源路径
    public readonly System.Type m_ResourceType;                 //资源类型
    private UnityEngine.Object m_LoadedObject;                //已加载资源对象
    private string m_ResourceLoadPath;                        //资源加载路径（传入 Unity API 的被截断路径）
    public readonly ResourceBundleSet m_ResourceBundles;        //资源 AssetBundle 集合
    private AssetBundleRequest m_AssetBundleRequest;          // AssetBundle.LoadAsync 资源异步加载对象
    private UnityEngine.ResourceRequest m_ResourceRequest;    // Resources.LoadAsync 资源异步加载对象
}

public class ResourceManager
{
    private Dictionary<string, ABFileDesc> m_PathToAssetBundle = new Dictionary<string, ABFileDesc>();
    private Dictionary<string, ABSceneDesc> m_PathToSceneBundle = new Dictionary<string, ABSceneDesc>();
    private KeyObjectPool<string, Bundle> m_AssetBundlePool = new KeyObjectPool<string, Bundle>();        // AssetBundle 回收缓存池
    private Dictionary<string, ResourceBundle> m_MapAssetBundle = new Dictionary<string, ResourceBundle>();   //已载入 AssetBundle 列表



    private Dictionary<string, ABDependencies> m_abDependeciesDic = new Dictionary<string, ABDependencies>();
    private GameObject m_GameObject = null;
    private bool m_UseAssetBundle = true;
    private static System.Text.StringBuilder m_StringBuilder = new System.Text.StringBuilder(512);
    private bool m_IsInit = false;
    private CoroutineJobQueue m_CoroutineJobQueue; //资源载入协程工作队列
    private readonly int m_AssetBundlePoolNum = 128;

    public List<string> m_needUpdateAtlasRefNameLs = new List<string>();

    public static ReadFileCallback m_readFileCallback = null;

    public HashSet<string> m_abInfoMapSet = new HashSet<string>();

    private HashSet<string> m_fileSet = new HashSet<string>();

    private Dictionary<string, ABFileDesc> m_AreaPathToAssetBundle = new Dictionary<string, ABFileDesc>();//地区包
    public static string AreaRootName = "PlayPack";

    #region single

    public static ResourceManager m_intance;

    public static ResourceManager Instance
    {
        get
        {
            if (m_intance == null)
            {
                m_intance = new ResourceManager();
            }
            return m_intance;
        }
    }

    #endregion

    private bool m_initDone = false;

    public static bool DebugLogEnable = false;

    public bool m_initLoginDone { get { return m_initDone; } }


    private bool m_isInitResourceMap = false;
    public void InitResourceMap()
    {
        if (!m_isInitResourceMap)
        {
            m_isInitResourceMap = true;

            var resources = Application.streamingAssetsPath + "/Resources.json";
            if (m_readFileCallback != null)
            {
                var jbytes = m_readFileCallback(resources);
                var strText = System.Text.Encoding.UTF8.GetString(jbytes);
                JsonData pa = JsonMapper.ToObject(strText);
                if (pa.IsArray)
                {
                    for (int i = 0; i < pa.Count; i++)
                    {
                        var str = (string)pa[i];
                        if (!m_fileSet.Contains(str))
                        {
                            m_fileSet.Add(str);
                        }
                    }
                }
            }
        }
    }

    public void Init()
    {
        if (m_initDone)
            return;
#if UNITY_EDITOR
        m_UseAssetBundle = Constants.FORCE_LOAD_AB;
#endif
        m_GameObject = GameObject.Find("_GameJobQueue");
        if (m_GameObject == null)
        {
            m_GameObject = new GameObject("_GameJobQueue");
            GameObject.DontDestroyOnLoad(m_GameObject);
        }
        EmptyComponent mono = GameCoreUtils.GetOrAddComponent<EmptyComponent>(m_GameObject);
        var streamPath = PathUtils.AssetBundlePath;
        var persistPath = PathUtils.AssetBundlePersistPath;
        if (m_CoroutineJobQueue == null)
        {
            m_CoroutineJobQueue = new CoroutineJobQueue(mono);
        }
        //DaemonSys.Instance.AddDaemonTask(() =>
        //{
        InitResourceMap();

         var tempStreamPath = streamPath + "ABDatabase.abMap";
        if (m_UseAssetBundle && m_readFileCallback != null && !m_abInfoMapSet.Contains(tempStreamPath))
        {
            m_abInfoMapSet.Add(tempStreamPath);
            var bytes = m_readFileCallback(tempStreamPath);
            if (bytes != null)
            {
                var strText = System.Text.Encoding.UTF8.GetString(bytes);
                var assetBundleDesc = GameCoreUtils.StrToResCABDataStructLs(strText);
                if (assetBundleDesc is ABDesc)
                    AddAbDesToDic(assetBundleDesc as ABDesc);
            }
        }
        InitUpdateAbDatas();
        
        // });
        if (m_CoroutineJobQueue.m_jobCoroutineState == CoroutineJobQueue.EJobCoroutineState.NotRunning)
        {
            m_CoroutineJobQueue.StartJobCoroutine();
        }
        if (m_AssetBundlePool.m_MaxCachedObjects != m_AssetBundlePoolNum)
        {
            m_AssetBundlePool.Initialize(m_AssetBundlePoolNum, _LoadAssetBundleCallback, _UnloadAssetBundleCallback);
        }
        m_initDone = true;
    }


    private void InitUpdateAbDatas()
    {
        var abParentPersistentPath = PathUtils.AssetBundlePersistPath;
        var dir = new DirectoryInfo(abParentPersistentPath);
        bool isShader = false;
        if (dir.Exists && m_readFileCallback != null)
        {
            var absDescs = dir.GetFiles("*.abMap", SearchOption.AllDirectories);
            for (int i = 0; i < absDescs.Length; i++)
            {
                var name = absDescs[i].FullName.Substring(absDescs[i].FullName.IndexOf("AssetBundles"));
                if (!m_abInfoMapSet.Contains(name))
                {
                    m_abInfoMapSet.Add(name);
                    var bytes = m_readFileCallback(absDescs[i].FullName);
                    if (bytes != null)
                    {
                        var assetBundleDesc = GameCoreUtils.StrToResCABDataStructLs(System.Text.Encoding.UTF8.GetString(bytes));
                        if (assetBundleDesc is ABDesc)
                        {
                            if (AddAbDesToDic(assetBundleDesc as ABDesc, false))
                            {
                                isShader = true;
                            }
                        }
                    }
                }
            }
        }
        if (isShader)
        {
            this.m_AssetBundlePool.UnLoadAllObject(EnumAB.shaders_ab.ToString());
            //万一上面没卸载完毕 正在被引用中
            if (m_MapAssetBundle.ContainsKey(EnumAB.shaders_ab.ToString()))
            {
                var data = m_MapAssetBundle[EnumAB.shaders_ab.ToString()];
                data.m_Bundle.m_AssetBundle.Unload(false);
                data.m_Bundle = _LoadAssetBundleCallback(EnumAB.shaders_ab.ToString(), null);
            }
        }
    }

    /// <summary>
    /// 加载地区包
    /// </summary>
    public void AddAreaAbDatas(string AreaName)
    {
        var abParentPersistentPath = Application.persistentDataPath + "/" + AreaRootName + "/" + AreaName + "/AssetBundles/";
        var dir = new DirectoryInfo(abParentPersistentPath);
        if (dir.Exists && m_readFileCallback != null)
        {
            var absDescs = dir.GetFiles("*.abMap", SearchOption.AllDirectories);
            for (int i = 0; i < absDescs.Length; i++)
            {
                var bytes = m_readFileCallback(absDescs[i].FullName);
                if (bytes != null)
                {
                    var assetBundleDesc = GameCoreUtils.StrToResCABDataStructLs(System.Text.Encoding.UTF8.GetString(bytes));
                    if (assetBundleDesc is ABDesc)
                    {
                        AddAreaAbDesToDic(assetBundleDesc as ABDesc, AreaName);
                    }
                }
            }
        }
    }

    private void AddAreaAbDesToDic(ABDesc assetBundleDesc,string AreaName)
    {
        if (assetBundleDesc != null)
        {
            for (int cnt = 0; cnt < assetBundleDesc.m_files.Count; ++cnt)
            {
                var desc = assetBundleDesc.m_files[cnt];
                if (m_AreaPathToAssetBundle.ContainsKey(desc.m_gbName))
                {
                    m_AreaPathToAssetBundle[desc.m_gbName] = desc;
                }
                else
                {
                    m_AreaPathToAssetBundle.Add(desc.m_gbName, desc);
                }

                //添加到文件目录
                if (!m_fileSet.Contains(desc.m_gbName))
                {
                    m_fileSet.Add(desc.m_gbName);
                }
            }
        }

        //
        for (int cnt = 0; cnt < assetBundleDesc.m_abDepends.Count; ++cnt)
        {
            var des = assetBundleDesc.m_abDepends[cnt];
            des.m_isStreamAsset = false;
            des.m_areaName = AreaName;

            var sceneName = des.m_abName;
            if (m_abDependeciesDic.ContainsKey(sceneName))
            {
                m_abDependeciesDic[sceneName] = des;
            }
            else
            {
                m_abDependeciesDic.Add(sceneName, des);
            }
        }
    }

    /// <summary>
    /// 卸载地区包
    /// </summary>
    public void RemoveAreaAbDatas() 
    {
        m_AreaPathToAssetBundle.Clear();
        this.m_AssetBundlePool.UnLoadAllObject(EnumAB.play_ab.ToString());
    }

    public bool ContaintsResources(string fileName)
    {
        return m_fileSet.Contains(fileName);
    }

    private bool AddAbDesToDic(ABDesc assetBundleDesc, bool isStreamingAsset = true)
    {
        bool isUpdateShader = false;
        if (assetBundleDesc != null)
        {
            for (int cnt = 0; cnt < assetBundleDesc.m_files.Count; ++cnt)
            {
                var desc = assetBundleDesc.m_files[cnt];
                if (!isStreamingAsset && desc.m_gbName.IndexOf("no_use_ref_", StringComparison.CurrentCultureIgnoreCase) > 0)
                {
                    m_needUpdateAtlasRefNameLs.Add(desc.m_gbName);
                }
                if (m_PathToAssetBundle.ContainsKey(desc.m_gbName))
                {
                    if (desc.m_ressVersion > m_PathToAssetBundle[desc.m_gbName].m_ressVersion)
                    {
                        m_PathToAssetBundle[desc.m_gbName] = desc;
                    }
                }
                else
                {
                    m_PathToAssetBundle.Add(desc.m_gbName, desc);
                }
                if (!m_fileSet.Contains(desc.m_gbName))
                {
                    m_fileSet.Add(desc.m_gbName);
                }
            }

            for (int cnt = 0; cnt < assetBundleDesc.m_scenes.Count; ++cnt)
            {
                var desc = assetBundleDesc.m_scenes[cnt];
                var names = desc.m_sceneName.Split('/');
                var sceneName = names[names.Length - 1];
                int nExtNameStart = sceneName.LastIndexOf('.');
                if (nExtNameStart > 0)
                {
                    sceneName = sceneName.Remove(nExtNameStart);
                }
#if RES_LOG
                Debug.Log("short name:" + sceneName);
                Debug.Log("desc.m_sceneName :" + desc.m_sceneName);
#endif
                if (m_PathToSceneBundle.ContainsKey(sceneName))
                {

                    if (desc.m_ressVersion > m_PathToSceneBundle[sceneName].m_ressVersion)
                    {
                        m_PathToSceneBundle[sceneName] = desc;
                    }
                }
                else
                {
                    m_PathToSceneBundle.Add(sceneName, desc);
                }
            }
            for (int cnt = 0; cnt < assetBundleDesc.m_abDepends.Count; ++cnt)
            {
                var des = assetBundleDesc.m_abDepends[cnt];
                des.m_isStreamAsset = isStreamingAsset;
                var sceneName = des.m_abName;
                //shader的话 直接替换
                
                if (m_abDependeciesDic.ContainsKey(sceneName))
                {
                    var oldData = m_abDependeciesDic[sceneName];
                    if (oldData.m_ressVersion < des.m_ressVersion && (isStreamingAsset
                        || (!isStreamingAsset && IsABExists(des))))
                    {
                        m_abDependeciesDic[sceneName] = des;
                        if (sceneName.Contains(EnumAB.shaders_ab.ToString()))
                        {
                            isUpdateShader = true;
                        }
                    }
                }
                else
                {
                    m_abDependeciesDic.Add(sceneName, des);
                }

            }
        }
        return isUpdateShader;
    }

    public void Release()
    {
        var jobCoroutineState = m_CoroutineJobQueue.GetJobCoroutineState();
        if (jobCoroutineState == CoroutineJobQueue.EJobCoroutineState.Running)
        {
            m_CoroutineJobQueue.StopJobCoroutine();
        }

        m_IsInit = false;
    }

    public bool IsInitialized()
    {
        return m_IsInit;
    }
    /// <summary>
    /// 注意，在StreamingAssets目录下的ab是后后缀的，因为android打gradlew需要后缀区分那些文件压缩
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="abd"></param>
    /// <returns></returns>
    protected string _GetAssetBundlePath(string assetBundleName, ABDependencies abd)
    {

        ResourceManager.m_StringBuilder.Remove(0, ResourceManager.m_StringBuilder.Length);
        if (abd == null || abd.m_isStreamAsset)
        {
            ResourceManager.m_StringBuilder.Append(PathUtils.AssetBundlePath);
            ResourceManager.m_StringBuilder.Append(assetBundleName);
            ResourceManager.m_StringBuilder.Append(".ezfunab");
        }
        else
        {
            //地区包
            if (!string.IsNullOrEmpty(abd.m_areaName))
            {
                ResourceManager.m_StringBuilder.Append(Application.persistentDataPath);
                ResourceManager.m_StringBuilder.Append("/" + AreaRootName + "/");
                ResourceManager.m_StringBuilder.Append(abd.m_areaName);
                ResourceManager.m_StringBuilder.Append("/AssetBundles/");

            }
            else 
            {
                ResourceManager.m_StringBuilder.Append(PathUtils.AssetBundlePersistPath);
                if (abd.m_abName.Contains(EnumAB.shaders_ab.ToString()))
                {
                    ResourceManager.m_StringBuilder.Append(abd.m_ressVersion);
                    ResourceManager.m_StringBuilder.Append('_');
                }
            }
            ResourceManager.m_StringBuilder.Append(assetBundleName);
        }
        string assetBundlePath = ResourceManager.m_StringBuilder.ToString();
        return assetBundlePath;
    }

    protected bool IsABExists(ABDependencies abd)
    {
        string path = _GetAssetBundlePath(abd.m_abName, abd);
        return File.Exists(path);
    }

    protected void _UnloadAssetBundleCallback(Bundle bundle)
    {
        bundle.m_AssetBundle.Unload(false);
    }

    protected Bundle _LoadAssetBundleCallback(string assetBundleName, System.Object param)
    {
        var bundle = new Bundle();
        ABDependencies cad = null;
        if (m_abDependeciesDic.ContainsKey(assetBundleName))
        {
            cad = m_abDependeciesDic[assetBundleName];
        }
        string assetBundleURL = _GetAssetBundlePath(assetBundleName, cad);

        bundle.m_AssetBundle = AssetBundle.LoadFromFile(assetBundleURL);
        if (bundle.m_AssetBundle == null)
        {
            Debug.LogError("AssetBundle.LoadFromFile error " + assetBundleURL);
        }
        return bundle;
    }

    private string GetAssetBundleName(string resPath)
    {
        ABFileDesc ret = null;
        if (m_UseAssetBundle == true) 
        {
            m_PathToAssetBundle.TryGetValue(resPath, out ret);
            //基础包优先
            if (ret == null)
            {
                m_AreaPathToAssetBundle.TryGetValue(resPath, out ret);
            }
        }

        if (ret != null)
        {
            return ret.m_bundleName;
        }
        return null;
    }

    protected ABSceneDesc GetSceneAssetBundleName(string sceneName)
    {
        ABSceneDesc ret = null;
        if (m_UseAssetBundle == true)
            m_PathToSceneBundle.TryGetValue(sceneName, out ret);
        if (ret != null)
        {
            return ret;
        }
        return null;
    }

    private string GetResourcePath(string resourcePath)
    {
        string resourceLoadPath = "";
        if (resourcePath.StartsWith(PathUtils.ResourceRefPath, StringComparison.OrdinalIgnoreCase))
        {
            //通过 Resources.Load 加载该资源
            resourceLoadPath = resourcePath.Remove(0, PathUtils.ResourceRefPath.Length);
            int nExtNameStart = resourceLoadPath.LastIndexOf('.');
            if (nExtNameStart > 0)
            {
                resourceLoadPath = resourceLoadPath.Remove(nExtNameStart);
            }
        }
        return resourceLoadPath;
    }

    protected ResourceBundle _LoadAssetBundle(string assetBundleName)
    {
        ResourceBundle bundle = null;
        if (m_MapAssetBundle.TryGetValue(assetBundleName, out bundle))
        {
            ++bundle.m_nRefCount;
            return bundle;
        }

        bundle = new ResourceBundle();
        bundle.m_Name = assetBundleName;
        bundle.m_Bundle = m_AssetBundlePool.LoadObject(assetBundleName);
        bundle.m_nRefCount = 1;

        m_MapAssetBundle.Add(assetBundleName, bundle);

        return bundle;
    }

    //卸载 AssetBundle
    protected void _UnloadAssetBundle(ResourceBundle bundle)
    {
        if (bundle.m_nRefCount <= 0)
        {
            Debug.LogErrorFormat("Call Bundle.Release() of asset bundle '{0}' with reference count 0.", bundle.m_Name);
            return;
        }

        --bundle.m_nRefCount;
        if (bundle.m_nRefCount == 0)
        {
            if (bundle.m_Bundle != null)
            {
                m_AssetBundlePool.UnloadObject(bundle.m_Name, bundle.m_Bundle);
                bundle.m_Bundle = null;
            }

            m_MapAssetBundle.Remove(bundle.m_Name);
        }
    }

    //卸载 ResourceBundleSet 内的所有 AssetBundle
    protected void _UnloadResourceBundleSet(ResourceBundleSet resourceBundleSet)
    {
        if (resourceBundleSet != null)
        {
            if (resourceBundleSet.m_ArrayDependentAssetBundles != null)
            {
                for (int nDependentBundle = 0; nDependentBundle < resourceBundleSet.m_ArrayDependentAssetBundles.Length; ++nDependentBundle)
                {
                    _UnloadAssetBundle(resourceBundleSet.m_ArrayDependentAssetBundles[nDependentBundle]);
                }
            }

            _UnloadAssetBundle(resourceBundleSet.m_MainAssetBundle);
        }
    }

    ResourceBundleSet LoadAllAssetBundles(string assetBundleName)
    {
        ResourceBundleSet resourceBundles = new ResourceBundleSet();

        string[] dependentBundles = null;
        if (m_abDependeciesDic.ContainsKey(assetBundleName))
        {
            dependentBundles = m_abDependeciesDic[assetBundleName].m_dependenciesAb.ToArray();
        }
        if (dependentBundles != null && dependentBundles.Length > 0)
        {
            resourceBundles.m_ArrayDependentAssetBundles = new ResourceBundle[dependentBundles.Length];

            for (int i = 0; i < dependentBundles.Length; ++i)
            {
                var dependentBundleName = dependentBundles[i];
                var resourceBundle = _LoadAssetBundle(dependentBundleName);
                resourceBundles.m_ArrayDependentAssetBundles[i] = resourceBundle;
            }
        }

        resourceBundles.m_MainAssetBundle = _LoadAssetBundle(assetBundleName);
        return resourceBundles;
    }

    /// <summary>
    /// 同步加载场景
    /// </summary>
    /// <param name="sceneName">场景名</param>
    /// <param name="callback">加载回调函数</param>
    public void LoadScene(string sceneName, SceneLoadingCallback callback, bool isSingle = true)
    {
        var caseName = sceneName;
#if RES_LOG
        Debug.Log("LoadScene.caseName:" + caseName);
#endif
        var data = GetSceneAssetBundleName(PathUtils.GetSceneNameFromPath(sceneName.ToLower()));
        string sceneBundleName = data != null ? data.m_bundleName : null;
        if (string.IsNullOrEmpty(sceneBundleName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(caseName,
                isSingle ? UnityEngine.SceneManagement.LoadSceneMode.Single : UnityEngine.SceneManagement.LoadSceneMode.Additive
                );
        }
        else
        {
#if RES_LOG
            Debug.Log("LoadScene.sceneBundleName:" + sceneBundleName);
#endif
            ResourceBundleSet resourceBundles = LoadAllAssetBundles(sceneBundleName);
#if RES_LOG
            Debug.Log("LoadScene.caseName:" + caseName);
#endif
            UnityEngine.SceneManagement.SceneManager.LoadScene(caseName,
                isSingle ? UnityEngine.SceneManagement.LoadSceneMode.Single : UnityEngine.SceneManagement.LoadSceneMode.Additive
                );
            _UnloadResourceBundleSet(resourceBundles);
        }
        if (callback != null)
        {
            callback(sceneName, null, 1.0f, true);
        }
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName">场景名</param>
    /// <param name="callback">加载回调函数</param>
    public AsyncOperation LoadSceneAsync(string sceneName, SceneLoadingCallback callback, bool isSingle = true)
    {
        var caseName = sceneName;
#if RES_LOG
        Debug.Log("LoadSceneAsync.sceneName:" + sceneName);
#endif
        var data = GetSceneAssetBundleName(PathUtils.GetSceneNameFromPath(sceneName.ToLower()));
        string sceneBundleName = data != null ? data.m_bundleName : null;
        sceneName = data != null ? data.m_sceneName : sceneName;
        ResourceBundleSet bundles = null;
        if (!string.IsNullOrEmpty(sceneBundleName))
        {
#if RES_LOG
            Debug.Log("LoadSceneAsync.sceneBundleName:" + sceneBundleName);
#endif
            bundles = LoadAllAssetBundles(sceneBundleName);
        }
        else
        {
            sceneName = caseName;
        }
#if RES_LOG
        Debug.Log("LoadSceneAsync.sceneName:" + sceneName);
#endif
        AsyncOperation request = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(caseName,
            isSingle ?
            UnityEngine.SceneManagement.LoadSceneMode.Single : UnityEngine.SceneManagement.LoadSceneMode.Additive
             );
        if (isSingle)
        {
            CheckSceneLoadingStatusParam jobParam = new CheckSceneLoadingStatusParam();
            jobParam.m_SceneLoadingCallback = callback;
            jobParam.m_SceneName = caseName;
            jobParam.m_AsyncOperation = request;
            jobParam.m_ResourceBundles = bundles;
            m_CoroutineJobQueue.PushJob(_JobCheckSceneLoadingStatus, jobParam);
        }
        return request;
    }

    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <param name="resourcePath">资源路径，必须是全路径加后缀，比如“Assets/XGame/Resources/Prefab/monster/monster_houzi_high.prefab”</param>
    /// <param name="resourceType">资源类型</param>
    /// <param name="resourceLoadedCallback">资源回调函数</param>
    /// <param name="callbackCustomParam">资源回调函数参数</param>
    public UnityEngine.Object LoadResource(string resourcePath, Type resourceType, ResourceLoadedCallback resourceLoadedCallback = null, System.Object callbackCustomParam = null)
    {
        if (string.IsNullOrEmpty(resourcePath))
        {
            Debug.LogError("LoadResource error: resource path is null " + resourcePath);
            return null;
        }
        if (PathUtils.HasExtension(resourcePath) == false)
        {
            Debug.LogError("Resource path must has extension " + resourcePath);
            return null;
        }
        //#if RES_LOG
        //        Debug.Log("LoadResource.resourcePath:" + resourcePath);
        //#endif
        UnityEngine.Object resource = null;
        string error = "";
        string assetBundleName = GetAssetBundleName(resourcePath.ToLower());
#if RES_LOG
        Debug.Log("LoadResource.assetBundleName:" + assetBundleName);
#endif
        if (string.IsNullOrEmpty(assetBundleName))
        {
            string resourceLoadPath = GetResourcePath(resourcePath);
            if (!string.IsNullOrEmpty(resourceLoadPath))
            {
                resource = UnityEngine.Resources.Load(resourceLoadPath, resourceType);
            }
        }
        else
        {
            ResourceBundleSet resourceBundleSet = LoadAllAssetBundles(assetBundleName);
#if RES_LOG
            Debug.Log("LoadResource.resourcePath:" + resourcePath);
#endif
            if (resourceBundleSet == null 
                || resourceBundleSet.m_MainAssetBundle == null 
                || resourceBundleSet.m_MainAssetBundle.m_Bundle == null 
                || resourceBundleSet.m_MainAssetBundle.m_Bundle.m_AssetBundle == null) 
            {
                if (resourceBundleSet == null)
                    Debug.LogError("resourceBundleSet == null");
                if (resourceBundleSet.m_MainAssetBundle == null)
                    Debug.LogError("resourceBundleSet.m_MainAssetBundle == null");
                if (resourceBundleSet.m_MainAssetBundle.m_Bundle == null)
                    Debug.LogError("resourceBundleSet.m_MainAssetBundle.m_Bundle == null");
                if (resourceBundleSet.m_MainAssetBundle.m_Bundle.m_AssetBundle == null)
                    Debug.LogError("esourceBundleSet.m_MainAssetBundle.m_Bundle.m_AssetBundle == null");

                return null;
            }

            resource = resourceBundleSet.m_MainAssetBundle.m_Bundle.m_AssetBundle.LoadAsset(resourcePath, resourceType);
            _UnloadResourceBundleSet(resourceBundleSet);
        }

        if (resourceLoadedCallback != null)
        {
            resourceLoadedCallback(resource, error, resourcePath, callbackCustomParam);
        }
        return resource;
    }

    /// <summary>
    /// 加载资源（包含子资源）
    /// </summary>
    /// <param name="resourcePath">资源路径，必须是全路径加后缀，比如“Assets/XGame/Resources/Prefab/monster/monster_houzi_high.prefab”</param>
    /// <param name="resourceType">资源类型</param>
    public UnityEngine.Object[] LoadResourceAll(string resourcePath, Type resourceType)
    {
        if (string.IsNullOrEmpty(resourcePath))
        {
            Debug.LogError("LoadResourceAll error: resource path is null");
            return null;
        }
        if (PathUtils.HasExtension(resourcePath) == false)
        {
            Debug.LogError("Resource path must has extension " + resourcePath);
            return null;
        }
        UnityEngine.Object[] resources = null;

        string assetBundleName = GetAssetBundleName(resourcePath.ToLower());
        if (string.IsNullOrEmpty(assetBundleName))
        {
            string resourceLoadPath = GetResourcePath(resourcePath);
            if (!string.IsNullOrEmpty(resourceLoadPath))
            {
                resources = UnityEngine.Resources.LoadAll(resourceLoadPath, resourceType);
            }
        }
        else
        {
            ResourceBundleSet resourceBundleSet = LoadAllAssetBundles(assetBundleName);
            resources = resourceBundleSet.m_MainAssetBundle.m_Bundle.m_AssetBundle.LoadAssetWithSubAssets(resourcePath, resourceType);
        }
        return resources;
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="resourcePath">资源路径，必须是全路径加后缀，比如“Assets/XGame/Resources/Prefab/monster/monster_houzi_high.prefab”</param>
    /// <param name="resourceType">资源类型</param>
    /// <param name="resourceLoadedCallback">资源回调函数</param>
    /// <param name="callbackCustomParam">资源回调函数参数</param>
    public ResourceLoader LoadResourceAsync(string resourcePath, Type resourceType, ResourceLoadedCallback resourceLoadedCallback = null, System.Object callbackCustomParam = null)
    {
        if (string.IsNullOrEmpty(resourcePath))
        {
            Debug.LogError("LoadResourceAsync error: resource path is null " + resourcePath);
            return null;
        }
        if (PathUtils.HasExtension(resourcePath) == false)
        {
            Debug.LogError("Resource path must has extension " + resourcePath);
            return null;
        }

        ResourceLoader resourceLoader = null;

        string assetBundleName = GetAssetBundleName(resourcePath.ToLower());
        if (string.IsNullOrEmpty(assetBundleName))
        {
            string resourceLoadPath = GetResourcePath(resourcePath);
            if (!string.IsNullOrEmpty(resourceLoadPath))
            {
                UnityEngine.ResourceRequest unityResourceRequest = UnityEngine.Resources.LoadAsync(resourceLoadPath, resourceType);
                if (unityResourceRequest != null)
                {
                    resourceLoader = new ResourceLoader(resourcePath, resourceType, resourceLoadPath, unityResourceRequest);
                }
            }
        }
        else
        {
            ResourceBundleSet resourceBundles = LoadAllAssetBundles(assetBundleName);
            if (resourceBundles != null)
            {
                resourceLoader = new ResourceLoader(resourcePath, resourceType, resourcePath, resourceBundles);
            }
        }
        if (resourceLoader != null)
        {
            CheckResourceLoadingStatusParam jobParam = new CheckResourceLoadingStatusParam();
            jobParam.m_ResourceLoader = resourceLoader;
            jobParam.m_ResourceLoadedCallback = resourceLoadedCallback;
            jobParam.m_CallbackCustomParam = callbackCustomParam;

            m_CoroutineJobQueue.PushJob(_JobCheckResourceLoadingStatus, jobParam);
        }
        return resourceLoader;
    }

    public IEnumerator _JobCheckResourceLoadingStatus(object jobParam)
    {
        var param = jobParam as CheckResourceLoadingStatusParam;
        IEnumerator coroutine = _CheckResourceLoadingStatus(param.m_ResourceLoader, param.m_ResourceLoadedCallback, param.m_CallbackCustomParam);
        while (true)
        {
            if (!coroutine.MoveNext())
            {
                yield break;
            }

            yield return coroutine.Current;
        }
    }

    public IEnumerator _JobCheckSceneLoadingStatus(object jobParam)
    {
        var param = jobParam as CheckSceneLoadingStatusParam;

        if (param.m_ResourceBundles != null && param.m_ResourceBundles.m_MainAssetBundle == null)
        {
            Debug.LogError("Load scene error: m_MainAssetBundle is null! " + param.m_SceneName);
            yield break;
        }

        while (!param.m_AsyncOperation.isDone)
        {
            if (param.m_SceneLoadingCallback != null)
            {
                param.m_SceneLoadingCallback(param.m_SceneName, null, param.m_AsyncOperation.progress, false);
            }

            yield return null;
        }

        //等待加载真正完成
        do
        {
            yield return new WaitForEndOfFrame();
        }
        while (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != param.m_SceneName);

        if (param.m_SceneLoadingCallback != null)
        {
            param.m_SceneLoadingCallback(param.m_SceneName, null, param.m_AsyncOperation.progress, true);
        }

        //卸载 AssetBundle
        if (param.m_ResourceBundles != null)
        {
            _UnloadResourceBundleSet(param.m_ResourceBundles);
        }
    }

    public IEnumerator _CheckResourceLoadingStatus(ResourceLoader resourceLoader, ResourceLoadedCallback resourceLoadedCallback, System.Object callbackCustomParam)
    {
        //通过 Unity Coroutine 机制隔帧循环等待资源加载完成
        while (!resourceLoader.isDone)
        {
            yield return null;
        }

        //如果加载完成通知回调函数不为空
        if (resourceLoadedCallback != null)
        {
            resourceLoadedCallback(resourceLoader.resource, resourceLoader.error, resourceLoader.m_ResourcePath, callbackCustomParam);
        }

        //卸载 AssetBundle
        _UnloadResourceBundleSet(resourceLoader.m_ResourceBundles);
    }
}
