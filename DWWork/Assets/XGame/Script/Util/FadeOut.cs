using UnityEngine;
using System;
using System.Collections;

public class FadeOut : MonoBehaviour
{
    public Action OnFadeOut;

    private bool m_needUpdate = true;
    private Renderer m_renderer;
    private Material m_material;
    public float FadeOutTime = 1.5f;

    // Use this for initialization
    public void Init(Action OnFadeOutEnd)
    {
        OnFadeOut = OnFadeOutEnd;

        m_renderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        m_material = m_renderer.material;
        m_material.shader = EZFunTools.FindShader("EZFun/Transparent");
        var c = m_renderer.material.GetColor("_Color");
        m_material.SetColor("_Color", new Color(c.r, c.g, c.b, 1F));
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_needUpdate)
        {
            return;
        }
        var c = m_material.GetColor("_Color");
        var f1 = c.a;
        if (f1 >= 0F)
        {
            m_material.SetColor("_Color", new Color(c.r, c.g, c.b, f1 - Time.deltaTime / FadeOutTime));
        }
        else
        {
            m_needUpdate = false;
            if (!OnFadeOut.IsNull())
            {
                OnFadeOut();
            }
        }
    }
}
