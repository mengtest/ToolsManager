/************************************************************
//     文件名      : DaemonTask.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2016-11-28 09:49:29.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;

public class DaemonTask
{
    public Action m_task;

    private volatile bool m_isDone = false;

    public bool IsDone { get { return m_isDone; } }

    public DaemonTask(Action action)
    {
        m_isDone = false;
        m_task = action;
    }

    public void Execute()
    {
        m_task();
        EndExecute();
    }

    private void EndExecute()
    {
        m_isDone = true;
    }
}
