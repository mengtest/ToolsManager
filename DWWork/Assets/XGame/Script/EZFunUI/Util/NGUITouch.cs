/************************************************************
//     文件名      : NGUITouch.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-02-03 12:15:09.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;


public delegate void NGUITouchHandle(bool isPress, Vector2 delta);

[RequireComponent(typeof(UIWidget))]
public class NGUITouch : MonoBehaviour
{
    public static NGUITouchHandle On_JoystickTouchUp;
    public static NGUITouchHandle On_JoystickMove;
    public static NGUITouchHandle On_JoystickTap;

    private Vector2 m_currentPos;

    private Transform m_trans;

    public static Camera m_cam;

    private void OnPress(bool isPress)
    {
        if (isPress)
        {
            if (On_JoystickTap != null)
            {
                On_JoystickTap(isPress, Vector2.zero);
            }
            if (m_cam == null)
            {
                return;
            }
            m_currentPos = UICamera.currentTouch.pos;
        }
        else
        {
            if (On_JoystickTouchUp != null)
            {
                On_JoystickTouchUp(isPress, Vector2.zero);
            }
            m_currentPos = Vector3.zero;
        }
    }

    private void OnEnable()
    {
        m_currentPos = Vector2.zero;
        m_trans = this.transform;
    }
    
    private void OnDisable()
    {
        m_currentPos = Vector2.zero;
    }

    private void OnDrag(Vector2 delta)
    {
        m_currentPos = UICamera.currentTouch.pos;
    }


    private void LateUpdate()
    {
        if (m_currentPos != Vector2.zero)
            NotifyMove();
    }

    private void NotifyMove()
    {
        if (On_JoystickMove != null)
        {
            Vector2 transPos = m_cam.WorldToScreenPoint(m_trans.position);
            var vec = m_currentPos - transPos;
            On_JoystickMove(true, vec);
        }
    }
}
