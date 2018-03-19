/************************************************************
//     文件名      : DaemonSys.cs
//     功能描述    : 后台工作任务系统
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2016-11-28 09:49:52.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Threading;
using System;


public class DaemonSys
{
    #region single
    private static DaemonSys m_instance;

    public static DaemonSys Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new DaemonSys();
            }
            return m_instance;
        }
    }
    #endregion

    private Queue m_queue = new Queue();

    private System.Object lockObj = new System.Object();

    private Thread m_daemonThread;

    private volatile bool m_terminateFlagMutex = false;

    private DaemonSys()
    {
        m_daemonThread = new Thread(Execute);
        m_daemonThread.Priority = System.Threading.ThreadPriority.AboveNormal;
        m_daemonThread.Name = "DaemonSys_thread";
    }

    private Exception m_netEx;

    /// <summary>
    /// 将一个Action扔到异步线程中去执行，切记不要乱用啊，它是不同线程调用的！！
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public DaemonTask AddDaemonTask(Action action)
    {
        
        if (action == null)
        {
            return null;
        }
        var task = new DaemonTask(action);
        if (m_terminateFlagMutex)
        {
            return task;
        }
        lock (lockObj)
        {
            m_queue.Enqueue(task);
        }
        try
        {
            if (m_daemonThread.ThreadState == ThreadState.Unstarted)
            {
                m_daemonThread.Start();
            }
            else if (m_daemonThread.ThreadState == ThreadState.Stopped)
            {
                m_daemonThread = new Thread(Execute);
                m_daemonThread.Start();
            }
        }
        finally
        {

        }
       
        return task;
    }

    public bool isDone { get { return m_queue.Count == 0; } }

    private int m_sleepCount = 0;

    /// <summary>
    /// 设置线程终止
    /// </summary>
    public void SetTerminateFlag()
    {
        lock(this)
        {
            m_terminateFlagMutex = true;
        }
       
    }

    public void Close()
    {
        SetTerminateFlag();
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // 在ios 下面会crash
            bool test = false;
            if (test && m_daemonThread.ThreadState == ThreadState.Running)
            {
                m_daemonThread.Abort();
            }
        }
        else
        {
            m_daemonThread.Abort();
        }
    }

    private void Execute()
    {
        while (true)
        {
            object obj = null;
            lock(this)
            {
                if (m_terminateFlagMutex)
                {
                    break;
                }
            }
            lock (lockObj)
            {
                if (m_queue.Count > 0)
                {
                    obj = m_queue.Dequeue();
                }
            }
            if (obj is DaemonTask)
            {
                //有任务，线程执行
                var task = obj as DaemonTask;
                m_sleepCount = 0;
                try
                {
                    task.Execute();
                }
                catch (Exception ex)
                {
                    m_netEx = ex;
                }
            }
            else
            {
                //没有任务就存活10秒
                if (m_sleepCount < 1000)
                {
                    m_sleepCount++;
                    Thread.Sleep(10);
                }
                else
                {
                    //没有任务，线程退出
                    return;
                }
            }
        }
    }
}
