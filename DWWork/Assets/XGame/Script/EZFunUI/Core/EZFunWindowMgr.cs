using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum WindowCloseBehaviour
{
    SetActiveFalseAndDestroyWhenNecessary,
    SetActiveFalse,
    FadeClose
}

public enum WindowBehaviourOnSceneChange
{
    Destroy,
    CloseButDontDestroy,
    DontClose
}

public interface IWindowCloseBehaviour
{
    //关闭时，是否能否根据情况销毁此窗口
    WindowCloseBehaviour GetCloseBehaviour();
}

public interface ISceneChangeBehaviour
{
    WindowBehaviourOnSceneChange GetSceneChangeBehaviour();
}

public interface IWindowMemoryOptmizePolicy
{
    bool ShouldDestroyWindowOnClose();
}

public class DefaultWindowMemoryOptmizePolicy : IWindowMemoryOptmizePolicy
{
    #region IWindowMemoryOptmizePolipcy implementation

    public bool ShouldDestroyWindowOnClose()
    {
        //系统总内存<=1G，则销毁; 后续可以检测可用内存，根据剩余内存修改当前返回值
        return systemMemorySize <= 1024;
    }

    #endregion

    private int systemMemorySize;

    public DefaultWindowMemoryOptmizePolicy()
    {
        systemMemorySize = SystemInfo.systemMemorySize;
        Debug.Log("systemMemorySize:" + systemMemorySize);
    }

}

public class AlwaysDontDestroyWindowPolicy : IWindowMemoryOptmizePolicy
{
    public bool ShouldDestroyWindowOnClose()
    {
        return false;
    }
}

public class AlwaysDestroyPossibleWindowPolicy : IWindowMemoryOptmizePolicy
{
    public bool ShouldDestroyWindowOnClose()
    {
        return true;
    }
}

public enum SpecialWindowDepthEnum
{
    SWE_ShowHidenWindow = -1000,
    SWE_MainCityWindow = 0,
    SWE_BelowPlayWindow = 2,
    SWE_PlayWindow = 3,
    SWE_MainUIWindow = 5,
    SWE_ActPopWindow = 6,
    SWE_JuXianUIWindow = 2,
    SWE_PlotBubble = 7,
    SWE_SmallChatWindow = 8,
    SWE_MainMenuWindow = 30,//一级界面的Depth小于30，其他界面的Depth大于30
    SWE_SceneInWindow = 50,
    SWE_ErrTipsWindow = 930,
    SWE_CombatValueTipsWindow = 940,
    SWE_NewWeaponWindow = 950,
    SWE_GuideWindow = 1000,
    SWE_GuideTarget = 1010,
    SWE_GuideFinger = 1020,
    SWE_WaitingWindow = 1030,
    SWE_ErrorWindow = 1075,
    SWE_LoadingWindow = 900,
    SWE_ShowTopMsgWindow = 1060,
    SWE_LoadingError = 1070,
    SWE_WEBVIEW = 1080,
    SWE_NetWorkError = 1200,
}

public struct CWindow
{
    public EZFunWindowEnum m_windowEnum;
    public string m_windowName;

    public CWindow(EZFunWindowEnum windowEnum, string luaWindowName)
    {
        m_windowEnum = windowEnum;
        if (!string.IsNullOrEmpty(luaWindowName))
        {
            m_windowName = luaWindowName;
        }
        else
        {
            m_windowName = windowEnum.ToString();
        }
    }

    public bool Equals(CWindow cwindow)
    {
        return m_windowName.Equals(cwindow.m_windowName);
    }

    public override int GetHashCode()
    {
        return m_windowName.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (!(obj is CWindow))
        {
            return false;
        }

        CWindow IDObj = (CWindow)obj;
        return Equals(IDObj);
    }
}

public delegate void OpenWindowListener(EZFunWindowEnum windowEnum, bool open, int state);
public class EZFunWindowMgr : MonoBehaviour
{

    private IWindowMemoryOptmizePolicy m_memoryOptmizePolicy;
    public void SetMemoryOptmizePolicy(IWindowMemoryOptmizePolicy policy)
    {
        m_memoryOptmizePolicy = policy;
    }

    static EZFunWindowMgr _instance = null;
    public static EZFunWindowMgr Instance
    {
        get
        {
            return _instance;
        }
    }

    const string UI_ROOT_PREFAB_PATH = "Prefab/EZFunUI/UIRoot";
    public Transform m_uiRootTrans = null;
    public Transform m_windowRootTrans = null;
    public Transform m_cameraRootTrans = null;
    public Transform m_storeItemRootTrans = null;
    public bool m_hasInitWinMgrDone = false;
    public static bool m_isUpdateSys = false;
    private Dictionary<string, System.Action> m_openWinFunctionDic = new Dictionary<string, Action>();
    public const int LUA_WINDOW_COVER_STATE = -1003;

    private Dictionary<string, WindowTypeAttribute> m_registerWindowInfoDic = new Dictionary<string, WindowTypeAttribute>();

    public OpenWindowListener WindowStateListener
    {
        get;
        set;
    }

    void Awake()
    {
        //instance
        _instance = this;
        //_instance.SetMemoryOptmizePolicy (new DefaultWindowMemoryOptmizePolicy ());
		_instance.SetMemoryOptmizePolicy (new AlwaysDontDestroyWindowPolicy());

        //for debug
//        _instance.SetMemoryOptmizePolicy(new AlwaysDestroyPossibleWindowPolicy());

        var data = EZFunTools.StreamPath;
        ResourceManager.m_readFileCallback = EZFunTools.ReadFile;
        X2Util.m_getShaderCb = EZFunTools.FindShader;
        DebugEz.m_getShaderCb = EZFunTools.FindShader;
        StartCoroutine(InitMgrCoroutine());
    }

    private bool m_needDisbleCamera = true;
    public void DestoryUpdateUI()
    {
        m_needDisbleCamera = false;
        GameObject.DestroyImmediate(GameObject.Find("UIRootTemp"));
        m_needDisbleCamera = true;
    }

    IEnumerator InitMgrCoroutine()
    {
        if (!X2UpdateSys.m_isUpdating)
        {
            yield return null;
        }

        //init uiroot
        var uiroot = GameObject.Find("UIRoot");
        if (uiroot != null)
        {
            GameObject.Destroy(uiroot);
        }

        GameObject gb = null;
        if (m_isUpdateSys)
        {
            gb = GameObject.Find("UIRootTemp");
        }
        else
        {
            gb = (GameObject)ResourceMgr.GetInstantiateAsset(RessType.RT_CommonUItem, "UIRoot", RessStorgeType.RST_Always);
            gb.name = "UIRoot";
        }
        EZFunUITools.SetActive(gb, true);
        m_uiRootTrans = gb.transform;
        m_uiRootTrans.parent = transform;

        m_windowRootTrans = EZFunUITools.GetTrans(m_uiRootTrans, "window_root");
        m_cameraRootTrans = EZFunUITools.GetTrans(m_uiRootTrans, "camera_root");

        MemoryWarnReporter.Register(OnReceiveMemoryWarning);

        m_hasInitWinMgrDone = true;
    }

    private List<CWindow> m_toDestroyList = new List<CWindow>(50);
    private void OnReceiveMemoryWarning()
    {
        DestroyClosedWindows(true);
    }

