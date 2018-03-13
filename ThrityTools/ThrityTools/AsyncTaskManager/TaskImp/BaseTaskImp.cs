/************************************************************
     File      : BaseTaskImp.cs
     author    : guoliang
     function  : 任务工具基类
     version   : 1.0
     date      : 11/29/2016.
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/

using System.Collections;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ThrityTools
{
    public class BaseTaskImp
    {
        //工作平台
        protected AsyncTaskWorkPlatform m_platform = AsyncTaskWorkPlatform.Window;
        //任务队列
        protected List<BaseTask> m_taskQueue = new List<BaseTask>();
        //是否暂停
        protected bool m_isPause = false;
        //是否完成
        protected bool m_isFinish = false;
        public bool IsFinish
        {
            get { return m_isFinish; }
        }
        //工作线程
        protected Thread m_thread;
        //当前工作任务
        protected BaseTask m_curWorkTask = null;
        //所有任务执行成功回调
        protected Action m_successCB = null;
        //所有任务执行失败回调
        protected Action m_failCB = null;

        public BaseTaskImp() { }

        public BaseTaskImp(AsyncTaskWorkPlatform platform)
        {
            m_platform = platform;
        }

        public virtual void Init(Action successCB = null, Action failCB = null)
        {
            m_isPause = false;
            m_isFinish = false;
            m_successCB = successCB;
            m_failCB = failCB;
        }

        //开始工作
        public virtual void Start()
        {
            m_thread = new Thread(new ThreadStart(ThreadMainFunc));
            m_thread.Start();
        }

        //线程工作主函数
        protected virtual void ThreadMainFunc()
        {
            while (!m_isFinish)
            {
                if (m_isPause)
                    Thread.Sleep(10);
                else
                    TaskFuncProcess();
            }
        }

        //任务功能处理
        protected virtual void TaskFuncProcess()
        {
            lock (m_taskQueue)
            {
                if (m_taskQueue.Count > 0)
                {
                    m_curWorkTask = Dequeue();
                    m_curWorkTask.WorkStart();

                    while (m_curWorkTask.m_workStatus == TaskWorkStatus.Working) ;

                    if (m_curWorkTask.m_workStatus == TaskWorkStatus.Fail)
                        TaskFailProcess();

                    m_curWorkTask = null;
                }
                else
                {
                    m_isFinish = true;

                    if (m_successCB != null)
                        m_successCB();
                    if (m_thread != null && m_platform != AsyncTaskWorkPlatform.IOS)
                        m_thread.Abort();
                }
            }
        }
        //单个任务失败处理时
        protected virtual void TaskFailProcess()
        {
        }

        //任务出队
        protected BaseTask Dequeue()
        {
            BaseTask task = null;
            if (m_taskQueue.Count > 0)
            {
                task = m_taskQueue[0];
                m_taskQueue.RemoveAt(0);
            }
            return task;
        }

        //任务入队
        protected virtual void Enqueue(BaseTask task)
        {
            if (task != null)
            {
                m_taskQueue.Insert(m_taskQueue.Count, task);
            }
        }

        //暂停工作
        public virtual void Pause()
        {
            m_isPause = true;

            if (m_curWorkTask != null)
                m_curWorkTask.Pasue();
        }

        //恢复工作
        public virtual void Resume()
        {
            m_isPause = false;

            if (m_curWorkTask != null)
                m_curWorkTask.Resume();
        }

        //添加一个任务
        public void AddTask(BaseTask task)
        {
            lock (m_taskQueue)
            {
                Enqueue(task);
            }
        }

        //添加一组任务
        public void AddTask(List<BaseTask> taskList)
        {
            lock (m_taskQueue)
            {
                for (int i = 0; i < taskList.Count; i++)
                {
                    Enqueue(taskList[i]);
                }
            }
        }

        //删除一个任务
        public void DelTask(BaseTask task)
        {
            lock (m_taskQueue)
            {
                m_taskQueue.Remove(task);
            }
        }

        //关闭线程
        public void Abort()
        {
            m_isFinish = true;
            if (m_thread != null && m_platform != AsyncTaskWorkPlatform.IOS)
                m_thread.Abort();
        }

        //主动提前结束工作
        public void FinishInAdvance()
        {
            m_isFinish = true;
            lock (m_taskQueue)
            {
                m_taskQueue.Clear();
            }
        }
    }
}
