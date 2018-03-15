using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UIShadowRender : MonoBehaviour
{
    private const int renderTexSize = 512;
    private int currentSize;
    private Material mat;

    public float xRotation;
    public float yRotation;

    private RenderTexture renderTexture = null;
    private Shader m_Shader = null;

    void  Start()
    {
        currentSize = renderTexSize;
        mat = new Material(EZFunTools.FindShader("XGame2/RenderShadow"));

        renderTexture = MakeRenderTexture(currentSize);
        GetComponentInChildren<Projector>().material.SetTexture("_ShadowTex", renderTexture);
        mat.SetTexture("_MainTex", renderTexture);

        Camera camera = GetComponent<Camera>();
        camera.targetTexture = renderTexture;

        Camera shadowCam = GetComponent<Camera>();
        if (shadowCam != null)
        {
            if (m_Shader == null)
            {
                m_Shader = EZFunTools.FindShader("Hidden/Shadow_Mask");
            }
            if (m_Shader != null)
            {
                shadowCam.SetReplacementShader(m_Shader, "RenderType");
            }
        }
    }

    RenderTexture MakeRenderTexture(int rtSize)
    {
        RenderTexture renderTex;

        if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8))
        {
            renderTex = new RenderTexture(rtSize, rtSize, 0, RenderTextureFormat.R8);
        }
        else
        {
            renderTex = new RenderTexture(rtSize, rtSize, 0, RenderTextureFormat.ARGB32);
        }

        renderTex.useMipMap = false;
        renderTex.depth = 0;
        renderTex.filterMode = FilterMode.Bilinear;
        renderTex.anisoLevel = 0;
        return renderTex;
    }


    public void ChangeTextureSize(int newSize)
    {
        if (currentSize == newSize)
        {
            return;
        }

        currentSize = newSize;
        GetComponent<Camera>().targetTexture.Release();
        GetComponent<Camera>().targetTexture = MakeRenderTexture(currentSize);
        mat.SetTexture("_MainTex", GetComponent<Camera>().targetTexture);
        GetComponentInChildren<Projector>().material.SetTexture("_ShadowTex", GetComponent<Camera>().targetTexture);
    }
}

