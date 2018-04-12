//--------------------------------------------------------------------------------
//   File      : DWHttpThread.cs
//   author    : guoliang
//   function   : http连接工作线程
//   date      : 2018-2-6
//   copyright : Copyright 2017 DW Inc.
//--------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net;
using System.IO;
using System.Text;


public class DWHttpThread
{
    private Thread m_thread;
    private volatile bool m_terminateFlag;
    private System.Object m_terminateFlagMutex;
    protected DWHttpConnection m_httpConnection;
    protected int m_seq;

    public void Run()
    {
        m_thread.Start(this);
    }

    public bool CheckIsRunning()
    {
//        Debug.LogError("m_thread.ThreadState " + m_thread.ThreadState);
        return m_thread.ThreadState == ThreadState.Running || m_thread.ThreadState == ThreadState.WaitSleepJoin;
    }

    public void Close()
    {
        SetTerminateFlag();
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            //
        }
        else
        {
            Debug.LogError("DWHttpThread abort");
            m_thread.Abort();
        }
    }

    protected static void ThreadProc(object obj)
    {
        DWHttpThread me = (DWHttpThread)obj;
        me.Main();
    }

    protected virtual void Main()
    {
        while (!IsTerminateFlagSet())
        {
            bool sleep = true;
            HttpClientInstance clientIns = null;

            clientIns = m_httpConnection.DequeueRequestMsg();
            if (clientIns != null)
            {
                sleep = false;
                UnityEngine.Debug.Log(DateTime.Now.TimeOfDay.ToString() + ":http send pkg");
                try
                {
                    clientIns.RequestSync();
                }
                catch (IOException e)
                {
                    Debug.LogError("HttpThread.SenderThread Main " + e.Message);
                    Debug.LogError("HttpThread.SenderThread Main " + e.StackTrace);
                    Debug.LogError("HttpThread.SenderThread Main" + e.InnerException.Message);
                }
            }
            if (sleep)
            {
                Thread.Sleep(10);
            }
        }
    }

    public void WaitTermination()
    {
        m_thread.Join();
    }

    public void SetTerminateFlag()
    {
        lock (m_terminateFlagMutex)
        {
            m_terminateFlag = true;
        }
    }

    protected bool IsTerminateFlagSet()
    {
        lock (m_terminateFlagMutex)
        {
            return m_terminateFlag;
        }
    }

    public DWHttpThread(DWHttpConnection connection)
    {
        m_thread = new Thread(ThreadProc);
        m_terminateFlag = false;
        m_httpConnection = connection;
        m_terminateFlagMutex = new System.Object();
    }
}
