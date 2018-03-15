using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum CNetLogLevel : int
{
    Log = 100,
    LogWarning = 200,
    LogException = 300,
    LogError = 400,
}

public class NetLog
{
    private static int m_netLogLevel = 500;


    private static bool CheckCanSendLog(CNetLogLevel level)
    {
        return (int)level > m_netLogLevel;
    }

    public static void SetNetLogLevel(int netLogLevel)
    {
        m_netLogLevel = netLogLevel;
    }

    public static void Log(object obj)
    {
        if (CheckCanSendLog(CNetLogLevel.Log))
        {
            try
            {
                CNetSys.Instance.SendImportantLog("Log:\n" + obj.ToString());
            }
            catch (System.Exception)
            {

            }
        }
    }

    public static void LogError(object obj)
    {
        if (CheckCanSendLog(CNetLogLevel.LogError))
        {
            try
            {
                CNetSys.Instance.SendImportantLog("LogError:\n" + obj.ToString());
            }
            catch (System.Exception)
            {

            }
        }
    }

    public static void LogError(object message, UnityEngine.Object context)
    {
        if (CheckCanSendLog(CNetLogLevel.LogError))
        {
            try
            {

                CNetSys.Instance.SendImportantLog("LogError:\n" + "message :" + message.ToString() + "\n" + context.ToString());
            }
            catch (System.Exception)
            {

            }
        }
    }

    public static void LogException(Exception e)
    {
        if (CheckCanSendLog(CNetLogLevel.LogException))
        {
            try
            {
                CNetSys.Instance.SendImportantLog("LogException:\n" + e.StackTrace + " \n" + e.ToString());
            }
            catch (System.Exception)
            {

            }
        }
    }

    public static void LogWarning(object obj)
    {
        if (CheckCanSendLog(CNetLogLevel.LogWarning))
        {
            try
            {
                CNetSys.Instance.SendImportantLog("LogWarning:\n" + obj.ToString());
            }
            catch (System.Exception)
            {

            }
        }
    }

    public static void LogWarning(object message, UnityEngine.Object context)
    {
        if (CheckCanSendLog(CNetLogLevel.LogWarning))
        {
            try
            {
                CNetSys.Instance.SendImportantLog("LogWarning:\n" + "message :" + message.ToString() + "\n" + context.ToString());
            }
            catch (System.Exception)
            {

            }
        }
    }
}

