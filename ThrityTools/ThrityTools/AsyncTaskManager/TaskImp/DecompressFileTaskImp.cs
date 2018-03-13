/************************************************************
     File      : FileDecompressTaskImp.cs
     author    : guoliang
     function  : 文件解压工具
     version   : 1.0
     date      : 11/29/2016.
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/
using System.Collections;
using System;

namespace ThrityTools
{
    public class DecompressFileTaskImp : BaseTaskImp
    {
        public DecompressFileTaskImp(AsyncTaskWorkPlatform platform)
        {
            m_platform = platform;
        }

        public DecompressFileTaskImp() { }

        public int CurrentTaskDecompressSize
        {
            get
            {
                if (m_curWorkTask is DecompressFileTask)
                    return (m_curWorkTask as DecompressFileTask).CurrentDecompressSize;
                return 0;
            }
        }

        //任务入队
        protected override void Enqueue(BaseTask task)
        {
            if (task is DecompressFileTask)
            {
                m_taskQueue.Insert(m_taskQueue.Count, task);
            }
        }

        protected override void TaskFailProcess()
        {
            m_taskQueue.Clear();
            m_isFinish = true;

            if (m_failCB != null)
                m_failCB();

            if (m_thread != null && m_platform != AsyncTaskWorkPlatform.IOS)
                m_thread.Abort();
        }
    }
}
