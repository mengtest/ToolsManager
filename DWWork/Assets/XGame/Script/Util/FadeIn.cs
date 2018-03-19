using UnityEngine;
using System;
using System.Collections;

public class FadeIn: MonoBehaviour
{
    public Action OnFadeIn;
    public string m_shader;

    private bool m_needUpdate = false;
    private Renderer m_renderer;
    private Material m_material;

    public void Init(Action OnFadeInEnd, string shader = "EZFun/SideRim_Spec_Shield")
    {
        OnFadeIn = OnFadeInEnd;
        m_shader = shader;

        m_renderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        m_material = m_renderer.material;
        m_material.shader = EZFunTools.FindShader("EZFun/Transparent");
        var c = m_renderer.material.GetColor("_Color");
        m_material.SetColor("_Color", new Color(c.r, c.g, c.b, 0F));

        m_needUpdate = true;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (!m_needUpdate)
        {
            return;
        }
        var c = m_material.GetColor("_Color");
        var f1 = c.a;
        if (f1 <= 1F)
        {
            m_material.SetColor("_Color", new Color(c.r, c.g, c.b, f1 + Time.deltaTime / 1.5F));
        }
        else 
        {
            m_needUpdate = false;
            m_material.shader = EZFunTools.FindShader(m_shader);
            if (!OnFadeIn.IsNull())
            {
                OnFadeIn();
            }
        }
    }
}
