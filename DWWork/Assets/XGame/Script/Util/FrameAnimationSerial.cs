//========================================================================
// Copyright(C): EZFun
//
// CLR Version : 4.0.30319.42000
// Machinename : USER-20160222CX
// NameSpace   : Assets.XGame.Script.Util
// FileName    : FrameAnimationSerial
//
// Created by  : dhf at 2016/2/29 11:20:54
//
// Function    : UISprite 序列帧动画
//
//========================================================================


using System.Collections.Generic;
using UnityEngine;

class FrameAnimationSerial:MonoBehaviour
{
    public float m_timeInterval;
    private UISprite m_sprite = null;
    private List<string> m_names = null;
    private int m_index = 0;
    private float m_time;

    void OnEnable()
    {
        if(m_sprite == null)
        {
            m_sprite = transform.GetComponent<UISprite>();
        }
        if(m_sprite == null)
        {
            enabled = false;
            return;
        }
        m_names = new List<string>();
        var name = m_sprite.spriteName;
        int index = 0;
        while(name != null)
        {
            m_names.Add(name);
            name = m_sprite.spriteName + "_" + ++index;
            if(m_sprite.atlas.GetSprite(name) == null)
            {
                break;
            }
        }
    }

    void OnDisable()
    {
        m_sprite.spriteName = m_names[0];
    }

    void Start()
    {
        m_index = 0;
        m_time = 0F;
    }

    void Update()
    {
        m_time = Time.deltaTime + m_time;
        if(m_time < m_timeInterval)
        {
            return;
        }
        if(m_index >= m_names.Count)
        {
            m_index = 0;
        }
        m_sprite.spriteName = m_names[m_index++];
        m_time = 0;
    }
}
