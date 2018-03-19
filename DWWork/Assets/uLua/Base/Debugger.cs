using UnityEngine;
using System.Collections;

public static class Debugger
{
    public static void Log(string str, params object[] args)
    {
        Debug.LogFormat(str, args);
    }

    public static void LogWarning(string str, params object[] args)
    {
        Debug.LogWarningFormat(str, args);
    }

    public static void LogError(string str, params object[] args)
    {
        Debug.LogErrorFormat(str, args);
    }
}
