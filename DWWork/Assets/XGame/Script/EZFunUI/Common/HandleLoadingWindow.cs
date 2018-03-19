using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ezfun_resource;

using SheetType = System.Collections.Generic.Dictionary<object, global::ProtoBuf.IExtensible>;

public class HandleLoadingWindow : WindowRoot
{
    private Transform m_yuhuRootTrans = null;
    public static bool m_inLoadingDone = false;
    //是否加载成功
    private static bool _m_loadingOver = true;
    public static bool m_loadingOver
    {
        get { return _m_loadingOver; }
    }

    const float M_SCENE_RATE = 0.25f;
    const float M_DELTA_RATE = 0.1f;
    //bool m_isChangeResolution = false;
    float slideValue = 0;
    TweenAlpha m_tweenAlpha;
    bool m_loadingWinClosing = false;
    int m_loadingType = 0;//0普通，1百叶窗,2天梯单独界面
    private bool m_outLoading = false;
    public override void InitWindow(bool open = true, int state = 0)
    {
        InitCamera(open, false, (int)SpecialWindowDepthEnum.SWE_LoadingWindow);
        base.InitWindow(open, state);

        if (open)
        {
            InitWindowDetail(state);
            _m_loadingOver = false;
        }
        else
        {
            _m_loadingOver = true;
        }
        m_outLoading = false;
    }

    //IEnumerator WaitForChangeResolution()
    //{
    //    yield return new WaitForSeconds(0.2f);
    //    ChangeAndroidResolution();
    //    yield return new WaitForSeconds(0.2f);
    //    SetActive(GetTrans("P_Root"), true);
    //    m_isChangeResolution = true;
    //}

    private const string TargetDeviceName = "HUAWEI";

