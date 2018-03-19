//#define WITHOUT_NETWORK

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

public delegate void EventSysCallBack(EEventType eventId, object param1, object param2);
public delegate void EventSysCallBack_(uint eventId, object param1, object param2);

// event parameter data define
public class EventParamData
{
	private EEventType m_eventId = 0;
	private object m_param1 = null;
	private object m_param2 = null;
	
	public EventParamData(EEventType eventId, object param1, object param2)
	{
		m_eventId = eventId;
		m_param1 = param1;
		m_param2 = param2;
	}

	public EEventType GetEventId() 	{ return m_eventId; }
	public object GetParam1()	{ return m_param1; }
	public object GetParam2()	{ return m_param2; }
}
[LuaWrap]
[RegisterSystem(typeof(EventSys), true)]
// Event system
public class EventSys :  TCoreSystem<EventSys>, IInitializeable, IUpdateable
{
	private static String ms_lock = "lock";
	
	private CDealerMap<int, CDealerCB> m_mapDealer = new CDealerMap<int, CDealerCB>();
	
	private Queue m_eventQueue = new Queue();

	private bool m_hasInit = false;

    private int m_mainThreadId = 0;

	public  void Init()
	{
		if(m_hasInit)
		{
			return;
		}
        m_mainThreadId = Thread.CurrentThread.ManagedThreadId;
		m_hasInit = true;
		m_eventQueue.Clear();
	}
    public void Release()
    {

    }

    public  void Update()
	{
		lock (ms_lock) 
		{
			while (m_eventQueue.Count > 0)
			{
				EventParamData data = m_eventQueue.Dequeue() as EventParamData;
				EEventType eventId = data.GetEventId();
				SendEvent(eventId, data.GetParam1(), data.GetParam2());
			}
		}
	}

	public void AddHandler(EEventType eventId, EventSysCallBack callBack)
	{
		lock (ms_lock)
		{
			m_mapDealer.AddHandle((int)eventId, new CDealerCB(callBack));
		}
	}

    public void RemoveHandleByEventType(EEventType evnetid, EventSysCallBack callBack)
    {
        lock(ms_lock)
        {
            m_mapDealer.RemoveHandleById((int)evnetid, new CDealerCB(callBack));
        }
    }

	public void RemoveHander(object target)
	{
		lock (ms_lock)
		{
			m_mapDealer.RemoveHandleByTarget(target);
		}
	}

    public void RemoveHandlerByType<T>()
    {
        lock(ms_lock)
        {
            m_mapDealer.RemoveHandleByType<T>();
        }
    }

	public void AddEvent(EEventType eventId, object param1=null, object param2=null)
	{
        var data = new EventParamData(eventId, param1, param2);
        lock (ms_lock) 
		{
			m_eventQueue.Enqueue(data);
		}
	}

	//此函数只能在主线程调用
	public void AddEventNow(EEventType eventId, object param1=null, object param2=null)
	{
        if (Thread.CurrentThread.ManagedThreadId != m_mainThreadId)
        {
            AddEvent(eventId, param1, param2);
        }
        else
        {
            SendEvent(eventId, param1, param2);
        }
    }

	private void SendEvent(EEventType eventId, object param1, object param2)
	{
		var list = m_mapDealer.GetDealer((int)eventId);
		if (list != null) 
		{
            for (int i = 0; i < list.Count; ++i)
            {
                var cb = list[i].cb as EventSysCallBack;
				cb(eventId, param1, param2);
			}
		}
	}
}