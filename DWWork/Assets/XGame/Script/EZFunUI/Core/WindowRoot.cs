using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ezfun_resource;

[LuaWrap]
public enum AdaptType
{
    none = 1,
    normal = 2,//其他分辨率屏幕保持与（1136*640）屏幕边界的距离相同
    rigid = 3,//不可拉伸，如left的适配类型是这个，宽度就固定，right的适配类型就必须是none
}
[LuaWrap]
public class WindowRoot : BaseUI, IWindowCloseBehaviour, ISceneChangeBehaviour
{
    static public string m_needClickBtnName = null;
    static public EZFunWindowEnum m_needClickWindow = EZFunWindowEnum.None;

    private Dictionary<int, ModuleInfo> m_modules = new Dictionary<int, ModuleInfo>();

    private List<Action> m_updateList = new List<Action>();
    private List<Action> m_updatePerSecondList = new List<Action>();
    protected Action m_OnPlayOpenAnimEnd = null;

    public bool m_IsTipWindow = false;  //是否为tip类型的window，如采集按钮、经验飘字等

    public static int s_argc1;

    protected bool m_canSeeWindow = false;

    protected bool m_isUpdateDelegate = true;

    protected int m_layer = -1;

    private List<EZFunScollView> m_scrollViewList = new List<EZFunScollView>();
    private List<EZfunLimitScrollView> m_limitScrollViewList = new List<EZfunLimitScrollView>();
    public int m_selfOpenState = 0;

    private bool m_isClosing = false;
    protected bool m_playOpenAnimEnd = false;

    public const int SYS_AUTO_CLOSE_STATE = -1001;
    public int m_needOpenMainWindow = -1;

    private int _m_scrollViewCnt = 1;
    public int m_scrollViewCnt
    {
        get
        {
            return _m_scrollViewCnt;
        }
        set
        {
            if (_m_scrollViewCnt > 50)
            {
                _m_scrollViewCnt = 1;
            }
        }
    }

    private bool m_playAnimOnOpen = true;

    public bool playAnimOnOpen
    {
        get
        {
            return m_playAnimOnOpen;
        }
    }

    private bool m_playAnimOnClose = true;

    public bool playAnimOnClose
    {
        get
        {
            return m_playAnimOnClose;
        }
    }
    private bool m_isNeedResetAnimated = false;
    /// <summary>
    /// 同一个名字 InitWindow(true,0,true) 编译器识别不了用这个还是下面一个
    /// </summary>
    /// <param name="open"></param>
    /// <param name="state"></param>
    /// <param name="animated"></param>
    /// <param name="parameters"></param>
    virtual public void InitWindow(bool open, int state, bool animated, object parameters = null)
    {
        m_isNeedResetAnimated = true;
        if (open)
        {
            m_playAnimOnOpen = animated;
        }
        else
        {
            m_playAnimOnClose = animated;
        }
        InitWindow(open, state, parameters);
    }

    protected T TryGetParameter<T>(object[] parameters, int index)
    {
        if (parameters.Length > index)
        {
            return (T)parameters[index];
        }
        else
        {
            return default(T);
        }
    }

    virtual public void OnReceiveMemoryWarning()
    {
        //sub class do some clean work
    }

	virtual public void InitWindow(bool open, int state, object param)
	{
		InitWindow(open, state);
	}

    virtual public void InitWindow(bool open = true, int state = 0)
    {
        if (open)
        {
            OnWindowWillOpen();
        }
        else
        {
            OnWindowWillClose();
        }

        m_open = open;
        if (!m_isNeedResetAnimated)
        {
            if (open)
            {
                m_playAnimOnOpen = true;
            }
            else
            {
                m_playAnimOnClose = true;
            }
        }
        else
        {
            m_isNeedResetAnimated = false;
        }
        if (open)
        {
            if (m_isClosing && m_currentWindowEnum == EZFunWindowEnum.error_ui_window)
            {
                SetActive(m_currentTrans, false);
            }
            m_isClosing = false;
            m_playOpenAnimEnd = false;
            m_selfOpenState = state;
            bool active = m_isWindowActive;

            if (!m_hasCreated)
            {
                m_windowRoot = this;
                BasePreCreateWindow();
                CreateUI();
                HandleLoadOrCreateModule();
                CreateWindow();
            }

            if (m_currentTrans != null)
                WindowRoot.SetLayer(m_currentTrans.gameObject, m_layer);

            SetWindowActive(true);

            if (m_hasWinAnim && !active)
            {
                PlayWinAnim(true, m_playAnimOnOpen);
            }
            else
            {
                m_playOpenAnimEnd = true;
            }

            if (GameStateMgr.isLoaded)
            {
                EventSys.Instance.AddEventNow(EEventType.OpenNewWin, m_currentWindowEnum, m_windowName);
            }
            EZFunWindowMgr.Instance.InsertOpenWindow(m_currentWindowEnum, state, m_windowName);
        }
        else if (!open && m_isWindowActive)
        {
            if (state == SYS_AUTO_CLOSE_STATE)
            {
                CloseWindow(SYS_AUTO_CLOSE_STATE);
            }
            else if (state == CLOSE_STATE)
            {
                CloseWindow(state);
            }
            else
            {
                CloseWindow(state);
            }
        }

        InitModules(open, state);
        if (!open)
        {
            m_updateList.Clear();
        }

      
        //if (m_currentWindowEnum == EZFunWindowEnum.luaWindow)
        //{
            //if (EventSys.Instance != null)
            //{
                 //EventSys.Instance.AddEvent(EEventType.OpenOrCloseWindow, open,m_windowName);
            //}
        //}
    }

    private Transform m_blkTrans;

    #region handle module 

    protected void BasePreCreateWindow()
    {
        PreCreateWindow();
    }

    protected virtual void PreCreateWindow()
    {

    }

