//#define ENABLE_TIME_PROFILER

using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeProfiler
{
    private static Dictionary<string, TimeProfiler> m_sTimerMap = new Dictionary<string, TimeProfiler>();
    public double m_Time = 0.0f;
    private static DateTime UTC_Start = new DateTime(1970, 1, 1, 0, 0, 0, 0);
#if ENABLE_TIME_PROFILER
    private static double m_tempTime = 0.0f;
#endif
    private TimeProfiler(bool deltaTime)
    {
        if (deltaTime)
        {
            m_Time = 0.0;
        }
        else
        {
            m_Time = GetLocalTime();
        }
    }

    private static double GetLocalTime()
    {
        return ((DateTime.Now.ToUniversalTime() - UTC_Start).TotalMilliseconds) * 1f / 1000;
    }

    public float GetDeltaTime()
    {
        return (float)(GetLocalTime() - m_Time);
    }
    static public void BeginTimer(string name, bool deltaTime = false)
    {
#if ENABLE_TIME_PROFILER
        TimeProfiler timer = null;
        m_sTimerMap.TryGetValue(name, out timer);
        if (timer == null)
        {
            timer = new TimeProfiler(deltaTime);
            m_sTimerMap.Add(name, timer);
        }
        m_tempTime = GetLocalTime();
#endif
    }
    static public float EndTimer(string name, bool deltaTime = false)
    {
#if ENABLE_TIME_PROFILER
        TimeProfiler timer = null;
        m_sTimerMap.TryGetValue(name, out timer);
        if (timer == null)
        {
            return 0.0f;
        }

        if (deltaTime == false)
        {
            float ret = timer.GetDeltaTime();
            m_sTimerMap.Remove(name);
            return ret;
        }
        else
        {
            timer.m_Time += (GetLocalTime() - m_tempTime);
            return (float)timer.m_Time;
        }
#else
        return 0.0f;
#endif
    }
    static private int StampIndex = 0;
    static public void ResetStamp()
    {
        StampIndex = 0;
    }
    static public void Stamp(string tag="")
    {
#if ENABLE_TIME_PROFILER
        Debug.LogError(tag + " Stamp " + StampIndex + " " + Time.realtimeSinceStartup);
        StampIndex++;
#endif
    }

    static public void EndTimerAndLog(string name)
    {
#if ENABLE_TIME_PROFILER
        Debug.LogError("TimeProfiler: " + name + " " + TimeProfiler.EndTimer(name));
#endif
    }
    static public void LogTimer(string name)
    {
#if ENABLE_TIME_PROFILER
        TimeProfiler timer = null;
        m_sTimerMap.TryGetValue(name, out timer);
        if (timer != null)
        {
            Debug.LogError("TimeProfiler: " + name + " " + timer.m_Time);
        }
#endif
    }
}