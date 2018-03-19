/************************************************************
    File      : Timer.cs
    brief     : 计时器系统，倒计时完成之后，发送相应的消息  
    author    : JanusLiu   janusliu@ezfun.cn
    version   : 1.0
    date      : 2014/12/22 14:18:2
    copyright : Copyright 2014 EZFun Inc.
**************************************************************/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

[LuaWrap]
[RegisterSystem(typeof(TimerSys), true)]
public class TimerSys : TCoreSystem<TimerSys>, IInitializeable, IResetable, IUpdateable
{

    public delegate void CallBack();

    private int m_utc = 8;
    private bool SetUTC_First = true;

    public int UTCZone
    {
        set
        {
            if (SetUTC_First)
            {
                m_utc = value;
            }
        }
        get
        {
            return m_utc;
        }
    }

    class TimerEvent
    {
        public CallBack m_callBack;
        public double m_destTime;
        public int m_seq;
        //执行次数，为0表示无限次执行,默认为1
        public int m_repeateTime = 1;
        //重复执行间隔时间
        public double m_interval; 

        public TimerEvent(CallBack callBack, double interval, int repeatTime = 1)
        {
            m_callBack = callBack;
            m_destTime = Time.realtimeSinceStartup + interval;
            m_interval = interval;
            m_repeateTime = repeatTime;
            m_seq = TimerSys.m_seq_++;
        }
    }

    class FrameEvent
    {
        public CallBack m_callBack;
        public int m_destFrame;
        public int m_seq;

        public FrameEvent(CallBack callBack, int destFrame)
        {
            m_callBack = callBack;
            m_destFrame = destFrame;
            m_seq = TimerSys.m_seq_++;
        }
    }

    class WaitUntilEvent
    {
        public CallBack m_callBack;
        public Func<bool> m_waitUnitl;
        public int m_seq;

        public WaitUntilEvent(CallBack callBack, Func<bool> waitUntil)
        {
            m_callBack = callBack;
            m_waitUnitl = waitUntil;
            m_seq = m_seq++;

        }
    }

    private int m_seq = 0;
    static public int m_seq_ = 0;
    //private static String ms_lock = "TimerLock";

    private DateTime UTC_Start = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    //当前服务器时间，主要用于和服务器相关时间的计算
    private double m_currentDateTime = 0;
    //当前的本地时间，主要用于校验是否用了加速器
    private double m_currentLocalTime = 0;

    private List<TimerEvent> m_timerEventList = new List<TimerEvent>();
    private List<TimerEvent> m_tmpList = new List<TimerEvent>();

    private List<FrameEvent> m_frameEvtList = new List<FrameEvent>();
    private List<FrameEvent> m_tmpFrameEvtList = new List<FrameEvent>();

    private List<WaitUntilEvent> m_waitUntilEvtList = new List<WaitUntilEvent>();
    private List<WaitUntilEvent> m_tmpWaitUntilEvtList = new List<WaitUntilEvent>();

    private double m_currentDateTimeOffset = 0;
    //凌晨五点刷新时间偏移,为了减轻后台的访问压力,凌晨五点的数据重置在10秒的区间内随机
    private int m_random5AMOffsetTime = 0;

    private bool m_hasInit = false;
    //普通时候心跳包间隔时间为5分钟
    private int m_normalSendHeartDeltaTime = 10 * 1000;
    //战斗时候心跳包间隔时间为10秒钟,具体数值读表
    private float m_battleSendHeartDeltaTime = 10 * 1000;
    //开始发送心跳包
    private bool m_beginSendHeart = false;

    public void Init()
    {
        if (m_hasInit)
        {
            return;
        }
        m_hasInit = true;

        m_timerEventList.Clear();
        m_currentDateTime = (DateTime.Now.ToUniversalTime() - UTC_Start).TotalMilliseconds;
        m_currentLocalTime = (DateTime.Now.ToUniversalTime() - UTC_Start).TotalMilliseconds;
        m_random5AMOffsetTime = UnityEngine.Random.Range(20, 30);
        //EventSys.Instance.AddHander(EEventType.TimeNetReq, (EEventType eventid, object o1, object o2) =>
        //{
        //    HandleTimeNetReq((ezfun.SCPackage)o1, (NetMsgSpecialFailCallBack)o2);
        //});
    }

