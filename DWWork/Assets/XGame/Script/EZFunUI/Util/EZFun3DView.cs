/************************************************************
//     文件名      : EZFun3DView.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-01-09 16:09:04.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;

public class EZFun3DView : MonoBehaviour
{

    protected WindowRoot m_window;

    protected Transform m_modelTrans;

    protected Camera m_camera;

    protected UITexture m_texture;

    protected RenderTexture m_renderTexture;

    protected Transform m_rootTrans;

    protected Transform m_rotationTrans;

    //protected MobileUberPostprocess m_postProcess;

    protected UITweener m_tweener;


    public Transform ModelTrans
    {
        get { return m_modelTrans; }
    }

    public UITexture UITexture
    {
        get { return m_texture; }
        set { m_texture = value; }
    }
   

    public int ShadowLayer
    {
        get;set;
    }

    protected int m_XOffset = 0;

    public void InitCameraTexture(bool enablePostprocess = true)
    {
        if (m_camera == null)
        {
            return;
        }
        var heroCameraTrans = m_camera;
        Camera hereCamera = null;
        if (heroCameraTrans != null)
        {
            hereCamera = heroCameraTrans;
            if (hereCamera.targetTexture == null)
            {
                if (m_renderTexture == null)
                {
                    m_renderTexture = RenderTexture.GetTemporary((int)(Screen.width),
                        (int)(Screen.height), 16, RenderTextureFormat.ARGB32);
                    m_renderTexture.MarkRestoreExpected();
                }
                hereCamera.targetTexture = m_renderTexture;
                hereCamera.backgroundColor = new Color(0, 0, 0, 0);
                hereCamera.clearFlags = CameraClearFlags.SolidColor;
                EZFunTools.SetActive(hereCamera.gameObject, true);
            }
        }
        var shadowTrans = EZFunUITools.GetTrans(this.transform, "ShadowCamera");
        EZFunUITools.SetActive(shadowTrans, false);
        //SetActive(renderTextureTrans, true);
        var tranTexture = m_texture;// EZFunTools.GetOrAddComponent<UITexture>(renderTextureTrans.gameObject);
        tranTexture.shader = EZFunTools.FindShader("Unlit/Premultiplied Colored");
        if (tranTexture.mainTexture != m_renderTexture)
        {
            tranTexture.enabled = false;
            tranTexture.mainTexture = m_renderTexture;
            tranTexture.enabled = true;
        }
        var uiroot = NGUITools.FindInParents<UIRoot>(this.gameObject);
        tranTexture.height = EZFunWindowMgr.Instance.GetScreenHeight();// (int)(uiroot.activeHeight * (hereCamera.rect.height));// - hereCamera.rect.y));
        int width = 1280;
        if (hereCamera != null)
        {
            width = EZFunWindowMgr.Instance.GetScreenWidth();// (int)(720 * Screen.width / Screen.height * (hereCamera.rect.width));
        }
        if (tranTexture.width != width)
        {
            tranTexture.width = width;
        }
        heroCameraTrans.cullingMask = 1 << m_window.m_cameraStruct.m_layer;
        hereCamera.transform.parent = m_rootTrans;
        m_rootTrans.localPosition = new Vector3(m_XOffset, ((int) m_window.m_currentWindowEnum), 0);
        hereCamera.transform.parent = this.transform;
        //if (hereCamera != null)
        //{
           // m_postProcess = EZFunTools.GetOrAddComponent<MobileUberPostprocess>(hereCamera.gameObject);
           // m_postProcess.enabled = enablePostprocess;
            //TimerSys.Instance.AddTimerEventByLeftTime(() =>
            //{
            //    m_postProcess.enabled = true;
            //},0.1f);
        //}
    }

    protected virtual void OnDisable()
    {
        if (m_renderTexture != null && m_renderTexture.IsCreated())
        {
            m_camera.GetComponent<Camera>().targetTexture = null;
            EZFunTools.SetActive(m_camera.gameObject, false);
            //m_renderTexture.Release();
            RenderTexture.ReleaseTemporary(m_renderTexture);
            m_renderTexture = null;
            m_texture.mainTexture = null;
        }
    }

    protected void HandleShadow()
    {
        if (m_camera != null)
        {
            m_camera.backgroundColor = new Color(18 / 255f, 18 / 255f, 18 / 255f, 0);
        }

        var trans = EZFunUITools.GetTrans(transform, "ShadowCamera");
        if (trans != null)
        {
            EZFunUITools.SetActive(trans, true);
            var UIRender = EZFunTools.GetOrAddComponent<UIShadowRender>(trans.gameObject);
            var cam = trans.GetComponent<Camera>();
            cam.cullingMask = (1 <<  this.m_window.m_cameraStruct.m_layer);
        }

        var receiverTrans = EZFunUITools.GetTrans(trans, "ShadowReceiver");
        if (receiverTrans != null)
        {
            receiverTrans.gameObject.layer = ShadowLayer;
        }

        var shadowProjectorTrans = EZFunUITools.GetTrans(trans, "ShadowProjector");
        if (shadowProjectorTrans != null)
        {
            var proj = shadowProjectorTrans.GetComponent<Projector>();
            if (proj != null)
            {
                proj.ignoreLayers = ~( 1 << ShadowLayer );
            }
        }
        m_camera.cullingMask = m_camera.cullingMask | (1 << ShadowLayer);
        if (trans != null && trans.parent != m_camera.transform.parent)
        {
            trans.parent = m_camera.transform.parent;
        }
    }
}