    private void DestroyClosedWindows(bool sendMemoryWarning = false)
    {
        if (m_windowDic == null || m_windowDic.Count == 0)
            return;

        var ite = m_windowDic.GetEnumerator();
        while (ite.MoveNext())
        {
            var kvp = ite.Current;
            var winRoot = kvp.Value.m_windowRoot;

            //没有显示的窗口接收memory warning
            if (winRoot != null && !winRoot.m_open)
            {

                if (sendMemoryWarning)
                {

                    winRoot.OnReceiveMemoryWarning();
                    m_toDestroyList.Add(kvp.Key);

                }
                else
                {
                    //两个原因调用destroyClosedWindows：1、memoryWarning 2、切换场景
                    //sendMemoryWarning不为true,表示切换场景。后续这里重构为传入Reason枚举

                    var sceneChangeBehaviour = winRoot.GetSceneChangeBehaviour();
                    if (sceneChangeBehaviour == WindowBehaviourOnSceneChange.Destroy)
                    {
                        m_toDestroyList.Add(kvp.Key);
                    }

                }
            }

        }

        for (int i = 0; i < m_toDestroyList.Count; i++)
        {
            var k = m_toDestroyList[i];
            var winRoot = m_windowDic[k].m_windowRoot;
            DestroyWindow(k, winRoot);
        }

        m_toDestroyList.Clear();
    }

    public int GetScreenHeight()
    {
        return m_uiRootTrans.GetComponent<UIRoot>().activeHeight;
    }

    public int GetScreenWidth()
    {
        return (int)(Screen.width * 1f / Screen.height * EZFunWindowMgr.Instance.GetScreenHeight());
    }

    const string WINDOW_PREFAB_PATH = "Prefab/EZFunUI/Window/";
    class CWindowType
    {
        public WindowRoot m_windowRoot;
        public RessStorgeType m_storeType;
        public RessType m_ressType;
        public string m_windowName;
        public CWindowType(WindowRoot windowRoot, RessStorgeType storeType, RessType ressType, string windowName)
        {
            m_windowRoot = windowRoot;
            m_storeType = storeType;
            m_ressType = ressType;
            this.m_windowName = windowName;
        }
    }
    Dictionary<CWindow, CWindowType> m_windowDic = new Dictionary<CWindow, CWindowType>();
    //Control window status
    /// <summary>
    /// 原来luawindowName 强调lua文件 资源文件 三者绑定关系
    /// 现在命名为windowName 强调windowname的唯一性，意味着上面三者关系的多样化
    /// </summary>
    /// <param name="windowEnum"></param>
    /// <param name="ressType"></param>
    /// <param name="open"></param>
    /// <param name="state"></param>
    /// <param name="windowName"></param>
    public void SetWindowStatusForLua(EZFunWindowEnum windowEnum, RessType ressType, bool open = true, int state = 0, string windowName = "", object param = null)
    {
        SetWindowStatus(windowEnum, ressType, open, state, windowName, null, true, param);
    }
    /// <summary>
    ///  原来luawindowName 强调lua文件 资源文件 三者绑定关系
    /// 现在命名为windowName 强调windowname的唯一性，意味着上面三者关系的多样化
    /// </summary>
    /// <param name="windowEnum"></param>
    /// <param name="open"></param>
    /// <param name="state"></param>
    /// <param name="windowName"></param>
    /// <param name="animated"></param>
    /// <param name="others"></param>
    /// <returns></returns>
    public WindowRoot SetWindowStatus(EZFunWindowEnum windowEnum, bool open = true, int state = 0, string windowName = "", bool animated = true, object others = null)
    {
        return SetWindowStatus(windowEnum, RessType.RT_Window, open, state, windowName, null, animated, others);
    }

    /// <summary>
    /// 原来luawindowName 强调lua文件 资源文件 三者绑定关系
    /// 现在命名为windowName 强调windowname的唯一性，意味着上面三者关系的多样化
    /// </summary>
    /// <param name="windowEnum"></param>
    /// <param name="ressType"></param>
    /// <param name="open"></param>
    /// <param name="state"></param>
    /// <param name="windowName"></param>
    /// <param name="cb"></param>
    /// <param name="animated"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public WindowRoot SetWindowStatus(EZFunWindowEnum windowEnum, RessType ressType, bool open = true, int state = 0, string windowName = "", Action cb = null, bool animated = true, object parameters = null)
    {
        WindowRoot windowRoot = null;
        CWindow cwindow = new CWindow(windowEnum, windowName);
        //add by janus 2015/04/21
        if (!open && !m_windowDic.ContainsKey(cwindow))
        {

            return null;
        }
        // Mingrui Jiang CGG, stop nav in main city when a window is open
        // stop nav tip when another window is open
        //if (checkNeedCover(windowEnum, state, windowName))
        //{
        //    HandleCoverWindow.m_openCB = () =>
        //    {
        //        windowRoot = SetWindowStatus(windowEnum, windowName, ressType, open, state, animated, parameters);
        //        if (cb != null)
        //        {
        //            cb();
        //        }
        //    };
        //    SetWindowStatus(EZFunWindowEnum.cover_ui_window, RessType.RT_CommonWindow);
        //}
        //else
        {
            windowRoot = SetWindowStatus(windowEnum, windowName, ressType, open, state, animated, parameters);
            if (cb != null)
            {
                cb();
            }
        }
        if (!WindowStateListener.IsNull())
        {
            WindowStateListener(windowEnum, open, state);
        }

        return windowRoot;
    }

    private WindowRoot SetWindowStatus(EZFunWindowEnum windowEnum, string WindowName, RessType ressType, bool open, int state, bool animated, object parameters = null)
    {
        CWindow cwindow = new CWindow(windowEnum, WindowName);
        if (!m_windowDic.ContainsKey(cwindow))
        {
            InitWindowDic(windowEnum, ressType, RessStorgeType.RST_Never, WindowName);
        }

        WindowRoot windowRoot = null;

        if (m_windowDic.ContainsKey(cwindow))
        {
            windowRoot = m_windowDic[cwindow].m_windowRoot;
            if (windowRoot != null)
            {
                windowRoot.m_currentWindowEnum = windowEnum;
                windowRoot.m_windowName = cwindow.m_windowName;
                windowRoot.InitWindow(open, state, animated, parameters);
            }
        }

        return windowRoot;
    }

    public void DoAfterCloseWindow(CWindow cWin, WindowRoot winRoot)
    {
        if (winRoot == null)
            return;

        //默认destroy被close的window
        //if (winRoot.GetCloseBehaviour() == WindowCloseBehaviour.SetActiveFalseAndDestroyWhenNecessary)
        //{
        //    if (m_memoryOptmizePolicy.ShouldDestroyWindowOnClose())
        //        DestroyWindow(cWin, winRoot);
        //}

    }

    private void DestroyWindow(CWindow cWin, WindowRoot winRoot)
    {
        Debug.Log("DestroyWindow:" + cWin.m_windowEnum + " " + cWin.m_windowName);
        if (m_windowDic.ContainsKey(cWin))
        {
            m_windowDic.Remove(cWin);

            if (winRoot != null)
            {
                winRoot.OnWillDestroy();
                GameObject.Destroy(winRoot.gameObject);
            }
        }
    }

    public IEnumerator PreLoadWindow(EZFunWindowEnum windowEnum, RessType ressType, RessStorgeType ressStorgeType =
        RessStorgeType.RST_Never, string luaWindowName = "")
    {
        var ite = InitWindowDicAsync(windowEnum, ressType, ressStorgeType, luaWindowName);
        while (ite.MoveNext())
        {
            yield return null;
        }
    }

    //public bool checkNeedCover(EZFunWindowEnum windowEnum, int state, string windowName = "", bool isClose = false)
    //{
    //    return false;
    //    //if (GameStateMgr.Instance == null || GameStateMgr.Instance.GetCurStateType() == EGameStateType.LoadingState)
    //    //{
    //    //    return false;
    //    //}

    //    //if (m_isRecovering)
    //    //{
    //    //    return false;
    //    //}

    //    //if (CheckWindowOpen(windowEnum) && !isClose)
    //    //{
    //    //    return false;
    //    //}