    public void Release()
    {

    }

    public double GetLocalTime()
    {
        return ((DateTime.Now.ToUniversalTime() - UTC_Start).TotalSeconds);
    }

    public double GetLocalMilliseconds()
    {
        var t =
        ((DateTime.Now.ToUniversalTime() - UTC_Start).TotalMilliseconds) * 1f;
        return t;
    }

    public void Add5AMEvent()
    {
        m_seq++;
        int seq = m_seq;
        AddTimerEventByDestTime(() =>
        {
            if (m_seq == seq)
            {
                //Debug.LogError(System.DateTime.Now.ToString() + "Add SysResetEvt5AM");
                TimerSys.Instance.AddTimerEventByLeftTime(() =>
                {
                    Add5AMEvent();
                }, 1);
            }
        }, GetUtcTime(false, 5, 0, m_random5AMOffsetTime));
    }

    public void Reset()
    {
        m_timerEventList.Clear();
        m_beginSendHeart = false;
    }

    public void Update()
    {
        m_currentDateTime = GetLocalMilliseconds() + m_currentDateTimeOffset;
        m_currentLocalTime += Time.unscaledDeltaTime * 1000;

        for (int tmpIndex = 0; tmpIndex < m_tmpList.Count; tmpIndex++)
        {
            m_timerEventList.Add(m_tmpList[tmpIndex]);
        }
        m_tmpList.Clear();
        m_timerEventList.Sort(delegate (TimerEvent x, TimerEvent y)
        {
            return (int)((x.m_destTime - y.m_destTime) * 1000);
        });

        for (int tmpIndex = 0; tmpIndex < m_tmpFrameEvtList.Count; tmpIndex++)
        {
            m_frameEvtList.Add(m_tmpFrameEvtList[tmpIndex]);
        }
        m_tmpFrameEvtList.Clear();
        m_frameEvtList.Sort(delegate (FrameEvent x, FrameEvent y)
        {
            return (int)((x.m_destFrame - y.m_destFrame) * 1000);
        });

        m_waitUntilEvtList.AddRange(m_tmpWaitUntilEvtList);
        m_tmpWaitUntilEvtList.Clear();

        UpdateTimerEvent();
    }
    //获取当天时间秒数
    public int GetCurrentHMSTime()
    {
        DateTime dateTime = GetCurrentDate();
        return dateTime.Hour * 3600 + dateTime.Minute * 60 + dateTime.Second;
    }


    public int GetCurrentDateTime()
    {
        return (int)(GetCurrentDoubleTime());
    }

    public double GetCurrentDoubleTime()
    {
        return m_currentDateTime / 1000;
    }
    //获得当前毫秒时间戳
    public long GetCurrentDateMTimes()
    {
        return (long)m_currentDateTime;
    }

    public int UtcNeedSeconds()
    {
        return m_utc * 60 * 60;
    }
    /// <summary>
    /// int 容易越界
    /// </summary>
    /// <returns></returns>
    public long GetCurrentDateTimeLong()
    {
        return (long)GetCurrentDoubleTime();
    }

    public string GetCurrentDateTimeDouble()
    {
        return DateTime.Now.ToString() + " " + DateTime.Now.Millisecond.ToString();
    }

    public DateTime GetCurrentDate()
    {
        DateTime dsStart = UTC_Start.AddSeconds(UtcNeedSeconds() * 1.0f);
        DateTime currentTime = dsStart.AddSeconds(GetCurrentDateTime());

        return currentTime;
    }

    public DateTime GetaServerUtcStart()
    {
        return UTC_Start.AddSeconds(UtcNeedSeconds() * 1.0f);
    }

    //注意，获取基于当前时区的秒数
    public long GetCurDateTimeLocalLong()
    {
        var ts = GetCurrentDate() - UTC_Start;
        return (long)ts.TotalSeconds;
    }

    public string GetTimeStr(int time)
    {
        if (time < 10)
        {
            return "0" + time;
        }
        else
        {
            return time.ToString();
        }
    }

