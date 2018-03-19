using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

[LuaWrap]
public enum RessStorgeType
{
    RST_Never, // 会被清理的资源
    RST_Always, // 不会清理的资源 
    RST_NONE,
}
[LuaWrap]
public enum RessType
{
    RT_Window,
    RT_UIItem,
    RT_UICamera,
    RT_Emoji,
    RT_Animation,
    RT_CommonWindow,
    RT_GuideUIItem,
    RT_FightUIItem,
    RT_CommonUItem,
    RT_LoadingUI,
    RT_None,
}

public class OBContainer
{
    public Object m_object;

    public OBContainer(Object ob)
    {
        m_object = ob;
    }
}

public class ResourceItem
{
    public OBContainer m_obContainer; //存放的具体物件
    public List<Object> m_instanceList = new List<Object>(); //实例化出来的对象列表
    public RessStorgeType m_storgeType = RessStorgeType.RST_Never;

    public ResourceItem(Object ob, RessStorgeType storeType)
    {
        m_obContainer = new OBContainer(ob);
        m_storgeType = storeType;
    }
}

public class RessInstantiateAsyncResult : GenericPool.IPoolable
{
    private static int REQ_SEQ = 0;

    public string m_path;
    public int m_requestId;
    public Object m_instance;

    public int GenerateReqID()
    {
        m_requestId = ++REQ_SEQ;

        return m_requestId;
    }

    #region IPoolable implementation

    public void ResetState()
    {
        m_path = null;
        m_instance = null;
    }

    #endregion


}

public class RessKey
{
    public RessType m_type;
    public string m_ressName;

    public string m_fullPath;

    public RessKey(RessType type, string ressName)
    {
        m_type = type;
        m_ressName = ressName;
        m_fullPath = ResourceMgr.GetRessTypePath(type, ressName);
    }

    public RessKey(string fullPath)
    {
        this.m_fullPath = fullPath;
        m_type = RessType.RT_None;
        m_ressName = null;
    }

    public bool Equals(RessKey resKey)
    {
        return resKey.m_fullPath == this.m_fullPath;
    }

    public override int GetHashCode()
    {
        return m_fullPath.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        RessKey IDObj = obj as RessKey;
        if (IDObj == null)
        {
            return false;
        }
        return Equals(IDObj);
    }
}

public class ResourceMgr
{
    public static Dictionary<RessKey, ResourceItem> m_ressDic = new Dictionary<RessKey, ResourceItem>();

    private static List<Object> m_tempCacheOBLs = new List<Object>();

    static OBContainer InitContainer(RessType ressType, string ressName,
        RessStorgeType ressStorgeType = RessStorgeType.RST_Never, bool isSync = true,
        System.Action<OBContainer> action = null)
    {
        RessKey ressKey = new RessKey(ressType, ressName);
        ResourceItem ressItem = null;
        if (m_ressDic.TryGetValue(ressKey, out ressItem))
        {
            if (action != null)
            {
                action(ressItem.m_obContainer);
            }
            return ressItem.m_obContainer;
        }

        if (isSync)
        {
            var ob = LoadRes(ressType, ressName);
            if (ob == null)
            {
                if (action != null)
                {
                    action(null);
                }
                return null;
            }
            ressItem = new ResourceItem(ob, ressStorgeType);
            if (!m_ressDic.ContainsKey(ressKey))
            {
                m_ressDic.Add(ressKey, ressItem);
            }
            var value = ressItem.m_obContainer;
            if (action != null)
            {
                action(value);
            }
            return value;
        }
        else
        {
            LoadASyncRec(ressType, ressName,
                (resource, error, resourcePath, customParam) =>
                {
                    if (resource != null)
                    {
                        ResourceItem item = new ResourceItem(resource, ressStorgeType);
                        if (!m_ressDic.ContainsKey(ressKey))
                        {
                            m_ressDic.Add(ressKey, item);
                        }
                        if (action != null)
                        {
                            action(m_ressDic[ressKey].m_obContainer);
                        }
                    }
                    else
                    {
                        if (action != null)
                        {
                            action(null);
                        }
                    }
                });
            return null;
        }
    }

    public static void InsertAsset(Object ob, string fullPath)
    {
        if (ob != null)
        {
            RessKey ressKey = new RessKey(fullPath);
            ResourceItem ressItem = new ResourceItem(ob, RessStorgeType.RST_Never);
            if (!m_ressDic.ContainsKey(ressKey))
            {
                m_ressDic.Add(ressKey, ressItem);
            }
        }
    }