    private bool IsTargetDevice(string deviceName)
    {
        if (deviceName.Contains(TargetDeviceName) && (deviceName.Contains("P7") || deviceName.Contains("P6")))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ChangeAndroidResolution()
    {
        //float targetHeight = Constants.ScreenMediumHeight;
        //float targetWidth = 0.0f;
        //int type = QualitySys.Instance.GetQualityLevelByType(QualityType.UIQuality);
        //switch (type)
        //{
        //    case (int)QualityLevel.Medium:
        //        targetHeight = Constants.ScreenMediumHeight;
        //        targetWidth = GameRoot.screen_height / targetHeight;
        //        break;
        //    case (int)QualityLevel.High:
        //    case (int)QualityLevel.Perfect:
        //        targetHeight = Screen.currentResolution.height;
        //        targetWidth = Screen.currentResolution.width;
        //        break;
        //    case (int)QualityLevel.Low:
        //        targetHeight = Constants.ScrrenLowHeight;
        //        targetWidth = GameRoot.screen_height / targetHeight;
        //        break;
        //}

        //if (type < (int)QualityLevel.High)
        //{
        //    Screen.SetResolution((int)(GameRoot.screen_width / targetWidth), (int)targetHeight, true);
        //}
        //else
        //{
        //    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        //}

    }

    public override void ClearEventHandler()
    {
        base.ClearEventHandler();
    }

    private float m_forceBGWidth = 850;
    private UISlider m_progressSlider;

    protected override void CreateWindow()
    {
        base.CreateWindow();
        m_progressSlider = GetTrans("progess").GetComponent<UISlider>();
        m_tweenAlpha = GetTrans("background").GetComponent<TweenAlpha>();
        m_tweenAlpha.onFinished.Add(new EventDelegate(() =>
        {
            InitWindow(false);
        }));
        float rate = EZFunWindowMgr.Instance.GetScreenHeight() * 1f / 720;
        Vector3 scale = GetTrans("fx_baiyechuan_guan").localScale;
        scale.y *= rate;
        GetTrans("fx_baiyechuan_guan").localScale = scale;
        scale = GetTrans("fx_baiyechuan_kai").localScale;
        scale.y *= rate;
        GetTrans("fx_baiyechuan_kai").localScale = scale;
        var trans = GetTrans("fore_bg");
        if (trans != null)
        {
            var uiwidth = trans.GetComponent<UIWidget>();
            m_forceBGWidth = uiwidth.width;
        }
        InitYuhu();
        m_yuhuRootTrans = GetTrans("yuhu_root");
    }

    void InitWindowDetail(int state)
    {
        m_loadingWinClosing = false;
        SetActive(GetTrans("background"), true);
        EZFunUITools.PlayForceAnimation(GetTrans("background"), null, false, false);
        slideValue = 0;
        SetActive(m_yuhuRootTrans, true);
        //if (Version.Instance.GetCurrentSdkPlatform() == ENUM_SDK_PLATFORM.ESP_YingyongBao)
        //{
        //            SetActive(GetTrans("logo_wushuang"), false);
        //            SetActive(GetTrans("logo_baye"), true);
        //}
        //else
        //{
        //    SetActive(GetTrans("logo_wushuang"), true);
        //    SetActive(GetTrans("logo_baye"), false);
        //}
        m_inLoadingDone = false;
//        SetProgress(0);
        if (state == 1)
        {
            SetActive(GetTrans("P_Root"), false);
            SetActive(GetTrans("fx_baiyechuan_guan"), false);
            SetActive(GetTrans("fx_baiyechuan_kai"), false);
            m_loadingType = 0;
            m_inLoadingDone = true;
        }
        else
        {
            RandTips();
            SetActive(GetTrans("P_Root"), true);

            //现在不用百叶窗了！！
            //屏蔽百叶窗功能
            //if((GameStateMgr.Instance.GetWillStateType() == EGameStateType.GameState) && GameStateMgr.Instance.GetCurSceneName() != "yinzi")
            //{
            //	m_loadingType = 1;
            //	OpenBaiYeChuan();
            //}
            //else
            {
                m_loadingType = 0;
                InLoading();
            }
        }

        //		if (Application.platform == RuntimePlatform.Android)
        //{
        //	if (!m_isChangeResolution)
        //	{
        //		SetActive(GetTrans("P_Root"), false);
        //		StartCoroutine("WaitForChangeResolution");
        //	}
        //}

    }

    private Transform m_yuhuTrans = null;
    private void InitYuhu()
    {
        //if (m_yuhuTrans == null)
        //{
        //    var gb = ResourceMgr.GetInstantiateAsset(RessType.RT_UIItem, "jingmai_yuhu"
        //        , RessStorgeType.RST_Always
        //        ) as GameObject;
        //    if (gb != null)
        //    {
        //        SetLayer(gb, this.m_cameraStruct.m_layer);
        //        m_yuhuTrans = gb.transform;
        //        m_yuhuTrans.parent = GetTrans("yuhu_root");
        //        m_yuhuTrans.localPosition = Vector3.zero;
        //        m_yuhuTrans.localScale = Vector3.one;
        //        m_yuhuTrans.localEulerAngles = Vector3.zero;
        //    }
        //}
    }

    //开启百叶窗
    void OpenBaiYeChuan()
    {
        SetActive(GetTrans("fx_baiyechuan_guan"), false);
        SetActive(GetTrans("fx_baiyechuan_kai"), false);
        SetActive(GetTrans("fx_baiyechuan_guan"), true);
        SetActive(GetTrans("background"), false);
        m_cameraStruct.m_camera.orthographicSize = 0.99f;
        TimerSys.Instance.AddFrameEvtByLeftFrame(() =>
        {
            SetActive(GetTrans("fx_baiyechuan_guan"), false);
            SetActive(GetTrans("background"), true);
            m_inLoadingDone = true;
        }, 33);
    }
    //关闭百叶窗
    void CloseBaiYeChuan()
    {
        m_cameraStruct.m_camera.orthographicSize = 1.0f;
        SetActive(GetTrans("fx_baiyechuan_kai"), true);
        SetActive(GetTrans("background"), false);

        TimerSys.Instance.AddFrameEvtByLeftFrame(() =>
        {
            InitWindow(false);
        }, 33);
    }

    //普通进入Loading, 开启黑幕
    void InLoading()
    {
        SetActive(GetTrans("fx_baiyechuan_guan"), false);
        SetActive(GetTrans("fx_baiyechuan_kai"), false);

        SetActive(GetTrans("P_Root"), true);
        TimerSys.Instance.AddFrameEvtByLeftFrame(() =>
        {
            m_inLoadingDone = true;
        }, 1);
    }
    //普通退出Loading, 黑幕
    private const float LoadingFinshTime = 1.0f;
    void OutLoading()
    {
        EZFunUITools.PlayForceAnimation(GetTrans("background"), () =>
        {
            InitWindow(false);
        },true, true);
        SetActive(m_yuhuRootTrans, false);
        TimerSys.Instance.AddTimerEventByLeftTime(() =>
        {
            EventSys.Instance.AddEventNow(EEventType.LoadingStateFinish);
        }, LoadingFinshTime);
    }

    void RandTips()
    {
      
    }

    protected override void Update()
    {
        if (!LoadingState.m_loadSceneComplete)
        {
            float loadSceneProgress = LoadingState.ProgressValue;
            float progressValue = M_SCENE_RATE * loadSceneProgress;
            slideValue = progressValue;
        }
        else
        {
            float loginInitProgress = BaseGameState.ProgressValue;
            float progressValue = Mathf.Lerp(M_SCENE_RATE, 1, loginInitProgress);
            slideValue = progressValue;
        }
 //       SetProgress(slideValue);
        if (Mathf.Abs(slideValue - 1f) <= 0.001f && !m_loadingWinClosing)
        {
            m_loadingWinClosing = true;
            switch (m_loadingType)
            {
                case 0:
                    OutLoading();
                    break;
                case 1:
                    CloseBaiYeChuan();
                    break;
            }
            m_outLoading = true;
        }
    }

    public void SetProgress(float progress)
    {
        if (m_yuhuRootTrans != null)
        {
            SetActive(m_yuhuRootTrans, true);
            m_yuhuRootTrans.localPosition = new Vector3(progress * m_forceBGWidth,0,0);
        }
        m_progressSlider.value = progress;
    }
}