    // 取当前时间 Hour:Minute
    public string GetCurrentTimeStrHM()
    {
        DateTime currentTime = GetCurrentDate();
        string str = GetTimeStr(currentTime.Hour) + ":" + GetTimeStr(currentTime.Minute);
        return str;
    }

    public string GetCurrentTimeStrMin()
    {
        DateTime currentTime = GetCurrentDate();
        string str = GetTimeStr(currentTime.Minute);
        return str;
    }

    public string GetCurrentTimeStrHour()
    {
        DateTime currentTime = GetCurrentDate();
        string str = GetTimeStr(currentTime.Hour);
        return str;
    }

    public string GetCurrentTimeStr()
    {
        DateTime currentTime = GetCurrentDate();

        string str = GetTimeStr(currentTime.Hour) + ":" + GetTimeStr(currentTime.Minute) + ":" + GetTimeStr(currentTime.Second);
        return str;
    }

    public int GetCurrentWeekOfDay()
    {
        return (int)GetCurrentDate().DayOfWeek;
    }
    public int GetCurrentYear()
    {
        return GetCurrentDate().Year;
    }

    public int GetCurrentMonth()
    {
        return GetCurrentDate().Month;
    }

    public int GetCurrentDay()
    {
        return GetCurrentDate().Day;
    }

    public int GetMonthAddSecends(int second)
    {
        DateTime dsStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        DateTime currentTime = dsStart.AddSeconds(GetCurrentDateTime() + second);
        return currentTime.Month;
    }


    public int GetDayAddSecends(int second)
    {
        DateTime dsStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        DateTime currentTime = dsStart.AddSeconds(GetCurrentDateTime() + second);
        return currentTime.Day;
    }

    //dateTime是毫秒
    public void UpdateCurrentDataTime(long dateTime)
    {
        double timeIsMs = dateTime;
        m_currentDateTime = timeIsMs;
        m_currentDateTimeOffset = timeIsMs - GetLocalMilliseconds();
        m_currentLocalTime = timeIsMs;

        m_beginSendHeart = true;
    }

    public void BeginSendHeart()
    {
        m_beginSendHeart = true;
    }

    public int AddTimerEventByDestTime(CallBack callBack, double destTime)
    {
        double interval = destTime - GetCurrentDoubleTime();
        var te = new TimerEvent(callBack, interval);
        m_tmpList.Add(te);
        return te.m_seq;
    }

    public int AddTimerEventByLeftTime(CallBack callBack, float leftTime)
    {
        var te = new TimerEvent(callBack, leftTime);
        m_tmpList.Add(te);
        return te.m_seq;
    }

    public int AddRepeatTimerEvent(CallBack callBack, float interval, int repeatTime, bool fireImmeditely = false)
    {
        if (fireImmeditely)
        {
            callBack();
            repeatTime--;
        }

        if (repeatTime <= 0)
        {
            return 0;
        }

        var te = new TimerEvent(callBack, interval, repeatTime);
        m_tmpList.Add(te);
        return te.m_seq;
    }

    public int AddWaitUntilEvent(CallBack callback, Func<bool> waitUntil)
    {
        if (waitUntil())
        {
            callback();
            return 0;
        }

        WaitUntilEvent we = new WaitUntilEvent(callback, waitUntil);
        m_tmpWaitUntilEvtList.Add(we);
        return we.m_seq;
    }