    public static Object CheckAssets(string fullPath)
    {
        RessKey ressKey = new RessKey(fullPath);
        ResourceItem ressItem = null;
        if (m_ressDic.TryGetValue(ressKey, out ressItem))
        {
            return ressItem.m_obContainer.m_object;
        }
        return null;
    }

    public static Object InitAsset(RessType ressType, string ressName,
        RessStorgeType ressStorgeType = RessStorgeType.RST_Never)
    {
        OBContainer container = InitContainer(ressType, ressName, ressStorgeType);
        if (container != null)
        {
            return container.m_object;
        }
        else
        {
            return null;
        }
    }

    private static OBContainer InitAssetWith2Param(RessType ressType, string ressName)
    {
        return InitContainer(ressType, ressName);
    }
    private static void LoadASyncRec(RessType rt, string rn, ResourceLoadedCallback callback)
    {
        string path = GetRessTypePath(rt, rn);
        ResourceManager.Instance.LoadResourceAsync(path, typeof(Object), callback);
    }

    private static Object LoadRes(RessType rt, string rn)
    {
        string path = GetRessTypePath(rt, rn);
        Object obj = ResourceManager.Instance.LoadResource(path, typeof(Object));
        return obj;
    }

    public static string GetRessTypePath(RessType rt, string rn)
    {
        return CommonStringBuilder.BuildString("Assets/XGame/Resources/", GetAssetPathByType(rt), rn, ".prefab");
    }

    public static string GetRessPath(RessKey rk)
    {
        return CommonStringBuilder.BuildString(GetAssetPathByType(rk.m_type), rk.m_ressName);
    }

    public static Object GetInstantiateAsset(RessType ressType, string ressName,
        RessStorgeType ressStorgeType = RessStorgeType.RST_Never, System.Action<UnityEngine.Object> callback = null)
    {
        RessKey ressKey = new RessKey(ressType, ressName);
        if (!m_ressDic.ContainsKey(ressKey))
        {
            InitAsset(ressType, ressName, ressStorgeType);
        }
        return InstantiateOb(ressKey, callback);
    }

    public static void GetAsyncInstantiateAsset(RessType ressType, string ressName,
        RessStorgeType ressStorgeType = RessStorgeType.RST_Never, System.Action<UnityEngine.Object> callback = null)
    {
        RessKey ressKey = new RessKey(ressType, ressName);
        if (!m_ressDic.ContainsKey(ressKey))
        {
            InitContainer(ressType, ressName, ressStorgeType, false,
                (ob) =>
                {
                    InstantiateOb(ressKey, callback);
                });
        }
        else
        {
            InstantiateOb(ressKey, callback);
        }

    }

    public static ResourceItem GetResourceItem(RessType ressType, string ressName)
    {
        RessKey ressKey = new RessKey(ressType, ressName);
        ResourceItem item = null;
        if (!m_ressDic.TryGetValue(ressKey, out item))
        {
        }
        return item;
    }

    private static Object InstantiateOb(RessKey ressKey, System.Action<UnityEngine.Object> callback = null)
    {
        if (!m_ressDic.ContainsKey(ressKey))
        {
            return null;
        }
        ResourceItem ressItem = m_ressDic[ressKey];
        for (int i = 0; i < ressItem.m_instanceList.Count; i++)
        {
            Object OB = ressItem.m_instanceList[i];
            if (OB is GameObject)
            {
                if (OB == null)
                {
                    ressItem.m_instanceList.RemoveAt(i);
                    --i;
                    continue;
                }
                if (!((GameObject)OB).activeSelf)
                {
                    if (callback != null)
                        callback(OB);
                    return OB;
                }
            }
        }
        Object instanceOB = MonoBehaviour.Instantiate(ressItem.m_obContainer.m_object);
        ressItem.m_instanceList.Add(instanceOB);
        m_ressDic[ressKey] = ressItem;
        if (callback != null)
        {
            callback(instanceOB);
        }
        return instanceOB;
    }

