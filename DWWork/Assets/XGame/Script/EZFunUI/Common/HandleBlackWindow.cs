using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class HandleBlackWindow : WindowRoot
{
    public static float m_animationLength = 0.4f;

    public static int m_textID = 0;

    private UITweener m_animationTrans = null;

    private int m_curIndex = 0;

    private string m_text;

    private float m_time = 0;

    Transform m_contextTrans = null;

    public static bool m_isPlayIn = false;

    protected override void CreateWindow()
    {
        base.CreateWindow();
        m_contextTrans = this.GetTrans("ContextTrans");
    }

    public override void InitWindow(bool open = true, int state = 0)
    {
        if (m_animationTrans == null)
        {
            var trans = GetTrans("animation1_ui_root");
            m_animationTrans = trans.GetComponent<UITweener>();
        }
        Debug.Log("HandleBlackWindow.InitWindow :" + open);
        m_animationTrans.duration = m_animationLength;
        InitCamera(open, false, (state == 1 || state == 2) ? EZFunWindowMgr.BLACK_MAX_DEPTH : -1);
        base.InitWindow(open, state);
        if (m_textID != 0)
        {
            m_text = TextData.GetText(m_textID);
            m_curIndex = 1;
            m_time = GameRoot.m_gameTime;
            SetActive(m_contextTrans, true);
            SetLabel(m_contextTrans, m_text.Substring(0, m_curIndex));
        }
        else
        {
            SetActive(m_contextTrans, false);
        }
        if (state == 2)
        {
            var ntrans = GetTrans("animation1_ui_root");
            if (ntrans != null)
            {
                var uiwidget = ntrans.GetComponent<UIWidget>();
                uiwidget.color = Color.white;
            }
        }
    }


    protected override void Update()
    {
        if (m_text == null || m_curIndex == m_text.Length)
        {
            return;
        }
        if (GameRoot.m_gameTime - m_time > 0.4f)
        {
            m_time = GameRoot.m_gameTime;
            m_curIndex++;
            SetLabel(m_contextTrans, m_text.Substring(0, m_curIndex));
        }
    }
}