    /// <summary>
    /// 注册模块
    /// </summary>
    /// <param name="moduleType">枚举类型</param>
    /// <param name="nodeName">模块的父节点</param>
    /// <param name="luaFileName">模块的父节点</param>
    /// <param name="isNeedLoadRes">模块是否需要加载资源，false表示不需要，直接用nodeName,true需要加载moduleType.toString()节点，并且其设置父节点为nodeName</param>
    public void RegisterModule(ModuleType moduleType, string nodeName, string luaFileName = "", bool isNeedLoadRes = false)
    {
        ModuleInfo moduleInfo = null;
        var moduleField = moduleType.GetAttributeOfType<ModuleFieldAttribute>();
        if (moduleField == null)
        {
            return;
        }
        //如果是lua模块
        if(moduleType == ModuleType.LuaModuleType && luaFileName.Length > 0)
        {
            moduleField.m_luaFile = luaFileName;
        }

        if (moduleField.m_moduleType == null && string.IsNullOrEmpty(moduleField.m_luaFile))
            return;

        if (!m_modules.ContainsKey((int)moduleType))
        {
            moduleInfo = new ModuleInfo();
            m_modules.Add((int)moduleType, moduleInfo);
        }
        moduleInfo = m_modules[(int)moduleType];
        moduleInfo.m_moduleType = moduleType;
        moduleInfo.m_nodeName = nodeName;
        moduleInfo.m_isNeedLoadRes = isNeedLoadRes;
        moduleInfo.m_moduleField = moduleType.GetAttributeOfType<ModuleFieldAttribute>();
    }