    public void RemoveTimerEvt(int seq)
    {
        if (m_tmpList != null)
        {
            for (int i = 0; i < m_tmpList.Count; ++i)
            {
                if (m_tmpList[i].m_seq == seq)
                {
                    m_tmpList.RemoveAt(i);
                    break;
                }
            }
        }
        if (m_timerEventList != null)
        {
            for (int i = 0; i < m_timerEventList.Count; ++i)
            {
                if (m_timerEventList[i].m_seq == seq)
                {
                    m_timerEventList.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public int AddFrameEvtByLeftFrame(CallBack callBack, int leftFrame)
    {
        var fe = new FrameEvent(callBack, leftFrame + Time.frameCount);
        m_tmpFrameEvtList.Add(fe);
        return fe.m_seq;
    }

    public void RemoveFrameEvtByLeftFrame(int seq)
    {
        if (m_tmpFrameEvtList != null)
        {
            for (int i = 0; i < m_tmpFrameEvtList.Count; ++i)
            {
                if (m_tmpFrameEvtList[i].m_seq == seq)
                {
                    m_tmpFrameEvtList.RemoveAt(i);
                    break;
                }
            }
        }
        if (m_frameEvtList != null)
        {
            for (int i = 0; i < m_frameEvtList.Count; ++i)
            {
                if (m_frameEvtList[i].m_seq == seq)
                {
                    m_frameEvtList.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public void RemoveWaitUntilEvent(int seq)
    {
        if (m_tmpWaitUntilEvtList != null)
        {
            for (int i = 0; i < m_tmpWaitUntilEvtList.Count; i++)
            {
                if (m_tmpWaitUntilEvtList[i].m_seq == seq)
                {
                    m_tmpWaitUntilEvtList.RemoveAt(i);
                    break;
                }
            }
        }

        if (m_waitUntilEvtList != null)
        {
            for (int i = 0; i < m_waitUntilEvtList.Count; i++)
            {
                if (m_waitUntilEvtList[i].m_seq == seq)
                {
                    m_waitUntilEvtList.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public DateTime GetDateByUtcTime(int seconds)
    {
        DateTime dsStart = UTC_Start.AddSeconds(UtcNeedSeconds() * 1.0f);
        DateTime currentTime = dsStart.AddSeconds((int)seconds);

        return currentTime;
    }

    public int GetUtcTime(bool forceToday, int hour, int minute = 0, int sec = 0)
    {
        double nt = GetCurrentDoubleTime();
        int todaySec = hour * 60 * 60 + minute * 60 + sec;
        int offset = (int)((GetCurrentDoubleTime() + m_utc * 60 * 60) % (24 * 60 * 60));
        if (offset > todaySec && !forceToday)
        {
            nt += 24 * 60 * 60;
        }

        nt += (todaySec - offset);

        return (int)nt;
    }

    private void UpdateTimerEvent()
    {
        for (int timerLsIndex = 0; timerLsIndex < m_timerEventList.Count; timerLsIndex++)
        {
            TimerEvent timerEvent = m_timerEventList[timerLsIndex];
            if (Time.realtimeSinceStartup < timerEvent.m_destTime)
            {
                continue;
            }
            else
            {
                CallBack cb = timerEvent.m_callBack as CallBack;
                try
                {
                    cb();
                }
                catch (Exception e)
                {
                    CBuglyPlugin.ReportException("client log", "client_log", "[Exception]" + e.ToString());
                    //CNetSys.Instance.SendImportantLogWithBugly(true, "[Exception]" + e.ToString());
                }
                timerEvent.m_repeateTime--;
                if (timerEvent.m_repeateTime > 0)
                {
                    timerEvent.m_destTime += timerEvent.m_interval;
                }
                else
                {
                    m_timerEventList.RemoveAt(timerLsIndex);
                    timerLsIndex--;
                }
                
            }
        }

        for (int frameLsIndex = 0; frameLsIndex < m_frameEvtList.Count; frameLsIndex++)
        {
            FrameEvent frameEvt = m_frameEvtList[frameLsIndex];
            if (Time.frameCount < frameEvt.m_destFrame)
            {
                continue;
            }
            else
            {
                CallBack cb = frameEvt.m_callBack as CallBack;
                try
                {
                    cb();
                }
                catch (Exception e)
                {
                    CBuglyPlugin.ReportException("client log", "client_log", "[Exception]" + e.ToString());
                    //CNetSys.Instance.SendImportantLogWithBugly(true, "[Exception]" + e.ToString());
                }
                m_frameEvtList.RemoveAt(frameLsIndex);
                frameLsIndex--;
            }
        }

        for (int frameLsIndex = 0; frameLsIndex < m_waitUntilEvtList.Count; frameLsIndex++)
        {
            WaitUntilEvent waitEvt = m_waitUntilEvtList[frameLsIndex];
            if (waitEvt.m_waitUnitl())
            {
                try
                {
                    waitEvt.m_callBack();
                }
                catch (Exception)
                {
                    
                }
                m_waitUntilEvtList.RemoveAt(frameLsIndex);
                frameLsIndex--;
            }
        }
    }

}