    public static string GetAssetPathByType(RessType ressType)
    {
        string path = "";
        switch (ressType)
        {
            case RessType.RT_Window:
                path = "EZFunUI/Window/UIWindow/";
                break;
            case RessType.RT_UIItem:
                path = "EZFunUI/Item/UIItem/";
                break;
            case RessType.RT_UICamera:
                path = "EZFunUI/UICamera";
                break;
            case RessType.RT_Emoji:
                path = "Prefab/Emoji/";
                break;
            case RessType.RT_Animation:
                path = "Prefab/Animation/";
                break;
            case RessType.RT_CommonWindow:
                path = "EZFunUI/Window/UIWindow/";
                break;

            case RessType.RT_GuideUIItem:
                path = "EZFunUI/Item/GuideItem/";
                break;
            case RessType.RT_FightUIItem:
                path = "EZFunUI/Item/FightItem/";
                break;
            case RessType.RT_CommonUItem:
                path = "EZFunUI/Item/CommonItem/";
                break;
            case RessType.RT_LoadingUI:
                path = "EZFunUI/Window/UIWindow/";
                break;
            case RessType.RT_None:
                path = "";
                break;
            default:
                path = ressType.ToString();
                Debug.LogWarning("[invalid ress type]" + ressType.ToString());
                break;


        }
        return path;
    }
    /// <summary>
    /// 加入一个参数,表示是否清除所有资源
    /// </summary>
    /// <param name="isClearAll"></param>
    public static void Clear(bool isClearAll = false)
    {
        Dictionary<RessKey, ResourceItem> tempDic = new Dictionary<RessKey, ResourceItem>();
        var enm = m_ressDic.GetEnumerator();
        while (enm.MoveNext())
        {
            ResourceItem item = (ResourceItem)enm.Current.Value;
            if (!isClearAll && (item.m_storgeType == RessStorgeType.RST_Always || CheckCanAlwaysStore(enm.Current.Key.m_type)))
            {
                tempDic.Add(enm.Current.Key, item);

                if (item.m_storgeType != RessStorgeType.RST_Always
                   && (!CheckIsUI(enm.Current.Key.m_type)))
                {
                    for (int i = 0; i < item.m_instanceList.Count; i++)
                    {
                        MonoBehaviour.Destroy(item.m_instanceList[i]);
                    }
                    item.m_instanceList.Clear();
                }
            }
            else
            {
                List<Object> tempLs = new List<Object>();
                for (int i = 0; i < item.m_instanceList.Count; i++)
                {
                    if (m_tempCacheOBLs.Contains(item.m_instanceList[i]))
                    {
                        tempLs.Add(item.m_instanceList[i]);
                    }
                    else
                    {
                        MonoBehaviour.Destroy(item.m_instanceList[i]);
                    }
                }

                if (tempLs.Count > 0)
                {
                    item.m_instanceList = tempLs;
                    tempDic.Add(enm.Current.Key, item);
                }
                else
                {
                    item.m_instanceList.Clear();
                    item.m_obContainer.m_object = null;
                }
            }
        }

        m_ressDic.Clear();
        m_ressDic = tempDic;
        m_tempCacheOBLs.Clear();

        //清除动态纹理
        ClearDynamicTextCache();
    }

    public static void Remove(RessType ressType, string ressName)
    {
        RessKey ressKey = new RessKey(ressType, ressName);
        if (m_ressDic.ContainsKey(ressKey))
        {
            ResourceItem item = m_ressDic[ressKey];
            for (int i = 0; i < item.m_instanceList.Count; i++)
            {
                MonoBehaviour.Destroy(item.m_instanceList[i]);
            }
            item.m_obContainer.m_object = null;
            m_ressDic.Remove(ressKey);
        }
    }

    static bool CheckCanAlwaysStore(RessType ressType)
    {
        bool can = CheckAllCanAlwaysStore();

        if (!can && CheckIsUI(ressType) && CheckCanUIAlwaysStore())
        {
            can = CheckCanUIAlwaysStore();
        }

        return can;
    }

    static bool CheckIsUI(RessType ressType)
    {
        if (ressType == RessType.RT_Window || ressType == RessType.RT_UIItem
           || ressType == RessType.RT_FightUIItem
           || ressType == RessType.RT_CommonWindow || ressType == RessType.RT_CommonUItem
           || ressType == RessType.RT_GuideUIItem
           || ressType == RessType.RT_LoadingUI)
        {
            return true;
        }

        return false;
    }

    //在Android机上，所有的资源不释放
    //由于内存，先改为资源释放
    static bool CheckAllCanAlwaysStore()
    {
        return false;

        //bool can = false;

        //if(Application.platform == RuntimePlatform.Android)
        //{
        //	can = true;
        //}
        //else if(Application.platform == RuntimePlatform.WindowsEditor)
        //{
        //	can = true;
        //}

        //return can;
    }