    private void HandleLoadOrCreateModule()
    {
        var enumerator = m_modules.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var moduleInfo = enumerator.Current.Value;
            Transform moduleTrans = null;
            if (string.IsNullOrEmpty(moduleInfo.m_nodeName))
            {
                moduleTrans = m_currentTrans;
            }
            else
            {
                moduleTrans = GetTrans(moduleInfo.m_nodeName);
            }
            if (moduleInfo.m_isNeedLoadRes)
            {
                var parentTrans = moduleTrans;
                GameObject gb = ResourceManager.Instance.LoadResource(ResourcePathConst.UI_ITEMS_PATH + moduleInfo.m_moduleType.ToString() + ".prefab", typeof(GameObject)) as GameObject;
                if (gb == null)
                {
                    continue;
                }
                gb = GameObject.Instantiate(gb) as GameObject;
                gb.transform.parent = parentTrans;
                moduleTrans = gb.transform;
                moduleTrans.localPosition = Vector3.zero;
                moduleTrans.localScale = Vector3.one;
            }
            if (moduleTrans == null)
            {
                continue;
            }
            if (moduleInfo.m_moduleField != null)
            {
                ModuleRoot moduleRoot = null;
                if (moduleInfo.m_moduleField.m_moduleType == null && !string.IsNullOrEmpty(moduleInfo.m_moduleField.m_luaFile))
                {
                    moduleRoot = EZFunTools.GetOrAddComponent<LuaModule>(moduleTrans.gameObject);
                }
                else if (moduleInfo.m_moduleField.m_moduleType != null)
                {
                    moduleRoot = EZFunTools.GetOrAddComponent(moduleTrans.gameObject, moduleInfo.m_moduleField.m_moduleType) as ModuleRoot;
                }
                if (moduleRoot != null)
                {
                    moduleRoot.BaseCreateModule(this, moduleInfo);
                }
                moduleRoot.m_windowName = this.m_windowName;
                moduleRoot.m_currentWindowEnum = this.m_currentWindowEnum;
                moduleInfo.m_moduleRoot = moduleRoot;
            }
        }
    }

    private void InitModules(bool isOpen, int state)
    {
        var enumerator = m_modules.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var moduleInfo = enumerator.Current.Value;
            if (moduleInfo.m_moduleRoot != null)
            {
                moduleInfo.m_moduleRoot.BaseInitModule(isOpen, state);
            }
        }
    }
    public ModuleRoot GetModuleByType(ModuleType moduleType)
    {
        if (m_modules.ContainsKey((int)moduleType))
        {
            return m_modules[(int)moduleType].m_moduleRoot;
        }
        return null;
    }

    public void RegisterUpdate(Action action)
    {
        if (m_updateList.Contains(action))
        {
            return;
        }
        m_updateList.Add(action);
    }

    public void RegisterPerSecondUpdate(Action action)
    {
        if (m_updatePerSecondList.Contains(action))
        {
            return;
        }
        m_updatePerSecondList.Add(action);
    }

    public void UnRegisterUpdate(Action action)
    {
        m_updateList.Remove(action);
    }

    public void UnRegisterPerSecondUpdate(Action action)
    {
        m_updatePerSecondList.Remove(action);
    }

    #endregion

    protected virtual void PlayWinAnim(bool open, bool isPlay = true)
    {
        GameObject gb = null;
        var trans = GetTransByDepth("animation_ui_root", 2);
        if (trans != null)
        {
            gb = trans.gameObject;
        }
        GameObject gb1 = null;
        if (gb == null)
        {
            var trasn1 = GetTransByDepth("animation1_ui_root", 2);
            if (trasn1 != null)
            {
                gb1 = trasn1.gameObject;
            }
        }
        if (gb == null && gb1 == null)
        {
            if (open)
            {
                PlayOpenAnimEnd();
            }
            else
            {
                PlayCloseAnimEnd();
            }
            return;
        }
        m_animationState = open ? 1 : -1;
        if (gb != null)
        {
            EZFunTools.GetOrAddComponent<UIWidget>(gb);
            TweenPosition tweenScale = EZFunTools.GetOrAddComponent<TweenPosition>(gb);
            TweenAlpha tweenAlpha = EZFunTools.GetOrAddComponent<TweenAlpha>(gb);
            GameObject winAnim = null;
            if (open)
            {
                winAnim = (GameObject)ResourceMgr.InitAsset(RessType.RT_CommonUItem, "win_kai", RessStorgeType.RST_Always);
            }
            else
            {
                winAnim = (GameObject)ResourceMgr.InitAsset(RessType.RT_CommonUItem, "win_guan", RessStorgeType.RST_Always);
            }

            CloneTweenScale(ref tweenScale, winAnim.GetComponent<TweenPosition>());
            CloneTweenAplha(ref tweenAlpha, winAnim.GetComponent<TweenAlpha>());
            //if (open)
            //{
            //    tweenScale.onFinished.Add(new EventDelegate(this, "PlayOpenAnimEnd"));
            //}
            //else
            //{
            //    tweenScale.onFinished.Add(new EventDelegate(this, "PlayCloseAnimEnd"));
            //}
            //tweenScale.onFinished.Clear();
            //tweenScale.ResetToBeginning();
            //tweenScale.PlayForward();
            //tweenAlpha.ResetToBeginning();
            //tweenAlpha.PlayForward();
            if (open)
            {
                this.PlayTweenCallBack(gb.transform, PlayOpenAnimEnd, true, isPlay);
            }
            else
            {
                this.PlayTweenCallBack(gb.transform, PlayCloseAnimEnd, true, isPlay);
            }

        }
        else if (gb1 != null)
        {
            if (open)
            {
                this.PlayTweenCallBack(gb1.transform, PlayOpenAnimEnd, open, isPlay);
            }
            else
            {
                this.PlayTweenCallBack(gb1.transform, PlayCloseAnimEnd, open, isPlay);
            }
        }
    }

    private Vector3 FADE_CLOSE_POS = new Vector3(9000f, 9000f, 0f);

    private void CloseWindow(int state)
    {
        if (m_hasWinAnim && state != SYS_AUTO_CLOSE_STATE)
        {
            PlayWinAnim(false, m_playAnimOnClose);
            //            var blur = this.GetComponentInChildren<BlurBackgroundRender>();
            //            if (blur != null)
            //            {
            //                blur.OnEndRender();
            //            }
            m_isClosing = true;
        }
        else
        {
            SetWindowActive(false);

            if (state != SYS_AUTO_CLOSE_STATE)
            {
                if (EventSys.Instance != null)
                {
                    EventSys.Instance.AddEventNow(EEventType.CloseWin, m_currentWindowEnum, m_windowName);
                }

                EZFunWindowMgr.Instance.RemoveWinCloseGroup(m_currentWindowEnum, m_windowName);
            }
        }

        if (state != SYS_AUTO_CLOSE_STATE)
        {
            PopupItem item = new PopupItem();
            item.m_WindowEnum = m_currentWindowEnum;
            PopupWindowManager.getinstance().PopWindow(item);
            EZFunWindowMgr.Instance.RemoveOpenWindow(m_currentWindowEnum, m_windowName);
            if (EventSys.Instance != null)
            {
                EventSys.Instance.AddEvent(EEventType.UI_Msg_CloseWindow, m_currentWindowEnum);
            }
        }
        //var typeField = EZFunWindowMgr.Instance.GetWindowAttribute(m_currentWindowEnum, this.m_windowName);
        //if ((m_currentWindowEnum < EZFunWindowEnum.FULL_WINDOW_SPLIT &&
        //        m_currentWindowEnum != MapMgr.Instance.GetCurrentMapUIType()) || (typeField != null && typeField.m_needClosePrimary))
        //{
        //    if (m_needOpenMainWindow != -1 && !EZFunWindowMgr.Instance.CheckHasFullWindowOpened() && (state != SYS_AUTO_CLOSE_STATE))
        //    {
        //        EZFunWindowMgr.Instance.SetWindowStatus(MapMgr.Instance.GetCurrentMapUIType(), true, m_needOpenMainWindow);
        //        m_needOpenMainWindow = -1;
        //    }
        //}
    }

    public void TriggerNewPlayerGuideUIWindow()
    {
        m_open = true;
    }

    float m_lastUpdateTime = 0f;
    protected virtual void Update()
    {
        m_lastUpdateTime += Time.deltaTime;
        if (m_lastUpdateTime >= 1f)
        {
            m_lastUpdateTime = 0;
            UpdatePerSecond();
            for (int i = 0; i < m_updatePerSecondList.Count; i++)
            {
                m_updatePerSecondList[i]();
            }
        }
        if (m_isUpdateDelegate)
        {
            for (int i = 0; i < m_updateList.Count; i++)
            {
                m_updateList[i]();
            }
        }
    }

    virtual protected void LateUpdate()
    { }

    //直接override这个UpdatePerSecond可用
    //如果继承的窗口有Update的话，就需要在继承窗口的Update里加上base.Update
    virtual protected void UpdatePerSecond()
    { }

    protected virtual void PlayCloseAnimEnd()
    {
        //EZFunWindowMgr.Instance.RemoveCamera(m_layer);
        if (EventSys.Instance != null)
        {
            EventSys.Instance.AddEventNow(EEventType.CloseWin, m_currentWindowEnum, m_windowName);
        }
        m_animationState = 0;
        EZFunWindowMgr.Instance.RemoveWinCloseGroup(m_currentWindowEnum, m_windowName);

        SetWindowActive(false);
        CloseWindowDelayProcess();
    }

    virtual public void CloseWindowDelayProcess()
    {

    }

    protected virtual void PlayOpenAnimEnd()
    {
        //if (GetTrans("animation_ui_root") != null)
        //{
        //    SetActive(GetTrans("animation_ui_root"), false);
        //    SetActive(GetTrans("animation_ui_root"), true);
        //}
        //if (GetTrans("animation_ui_root1") != null)
        //{
        //    SetActive(GetTrans("animation1_ui_root"), false);
        //    SetActive(GetTrans("animation1_ui_root"), true);
        //}
        for (int scollViewIndex = 0; scollViewIndex < m_scrollViewList.Count; scollViewIndex++)
        {
            m_scrollViewList[scollViewIndex].ResetCameraRect();
        }

        for (int scollViewIndex = 0; scollViewIndex < m_limitScrollViewList.Count; scollViewIndex++)
        {
            m_limitScrollViewList[scollViewIndex].ResetCameraRect();
        }

        if (!m_OnPlayOpenAnimEnd.IsNull())
        {
            m_OnPlayOpenAnimEnd();
        }

        m_playOpenAnimEnd = true;
    }

    public virtual void InitDic()
    {
    }

    public int GetLayer()
    {
        return m_layer;
    }

    public virtual void OnWillDestroy()
    {
        ClearEventHandler();
    }

    public override void OnDestroy()
    {
        Debug.Log("Destroy Window:" + this.m_currentWindowEnum);

    }

    virtual public void ClearEventHandler()
    {
        var enumerator = m_modules.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Value.m_moduleRoot != null)
            {
                enumerator.Current.Value.m_moduleRoot.Destroy();
            }
        }
        if (CNetSys.Instance != null)
        {
            CNetSys.Instance.RemoveAllHandler(this);
        }
        if (EventSys.Instance != null)
        {
            EventSys.Instance.RemoveHander(this);
        }
    }

    virtual public void ClearTempData()
    {
    }

    //When the window is first opened call this function
    protected bool m_hasCreated = false;

    protected bool has_scale = true;
    public bool m_hasWinAnim = false;
    public int m_animationState = 0;
    virtual protected void CreateWindow()
    {
        m_hasCreated = true;
        if (EventSys.Instance != null)
        {
            EventSys.Instance.AddHandler(EEventType.UI_Msg_ClearCamera, (EEventType eventId, object p1, object p2) => { m_otherCamera.Clear(); });
            EventSys.Instance.AddHandler(EEventType.UI_Msg_CloseWindow, (EEventType eventId, object p1, object p2) => { RegisterClosWindow((EZFunWindowEnum)p1); });
        }

        if ((GetTransByDepth("animation_ui_root", 2) != null || GetTransByDepth("animation1_ui_root", 2) != null) && has_scale)
        {
            m_hasWinAnim = true;
        }
        else
        {
            m_hasWinAnim = false;
        }

        var anchors = GetComponentsInChildren<UIAnchor>();
        for (int i = 0; i < anchors.Length; i++)
        {
            anchors[i].uiCamera = m_cameraStruct.m_camera;
        }
    }

    private void RegisterClosWindow(EZFunWindowEnum windowEnum)
    {
        if (m_open)
        {
            if (windowEnum != EZFunWindowEnum.err_tips_ui_window && windowEnum != EZFunWindowEnum.cover_ui_window)
            {
                BaseOnFocus();
            }
        }
    }

    public void AddScrollView(EZFunScollView scrollView)
    {
        if (!m_scrollViewList.Contains(scrollView))
        {
            m_scrollViewList.Add(scrollView);
        }
    }

    public void AddLimitScrollView(EZfunLimitScrollView limitScr)
    {
        if (!m_limitScrollViewList.Contains(limitScr))
        {
            m_limitScrollViewList.Add(limitScr);
        }
    }

    public EZFunWindowMgr.UICameraStruct m_cameraStruct;

    /// <summary>
    /// 由于有了FadeClose，CloseWindow时不能真正SetActive(false)，所以用m_isWindowActive来记录
    /// </summary>
    private bool m_isWindowActive;

    public void SetWindowActive(bool active)
    {
        var closeBehaviour = GetCloseBehaviour();
        if (active)
        {
            if (m_currentTrans != null)
            {
                EZFunTools.SetActive(m_currentTrans.gameObject, true);

                if (closeBehaviour == WindowCloseBehaviour.FadeClose)
                {
                    m_currentTrans.localPosition = Vector3.zero;
                }
            }
        }
        else
        {
            EZFunWindowMgr.Instance.RemoveCamera(m_layer);

            if (m_currentTrans != null)
            {
                if (closeBehaviour == WindowCloseBehaviour.FadeClose)
                {
                    m_currentTrans.localPosition = FADE_CLOSE_POS;
                }
                else
                {
                    EZFunTools.SetActive(m_currentTrans.gameObject, false);
                }
                if (m_isWindowActive)
                {
                    EZFunWindowMgr.Instance.RemoveCamera(m_layer);

                    EZFunWindowMgr.Instance.RemoveWinCloseGroup(m_currentWindowEnum, m_windowName);
                }
            }
            OpenOtherCamera();
        }

        m_isWindowActive = active;

        if (active)
        {
            this.OnWindowDidOpen();
        }
        else
        {
            this.OnWindowDidClose();

            EZFunWindowMgr.Instance.DoAfterCloseWindow(new CWindow(m_currentWindowEnum, m_windowName), this);
        }
    }

    public void InitCamera(bool open, bool hasScrollView, bool fullCoverage, bool sysWindow = false, int specialDepth = -1)
    {
        if (!open || gameObject == null || m_isWindowActive)
        {
            return;
        }
        //记录获取摄像机的时候的最前一层的window
        m_cameraStruct = EZFunWindowMgr.Instance.GetUICameraLayer(hasScrollView, specialDepth, sysWindow, m_currentTrans, m_IsTipWindow);
        m_cameraStruct.m_curWindow = m_currentWindowEnum;
        m_cameraStruct.m_luaWindowName = m_windowName;
        m_layer = m_cameraStruct.m_layer;
        int panelDapth = 2 * (m_cameraStruct.m_windowNumber - 1);
        transform.GetComponent<UIPanel>().depth = panelDapth;

        if (fullCoverage)
        {
            SaveCamera();
            CloseOtherCamera();
        }

        if (specialDepth == -1)
        {
            EZFunWindowMgr.Instance.AddNewWinCloseGroup(fullCoverage, m_currentTrans);
        }
        m_cameraStruct.m_camera.nearClipPlane = -5;
    }

    protected virtual void OnWindowWillOpen()
    {

    }

    protected virtual void OnWindowWillClose()
    {
        if (!m_IsTipWindow)
        {
            SetMultiTouch(false);
        }
    }

    private void SetMultiTouch(bool enable)
    {
        EZFunWindowMgr.SetUIAllowMultiTouch(enable);
    }

    protected virtual void OnWindowDidOpen()
    {
        Debug.Log("OpenWindow:" + m_currentWindowEnum.ToString() + " " + m_windowName);

        ////加模糊效果
        //m_blkTrans = GetTransByDepth("bg_blk_blur", 2);


        //if (m_blkTrans != null)
        //{
        //    Debug.Log("Show Blur, frameCount:" + Time.frameCount);
        //    EZFunTools.GetOrAddComponent<BlurBackgroundRender>(m_blkTrans.gameObject).m_windowRoot = this;
        //    m_blkTrans.gameObject.SetActive(true);
        //}

    }

    protected virtual void OnWindowDidClose()
    {
        Debug.Log("CloseWindow:" + m_currentWindowEnum.ToString() + " " + m_windowName);

        if (m_blkTrans != null)
        {
            Debug.Log("Close Blur, frameCount:" + Time.frameCount);
            m_blkTrans.gameObject.SetActive(false);
        }
    }

    public void InitCamera(bool open, bool hasScrollView = false, int specialDepth = -1, bool sysWindow = false)
    {
        InitCamera(open, hasScrollView, false, sysWindow, specialDepth);
    }

    public EZFunWindowMgr.UICameraStruct GetCameraStruct()
    {
        return m_cameraStruct;
    }

    /// <summary>
    /// 获取焦点的时候，不可继承
    /// </summary>
    private void BaseOnFocus()
    {
        OnFucus();
    }

    /// <summary>
    /// 当窗口获取到焦点时，给子类覆盖
    /// </summary>
    virtual protected void OnFucus()
    {

    }

    //增加这个字段是为了，点击按钮的时候不一定是ES.BtnClick的这个声音
    static float m_lastClickTime = float.MinValue;
    const float BTN_CLICK_DELTA_TIME = 0.2f;
    public bool ignoreDeltaTime = false;    //允许连续点击 目前用于背包双击判定
    //any widget under this window is clicked will call this function
    override protected void HandleWidgetClick(GameObject gb)
    {
        //Debug.Log ("[Click Widget]" + gb.name);
        base.HandleWidgetClick(gb);
    }

    virtual public bool CheckBtnCanClickForLua(GameObject go, bool resetTime)
    {
        return CheckBtnCanClickSrc(go, resetTime);
    }

    //监测点击间隔 单独拿出来 加在需要的地方
    public bool CheckBtnClickDiff()
    {
        if ((Time.realtimeSinceStartup - m_lastClickTime) < BTN_CLICK_DELTA_TIME && !ignoreDeltaTime)
        {
            return false;
        }

        m_lastClickTime = Time.realtimeSinceStartup;

        return true;
    }

    public bool CheckBtnCanClickSrc(GameObject go, bool resetTime = false)
    {
        if (m_isClosing || m_canSeeWindow)
        {
            return false;
        }
        else if (GameRoot.Instance != null && GameStateMgr.Instance != null  &&
            (Time.realtimeSinceStartup - m_lastClickTime) < BTN_CLICK_DELTA_TIME && !ignoreDeltaTime)
        {
            return false;
        }

        if (resetTime)
        {
            m_lastClickTime = Time.realtimeSinceStartup;
        }

        // 当前窗口是erroruiwindow，需要点击的窗口不是，则可以点击erroruiwindow的btn
        if (m_currentWindowEnum == EZFunWindowEnum.error_ui_window)
        {
            if (m_needClickWindow != EZFunWindowEnum.error_ui_window)
            {
                return true;
            }
        }

        if (m_currentWindowEnum == EZFunWindowEnum.cover_ui_window)
        {
            return true;
        }

        if (go.name.Equals("skipbtn"))
        {
            return true;
        }

        if (m_needClickWindow != EZFunWindowEnum.None &&
            m_needClickWindow != m_currentWindowEnum)
        {
            return false;
        }
        if (m_needClickBtnName != null)
        {
            var name = go.name;

            if (m_needClickBtnName.Equals("") ||
                m_needClickBtnName.Equals("0"))
            {
                return true;
            }
            else
            {
                var btnNames = m_needClickBtnName.Split('/');
                var btnName = btnNames[btnNames.Length - 1];
                if (!name.Equals(btnName))
                {
                    return false;
                }
                int parentsCount = btnNames.Length - 1;
                var goTrans = go.transform;
                Transform parent = null;
                for (int i = 0; i < parentsCount; ++i)
                {
                    if (i == 0)
                    {
                        parent = goTrans.parent;
                    }
                    else
                    {
                        parent = parent.parent;
                    }
                    if (parent.IsNull() ||
                        !parent.name.Equals(btnNames[parentsCount - 1 - i]))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    //监测是否有锁定点击按钮
    public static bool CheckNeedClickBtn()
    {
        if (!(m_needClickBtnName == null ||
           m_needClickBtnName.Equals("") ||
               m_needClickBtnName.Equals("0")))
        {
            return true;
        }

        return false;
    }

    public void SetLabelSelfActive(Transform trans, string txt)
    {
        if (trans == null)
        {
            return;
        }

        if (!trans.gameObject.activeSelf)
        {
            return;
        }

        UILabel uilabel = (UILabel)trans.GetComponent<UILabel>();
        if (uilabel == null)
        {
            Debug.LogWarning(trans.name + "[Does not have UILabel component, Try find in child]");
            uilabel = (UILabel)trans.GetComponentInChildren<UILabel>();
        }
        if (uilabel == null)
        {
            return;
        }

        txt = txt.Replace("\\n", "\n");
        uilabel.text = txt;
    }
    public void ResetSpriteSize(Transform trans, Vector2 size)
    {
        if (trans == null)
        {
            Debug.LogError("ResetSprite is null , " + name);
            return;
        }
        UISprite sprite = (UISprite)trans.GetComponent<UISprite>();
        sprite.SetDimensions((int)size.x + 5, (int)size.y);
    }
    public void ResetSpriteSize(Transform trans, int width)
    {
        if (trans == null)
        {
            Debug.LogError("ResetSprite is null , " + name);
            return;
        }
        UISprite sprite = (UISprite)trans.GetComponent<UISprite>();
        sprite.SetDimensions(width + 5, (int)sprite.localSize.y);
    }

    void Activate(Transform trans, bool active = true)
    {
        if (trans.name != "defenseEnergy")
        {
            trans.gameObject.SetActive(active);
            for (int i = 0, imax = trans.childCount; i < imax; ++i)
            {
                Transform child = trans.GetChild(i);
                Activate(child, active);
            }
        }
    }

    public static bool ClickPointInCollider(string colliderName)
    {
        //         RaycastHit lastHit;
        //         if (UICamera.Raycast(Input.mousePosition, out lastHit))
        //         {
        //             return lastHit.collider.name == colliderName ? true : false;
        //         }

        if (UICamera.lastHit.collider != null)
        {
            return UICamera.lastHit.collider.name == colliderName ? true : false;
        }
        else
        {
            return false;
        }
    }

    public void ScaleParticle(GameObject gb, Vector3 scaleVec)
    {
        ParticleSystem[] partsysArray = gb.GetComponentsInChildren<ParticleSystem>();
        for (int arrayIndex = 0; partsysArray != null && arrayIndex < partsysArray.Length; arrayIndex++)
        {
            partsysArray[arrayIndex].startSpeed = scaleVec.x;
            partsysArray[arrayIndex].startSize = scaleVec.y;
            partsysArray[arrayIndex].gravityModifier = scaleVec.z;
        }
    }

    public static void AutoSetBgScale(Transform bgTrans, Transform widgetRoot, Vector4 board)
    {
        UIWidget bgWidget = bgTrans.GetComponent<UIWidget>();
        if (bgWidget == null)
        {
            return;
        }

        Vector4 bounds = new Vector4(float.MaxValue, -float.MaxValue, -float.MaxValue, float.MaxValue);
        UIWidget[] uiwidgetArray = widgetRoot.GetComponentsInChildren<UIWidget>();
        UIWidget selfWidget = widgetRoot.GetComponent<UIWidget>();
        if (selfWidget != null)
        {
            bounds = GetWidgetBounds(selfWidget, null);
        }

        for (int index = 0; uiwidgetArray != null && index < uiwidgetArray.Length; index++)
        {
            UIWidget item = uiwidgetArray[index];
            Vector4 itemBounds = GetWidgetBounds(item, widgetRoot);
            bounds.x = itemBounds.x < bounds.x ? itemBounds.x : bounds.x;
            bounds.y = itemBounds.y > bounds.y ? itemBounds.y : bounds.y;
            bounds.z = itemBounds.z > bounds.z ? itemBounds.z : bounds.z;
            bounds.w = itemBounds.w < bounds.w ? itemBounds.w : bounds.w;
        }

        Vector3 pos = bgTrans.localPosition;
        bounds.x -= board.x;
        bounds.y += board.y;
        bounds.z += board.z;
        bounds.w -= board.w;

        pos.x = (bounds.x + bounds.z) / 2;
        pos.y = (bounds.y + bounds.w) / 2;
        bgTrans.localPosition = pos;
        bgWidget.width = (int)(bounds.z - bounds.x);
        bgWidget.height = (int)(bounds.y - bounds.w);
    }

    public static Vector4 GetWidgetBounds(UIWidget uiwidget, Transform rootTrans)
    {
        if (!uiwidget.gameObject.activeSelf)
        {
            return Vector4.zero;
        }

        Vector4 vect = Vector4.zero;
        Vector2 centerPos = new Vector2(uiwidget.transform.localPosition.x, uiwidget.transform.localPosition.y);
        if (rootTrans != null && rootTrans != uiwidget.transform)
        {
            Vector3 pos = rootTrans.InverseTransformPoint(uiwidget.transform.position);
            centerPos = new Vector2(pos.x, pos.y);
        }

        if (uiwidget.pivot == UIWidget.Pivot.Bottom
            || uiwidget.pivot == UIWidget.Pivot.BottomLeft
            || uiwidget.pivot == UIWidget.Pivot.BottomRight)
        {
            vect.y = centerPos.y + uiwidget.height;
            vect.w = centerPos.y;
        }
        else if (uiwidget.pivot == UIWidget.Pivot.Top
                 || uiwidget.pivot == UIWidget.Pivot.TopLeft
                 || uiwidget.pivot == UIWidget.Pivot.TopRight)
        {
            vect.y = centerPos.y;
            vect.w = centerPos.y - uiwidget.height;
        }
        else
        {
            vect.y = centerPos.y + uiwidget.height / 2f;
            vect.w = centerPos.y - uiwidget.height / 2f;
        }

        if (uiwidget.pivot == UIWidget.Pivot.Left
            || uiwidget.pivot == UIWidget.Pivot.TopLeft
            || uiwidget.pivot == UIWidget.Pivot.BottomLeft)
        {
            vect.x = centerPos.x;
            vect.z = centerPos.x + uiwidget.width;
        }
        else if (uiwidget.pivot == UIWidget.Pivot.Right
                 || uiwidget.pivot == UIWidget.Pivot.TopRight
                 || uiwidget.pivot == UIWidget.Pivot.BottomRight)
        {
            vect.x = centerPos.x - uiwidget.width;
            vect.z = centerPos.x;
        }
        else
        {
            vect.x = centerPos.x - uiwidget.width / 2f;
            vect.z = centerPos.x + uiwidget.width / 2f;
        }

        return vect;
    }

    public string RemoveColorStr(string str)
    {
        Char[] charArray = str.ToCharArray();
        int leftIndex = 0;
        int rightIndex = 0;
        for (int i = 0; i < charArray.Length; i++)
        {
            if (charArray[i] == '[')
            {
                leftIndex = i;
            }
            else if (charArray[i] == ']')
            {
                rightIndex = i;
                if (rightIndex - leftIndex == 7)
                {
                    for (int colorIndex = leftIndex + 1; colorIndex < rightIndex; colorIndex++)
                    {
                        charArray[colorIndex] = 'f';
                    }
                }
                leftIndex = 0;
                rightIndex = 0;
            }
        }

        return new string(charArray);
    }

    static public void SetLayer(GameObject go, int layer)
    {
        if (go == null)
        {
            return;
        }
        NGUITools.SetLayer(go, layer);
    }

    static public void SetLayer(Transform trans, int layer)
    {
        if (trans == null)
        {
            return;
        }
        NGUITools.SetLayer(trans.gameObject, layer);
    }

    public void SetParentAndReset(Transform self, Transform parent)
    {
        if (self == null || parent == null)
        {
            return;
        }

        self.parent = parent;
        self.localPosition = Vector3.zero;
        self.localEulerAngles = Vector3.zero;
        self.localScale = Vector3.one;

        WindowRoot.SetLayer(self.gameObject, parent.gameObject.layer);
    }

    //type 0:横向；1：纵向;非1和0（任取）为横纵向都居中 
    public void SetMiddle(Transform trans, Vector2 off_set_adjust, int type = 0)
    {
        if (trans == null)
        {
            return;
        }

        Transform tmp_trans = trans;
        Rect box = EZFunUITools.GetRect(tmp_trans);
        Vector2 off_set = new Vector2(-(box.xMin + box.xMax) / 2, -(box.yMin + box.yMax) / 2);

        off_set += off_set_adjust;

        if (type == 0)
        {
            off_set.y = 0;
        }
        else if (type == 1)
        {
            off_set.x = 0;
        }

        trans.localPosition = new Vector3(trans.localPosition.x + off_set.x, trans.localPosition.y + off_set.y, trans.localPosition.z);
    }

    #region camera
    List<Camera> m_otherCamera = new List<Camera>();
    int m_mainCameraCullMask = 0;
    private void SaveCamera()
    {
        m_otherCamera.Clear();

        Transform rootTrans = EZFunWindowMgr.Instance.m_cameraRootTrans;
        for (int i = 0; i < rootTrans.childCount; i++)
        {
            if (NGUITools.GetActive(rootTrans.GetChild(i).gameObject) && rootTrans.GetChild(i).GetComponent<Camera>().enabled)
            {
                Camera camera = rootTrans.GetChild(i).GetComponent<Camera>();
                if (camera.depth >= m_cameraStruct.m_depth
                   || camera.name == "fingercam" || camera.name == "targetcam")
                {
                    continue;
                }
                m_otherCamera.Add(camera);
            }
        }

        m_mainCameraCullMask = 0;
    }

    private void CloseOtherCamera()
    {
        for (int i = 0; i < m_otherCamera.Count; i++)
        {
            if (m_otherCamera[i] == null)
            {
                continue;
            }

            m_otherCamera[i].enabled = false;
        }

        //Camera.main.cullingMask = 0;
        //SetActive(GetTrans(Camera.main.transform, "Camera"), false);
    }

    private void OpenOtherCamera()
    {
        for (int i = 0; i < m_otherCamera.Count; i++)
        {
            if (m_otherCamera[i] == null)
            {
                continue;
            }
            m_otherCamera[i].enabled = true;

            if (EZFunWindowMgr.Instance.CheckWindowIsOpenByCamera(m_otherCamera[i]))
            {
                if (!NGUITools.GetActive(m_otherCamera[i].gameObject))
                {
                    CNetSys.Instance.SendImportantLog(m_currentWindowEnum.ToString() + " Camera is disable!");
                    Debug.LogError(m_currentWindowEnum.ToString() + " Camera is disable!");
                }
                SetActive(m_otherCamera[i].transform, true);
            }
        }

        if (m_mainCameraCullMask != 0 && Camera.main != null)
        {
            Camera.main.cullingMask = -2147450919 | m_mainCameraCullMask;

            SetActive(GetTrans(Camera.main.transform, "Camera"), true);
        }
    }
    #endregion

    public delegate void uiInputOnChangeCB(string str);
  

    public void AdaptChildPosition(Transform parent, int offset_x, int offset_y, int offset_z)
    {
        if (parent == null)
        {
            return;
        }

        Transform trans = null;
        Vector3 first_pos = Vector3.zero;
        Vector3 offset = new Vector3(offset_x, offset_y, offset_z);

        for (int i = 0, j = 0; i < parent.childCount; i++)
        {
            trans = parent.GetChild(i);

            if (i == 0)
            {
                first_pos = trans.localPosition;
            }

            if (trans.gameObject.activeSelf)
            {
                trans.localPosition = first_pos + j * offset;
                j++;
            }
        }
    }

    /// <summary>
    /// 给parent下面的字节点排序定位置用，假如先往一个方向移动maxNum个子节点后，再朝另一向移动一个off，然后再排一排
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="offset_x"></param>
    /// <param name="offset_y"></param>
    /// <param name="maxNum"></param>
    /// <param name="type">1表示maxnum是横向的，其他表示是纵向的</param>
    public void AdaptChildPositionLimitMaxNum(Transform parent, int offset_x, int offset_y, int maxNum, int type, int show_flag)
    {
        //Debug.Log("+++++++++++++ " + maxNum);
        if (parent == null || parent.childCount == 0)
        {
            return;
        }
        Vector3 moveX = new Vector3(offset_x, 0, 0);
        Vector3 moveY = new Vector3(0, offset_y, 0);
        if (type != 1)
        {
            moveX = new Vector3(0, offset_y, 0);
            moveY = new Vector3(offset_x, 0, 0);
        }
        Transform trans = null;
        Vector3 first_pos = Vector3.zero;
        int j = 0;
        for (int i = 0; i < parent.childCount; i++)
        {
            trans = parent.GetChild(i);

            if (i == 0)
            {
                first_pos = trans.localPosition;
            }

            if (trans.gameObject.activeSelf)
            {
                trans.localPosition = first_pos + (j % maxNum) * moveX + (j / maxNum) * moveY;
                j++;
            }
        }

        if (show_flag != 1)
        {

            parent.gameObject.SetActive(false);
            parent.gameObject.SetActive(true);
            //            SetActive(parent, false);
            //            SetActive(parent, true);
        }
    }

    public virtual void SetScrollViewCanDrag(bool canDrag)
    {
        var enumerator = m_modules.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Value.m_moduleRoot != null)
            {
                enumerator.Current.Value.m_moduleRoot.SetScrollViewCanDrag(canDrag);
            }
        }
    }

    public static void AutoSizeParticle(GameObject gb, float size)
    {
        ParticleSystem[] partsysArray = gb.GetComponentsInChildren<ParticleSystem>();
        for (int arrayIndex = 0; partsysArray != null && arrayIndex < partsysArray.Length; arrayIndex++)
        {
            partsysArray[arrayIndex].startSpeed *= size;
            partsysArray[arrayIndex].startSize *= size;
            partsysArray[arrayIndex].gravityModifier *= size;
        }
    }

    /// <summary>
    /// hack,不知为何关闭login_ui_window没有调用InitWindow(false),这里补一下
    /// </summary>
    void OnDisable()
    {
    }

    public float GetTrueWindowWidth()
    {
        if (EZFunWindowMgr.Instance == null)
        {
            return 0;
        }
        return (Screen.width * 1f / Screen.height * EZFunWindowMgr.Instance.GetScreenHeight());
    }

    public void EnableTween(Transform trans, bool flag)
    {
        UITweener[] tweenList = trans.GetComponents<UITweener>();
        for (int i = 0; i < tweenList.Length; i++)
        {
            trans.GetComponent<UIWidget>().alpha = 1f;
            tweenList[i].ResetToBeginning();
            tweenList[i].Play(flag);
        }
    }
    public Vector2 GetLableSize(Transform trans)
    {
        UILabel label = trans.GetComponent<UILabel>();
        if (label != null)
        {
            return new Vector2(label.localSize.x * trans.localScale.x, label.localSize.y * trans.localScale.y);
        }
        else
        {
            return Vector2.zero;
        }
    }

    public static void GenTipsIndependently(string text)
    {
        HandleErrTipsWindow.m_contentStr = text;
        EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.err_tips_ui_window, RessType.RT_CommonWindow, true, 1);
    }

    public Color GetColor(float r, float g, float b)
    {
        float low_num = 0f;
        float up_num = 255f;

        r = FloatInLimit(r, low_num, up_num);
        g = FloatInLimit(g, low_num, up_num);
        b = FloatInLimit(b, low_num, up_num);

        return new Color(r / 255, g / 255, b / 255);
    }

    public float FloatInLimit(float num, float bottom_num, float top_num)
    {
        if (num < bottom_num)
        {
            return bottom_num;
        }
        else if (num > top_num)
        {
            return top_num;
        }
        else
        {
            return num;
        }
    }

    public static void SetLabelGradient(Transform trans, Color topColor, Color bottomColor)
    {
        if (trans == null)
        {
            return;
        }

        if (!NGUITools.GetActive(trans.gameObject))
        {
            return;
        }

        UILabel uiLable = trans.GetComponent<UILabel>();

        if (uiLable == null)
        {
            uiLable = trans.GetComponentInChildren<UILabel>();
        }

        if (uiLable == null)
        {
            return;
        }

        if (!uiLable.applyGradient)
        {
            uiLable.applyGradient = true;
        }

        uiLable.gradientTop = topColor;
        uiLable.gradientBottom = bottomColor;
    }

    public virtual WindowCloseBehaviour GetCloseBehaviour()
    {
        var attrType = EZFunWindowMgr.Instance.GetWindowAttribute(this.m_currentWindowEnum, this.m_windowName);
        if (attrType != null)
        {
            return attrType.m_windowCloseBehaviour;
        }

        return WindowCloseBehaviour.SetActiveFalseAndDestroyWhenNecessary;
    }

    #region ISceneChangeBehaviour implementation
    public WindowBehaviourOnSceneChange GetSceneChangeBehaviour()
    {
        var attrType = EZFunWindowMgr.Instance.GetWindowAttribute(this.m_currentWindowEnum, this.m_windowName);
        if (attrType != null)
        {
            return attrType.m_windowBehaviourOnSceneChange;
        }

        return WindowBehaviourOnSceneChange.Destroy;
    }
    #endregion

    #region tween
    protected void CloneTweenScale(ref TweenPosition toTween, TweenPosition fromTween)
    {
        toTween.style = fromTween.style;
        toTween.from = fromTween.from;
        toTween.to = fromTween.to;
        toTween.animationCurve = fromTween.animationCurve;
        toTween.duration = fromTween.duration;
        toTween.delay = fromTween.delay;
        toTween.ignoreTimeScale = fromTween.ignoreTimeScale;
    }

    protected void CloneTweenAplha(ref TweenAlpha toTween, TweenAlpha fromTween)
    {
        toTween.style = fromTween.style;
        toTween.from = fromTween.from;
        toTween.to = fromTween.to;
        toTween.animationCurve = fromTween.animationCurve;
        toTween.duration = fromTween.duration;
        toTween.delay = fromTween.delay;
        toTween.ignoreTimeScale = fromTween.ignoreTimeScale;
    }

    public void PlayTweenAlpha(Transform trans, float from, float to, double duration)
    {
        TweenAlpha tween = EZFunTools.GetOrAddComponent<TweenAlpha>(trans.gameObject);
        tween.from = from;
        tween.to = to;
        tween.duration = (float)duration;
        tween.ResetToBeginning();
        tween.PlayForward();
    }

    //缓动 替换参数 目前lua用
    public void PlayTweenPos(Transform trans,float time,bool isX,float from, float to)
    {
        TweenPosition tween = EZFunTools.GetOrAddComponent<TweenPosition>(trans.gameObject);
        tween.duration = time;
        if (isX)
        {
            tween.from.x = from;
            tween.to.x = to;
        }
        else 
        {
            tween.from.y = from;
            tween.to.y = to;
        }

        tween.ResetToBeginning();
        tween.PlayForward();
    }

    //由于lua Vector精度问题 这里直接传transform定位
    public void PlayTweenPos(Transform trans, Vector3 fromPos, Vector3 toPos, double duration,bool isWorld)
    {
        TweenPosition tween = EZFunTools.GetOrAddComponent<TweenPosition>(trans.gameObject);
        if (isWorld) 
        {
            fromPos = trans.InverseTransformVector(fromPos);
            toPos = trans.InverseTransformVector(toPos);
        }
        tween.from = fromPos;
        tween.to = toPos;
        tween.duration = (float)duration;
        tween.ResetToBeginning();
        tween.PlayForward();
    }
    public void PlayAndStopTweens(Transform trans, bool isPlay)
    {
        if (trans == null)
        {
            return;
        }
        UITweener[] tweenArray = trans.GetComponentsInChildren<UITweener>();
        if (tweenArray == null)
        {
            return;
        }
        for (int i = 0; i < tweenArray.Length; i++)
        {
            tweenArray[i].ResetToBeginning();
            tweenArray[i].enabled = isPlay;
        }
    }

    public void SetDesAndPlayTweenRotation(Transform trans, Vector3 des)
    {
        if (trans == null)
        {
            return;
        }
        TweenRotation tweenRotation = trans.GetComponent<TweenRotation>();
        if (tweenRotation == null)
        {
            return;
        }

        tweenRotation.from = trans.localEulerAngles;
        tweenRotation.to = des;
        tweenRotation.ResetToBeginning();
        tweenRotation.Play(true);
    }
    #endregion

    //加载图片 url是网络地址 isPixelPerfect表示是否重置大小 StorgeType表示是否要一直缓存
	public void LoadImag(Transform trans, string url, string default_image, bool isPixelPerfect,RessStorgeType StorgeType) 
    {
        ResourceMgr.LoadImag(trans, url, default_image, isPixelPerfect, StorgeType);
    }

    //新加参数 是否存在本地目录 防止浪费流量 由于上面lua大量在用 新加这个
    public void LoadImag(Transform trans, string url, string default_image, bool isPixelPerfect, RessStorgeType StorgeType, bool localSave)
    {
        ResourceMgr.LoadImag(trans, url, default_image, isPixelPerfect, StorgeType, localSave);
    }
}
