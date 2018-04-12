//--------------------------------------------------------------------------------
//   File      : DWHttpConnection.cs
//   author    : guoliang
//   function   : http连接对象
//   date      : 2018-2-6
//   copyright : Copyright 2017 DW Inc.
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DWHttpConnection
{
    private Queue<HttpClientInstance> m_queSendPack;
    private Queue<WebSCPackage> m_queRecvPack;

    private Dictionary<string,HttpClientInstance> m_sendInstances;


    private HttpConnectRecvPack m_fnRecvCB;

    private DWHttpThread m_thSend;

    public NetCrypte m_crypte;

    private const int m_timeOutLimit = 5;
    private int m_timeOutTimeCount = 0;

    public DWHttpConnection(HttpConnectRecvPack recvCB)
    {
        m_fnRecvCB = recvCB;
        m_queRecvPack = new Queue<WebSCPackage>();
        m_queSendPack = new Queue<HttpClientInstance>();
        m_sendInstances = new Dictionary<string, HttpClientInstance>();
        m_crypte = new NetCrypte();
    }

    public void SendMsg(WebCSPackage pack)
    {
        UnityEngine.Debug.Log("http :send msg: " + pack.m_url);
        m_thSend.CheckIsRunning();
        lock (m_queSendPack)
        {
            float curTime = UnityEngine.Time.realtimeSinceStartup;
            var httpInstance = new HttpClientInstance(this,pack, curTime);
            if (!m_sendInstances.ContainsKey(pack.m_msgKey))
            {
                m_queSendPack.Enqueue(httpInstance);
                m_sendInstances.Add(pack.m_msgKey, httpInstance);
            }
        }
    }
    public void clearQueue()
    {
        lock (m_queRecvPack)
        {
            m_queRecvPack.Clear();
        }
        lock (m_queSendPack)
        {
            m_queSendPack.Clear();
        }
        lock (m_sendInstances)
        {
            m_sendInstances.Clear();
        }
    }

    public void EnqueueRespondMsg(WebSCPackage package)
    {
//        UnityEngine.Debug.LogError(package.m_msgKey + "is EnqueueRespondMsg");
        lock (m_queRecvPack)
        {
            m_queRecvPack.Enqueue(package);
        }

        lock (m_sendInstances)
        {
            HttpClientInstance ins = null;
            if (m_sendInstances.TryGetValue(package.m_msgKey,out ins))
            {
//                UnityEngine.Debug.LogError(package.m_msgKey + "is remove");
                m_sendInstances.Remove(package.m_msgKey);
            }
        }
    }

    public HttpClientInstance DequeueRequestMsg()
    {
        HttpClientInstance pkg = null;
        lock (m_queSendPack)
        {
            if (m_queSendPack.Count > 0)
            {
                pkg = m_queSendPack.Dequeue();
            }
        }
        return pkg;
    }


    public void Update()
    {
        BlockCheckUpdate();

        TriggerEvent();
    }

    void BlockCheckUpdate()
    {
        lock (m_sendInstances)
        {
            bool isTimeOut = false;
            double nowTime = UnityEngine.Time.realtimeSinceStartup;
            string[] ids = new string[m_sendInstances.Count];
            m_sendInstances.Keys.CopyTo(ids, 0);
            for (int i = 0; i < ids.Length; ++i)
            {
                var key = ids[i];
                if (m_sendInstances.ContainsKey(key))
                {
                    if (m_sendInstances[key] != null)
                    {
                        if(m_sendInstances[key].CheckIsExcept())
                        {
                            m_sendInstances[key].HanldeFailCB();
                        }
                        else if (m_sendInstances[key].CheckIsTimeOut(nowTime, -2))
                        {
                            UnityEngine.Debug.Log(m_sendInstances[key].m_csPackage.m_msgKey + " is timeout");
                            isTimeOut = true;
                            break;
                        }
                    }
                }
            }

            if(isTimeOut)
            {
                for (int i = 0; i < ids.Length; ++i)
                {
                    var key = ids[i];
                    if (m_sendInstances.ContainsKey(key))
                    {
                        if (m_sendInstances[key] != null)
                        {
                            m_sendInstances[key].HanldeFailCB();
                        }
                    }
                }

                //累计超时次数
                m_timeOutTimeCount++;
                if(m_timeOutTimeCount > m_timeOutLimit)
                {
                    m_timeOutTimeCount = 0;
                    ReStartWork();
                }
            }
        }
    }

    void TriggerEvent()
    {
        lock (m_queRecvPack)
        {
            while (m_queRecvPack.Count > 0)
            {
                WebSCPackage entry = m_queRecvPack.Dequeue();
                if (m_fnRecvCB != null)
                    m_fnRecvCB(entry);
            }
        }
    }

    public void SetTerminateFlag()
    {
        if (m_thSend != null)
        {
            m_thSend.SetTerminateFlag();
            m_thSend = null;
        }
    }

    public void WaitTermination()
    {
        if (m_thSend != null)
            m_thSend.WaitTermination();
    }

    public void CloseThread()
    {
        if (m_thSend != null)
        {
            m_thSend.Close();
        }
    }

    protected void CloseConnect()
    {
        SetTerminateFlag();
    }

    public void StartWork()
    {
        // todo
        m_thSend = new DWHttpThread(this);
        m_thSend.Run();
    }

    public void ReStartWork()
    {
        UnityEngine.Debug.LogError("http :m_thSend is restart work ");
        CloseThread();
        // todo
        m_thSend = new DWHttpThread(this);
        m_thSend.Run();
    }

}