    //    //if (windowEnum == EZFunWindowEnum.luaWindow)
    //    //{
    //    //    return CheckLuaWindowWhenCover(windowName, state);
    //    //}
    //    //else if ((int)windowEnum < (int)EZFunWindowEnum.FULL_WINDOW_SPLIT)
    //    //{
    //    //    return CheckSomeSpecialWindowWhenCover(windowEnum, state);
    //    //}
    //    //else
    //    //{
    //    //    return false;
    //    //}
    //}

    public bool CheckLuaWindowWhenCover(string windowName, int state)
    {
        WindowRoot currentWinroot = GetCurWindowRoot();
        string currentWinName = "";
        if (currentWinroot != null)
        {
            currentWinName = currentWinroot.m_windowName;
        }

        return WindowBaseLua.GetValueFrom<bool>("ezfunLuaTool", "CheckLuaWindowNeedCover", false, currentWinName, windowName, state);
    }


    public bool InitWindowDic(EZFunWindowEnum windowEnum, RessStorgeType ressStorgeType = RessStorgeType.RST_Never, string luaWindowName = "")
    {
        return InitWindowDic(windowEnum, RessType.RT_Window, ressStorgeType, luaWindowName);
    }

    public WindowTypeAttribute GetWindowAttribute(EZFunWindowEnum windowEnum, string windowName)
    {
        //Debug.Assert (windowName != null, "windowName is null, enum:" + windowEnum.ToString());

        var windowField = windowEnum.GetAttributeOfType<WindowTypeAttribute>();
        if (windowName != null && m_registerWindowInfoDic.ContainsKey(windowName))
        {
            windowField = m_registerWindowInfoDic[windowName];
        }
        return windowField;
    }

    public bool InitWindowDic(EZFunWindowEnum windowEnum, RessType ressType, RessStorgeType ressStorgeType =
        RessStorgeType.RST_Never, string luaWindowName = "")
    {
        CWindow cwindow = new CWindow(windowEnum, luaWindowName);

        if (!m_windowDic.ContainsKey(cwindow))
        {
            string windowName = cwindow.m_windowName;
            string windowPath = windowName;
            var windowField = windowEnum.GetAttributeOfType<WindowTypeAttribute>();
            if (m_registerWindowInfoDic.ContainsKey(windowName))
            {
                windowField = m_registerWindowInfoDic[windowName];
            }
            //配置的有路径就用配置的路径
            if (windowField != null && !string.IsNullOrEmpty(windowField.m_resPath))
            {
                windowPath = windowField.m_resPath;
            }
            //如果窗口是lua窗口，且窗口名称带有"."
            if (windowEnum == EZFunWindowEnum.luaWindow && windowName.Contains("."))
            {
                windowPath = windowPath.Replace('.', '/');
                Debug.Log(windowPath);
            }

            //update_ui_window特殊处理
            GameObject gb = null;
            if (windowEnum == EZFunWindowEnum.update_ui_window)
            {
                gb = GameObject.Find(windowEnum.ToString());
            }
            else
            {
                gb = ResourceMgr.GetInstantiateAsset(ressType, windowPath, ressStorgeType) as GameObject;
            }
            if (gb == null)
            {
                return false;
            }
            Transform trans = gb.transform;
            trans.parent = m_windowRootTrans;
            trans.name = windowName;
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;
            EZFunUITools.SetActive(trans.gameObject, false);

            WindowRoot windowRoot = trans.GetComponent<WindowRoot>();
            if (windowRoot == null)
            {
                windowRoot = getWindowRoot(gb, windowEnum, windowName);
                if (windowRoot == null)
                {
                    //Debug.LogError("[Can not find component]" + trans.name);
                    return false;
                }
                else
                {
                    m_windowDic.Add(cwindow, new CWindowType(windowRoot, ressStorgeType, ressType, windowName));
                    windowRoot.InitDic();
                    return true;
                }
            }
            else
            {
                m_windowDic.Add(cwindow, new CWindowType(windowRoot, ressStorgeType, ressType, windowName));
                windowRoot.InitDic();
                return true;
            }
        }
        else
        {
            //这段代码表示没看懂在干什么--
            //因为这个黑幕、task_hint这个页面需要在几个场景一直用。。
            //			if (windowEnum == EZFunWindowEnum.black_ui_window || windowEnum == EZFunWindowEnum.hint_ui_window)
            //            {
            //                string windowName = cwindow.m_windowName;
            //                string windowPath = windowName;
            //                var windowField = windowEnum.GetAttributeOfType<WindowTypeAttribute>();
            //                if (m_registerWindowInfoDic.ContainsKey(windowName))
            //                {
            //                    windowField = m_registerWindowInfoDic[windowName];
            //                }
            //                if (windowField != null && !string.IsNullOrEmpty(windowField.m_resPath))
            //                {
            //                    windowPath = windowField.m_resPath;
            //                }
            //                var item = m_windowDic[cwindow];
            //                item.m_storeType = ressStorgeType;
            //                item.m_ressType = ressType;
            //                var ritem = ResourceMgr.GetResourceItem(ressType, windowPath);
            //                ritem.m_storgeType = ressStorgeType;
            //            }
            //            return true;

            var winRoot = m_windowDic[cwindow].m_windowRoot;
            if (winRoot != null)
            {

                if (winRoot.GetSceneChangeBehaviour() != WindowBehaviourOnSceneChange.Destroy)
                {


                    string windowName = cwindow.m_windowName;
                    string windowPath = windowName;
                    var windowField = windowEnum.GetAttributeOfType<WindowTypeAttribute>();
                    if (m_registerWindowInfoDic.ContainsKey(windowName))
                    {
                        windowField = m_registerWindowInfoDic[windowName];
                    }
                    if (windowField != null && !string.IsNullOrEmpty(windowField.m_resPath))
                    {
                        windowPath = windowField.m_resPath;
                    }
                    //如果窗口是lua窗口，且窗口名称带有"."
                    if (windowEnum == EZFunWindowEnum.luaWindow && windowName.Contains("."))
                    {
                        windowPath = windowPath.Replace('.', '/');
                        Debug.LogError(windowPath);
                    }


                    var item = m_windowDic[cwindow];
                    item.m_storeType = ressStorgeType;
                    item.m_ressType = ressType;
                    var ritem = ResourceMgr.GetResourceItem(ressType, windowPath);
                    ritem.m_storgeType = ressStorgeType;

                }

                return true;

            }
            else
            {
                Debug.LogError("WinRoot is null");
                return false;
            }
        }
    }

    public IEnumerator InitWindowDicAsync(EZFunWindowEnum windowEnum, RessType ressType, RessStorgeType ressStorgeType =
        RessStorgeType.RST_Never, string luaWindowName = "")
    {
        CWindow cwindow = new CWindow(windowEnum, luaWindowName);

        if (!m_windowDic.ContainsKey(cwindow))
        {
            string windowName = cwindow.m_windowName;
            string windowPath = windowName;
            var windowField = GetWindowAttribute(windowEnum, windowName);
            if (windowField != null && !string.IsNullOrEmpty(windowField.m_resPath))
            {
                windowPath = windowField.m_resPath;
            }

            //如果窗口是lua窗口，且窗口名称带有"."
            if (windowEnum == EZFunWindowEnum.luaWindow && windowName.Contains("."))
            {
                string[] splitArray = windowName.Split('.');
                if (splitArray.Length > 1)
                {
                    windowPath = "";
                    for (int i = 0; i < splitArray.Length; i++)
                    {
                        windowPath += splitArray[i];
                    }
                }
                Debug.LogError(windowPath);
            }

            //update_ui_window特殊处理
            GameObject gb = null;
            if (windowEnum == EZFunWindowEnum.update_ui_window)
            {
                gb = GameObject.Find(windowEnum.ToString());
            }
            else
            {
                bool loadFinished = false;
                ResourceMgr.GetAsyncInstantiateAsset(ressType, windowPath, ressStorgeType, (o) =>
                {
                    loadFinished = true;
                    gb = o as GameObject;
                });

                while (!loadFinished)
                {
                    yield return null;
                }
            }
            if (gb == null)
            {
                Debug.LogError("cannot load window obj");
                yield break;
            }
            Transform trans = gb.transform;
            trans.parent = m_windowRootTrans;
            trans.name = windowName;
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;
            EZFunUITools.SetActive(trans.gameObject, false);

            WindowRoot windowRoot = trans.GetComponent<WindowRoot>();
            if (windowRoot == null)
            {
                windowRoot = getWindowRoot(gb, windowEnum, windowName);
                if (windowRoot == null)
                {
                    Debug.LogError("[Can not find component]" + trans.name);
                    yield break;
                }
                else
                {
                    m_windowDic.Add(cwindow, new CWindowType(windowRoot, ressStorgeType, ressType, windowName));
                    windowRoot.InitDic();

                    yield break;
                }
            }
            else
            {
                m_windowDic.Add(cwindow, new CWindowType(windowRoot, ressStorgeType, ressType, windowName));
                windowRoot.InitDic();

                yield break;
            }
        }
        else
        {
            yield break;
        }
    }

