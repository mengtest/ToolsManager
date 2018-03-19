/************************************************************
//     文件名      : EventMap.cs
//     功能描述    : 
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/07/12.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/
using System;
using System.Collections.Generic;

public class EventMap<TKey, TEvent>
{
    public Dictionary<TKey, FastEvent<TEvent>> m_EventMap = new Dictionary<TKey, FastEvent<TEvent>>();

    public void AddEvent(TKey key, TEvent evt)
    {
        FastEvent<TEvent> evtList = null;
        m_EventMap.TryGetValue(key, out evtList);
        if (evtList == null)
        {
            evtList = new FastEvent<TEvent>();
            m_EventMap.Add(key, evtList);
        }
        evtList.RegisterHandler(evt);
    }

    public void RemoveEvent(TKey key, TEvent evt)
    {
        if (m_EventMap.ContainsKey(key))
        {
            m_EventMap[key].UnRegisterHandler(evt);
        }
    }

    FastEvent<TEvent> GetEvent(TKey key)
    {
        FastEvent<TEvent> ret = null;
        m_EventMap.TryGetValue(key, out ret);
        return ret;
    }

    public void Invoke(params object[] args)
    {
        TKey key = (TKey)args[0];
        FastEvent<TEvent> evtList = null;
        m_EventMap.TryGetValue(key, out evtList);
        if (evtList != null)
        {
            evtList.Invoke(args);
        }
    }

    public void Remove(TKey key)
    {
        m_EventMap.Remove(key);
    }

    public void Clear()
    {
        m_EventMap.Clear();
    }
}
