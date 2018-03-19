/************************************************************
     File      : CoroutineJobQueue.cs
     author    : blackzhou 
     version   : 1.0
     date      : 2016/07/01.
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CoroutineJobQueue
{
    private MonoBehaviour m_CoroutineObj = null;

    public delegate IEnumerator JobCallback(object jobParam);
    public enum EJobCoroutineState
    {
        NotRunning,
        Running,
        Stopping,
    }

    public CoroutineJobQueue(MonoBehaviour gameObject)
    {
        this.m_CoroutineObj = gameObject;
    }

    public class WaitToFinish : CustomYieldInstruction
    {
        CoroutineJobQueue jobQueue;
        public WaitToFinish(CoroutineJobQueue _jobQueue)
        {
            jobQueue = _jobQueue;
        }
        public override bool keepWaiting
        {
            get
            {
                if (jobQueue.m_jobCoroutineState == EJobCoroutineState.Running && jobQueue._jobQueue.Count > 0)
                {
                    return true;
                }
                return false;
            }
        }
    }

    public void StartJobCoroutine()
    {
        if (_jobCoroutineState == EJobCoroutineState.Running)
            return;
        switch (_jobCoroutineState)
        {
            case EJobCoroutineState.NotRunning:
                {
                    _jobCoroutineState = EJobCoroutineState.Running;
                    m_CoroutineObj.StartCoroutine(_JobProcessingCoroutine());
                    break;
                }
            case EJobCoroutineState.Stopping:
                {
                    _jobCoroutineState = EJobCoroutineState.Running;
                    break;
                }
            default:
                {
                    Debug.LogError("StartJobCoroutine error!");
                    break;
                }
        }
    }
    public void StopJobCoroutine()
    {
        switch (_jobCoroutineState)
        {
            case EJobCoroutineState.Running:
                {
                    _jobCoroutineState = EJobCoroutineState.Stopping;
                    break;
                }
            default:
                {
                    Debug.LogError("StopJobCoroutine Error!");
                    break;
                }
        }
    }
    public EJobCoroutineState GetJobCoroutineState()
    {
        return _jobCoroutineState;
    }
    public void PushJob(JobCallback callback, object param)
    {
        Job job = new Job(callback, param);
        _jobQueue.Enqueue(job);
    }
    public void ClearAllJobs()
    {
        _jobQueue.Clear();
    }
    IEnumerator _JobProcessingCoroutine()
    {
        while (_jobCoroutineState == EJobCoroutineState.Running)
        {
            if (_jobQueue.Count > 0)
            {
                Job job = _jobQueue.Dequeue();
                IEnumerator coroutine = job.Invoke();
                while (true)
                {
                    if (!coroutine.MoveNext())
                    {
                        break;
                    }

                    yield return coroutine.Current;
                }
            }
            else
            {
                yield return null;
            }
        }

        _jobCoroutineState = EJobCoroutineState.NotRunning;
    }
    protected class Job
    {
        public Job(JobCallback callback, object param)
        {
            _callback = callback;
            _param = param;
        }
        public IEnumerator Invoke()
        {
            return _callback(_param);
        }

        protected JobCallback _callback = null;
        protected object _param = null;
    }
    protected Queue<Job> _jobQueue = new Queue<Job>();
    protected EJobCoroutineState _jobCoroutineState = EJobCoroutineState.NotRunning;
    public EJobCoroutineState m_jobCoroutineState
    {
        get { return _jobCoroutineState; }
    }
}