    /// <summary>
    /// 以后新加的模块不要再在这儿添加case了，都把枚举加上WindowTypeAttribute
    /// </summary>
    /// <param name="gb"></param>
    /// <param name="windowEnum"></param>
    /// <param name="luaWindowName"></param>
    /// <returns></returns>
    WindowRoot getWindowRoot(GameObject gb, EZFunWindowEnum windowEnum, string windowName)
    {
        WindowRoot windowRoot = null;
        var luaWindowName = windowName;
        WindowTypeAttribute attribute = windowEnum.GetAttributeOfType<WindowTypeAttribute>();
        if (m_registerWindowInfoDic.ContainsKey(windowName))
        {
            attribute = m_registerWindowInfoDic[windowName];
        }
        if (attribute != null)
        {
            if (attribute.m_classType == null && !string.IsNullOrEmpty(attribute.m_luaFile))
            {
                windowRoot = EZFunTools.GetOrAddComponent<LuaWindowRoot>(gb);
                luaWindowName = attribute.m_luaFile;
                if (!luaWindowName.Contains("."))
                {
                    luaWindowName = "LuaWindow." + luaWindowName;
                }
                ((LuaWindowRoot)windowRoot).InitLua(luaWindowName, attribute.m_needCreate);
            }
            else if (attribute.m_classType != null)
            {
                windowRoot = EZFunTools.GetOrAddComponent(gb, attribute.m_classType) as WindowRoot;
            }
            return windowRoot;
        }
        // TODO: lua系统window
        if (windowEnum == EZFunWindowEnum.luaWindow)
        {
            windowRoot = EZFunTools.GetOrAddComponent<LuaWindowRoot>(gb);
            if (!luaWindowName.Contains("."))
            {
                luaWindowName = "LuaWindow." + luaWindowName;
            }
            ((LuaWindowRoot)windowRoot).InitLua(luaWindowName, false);
        }
        return windowRoot;
    }

    // shwo并返回该window
    public WindowRoot ShowAndReturnWindow(EZFunWindowEnum windowEnum,
        bool open = true,
        int state = 0,
        RessStorgeType ressStorgeType = RessStorgeType.RST_Never,
        string luaWindowName = "")
    {
        WindowRoot wr = null;

        CWindow cwindow = new CWindow(windowEnum, luaWindowName);

        if (!m_windowDic.ContainsKey(cwindow))
        {
            string windowName = cwindow.m_windowName;

            GameObject gb = (GameObject)ResourceMgr.GetInstantiateAsset(RessType.RT_Window, windowName, ressStorgeType);
            Transform trans = gb.transform;
            trans.parent = m_windowRootTrans;
            trans.name = windowName;
            trans.localScale = Vector3.one;
            EZFunUITools.SetActive(trans.gameObject, false);

            wr = trans.GetComponent<WindowRoot>();
            if (wr == null)
            {
                wr = getWindowRoot(gb, windowEnum, luaWindowName);
                if (wr == null)
                {
                    Debug.LogError("[Can not find component]" + trans.name);
                    return null;
                }
                else
                {
                    m_windowDic.Add(cwindow, new CWindowType(wr, ressStorgeType, RessType.RT_Window, luaWindowName));
                    wr.InitDic();
                }
            }
            else
            {
                m_windowDic.Add(cwindow, new CWindowType(wr, ressStorgeType, RessType.RT_Window, luaWindowName));
                wr.InitDic();
            }
        }
        else
        {
            wr = m_windowDic[cwindow].m_windowRoot;
        }

        if (wr)
        {
            SetWindowStatus(windowEnum, open, state);
        }

        return wr;
    }

    bool m_isClearingWin = false;
    public void ClearWindow()
    {
        Debug.Log("WindowMgr.ClearWindow");

        EventSys.Instance.AddEventNow(EEventType.UI_Msg_ClearCamera);
        m_isClearingWin = true;
        List<WindowRoot> needCloseWinRoot = new List<WindowRoot>();

        foreach (var kv in m_windowDic)
        {
            var winRoot = kv.Value.m_windowRoot;
            var sceneChangeBehaviour = winRoot.GetSceneChangeBehaviour();

            if (sceneChangeBehaviour != WindowBehaviourOnSceneChange.DontClose)
            {
                needCloseWinRoot.Add(kv.Value.m_windowRoot);
            }

        }

        for (int i = 0; i < needCloseWinRoot.Count; i++)
        {
            var winRoot = needCloseWinRoot[i];

            winRoot.InitWindow(false, WindowRoot.SYS_AUTO_CLOSE_STATE);
            winRoot.ClearTempData();
        }

        DestroyClosedWindows();

        m_winCloseGroupLs.Clear();
        m_isClearingWin = false;
    }

    public void ClearOpenedWindow()
    {
        List<WindowRoot> needCloseWinRoot = new List<WindowRoot>();
        foreach (var kv in m_windowDic)
        {
            //if (kv.Value.m_storeType == RessStorgeType.RST_Never)
            //{
            needCloseWinRoot.Add(kv.Value.m_windowRoot);
            //}
        }

        for (int i = 0; i < needCloseWinRoot.Count; i++)
        {
            if (needCloseWinRoot[i] != null)
            {
                needCloseWinRoot[i].InitWindow(false);
                needCloseWinRoot[i].ClearTempData();
            }
        }
    }

    public void RemoveWindow(EZFunWindowEnum windowEnum, string luaWindowName = "")
    {
        CWindow cwindow = new CWindow(windowEnum, luaWindowName);

        if (m_windowDic.ContainsKey(cwindow))
        {
            m_windowDic.Remove(cwindow);
        }
    }
    public void InitWaitWindow()
    {
        //InitWindowDic(EZFunWindowEnum.wait_ui_window, RessType.RT_CommonWindow, RessStorgeType.RST_Always);
        //InitWindowDic(EZFunWindowEnum.error_ui_window, RessType.RT_CommonWindow, RessStorgeType.RST_Always);
        //InitWindowDic(EZFunWindowEnum.ToolTips_ui_window, RessType.RT_CommonWindow, RessStorgeType.RST_Always);
        //InitWindowDic(EZFunWindowEnum.pvp_wait_ui_window, RessType.RT_CommonWindow, RessStorgeType.RST_Always);
        StartCoroutine(DelayNetCallBack());
    }

    IEnumerator DelayNetCallBack()
    {
        while (GameRoot.Instance == null || CNetSys.Instance == null)
        {
            yield return null;
        }
        CNetSys.Instance.m_BlockCallBack = EZFunWindowMgr.BlockFunc;
    }

