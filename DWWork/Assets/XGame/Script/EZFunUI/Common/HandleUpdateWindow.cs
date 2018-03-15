//========================================================================
// Copyright(C): EZFun
//
// CLR Version : 4.0.30319.34209
// NameSpace : Assets.XGame.Script.EZFunUI.Common
// FileName : HandleUpdateWindow
//
// Created by : dhf at 2/6/2015 10:54:44 AM
//
// Function : window for updatesys
//
//========================================================================

using UnityEngine;
using System.Collections;
using LitJson;
using System.IO;

public class HandleUpdateWindow : WindowRoot
{
    private UISlider m_progressSlider;
    private UILabel m_progressLabel;
    private string m_text;
	private JsonData m_loadingJson = null;

	public static HandleUpdateWindow Instance = null;

    private float m_forceBGWidth = 850;
    public override void InitWindow(bool open = true, int state = 0)
    {
        InitCamera(open, false, false, false, (int)SpecialWindowDepthEnum.SWE_LoadingWindow);
        base.InitWindow(open, state);
		Instance = this;
		SetActive(GetTrans("showRoot"), false);

        m_progressSlider.value = 0F;
		m_progressLabel.enabled = true;
        m_progressLabel.text = "";

		if(open)
		{
            ShowUI(false);
            CancelInvoke("UpdateTitle");
			UpdateTitle();
			InvokeRepeating("UpdateTitle", 5, 5);
		}
		else
		{
			CancelInvoke("UpdateTitle");
		}
    }

    protected override void CreateWindow()
    {
        base.CreateWindow();
        var item = ResourceMgr.GetInstantiateAsset(RessType.RT_UIItem, "update_root_item") as GameObject;
        if (item != null)
        {
            item.transform.parent = GetTrans("update_root_parent");
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
        }
        m_progressSlider = GetTrans("progess").GetComponent<UISlider>();
		m_progressLabel = GetTrans("progressLabel").GetComponent<UILabel>();

		byte[] b = EZFunTools.ReadFile(EZFunTools.GetJsonTablePath("ResLoadingTextList"));
		if (b != null)
		{
			m_loadingJson = JsonMapper.ToObject(System.Text.Encoding.UTF8.GetString(b))["list"];
		}

        var trans  = GetTrans("fore_bg");
        if (trans != null)
        {
            var uiwidth = trans.GetComponent<UIWidget>();
            m_forceBGWidth = uiwidth.width;
        }
    }

	public void ShowUI(bool show)
	{
        if (show)
        {
            InitYuhu();
        }
		SetActive(GetTrans("update_root"), show);
	}

    public void ShowLogin(bool isShow, string text = "")
    {
        SetActive(GetTrans("login_root"), isShow);
        EZFunUITools.SetLabel(GetTrans("login_root"), text);
    } 

    public void SetText(string text)
    {
        m_progressLabel.text = text;
        m_text = text;
    }

    private Transform m_yuhuTrans = null;

    private void InitYuhu()
    {
        if (m_yuhuTrans == null)
        {
            var gb = ResourceMgr.GetInstantiateAsset(RessType.RT_UIItem, "jingmai_yuhu") as GameObject;
            if (gb != null)
            {
                SetLayer(gb, this.m_cameraStruct.m_layer);
                m_yuhuTrans = gb.transform;
                m_yuhuTrans.parent = GetTrans("yuhu_root");
                m_yuhuTrans.localPosition = Vector3.zero;
                m_yuhuTrans.localScale = Vector3.one;
                m_yuhuTrans.localEulerAngles = Vector3.zero;
            }
        }
    }

    public void SetProgress(float progress)
    {
        if (progress < 0)
            progress = 0;

        if (progress > 1)
            progress = 1;

        if (GetTrans("yuhu_root") != null)
        {
            GetTrans("yuhu_root").localPosition = new Vector3(0, 0, progress * m_forceBGWidth);
        }
        m_progressSlider.value = progress;
        m_progressLabel.text = m_text + string.Format("{0}%",  (int)(progress * 100));
    }

	private void UpdateTitle()
	{
        if (m_loadingJson != null)
        {
            int index = UnityEngine.Random.Range(0, m_loadingJson.Count);
            string title = m_loadingJson[index]["title"].ToString() + m_loadingJson[index]["information"].ToString();
            title = title.Replace("\\n", "\n");
        }
	}
}
