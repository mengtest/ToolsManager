/************************************************************
     File      : DownloadFileTaskImp.cs
     author    : guoliang
     version   : 1.0
     function  : 文件下载任务工具
     date      : 11/28/2016.
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/

using System.Collections;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ThrityTools
{
    public delegate bool NetConnectCheckCB();

    public class DownloadFileTaskImp : BaseTaskImp
    {
        protected int m_tryTimeLimit = 0;//默认0表示一直尝试，直到成功
        private int m_curTryTime = 0;

        public DownloadFileTaskImp() { }

        public DownloadFileTaskImp(AsyncTaskWorkPlatform platform, int tryTimeLimit = 0)
        {
            m_platform = platform;
            m_tryTimeLimit = tryTimeLimit;
            m_curTryTime = 0;
        }
        //本地网络连接检测回调
        protected NetConnectCheckCB m_netConnectCheckCB = null;

        public void SetNetConnectCheckCB(NetConnectCheckCB netConnectCheckCB = null)
        {
            m_netConnectCheckCB = netConnectCheckCB;
        }

        //线程工作主函数
        protected override void ThreadMainFunc()
        {
            while (!m_isFinish)
            {
                if (m_isPause)
                {
                    Thread.Sleep(10);
                }
                else
                {
                    //有网络连接检测回调且检测到断开状态
                    if (m_netConnectCheckCB != null && !m_netConnectCheckCB())
                        Thread.Sleep(10);
                    else
                        TaskFuncProcess();
                }
            }
        }

        //任务失败处理
        protected override void TaskFailProcess()
        {
            if (m_tryTimeLimit > 0)
            {
                if (m_curTryTime >= m_tryTimeLimit)//有设置尝试次数时，超过停止线程工作
                {
                    m_taskQueue.Clear();
                    m_isFinish = true;

                    if (m_failCB != null)
                        m_failCB();

                    if (m_thread != null && m_platform != AsyncTaskWorkPlatform.IOS)
                        m_thread.Abort();

                    return;
                }
                else
                {
                    m_curTryTime++;
                }
            }
            //任务重新入队
            Enqueue(m_curWorkTask);
        }

        //任务入队
        protected override void Enqueue(BaseTask task)
        {
            if (task is DownloadBigFileTask || task is DownloadSmallFileTask)
            {
                m_taskQueue.Insert(m_taskQueue.Count, task);
            }
        }
    }
}