    public static void BlockFunc(int preState, TransferStateInput input, int curState)
    {
        switch (curState)
        {
            case EBLOCKSTATE.wait1:
                var inputMsg = input as TransferStateInputMsg<CDealerCBOP>;
                if (inputMsg != null && inputMsg.m_sMsg.m_req.needLockScreen)
                {
                    //                           Debug.LogError("cmd = " + inputMsg.m_sMsg.m_req.cmd + "lock screen");
                    EventSys.Instance.AddEvent(EEventType.ShowWaitWindow, true, 1);
                }
                else
                {
                    if (inputMsg != null)
                    {
                        //                                Debug.LogError("cmd = " + inputMsg.m_sMsg.m_req.cmd + " not need lock screen");
                    }
                }
                break;
            case EBLOCKSTATE.wait2:
                EventSys.Instance.AddEvent(EEventType.ShowWaitWindow, true, 2);
                break;
            case EBLOCKSTATE.normal:
                EventSys.Instance.AddEvent(EEventType.ShowWaitWindow, false);
                break;
        }
    }

    public int GetOpenedWindowNum()
    {
        if (m_usedCameraList.Count > 0)
        {
            int num = 0;
            for (int lsIndex = 0; lsIndex < m_usedCameraList.Count; lsIndex++)
            {
                num += m_usedCameraList[lsIndex].m_windowNumber;
            }
            return num;
        }
        else
        {
            return 0;
        }
    }

    public bool CheckWindowOpen(EZFunWindowEnum enumWindow, string luaWindowName = "")
    {
        CWindow cwindow = new CWindow(enumWindow, luaWindowName);

        if (m_windowDic.ContainsKey(cwindow))
        {
            WindowRoot windowRoot = m_windowDic[cwindow].m_windowRoot;
            if (windowRoot == null)
            {
                return false;
            }
            return NGUITools.GetActive(windowRoot.gameObject);
        }
        else
        {
            return false;
        }
    }

    public Camera GetCameraByLayer(int layer)
    {
        for (int i = 0; i < m_usedCameraList.Count; ++i)
        {
            var cam = m_usedCameraList[i];
            if (cam.m_layer == layer)
            {
                return cam.m_camera;
            }
        }
        return null;
    }

    #region UICamera
    //ui camera begin
    public class UICameraStruct
    {
        public Transform m_trans;
        public bool m_hasScrollView;
        public Camera m_camera;
        public UICamera m_uicamera;
        public int m_depth;
        public int m_layer;
        public int m_windowNumber;
        public bool m_isSpecialWindow;
        public bool m_systemWindow;
        public EZFunWindowEnum m_curWindow;
        public string m_luaWindowName;
    }

    List<UICameraStruct> m_usedCameraList = new List<UICameraStruct>();
    Dictionary<int, CLayerData> m_usedLayerToWinNumList = new Dictionary<int, CLayerData>();
    public UICameraStruct GetUICameraLayer(bool hasScrollView, int SpecialDepth = -1, bool sysWindow = false, Transform trans = null, bool isTipWindow = false)
    {
        hasScrollView = true;
        UICameraStruct uicameraItem = new UICameraStruct();

        //last window do not has scroll view, share the same camera
        if (m_usedCameraList.Count > 0 && SpecialDepth == -1)
        {
            int index = m_usedCameraList.Count - 1;
            UICameraStruct item = m_usedCameraList[index];
            while (item.m_isSpecialWindow && index > 0)
            {
                --index;
                item = m_usedCameraList[index];
            }

            if (!item.m_isSpecialWindow)
            {
                if (!item.m_systemWindow)
                {
                    sysWindow = false;
                }
            }

            if (!item.m_isSpecialWindow && !item.m_hasScrollView && item.m_systemWindow == sysWindow)
            {
                item.m_hasScrollView = hasScrollView;
                item.m_windowNumber++;
                m_usedCameraList[index] = item;
                m_usedLayerToWinNumList[item.m_layer] = new CLayerData(1, null, item.m_windowNumber);

                //Debug.Log("Reuse Camera:" + item.m_camera.gameObject.name + " for win:" + item.m_curWindow.ToString());

                return item;
            }
        }


        GameObject gb = (GameObject)ResourceMgr.GetInstantiateAsset(RessType.RT_CommonUItem, "UICamera", RessStorgeType.RST_Always);
        Transform uicamera_trans = gb.transform;
        uicamera_trans.parent = m_cameraRootTrans;
        uicamera_trans.localScale = Vector3.one;
        uicamera_trans.localPosition = Vector3.zero;

        //Debug.Log("Create New Camera:" + (trans == null ? "null" : trans.name));

        uicameraItem.m_trans = uicamera_trans;
        uicameraItem.m_camera = uicameraItem.m_trans.GetComponent<Camera>();
        uicameraItem.m_camera.enabled = true;
        uicameraItem.m_uicamera = uicameraItem.m_trans.GetComponent<UICamera>();
        if (!isTipWindow)
        {
            EZFunWindowMgr.SetUIAllowMultiTouch(false);
        }

        //set uicamera
        uicameraItem = SetUICamera(uicameraItem, SpecialDepth, sysWindow, trans);
        uicameraItem.m_hasScrollView = hasScrollView;
        uicameraItem.m_windowNumber = 1;
        m_usedCameraList.Add(uicameraItem);

        uicameraItem.m_uicamera.enabled = true;

        return uicameraItem;
    }

    public bool CheckWindowIsOpenByCamera(Camera camera)
    {
        for (int i = 0; i < m_usedCameraList.Count; i++)
        {
            if (m_usedCameraList[i].m_camera == camera)
            {
                return true;
            }
        }

        return false;
    }

    public void RemoveCamera(int m_layer)
    {
        if (!m_needDisbleCamera)
        {
            return;
        }
        for (int i = m_usedCameraList.Count - 1; i >= 0; i--)
        {
            UICameraStruct item = m_usedCameraList[i];
            if (item != null &&
                item.m_layer == m_layer)
            {
                CLayerData ld = null;
                if (!m_usedLayerToWinNumList.TryGetValue(m_layer, out ld))
                {
                    continue;
                }
                item.m_windowNumber--;
                ld = new CLayerData(1, null, item.m_windowNumber);
                if (item.m_windowNumber == 0)
                {
                    m_usedCameraList.RemoveAt(i);
                    m_usedLayerToWinNumList.Remove(m_layer);

                    if (item.m_uicamera != null)
                    {
                        item.m_uicamera.enabled = false;
                    }

                    if (item.m_trans != null)
                    {
                        EZFunUITools.SetActive(item.m_trans.gameObject, false);
                    }
                }
                else
                {
                    item.m_hasScrollView = false;
                }
                break;
            }
        }
    }

    //set camera
    const int UICAMERA_SysWindow_START_DEPTH = 15;

    public const int BLACK_MAX_DEPTH = 200;

    const int UICAMERA_DELTA_DEPTH = 2; //摄像机层级累加 这里设想应该是给界面内 多个摄像机留空间
    const int UI_CAN_USE_START_LAYER = 21;
    const int UICAMERA_START_LAYER = 21;
    const int UICAMERA_PopWindow_START_DEPTH = 35;
    const int UICAMERA_END_LAYER = 30;
    UICameraStruct SetUICamera(UICameraStruct item, int specialDepth, bool sysWindow, Transform trans)
    {
        //set depty
        if (specialDepth != -1)
        {
            item.m_depth = specialDepth;
            item.m_isSpecialWindow = true;
        }
        else
        {
            item.m_depth = GetNotSpecialWindowDepth(sysWindow);
            item.m_isSpecialWindow = false;
        }
        //set layer
        if (m_usedCameraList.Count == 0)
        {
            item.m_layer = getUnusedLayer(1, trans);
        }
        else
        {
            item.m_layer = getUnusedLayer(1, trans);
        }

        GameObject cameraGB = item.m_trans.gameObject;
        EZFunTools.SetActive(cameraGB, true);
        cameraGB.name = "Camera_" + item.m_layer;
        cameraGB.layer = item.m_layer;
        EZFunTools.SetCameraDepth(item.m_camera, item.m_depth);
        item.m_camera.cullingMask = 1 << item.m_layer;
        item.m_uicamera.eventReceiverMask = (LayerMask)(1 << item.m_layer);
        item.m_systemWindow = sysWindow;

        return item;
    }

