/************************************************************
//     文件名      : FastEvent.cs
//     功能描述    : 消息类，用于替代c#默认消息
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/07/11.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

//是否捕获回调函数异常
#if !UNITY_EDITOR
#   define CATCH_HANDLER_EXCEPTION
#endif

using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class FastEvent<DelegateType>
{
    public FastEvent()
    {
    }

    public void Invoke()
    {
        //遍历调用回调函数
        m_IsDuringInvocation ++;

        //在编辑器下不捕获异常，以便直接显示错误堆栈。
#if CATCH_HANDLER_EXCEPTION
        try
        {
#endif // #if CATCH_HANDLER_EXCEPTION
        for (int nHandler = 0; nHandler < m_Handlers.Count; ++nHandler)
        {
            Action handlerCallback = m_Handlers[nHandler] as Action;
            if (handlerCallback != null)
            {
                handlerCallback();
            }
        }
#if CATCH_HANDLER_EXCEPTION
        }
        catch (System.Exception exception)
        {
            LogException(exception);
        }
        finally
        {
#endif
        m_IsDuringInvocation --;
#if CATCH_HANDLER_EXCEPTION
        }
#endif
        RemoveUnuseHandles();
    }

    public void Invoke<T>(T arg1)
    {
        //遍历调用回调函数
        m_IsDuringInvocation ++;

        //在编辑器下不捕获异常，以便直接显示错误堆栈。
#if CATCH_HANDLER_EXCEPTION
        try
        {
#endif // #if CATCH_HANDLER_EXCEPTION
            for (int nHandler = 0; nHandler < m_Handlers.Count; ++nHandler)
            {
                Action<T> handlerCallback = m_Handlers[nHandler] as Action<T>;
                if (handlerCallback != null)
                {
                    handlerCallback(arg1);
                }
            }
#if CATCH_HANDLER_EXCEPTION
        }
        catch (System.Exception exception)
        {
            LogException(exception);
        }
        finally
        {
#endif
        m_IsDuringInvocation --;
#if CATCH_HANDLER_EXCEPTION
        }
#endif
        RemoveUnuseHandles();
    }

    public void Invoke<T1, T2>(T1 arg1, T2 arg2)
    {
        //遍历调用回调函数
        m_IsDuringInvocation ++;

        //在编辑器下不捕获异常，以便直接显示错误堆栈。
#if CATCH_HANDLER_EXCEPTION
        try
        {
#endif // #if CATCH_HANDLER_EXCEPTION
        for (int nHandler = 0; nHandler < m_Handlers.Count; ++nHandler)
        {
            Action<T1, T2> handlerCallback = m_Handlers[nHandler] as Action<T1, T2>;
            if (handlerCallback != null)
            {
                handlerCallback(arg1, arg2);
            }
        }
#if CATCH_HANDLER_EXCEPTION
        }
        catch (System.Exception exception)
        {
            LogException(exception);
        }
        finally
        {
#endif
        m_IsDuringInvocation--;
#if CATCH_HANDLER_EXCEPTION
        }
#endif
        RemoveUnuseHandles();
    }

    public void Invoke<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
    {
        //遍历调用回调函数
        m_IsDuringInvocation ++ ;

        //在编辑器下不捕获异常，以便直接显示错误堆栈。
#if CATCH_HANDLER_EXCEPTION
        try
        {
#endif // #if CATCH_HANDLER_EXCEPTION
        for (int nHandler = 0; nHandler < m_Handlers.Count; ++nHandler)
        {
            Action<T1, T2, T3> handlerCallback = m_Handlers[nHandler] as Action<T1, T2, T3>;
            if (handlerCallback != null)
            {
                handlerCallback(arg1, arg2, arg3);
            }
        }
#if CATCH_HANDLER_EXCEPTION
        }
        catch (System.Exception exception)
        {
            LogException(exception);
        }
        finally
        {
#endif
        m_IsDuringInvocation--;
#if CATCH_HANDLER_EXCEPTION
        }
#endif
        RemoveUnuseHandles();
    }

    public void Invoke<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        //遍历调用回调函数
        m_IsDuringInvocation++;

        //在编辑器下不捕获异常，以便直接显示错误堆栈。
#if CATCH_HANDLER_EXCEPTION
        try
        {
#endif // #if CATCH_HANDLER_EXCEPTION
        for (int nHandler = 0; nHandler < m_Handlers.Count; ++nHandler)
        {
            Action<T1, T2, T3, T4> handlerCallback = m_Handlers[nHandler] as Action<T1, T2, T3, T4>;
            if (handlerCallback != null)
            {
                handlerCallback(arg1, arg2, arg3, arg4);
            }
        }
#if CATCH_HANDLER_EXCEPTION
        }
        catch (System.Exception exception)
        {
            LogException(exception);
        }
        finally
        {
#endif
        m_IsDuringInvocation --;
#if CATCH_HANDLER_EXCEPTION
        }
#endif
        RemoveUnuseHandles();
    }

    private void LogException(System.Exception exception)
    {
        StringBuilder stringBuilder = new StringBuilder(512);
        stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.AppendLine("Caught an exception while calling FastEvent.Invoke().");
        stringBuilder.AppendLine("Arguments :");
        stringBuilder.AppendLine("Exception message :");
        stringBuilder.AppendLine(exception.Message);
        stringBuilder.AppendLine("Call stack :");
        stringBuilder.Append(exception.StackTrace);
        Debug.LogError(stringBuilder.ToString());
    }

    private void RemoveUnuseHandles()
    {
        //如果存在需要删除的回调函数
        if (m_IsDuringInvocation <= 0)
        {
            int nToRemoveHandlerCount = m_ListToRemoveHandlers.Count;
            if (nToRemoveHandlerCount > 0)
            {
                for (int nToRemoveHandler = 0; nToRemoveHandler < nToRemoveHandlerCount; ++nToRemoveHandler)
                {
                    var handlerCallback = m_ListToRemoveHandlers[nToRemoveHandler];
                    if (m_Handlers.Contains(handlerCallback))
                    {
                        m_Handlers.Remove(handlerCallback);
                    }
                }
                m_ListToRemoveHandlers.Clear();
            }
        }
    }

    /// <summary>
    /// 注册事件处理回调函数，如已重复则不会重复注册。
    /// </summary>
    /// <param name="handlerCallback">回调函数</param>
    public void RegisterHandler(DelegateType handlerCallback)
    {
        if (!m_Handlers.Contains(handlerCallback))
        {
            m_Handlers.Add(handlerCallback);
        }
    }

    /// <summary>
    /// 注销事件处理回调函数，如不存在则直接返回。
    /// </summary>
    /// <param name="handlerCallback">回调函数</param>
    public void UnRegisterHandler(DelegateType handlerCallback)
    {
        //调用过程中不允许修改回调函数字典
        if (m_IsDuringInvocation > 0)
        {
            //将需要删除的回调函数加入待删除列表，于下次调用 Event 前再删除。
            if (!m_ListToRemoveHandlers.Contains(handlerCallback))
            {
                m_ListToRemoveHandlers.Add(handlerCallback);
            }
        }
        else
        {
            if (m_Handlers.Contains(handlerCallback))
            {
                m_Handlers.Remove(handlerCallback);
            }
        }
    }

    protected List<DelegateType> m_Handlers = new List<DelegateType>();

    //是否正在调用（调用过程中不允许修改回调函数字典）
    protected int m_IsDuringInvocation = 0;

    //回调函数延迟删除列表
    protected List<DelegateType> m_ListToRemoveHandlers = new List<DelegateType>();
}