    public static bool CheckCanUIAlwaysStore()
    {
        return true;
        //        bool can = false;
        //
        //        if (Application.platform == RuntimePlatform.Android)
        //        {
        //            can = SystemInfo.systemMemorySize > 1024;
        //        }
        //        else if (Application.platform == RuntimePlatform.WindowsEditor)
        //        {
        //            can = false;
        //        }
        //        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        //        {
        //            can = false;
        //        }
        //
        //        return can;
    }

    public static RessStorgeType GetCacheStoreType()
    {
        if (CheckCanUIAlwaysStore())
        {
            return RessStorgeType.RST_Always;
        }
        else
        {
            return RessStorgeType.RST_Never;
        }
    }

    #region WWW loadTexture
    private static int m_tempCacheTextureNum = 100;
    public static int CacheTextureNum
    {
        set { m_tempCacheTextureNum = value; }
        get { return m_tempCacheTextureNum; }
    }
    private static int m_cacheTextureDeleteCount = 0;

    //缓存的图片
    private static Dictionary<string, Texture> SaveTextureDict = new Dictionary<string, Texture>();
    //可删除的图片列表 超过就顺序删
    private static List<string> NeverTextureDictKeys = new List<string>();
    // 记录正在下载设置的UITexture，防止下载过程中别的地方设置UITexture，然后被下载后的操作覆盖掉
    private static Dictionary<UITexture, WWW> DownloadingTexture = new Dictionary<UITexture, WWW>();

    private static Texture SetTextureInDict(string url, Texture loadTexture, RessStorgeType StorgeType)
    {
        if (StorgeType == RessStorgeType.RST_Never)
        {
            if (!SaveTextureDict.ContainsKey(url))
            {
                if (NeverTextureDictKeys.Count > m_tempCacheTextureNum)
                {
                    Texture tempTexture = null;
                    if (SaveTextureDict.TryGetValue(NeverTextureDictKeys[0], out tempTexture))
                    {
                        MonoBehaviour.DestroyImmediate(tempTexture, true);
                        m_cacheTextureDeleteCount++;
                        if (m_cacheTextureDeleteCount > m_tempCacheTextureNum / 2)
                        {
                            m_cacheTextureDeleteCount = 0;
                            Resources.UnloadUnusedAssets();
                        }
                    }
                    SaveTextureDict.Remove(NeverTextureDictKeys[0]);
                    NeverTextureDictKeys.RemoveAt(0);
                }

                SaveTextureDict.Add(url, loadTexture);
                NeverTextureDictKeys.Add(url);
            }
        }
        else
        {
            if (!SaveTextureDict.ContainsKey(url))
            {
                SaveTextureDict.Add(url, loadTexture);
            }
        }
        return null;
    }

    public static void ClearDynamicTextCache()
    {
        foreach (var texture in SaveTextureDict.Values)
        {
            MonoBehaviour.DestroyImmediate(texture,true);
        }
        SaveTextureDict.Clear();
        NeverTextureDictKeys.Clear();
        m_cacheTextureDeleteCount = 0;
    }