    class CLayerData
    {
        public int m_type; // 1, window, 2, scroll view
        public Transform m_winTrans;
        public int m_num;

        public CLayerData(int type, Transform trans, int num)
        {
            m_type = type;
            m_winTrans = trans;
            m_num = num;
        }
    }

    public int getUnusedLayer(int type, Transform winTrans) //type 1: 只提供给窗口自己使用；2，同一个窗口的会公用一个layer；3，完全属于自己的layer
    {
        int layer = CaculateUnusedLayer(type, winTrans);
        if (layer == 0)
        {
            HandleNotEnoughLayer(winTrans);
            layer = CaculateUnusedLayer(type, winTrans);
            if (layer == 0)
            {
                var enumertor = m_usedLayerToWinNumList.GetEnumerator();
                while (enumertor.MoveNext())
                {
                    Debug.LogError(" Not Enough Layer:" + enumertor.Current.Key + " winName:" +
                        enumertor.Current.Value.m_winTrans.name +
                        " type:" + enumertor.Current.Value.m_type);
                }
            }
        }
        return layer;
    }

    private int CaculateUnusedLayer(int type, Transform winTrans) //type 1: 只提供给窗口自己使用；2，同一个窗口的会公用一个layer；3，完全属于自己的layer
    {
        //Recovery scroll view layer
        Dictionary<int, CLayerData> tempDic = new Dictionary<int, CLayerData>();
        foreach (var kv in m_usedLayerToWinNumList)
        {
            CLayerData layerData = kv.Value;
            if (layerData.m_type == 2 && (layerData.m_winTrans == null || !NGUITools.GetActive(layerData.m_winTrans.gameObject)))
            {
                continue;
            }

            if (layerData.m_type == 3 && (layerData.m_winTrans == null || !NGUITools.GetActive(layerData.m_winTrans.gameObject)))
            {
                continue;
            }

            if (layerData.m_type == 1 && layerData.m_winTrans == null)
            {
                continue;
            }

            tempDic.Add(kv.Key, kv.Value);
        }

        m_usedLayerToWinNumList.Clear();
        m_usedLayerToWinNumList = tempDic;

        if (type == 2)
        {
            CLayerData temp = null;
            int tempKey = 0;
            foreach (var kv in m_usedLayerToWinNumList)
            {
                CLayerData layerData = kv.Value;
                if (layerData.m_type == 2 && layerData.m_winTrans == winTrans)
                {
                    tempKey = kv.Key;
                    temp = new CLayerData(layerData.m_type, layerData.m_winTrans, layerData.m_num + 1);
                    break;
                }
            }

            if (temp != null)
            {
                m_usedLayerToWinNumList[tempKey] = temp;
                return tempKey;
            }
        }

        int startLayer = UICAMERA_START_LAYER;
        //if (!GameRoot.Instance.IsNull() && GameStateMgr.Instance.GetCurStateType() == EGameStateType.JuXianZhuangState)
        //{
        //	startLayer = UI_JUXIAN_USE_START_LAYER;
        //}
        //else
        if (!GameRoot.Instance.IsNull() &&
           ( GameStateMgr.Instance.GetCurStateType() != EGameStateType.LoadingState))
        {
            startLayer = UI_CAN_USE_START_LAYER;
        }

        for (int i = startLayer; i <= UICAMERA_END_LAYER; i++)
        {
            if (!m_usedLayerToWinNumList.ContainsKey(i))
            {
                if (type == 1)
                {
                    CLayerData layerData = new CLayerData(1, winTrans, 1);
                    m_usedLayerToWinNumList.Add(i, layerData);
                }
                else if (type == 2)
                {
                    CLayerData layerData = new CLayerData(2, winTrans, 1);
                    m_usedLayerToWinNumList.Add(i, layerData);
                }
                else if (type == 3)
                {
                    CLayerData layerData = new CLayerData(3, winTrans, 1);
                    m_usedLayerToWinNumList.Add(i, layerData);
                }
                return i;
            }
        }

        return 0;
    }

    public void RecoverUsedLayer(int layer)
    {
        Dictionary<int, CLayerData> tempDic = new Dictionary<int, CLayerData>();
        foreach (var kv in m_usedLayerToWinNumList)
        {
            CLayerData layerData = kv.Value;
            CLayerData tempLayerData = new CLayerData(layerData.m_type, layerData.m_winTrans, layerData.m_num);
            if (tempLayerData.m_type == 2 && (tempLayerData.m_winTrans == null || !NGUITools.GetActive(tempLayerData.m_winTrans.gameObject)))
            {
                continue;
            }

            if (tempLayerData.m_type == 3 && (tempLayerData.m_winTrans == null || !NGUITools.GetActive(tempLayerData.m_winTrans.gameObject)))
            {
                continue;
            }
            if (kv.Key == layer)
            {
                continue;
            }
            tempDic.Add(kv.Key, tempLayerData);
        }

        m_usedLayerToWinNumList.Clear();
        m_usedLayerToWinNumList = tempDic;
    }
    class CWindowCloseGroup
    {
        public bool m_openState = true;
        public List<CWindowCloseData> m_memberLs = new List<CWindowCloseData>();

        public Transform m_parentTrans;
        public CWindowCloseGroup(Transform parentTrans)
        {
            m_parentTrans = parentTrans;
            m_memberLs.Clear();
            AddMember(parentTrans);
        }

        public void AddMember(Transform trans)
        {
            WindowRoot windowRoot = trans.GetComponent<WindowRoot>();
            if (windowRoot != null)
            {
                CWindowCloseData item = m_memberLs.Find((CWindowCloseData findValue) =>
                {
                    return findValue.m_windowEnum == windowRoot.m_currentWindowEnum
                        && findValue.m_windowName == windowRoot.m_windowName;
                });
                if (item == null)
                {
                    m_memberLs.Add(new CWindowCloseData(windowRoot));
                    //					Debug.LogError("[WinClose Parent] " + m_parentTrans.name + "[Add member]" + trans.name);
                }
            }
        }

        public void Close()
        {
            m_openState = false;
            //			Debug.LogError("[WinGroup SysClose]" + m_parentTrans.name);
            for (int i = m_memberLs.Count - 1; i >= 0; i--)
            {
                //				Debug.LogError("[WinGroup SysClose]" + m_parentTrans.name + "[member]" + m_memberLs[i].m_windowEnum);
                m_memberLs[i].CloseWindow();
            }
        }

        public void Open()
        {
            m_openState = true;
            //			Debug.LogError("[WinGroup ReOpen]" + m_parentTrans.name);
            for (int i = 0; i < m_memberLs.Count; i++)
            {
                //				Debug.LogError("[WinGroup ReOpen]" + m_parentTrans.name + "[member]" + m_memberLs[i].m_windowEnum);
                m_memberLs[i].ReOpenWindow();
            }
        }
    }

    List<CWindowCloseGroup> m_winCloseGroupLs = new List<CWindowCloseGroup>();
    public void AddNewWinCloseGroup(bool fullCover, Transform trans)
    {
        if (trans == null || trans.name == EZFunWindowEnum.cover_ui_window.ToString())
        {
            return;
        }

        if (fullCover || m_winCloseGroupLs.Count == 0)
        {
            CWindowCloseGroup item = m_winCloseGroupLs.Find((CWindowCloseGroup findValue) =>
            {
                return findValue.m_parentTrans == trans;
            });
            if (item == null)
            {
                m_winCloseGroupLs.Add(new CWindowCloseGroup(trans));
                //				Debug.LogError("[Add WinClose Group]" + trans.name);
            }
        }
        else
        {
            m_winCloseGroupLs[m_winCloseGroupLs.Count - 1].AddMember(trans);
        }
    }

