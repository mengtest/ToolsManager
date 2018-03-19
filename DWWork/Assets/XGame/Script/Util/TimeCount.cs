using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[LuaWrap]
public class TimeCount
{
    private double m_curTime = 0;

    private double m_count = 0;

    public bool m_isRealTime = true;

    public TimeCount()
    {

    }

    public TimeCount(bool isRealTime)
    {
        this.m_isRealTime = isRealTime;
    }

    private double curTime
    {
        get
        {
            if (m_isRealTime)
            {
                return GameRoot.m_realTime;
            }
            return GameRoot.m_gameTime;
        }
    }

    public double Value
    {
        get
        {
            double lastTime = curTime - m_curTime;
            double leftVaue = m_count - lastTime;
            if ((leftVaue) <= 0)
            {
                return 0;
            }
            return leftVaue;
        }

        set
        {
            m_count = value;
            m_curTime = curTime;
        }
    }
}
