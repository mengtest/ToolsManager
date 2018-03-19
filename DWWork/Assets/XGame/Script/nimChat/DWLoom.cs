﻿using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

/// <summary>
/// 工具类，方便在非UI线程更新UI
/// </summary>
public class DWLoom : MonoBehaviour
{
    public static int maxThreads = 8;
    static int numThreads;

    private static DWLoom _current;
    public static DWLoom Current
    {
        get
        {
            return _current;
        }
    }

    void Awake()
    {
        _current = this;
        initialized = true;
    }

    static bool initialized;

    public static void Initialize()
    {
        if (!initialized)
        {
            if (!Application.isPlaying)
                return;
            initialized = true;
            //var g = new GameObject("DWLoom");
            //_current = g.AddComponent<DWLoom>();
            GameObject gameRoot = GameObject.Find("_GameRoot");
            _current = EZFunTools.GetOrAddComponent<DWLoom>(gameRoot);
        }

    }

    private List<Action> _actions = new List<Action>();
    public struct DelayedQueueItem
    {
        public float time;
        public Action action;
    }
    private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

    List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

    public static void QueueOnMainThread(Action action)
    {
        if (initialized)
            QueueOnMainThread(action, 0f);
        else
            Debug.Log("dwloom is not init =============");
    }
    public static void QueueOnMainThread(Action action, float time)
    {
        if (time > 0)
        {
            lock (_current._delayed)
            {
                _current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
            }
        }
        else
        {
            lock (_current._actions)
            {
                _current._actions.Add(action);
            }
        }
    }

    public static Thread RunAsync(Action a)
    {
        Initialize();
        while (numThreads >= maxThreads)
        {
            Thread.Sleep(1);
        }
        Interlocked.Increment(ref numThreads);
        ThreadPool.QueueUserWorkItem(RunAction, a);
        return null;
    }

    private static void RunAction(object action)
    {
        try
        {
            ((Action)action)();
        }
        catch
        {
        }
        finally
        {
            Interlocked.Decrement(ref numThreads);
        }

    }


    void OnDisable()
    {
        if (_current == this)
        {
            _current = null;
            initialized = false;
        }
    }



    // Use this for initialization
    void Start()
    {

    }

    List<Action> _currentActions = new List<Action>();

    // Update is called once per frame
    void Update()
    {
        lock (_actions)
        {
            _currentActions.Clear();
            _currentActions.AddRange(_actions);
            _actions.Clear();
        }
        foreach (var a in _currentActions)
        {
            a();
        }
        lock (_delayed)
        {
            _currentDelayed.Clear();
            _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
            foreach (var item in _currentDelayed)
                _delayed.Remove(item);
        }
        foreach (var delayed in _currentDelayed)
        {
            delayed.action();
        }
    }
}