    public void RemoveWinCloseGroup(EZFunWindowEnum windowEnum, string luaWinName)
    {
        for (int groupIndex = m_winCloseGroupLs.Count - 1; groupIndex >= 0; groupIndex--)
        {
            CWindowCloseGroup item = m_winCloseGroupLs[groupIndex];
            for (int memberIndex = item.m_memberLs.Count - 1; memberIndex >= 0; memberIndex--)
            {
                CWindowCloseData member = item.m_memberLs[memberIndex];
                if (member.m_windowEnum == windowEnum && member.m_windowName == luaWinName)
                {
                    item.m_memberLs.RemoveAt(memberIndex);
                    //					Debug.LogError("[WinClose]" + item.m_parentTrans.name + "[Remove Member]" + member.m_windowEnum);
                    if (item.m_memberLs.Count == 0)
                    {
                        m_winCloseGroupLs.RemoveAt(groupIndex);
                        //						Debug.LogError("[Remove WinClose]" + item.m_parentTrans.name);
                    }
                    break;
                }
            }
        }
        RecoverySaveWindow();
    }

    private void HandleNotEnoughLayer(Transform trans)
    {
        for (int i = 0; i < m_winCloseGroupLs.Count; i++)
        {
            CWindowCloseGroup item = m_winCloseGroupLs[i];
            if (item.m_openState)
            {
                item.Close();
                break;
            }
        }
    }

    public void RecoverySaveWindow()
    {
        for (int i = m_winCloseGroupLs.Count - 1; i >= 0; i--)
        {
            CWindowCloseGroup item = m_winCloseGroupLs[i];
            if (item.m_openState)
            {
                break;
            }
            item.Open();
            return;
        }
    }

    class CWindowCloseData
    {
        public EZFunWindowEnum m_windowEnum;
        public string m_windowName;
        public int m_openState;

        public CWindowCloseData(WindowRoot windowRoot)
        {
            m_windowEnum = windowRoot.m_currentWindowEnum;
            m_openState = windowRoot.m_selfOpenState;
            m_windowName = windowRoot.m_windowName;
        }

        public void ReOpenWindow()
        {
            EZFunWindowMgr.Instance.m_isRecovering = true;
            EZFunWindowMgr.Instance.SetWindowStatus(m_windowEnum, true, m_openState, m_windowName);
            EZFunWindowMgr.Instance.m_isRecovering = false;
        }

        public void CloseWindow()
        {
            EZFunWindowMgr.Instance.m_isRecovering = true;
            EZFunWindowMgr.Instance.SetWindowStatus(m_windowEnum, false, WindowRoot.SYS_AUTO_CLOSE_STATE, m_windowName);
            EZFunWindowMgr.Instance.m_isRecovering = false;
        }
    }

    public void RevertUnusedLayer(int layer)
    {
        if (m_usedLayerToWinNumList.ContainsKey(layer))
        {
            m_usedLayerToWinNumList.Remove(layer);
        }
    }

    private int GetNotSpecialWindowDepth(bool sysWindow)
    {
        int depth;
        if (m_usedCameraList.Count == 0 && sysWindow)
        {
            depth = UICAMERA_SysWindow_START_DEPTH;
        }
        else if (m_usedCameraList.Count == 0 && !sysWindow)
        {
            depth = UICAMERA_PopWindow_START_DEPTH;
        }
        else
        {
            int index = m_usedCameraList.Count - 1;
            UICameraStruct lastCamera = m_usedCameraList[index];
            while (lastCamera.m_isSpecialWindow && index > 0)
            {
                --index;
                lastCamera = m_usedCameraList[index];
            }

            if (lastCamera.m_isSpecialWindow || lastCamera.m_systemWindow != sysWindow)
            {
                if (sysWindow)
                {
                    depth = UICAMERA_SysWindow_START_DEPTH;
                }
                else
                {
                    depth = UICAMERA_PopWindow_START_DEPTH;
                }
            }
            else
            {
                depth = lastCamera.m_depth + UICAMERA_DELTA_DEPTH;
            }
        }

        return depth;
    }

    public Camera GetMainWindowCamera()
    {
        Camera camera = null;
        if (m_usedCameraList.Count > 0)
        {
            camera = m_usedCameraList[0].m_camera;
        }
        return camera;
    }
    #endregion


    //LOG
    public void Log(string log)
    {
        if (m_logList.Count >= 30)
        {
            m_logList.RemoveAt(0);
        }

        m_logList.Add(log);
    }

    public void ClearLog()
    {
        m_logList.Clear();
    }

    List<string> m_logList = new List<string>();

#if UNITY_EDITOR
    void OnGUI()
    {
        GUI.color = Color.red;
        GUI.skin.label.fontSize = 20;
        GUI.skin.label.fixedWidth = 1000;
        for (int i = 0; i < m_logList.Count; i++)
        {
            GUILayout.Label(m_logList[i]);
        }
    }
#endif

    public WindowRoot GetWindowRoot(EZFunWindowEnum windowEnum, string luaWindowName = "")
    {
        CWindow cwidow = new CWindow(windowEnum, luaWindowName);
        return m_windowDic.ContainsKey(cwidow) ? m_windowDic[cwidow].m_windowRoot : null;
    }

    public WindowRoot GetCurWindowRoot(params EZFunWindowEnum[] windowEnums)
    {
        bool mainuiWin = false;
        bool mainMenuWin = false;
        return EZFunWindowMgr.Instance.GetCurWindowRoot(ref mainuiWin, ref mainMenuWin, 0, windowEnums);
    }

    private List<UICameraStruct> m_tempList = new List<UICameraStruct>();

    /// <summary>
    ///  不包括战力提升tips 和错误提示tips 和cover
    /// </summary>
    /// <param name="mainuiWindow"></param>
    /// <param name="mainMenuWindow"></param>
    /// <param name="maxIndex">当属第几</param>
    /// <returns></returns>
    public WindowRoot GetCurWindowRoot(ref bool mainuiWindow, ref bool mainMenuWindow, int maxIndex = 0, params EZFunWindowEnum[] windowEnums)
    {
        mainuiWindow = false;
        mainMenuWindow = false;
        m_tempList.Clear();
        m_tempList.AddRange(m_usedCameraList);
        var camList = m_tempList;
        camList.Sort((UICameraStruct cam1, UICameraStruct cam2) =>
             {
                 if (cam1.m_camera.depth > cam2.m_camera.depth)
                 {
                     return -1;
                 }
                 else if (cam1.m_camera.depth < cam2.m_camera.depth)
                 {
                     return 1;
                 }
                 else
                 {
                     return 0;
                 }
             });
        int cur = 0;
        for (int i = 0; i < camList.Count; ++i)
        {
            var cam = camList[i];
            if (cam.IsNull())
            {
                break;
            }
            WindowRoot wr = EZFunWindowMgr.Instance.GetWindowRoot(cam.m_curWindow, cam.m_luaWindowName);
            if (wr.IsNull())
            {
                continue;
            }
            for (int j = 0; j < windowEnums.Length; ++j)
            {
                if (wr.m_currentWindowEnum == windowEnums[j])
                {
                    return wr;
                }
            }
            if (wr.m_currentWindowEnum == EZFunWindowEnum.err_tips_ui_window ||
                wr.m_currentWindowEnum == EZFunWindowEnum.cover_ui_window
                )
            {
                continue;
            }
            if (!wr.m_open)
            {
                continue;
            }

            if (cur == maxIndex)
            {
                return wr;
            }
            else
            {
                cur++;
            }
        }

        return null;
    }