    public static void LoadImag(Transform trans, string url, string default_image, bool isPixelPerfect, RessStorgeType StorgeType, bool localSave = false)
    {
        if (trans == null || (string.IsNullOrEmpty(url) && string.IsNullOrEmpty(default_image)))
        {
            Debug.Log("LoadImag trans is null or (url and default_image) IsNullOrEmpty");
            return;
        }

 //       Debug.LogError("LoadImag name = "+ trans.name + " url " + url);

        UITexture texture = trans.GetComponent<UITexture>();
        if (texture == null)
            return;

        //先判断内存有没有对应的url图片缓存 ，没有再进行本地图片的操作

        Texture backup_tex = null;
        if (string.IsNullOrEmpty(url))
        {
            // url为空，则直接看看default图片有没有加载过，加载过备份则直接使用缓存数据
            if (SaveTextureDict.ContainsKey(default_image))
            {
                backup_tex = SaveTextureDict[default_image];
            }
        }
        else
        {
            // url非空且能找到对应url的缓存数据，则直接使用
            if (SaveTextureDict.ContainsKey(url))
            {
 //               Debug.LogError("url " + url + "real in SaveTextureDict");
                backup_tex = SaveTextureDict[url];
            }
        }

        bool hasLocalPic = false;//是否本地存了图片
        string localUrl = "";

        if (backup_tex == null)
        {
            localUrl = GetLocalDownImgUrl(url);
            if (Util.CheckFileExists(localUrl))
            {
 //               Debug.LogError("LoadImgIEn url= " + url + " has localurl= " + localUrl);
                hasLocalPic = true;
                url = "file:///" + localUrl;
            }


            backup_tex = null;
            if (string.IsNullOrEmpty(url))
            {
                // url为空，则直接看看default图片有没有加载过，加载过备份则直接使用缓存数据
                if (SaveTextureDict.ContainsKey(default_image))
                {
                    backup_tex = SaveTextureDict[default_image];
                }
            }
            else
            {
                // url非空且能找到对应url的缓存数据，则直接使用
                if (SaveTextureDict.ContainsKey(url))
                {
 //                   Debug.LogError("url " + url + "real in SaveTextureDict");
                    backup_tex = SaveTextureDict[url];
                }
            }
        }

        //如果有缓存纹理，直接开始设置
        if (backup_tex != null)
        {
 //           Debug.LogError("url =" + url + " is cache in memory,no need download");
            texture.mainTexture = backup_tex;
            if (isPixelPerfect)
                texture.MakePixelPerfect();
            // 如果有正在下载的，清掉记录，会在下载返回的地方判断，清掉了证明下载的图片是过期的，直接丢弃
            if (DownloadingTexture.ContainsKey(texture))
            {
                DownloadingTexture.Remove(texture);
            }
            texture.enabled = false;
            texture.enabled = true;
            return;
        }

        GameRoot.Instance.StartCoroutine(LoadImgIEn(texture, url, default_image, isPixelPerfect, StorgeType, localSave, hasLocalPic,localUrl));
    }

    private static IEnumerator LoadImgIEn(UITexture texture, string url, string default_image, bool isPixelPerfect, RessStorgeType StorgeType, bool localSave,bool hasLocalPic,string localUrl)
    {
        Texture texture2D = null;
        if (!string.IsNullOrEmpty(url) && (url.Contains("http") || hasLocalPic))
        {
            //开始下载图片
            var www = new WWW(url);
            //记录下正在下载设置的UITexture,如果已经有了就后来的覆盖前边的
            if (DownloadingTexture.ContainsKey(texture))
            {
                DownloadingTexture[texture] = www;
            }
            else
            {
                DownloadingTexture.Add(texture, www);
            }
            // 去下载的时候将图片置空，或者置一个转菊花图片
            texture.mainTexture = null;

            yield return www;

            // 如果没有被清掉，证明下载期间没有别的地方来设置UITexture，正常往下走
            // 如果被清掉了或者被覆盖了，则直接返回
            if (DownloadingTexture.ContainsKey(texture) && DownloadingTexture[texture] == www)
            {
                if (www != null && www.isDone && www.texture != null && (www.texture.width != 8 || www.texture.height != 8))
                {
                    texture2D = www.texture;
                    SetTextureInDict(url, texture2D, StorgeType);

                    if (texture2D && localSave && !hasLocalPic)
                    {
                        File.WriteAllBytes(localUrl, www.bytes);
                    }
                }
            }
            else
            {
                yield break;
            }
        }

        // url为空或者url加载图片不成功，则用默认图片 进度列会删掉本地资源 再也用不了
        if (texture2D == null)
        {
            texture2D = TextureManager.Instance.LoadTexture(default_image);
        }
        else
        {
            SetTextureInDict(url, texture2D, StorgeType);
        }

        // 没有默认图片这个就变空好了,设置成null吧
        texture.mainTexture = texture2D;

        if (isPixelPerfect)
            texture.MakePixelPerfect();
        // 如果有正在下载的，清掉记录，会在下载返回的地方判断，清掉了证明下载的图片是过期的，直接丢弃
        if (DownloadingTexture.ContainsKey(texture))
        {
            DownloadingTexture.Remove(texture);
        }

        texture.enabled = false;
        texture.enabled = true;
    }

    /// <summary>
    /// 获取本地资源地址
    /// </summary>
    private static string GetLocalDownImgUrl(string url)
    {
        string localUrl = "";
        int index = url.LastIndexOf("/");
        if (index > 0)
        {
            string picName = url.Substring(index);
            localUrl = Util.m_persistPath + "/LocalDownImg/" + picName;//目录在gameRoot里面已经创建好
        }

        return localUrl;
    }
    #endregion
}
