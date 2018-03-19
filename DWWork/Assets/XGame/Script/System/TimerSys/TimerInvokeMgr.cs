using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class TimerInvokeMgr
{
    Dictionary<string, int> m_nameSeq = new Dictionary<string, int>();

    private static TimerInvokeMgr m_instance;

    public static TimerInvokeMgr Instance
    {
        get { if (m_instance == null) m_instance = new TimerInvokeMgr(); return m_instance; }
    }

    private List<int> m_invokeListHandles = new List<int>();

    public void CancelInvoke(string invokeName)
    {
        if (m_nameSeq.ContainsKey(invokeName))
        {
            TimerSys.Instance.RemoveTimerEvt(m_nameSeq[invokeName]);
            m_invokeListHandles.Remove(m_nameSeq[invokeName]);
            m_nameSeq.Remove(invokeName);
        }
    }

    public void CancelInvoke()
    {
        m_nameSeq.Clear();
        for (int i = 0; i < m_invokeListHandles.Count; i++)
        {
            TimerSys.Instance.RemoveTimerEvt(m_invokeListHandles[i]);
        }
    }

    public int InvokeRepeating(TimerSys.CallBack callBack, float repetaTime, string invokeName = "")
    {
        int seq = TimerSys.Instance.AddRepeatTimerEvent(callBack, repetaTime, 0);
        m_invokeListHandles.Add(seq);
        if (!string.IsNullOrEmpty(invokeName))
        {
            if (!m_nameSeq.ContainsKey(invokeName))
            {
                m_nameSeq.Add(invokeName, seq);
            }
            else
            {
                TimerSys.Instance.RemoveTimerEvt(m_nameSeq[invokeName]);
                m_nameSeq[invokeName] = seq;
            }
        }
        return seq;
    }


    public int Invoke(TimerSys.CallBack callBack, float repetaTime, string invokeName = "")
    {
        int seq = TimerSys.Instance.AddTimerEventByLeftTime(callBack, repetaTime);
        m_invokeListHandles.Add(seq);
        if (!string.IsNullOrEmpty(invokeName))
        {
            if (!m_nameSeq.ContainsKey(invokeName))
            {
                m_nameSeq.Add(invokeName, seq);
            }
            else
            {
                TimerSys.Instance.RemoveTimerEvt(m_nameSeq[invokeName]);
                m_nameSeq[invokeName] = seq;
            }
        }
        return seq;
    }
}