    static public float GetMaxCameraDepth(Camera cam = null)
    {
        var cams = Resources.FindObjectsOfTypeAll<Camera>();
        float depth = 0F;
        for (int i = 0; i < cams.Length; ++i)
        {
            if (cams[i] != cam)
            {
                depth = depth > cams[i].depth ? depth : cams[i].depth;
            }
        }
        return depth;
    }

    public bool m_returnToMainCity = false;             // 某些窗口需要知道当前是被这个方法关闭的
    public void ReturnToMainCity()
    {
        CloseAllWindowExceptWindow("");
    }


    public void CloseAllWindowExceptWindow(string windowName = "")
    {
        m_returnToMainCity = true;
        var needCloseCams = new List<UICameraStruct>();
        for (int i = 0; i < m_usedCameraList.Count; ++i)
        {
            var cam = m_usedCameraList[i];
            if (cam.IsNull())
            {
                break;
            }
            var wr = GetWindowRoot(cam.m_curWindow, cam.m_luaWindowName);
            if (wr.IsNull())
            {
                continue;
            }
            if (
                (wr.m_currentWindowEnum == EZFunWindowEnum.luaWindow && wr.m_windowName == "main_ui_window") 
                || windowName == wr.m_windowName)
            {
                continue;
            }
            needCloseCams.Add(cam);
        }
        for (int i = 0; i < needCloseCams.Count; ++i)
        {
            var cam = needCloseCams[i];
            if (cam.IsNull())
            {
                break;
            }
            var wr = GetWindowRoot(cam.m_curWindow, cam.m_luaWindowName);
            if (wr.IsNull())
            {
                continue;
            }
            SetWindowStatus(wr.m_currentWindowEnum, false, 0, wr.m_windowName);
        }
        m_returnToMainCity = false;
    }

    #region opened window list
    public bool m_isRecovering = false;
    class COpenedWindow
    {
        public int m_state = 0;
        public string m_windowName = "";
        public EZFunWindowEnum m_windowEnum;
        public COpenedWindow(EZFunWindowEnum windowEnum, int state, string luaWindowName = "")
        {
            m_state = state;
            m_windowName = luaWindowName;
        }
    }

    List<COpenedWindow> m_openedWinLs = new List<COpenedWindow>();
    public void InsertOpenWindow(EZFunWindowEnum windowEnum, int state, string windowName = "")
    {
        if (isCanAction(windowName))
        {
            return;
        }
        COpenedWindow findValue = m_openedWinLs.Find((COpenedWindow item) =>
        {
            return item.m_windowName == windowName;
        });
        if (findValue != null)
        {
            m_openedWinLs.Remove(findValue);
        }
        // 		if(windowEnum == EZFunWindowEnum.fuben_ui_window)
        // 		{
        // 			state = 0;
        // 		}
        m_openedWinLs.Add(new COpenedWindow(windowEnum, state, windowName));
    }

    public bool CheckHasFullWindowOpened()
    {
        for (int i = 0; i < m_openedWinLs.Count; ++i)
        {
            if (m_openedWinLs[i].m_windowEnum < EZFunWindowEnum.FULL_WINDOW_SPLIT)
            {
                return true;
            }
        }
        return false;
    }

    bool isCanAction(string luaWindowName = "")
    {
        if (GameRoot.Instance == null)
        {
            return false;
        }

        if (GameStateMgr.Instance.GetCurStateType() != EGameStateType.MainCityState)
        {
            return true;
        }
        return CheckLuaWindowAction(luaWindowName);

    }

    private bool CheckLuaWindowAction(string luaWindowName = "")
    {
        return WindowBaseLua.GetValueFrom<bool>("ezfunLuaTool", "CheckLuaWindowAction", false, luaWindowName);
    }


    public void RemoveOpenWindow(EZFunWindowEnum windowEnum, string windowName = "")
    {
        if (isCanAction(windowName))
        {
            return;
        }

        if (m_isClearingWin)
        {
            return;
        }

        COpenedWindow findValue = m_openedWinLs.Find((COpenedWindow item) =>
        {
            return item.m_windowName == windowName;
        });
        if (findValue != null)
        {
            m_openedWinLs.Remove(findValue);
        }
    }

    public void ClearOpenedWindowList()
    {
        m_openedWinLs.Clear();
    }

    //public void ClearOpenedWindow(EZFunWindowEnum windowEnum)
    //{
    //    for (int i = 0; i < m_openedWinLs.Count; i++)
    //    {
    //        if (m_openedWinLs[i].m_windowEnum == windowEnum)
    //        {
    //            m_openedWinLs.RemoveAt(i);
    //        }
    //    }
    //}
    public void RecoveryWindow()
    {
        m_isRecovering = true;
        List<COpenedWindow> tempLs = new List<COpenedWindow>();
        for (int i = 0; i < m_openedWinLs.Count; i++)
        {
            tempLs.Add(m_openedWinLs[i]);
        }
        for (int i = 0; i < tempLs.Count; i++)
        {
            COpenedWindow openWindow = tempLs[i];

            if (CheckHasOpenWinFunction(openWindow.m_windowEnum, openWindow.m_windowName))
            {
            }
            else
            {
                SetWindowStatus(openWindow.m_windowEnum, true, openWindow.m_state, openWindow.m_windowName);
            }
        }

        m_isRecovering = false;
    }

    public void AddOpenWinFunction(EZFunWindowEnum winEnum, string luaWindow, System.Action ac)
    {
        string winName = winEnum.ToString();
        if (winEnum == EZFunWindowEnum.luaWindow)
        {
            winName = luaWindow;
        }
        m_openWinFunctionDic[winName] = ac;
    }

    private bool CheckHasOpenWinFunction(EZFunWindowEnum winEnum, string luaWindow)
    {
        string winName = luaWindow;
        if (!string.IsNullOrEmpty(luaWindow))
        {
            winName = winEnum.ToString();
        }
        if (m_openWinFunctionDic.ContainsKey(winName) && m_openWinFunctionDic[winName] != null)
        {
            m_openWinFunctionDic[winName]();
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion


    /// <summary>
    /// 主要给lua的一些接口用，因为新增lua文件的话，不能再EZFunWindowEnum中新加行，所以必须支持只根据名字去加载lua相关
    /// 而活动相关界面又支持同一个界面有两个出来，所以扩展了这儿
    /// </summary>
    /// <param name="windowName"></param>
    /// <param name="windowType"></param>
    /// <param name="resPath"></param>
    /// <param name="luaFile"></param>
    /// <param name="isNeedCreate"></param>
    public void RegisterWindowInfo(string windowName, Type windowType, string resPath, string luaFile, bool isNeedCreate = true, bool isClosePrimary = true)
    {
        //   if (!m_registerWindowInfoDic.ContainsKey(windowName))
        {
            WindowTypeAttribute attr = null;
            if (windowType != null)
            {
                attr = new WindowTypeAttribute(windowType, resPath);
            }
            else
            {
                attr = new WindowTypeAttribute(luaFile, resPath);
                attr.m_needCreate = isNeedCreate;
            }
            attr.m_needClosePrimary = isClosePrimary;
            m_registerWindowInfoDic[windowName] = attr;
        }
    }

    private Dictionary<int, string> m_enumsNameDic = new Dictionary<int, string>();

    private string GetNamesByStr(EZFunWindowEnum windowEnum)
    {
        if (!m_enumsNameDic.ContainsKey((int)windowEnum))
        {
            m_enumsNameDic[(int)windowEnum] = windowEnum.ToString();
        }
        return m_enumsNameDic[(int)windowEnum];
    }

    public static void SetUIAllowMultiTouch(bool allow)
    {
        var c = UICamera.eventHandler;

        if (c != null)
        {
            //			Debug.Log ("Set AllowMultiTouch:" + allow);
            c.allowMultiTouch = allow;
        }
        else
        {
            //			Debug.LogError ("UICamera.EventHandler is null");
        }
    }
}
