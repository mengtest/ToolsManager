using UnityEngine;
using System;

public class Interpolator<T>
{
    private float m_T = 0;
    private Func<T, T, float, T> m_Func = null;
    private Action<T> m_callBackFunc = null;
    private bool m_IsStarted = false;
    public bool IsStartWorking
    {
        get { return m_IsStarted; }
    }
    private bool m_IsPause = false;
    public float m_Speed = 0;
    public T m_SrcValue;
    public T m_DestValue;

    public Interpolator(Func<T, T, float, T> func)
    {
        m_Func = func;
    }
    public Interpolator(Func<T, T, float, T> func,Action<T> callBackFunc)
    {
        m_Func = func;
        m_callBackFunc = callBackFunc;
    }

    public void Start()
    {
        m_IsStarted = true;
        m_T = 0.0f;
        m_IsPause = false;
    }

    public void Start(T srcValue, T destValue, float speed)
    {
        m_Speed = speed;
        m_SrcValue = srcValue;
        m_DestValue = destValue;
        Start();
    }

    public void Stop()
    {
        m_IsStarted = false;
        m_IsPause = false;
    }

    public bool IsStop()
    {
        return !m_IsStarted;
    }

    public void Pause(bool isPause)
    {
        m_IsPause = isPause;
    }

    public bool IsPaused()
    {
        return m_IsPause;
    }

    public bool Update(ref T value)
    {
        if (m_IsStarted && !m_IsPause)
        {
            if (m_T < 1.0f)
            {
                m_T += m_Speed * Time.deltaTime;
                value = m_Func(m_SrcValue, m_DestValue, m_T);
                if (m_callBackFunc != null)
                    m_callBackFunc(value);
                return true;
            }
            else
            {
                m_IsStarted = false;
            }
        }
        return false;
    }

    //不返回更新值
    public bool Update()
    {
        if (m_IsStarted && !m_IsPause)
        {
            if (m_T < 1.0f)
            {
                m_T += m_Speed * Time.deltaTime;
                T value = m_Func(m_SrcValue, m_DestValue, m_T);
                if(m_callBackFunc != null)
                    m_callBackFunc(value);
                return true;
            }
            else
            {
                m_IsStarted = false;
            }
        }
        return false;
    }
}
