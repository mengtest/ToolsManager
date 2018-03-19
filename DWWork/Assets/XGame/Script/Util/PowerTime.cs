using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 一个跟随时间增长的类
/// </summary>
public class PowerTime
{
    /// <summary>
    /// 最大值
    /// </summary>
    private int m_MaxValue;
    /// <summary>
    /// 当前值
    /// </summary>
    private EvalueInit m_CurValue;
    /// <summary>
    /// 间隔多少时间涨一点
    /// </summary>
    private int m_IntervalTime;
    /// <summary>
    /// 开始时间
    /// </summary>
    private DateTime m_StartTime;

    private EEventType m_eventType;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="MaxValue">最大值</param>
    /// <param name="IntervalTime">每隔多少时间涨</param>
    /// <param name="CurValue">当前的值</param>
    /// <param name="NextTime">下一次增长时间</param>
    public PowerTime(int MaxValue, int IntervalTime, int CurValue, int NextTime, EEventType eventType = EEventType.EventTypeNull)
    {
        this.m_MaxValue = MaxValue;
        this.m_IntervalTime = IntervalTime;
        this.m_CurValue = new EvalueInit(CurValue);
        this.m_StartTime = DateTime.Now.AddMilliseconds(-NextTime);
        this.m_eventType = eventType;
    }


    public int MaxValue
    {
        get
        {
            return m_MaxValue;
        }
        set
        {
            m_MaxValue = value;
            Refresh();
        }
    }

    public bool Add(int sub)
    {
        int curValue = this.CurValue;
        if (curValue + sub >= 0)
        {
            m_CurValue.m_value = curValue + sub;
            if (sub != 0 && m_eventType != EEventType.EventTypeNull)
            {
                EventSys.Instance.AddEventNow(m_eventType, 3);
            }
            Refresh();
            return true;
        }
        else
        {
            m_CurValue.m_value = 0;
        }
        return false;
    }

    /// <summary>
    /// 当前值，会自动和时间绑定上
    /// </summary>
    public int CurValue
    {
        get
        {
            Refresh();
            return m_CurValue.m_value;
        }
    }

    private void Refresh()
    {
        if (m_CurValue.m_value > m_MaxValue)
        {
            m_StartTime = DateTime.Now;
            return;
        }
        int sub = (int)(DateTime.Now.Subtract(m_StartTime).TotalSeconds / m_IntervalTime);
        if (sub + m_CurValue.m_value >= m_MaxValue)
        {
            m_CurValue.m_value = m_MaxValue;
            m_StartTime = DateTime.Now;
        }
        else if (sub >= 1)
        {
            m_CurValue.m_value += sub;
            m_StartTime = m_StartTime.AddSeconds(sub * m_IntervalTime);
        }
        if (sub != 0 && m_eventType != EEventType.EventTypeNull)
        {
            EventSys.Instance.AddEventNow(m_eventType, 3);
        }
    }

    public bool IsFull
    {
        get
        {
            return CurValue >= m_MaxValue;
        }
    }

    /// <summary>
    /// 返回单位为秒
    /// </summary>
    public int NextTime
    {
        get
        {
            //其实我只是想计算一遍而已
            int value = this.CurValue;
            if (value == m_MaxValue)
            {
                return 0;
            }
            return m_IntervalTime - (int)(DateTime.Now.Subtract(m_StartTime).TotalSeconds);
        }
    }

    public string FullTimeStr
    {
        get
        {
            int curValue = CurValue;
            if (curValue == m_MaxValue)
            {
                return "";
            }

            int needTime = m_MaxValue - curValue;
            needTime *= m_IntervalTime;
            needTime = (int)m_StartTime.AddSeconds(needTime).Subtract(DateTime.Now).TotalSeconds;

            //(MaxValue - curValue) * m_IntervalTime + 
            return EZFunTools.GetSecToStr(needTime);
        }
    }


    public string NextTimeStr
    {
        get
        {
            if (NextTime == 0)
            {
                return "";
            }
            int nextTime = NextTime;
            return  EZFunTools.GetSecToStr(nextTime);
          
        }
    }

    string TimeToStr(int time)
    {

        StringBuilder sb = new StringBuilder();
        if (time > 3600)
        {
            sb.Append(((int)(time / 3600)));
            sb.Append(TextData.GetText(400591));
        }
        if (((int)((time % 3600) / 60)) > 0)
        {
            sb.Append(((int)((time % 3600) / 60)));
            sb.Append(TextData.GetText(400191));
        }
        else
        {
            if (time > 3600)
            {
                sb.Append(TextData.GetText(400592));
            }
        }
        sb.Append(((int)(time % 60)));
        sb.Append(TextData.GetText(400192));

        return sb.ToString();

    }

}

