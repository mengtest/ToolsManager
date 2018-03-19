/************************************************************
//     文件名      : CoreSystem.cs
//     功能描述    : 系统接口类
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/07/01.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/
using System;

public enum SystemModuleType
{
    Initializeable = 0,
    Resetable,
    Updateable,
    FixedUpdateable,
    LateUpdateable,
    Count,
}

public enum SystemState
{
    Ready,          
    Shutdown,       
    Initializing,   
}

public enum SystemInitPriority
{
    Backgroud = 0,
    Startup = 1,
}

public interface IInitializeable
{
    void Init();
    void Release();
}

public interface IUpdateable
{
    void Update();
}

public interface IFixedUpdateable
{
    void FixedUpdate();
}

public interface ILateUpdateable
{
    void LateUpdate();
}

public interface IResetable
{
    void Reset();
}

public interface ICoreSystem
{
    int GetExecuteOrder(SystemModuleType type);
    void SetExecuteOrder(SystemModuleType type, int value);
}

public enum SystemLevel
{
    Persistent = 0,
    Loading,
    EnterGame,
}

[AttributeUsage(AttributeTargets.Class)]
public class RegisterSystemAttribute : Attribute
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="isInitNeed">是否进入就必须加载</param>
    public RegisterSystemAttribute(Type type, bool isInitNeed = false)
    {
        m_Type = type;
        m_isInit = isInitNeed;
    }
    public Type m_Type;

    public bool m_isInit;
}

public class TSingleton<T> 
    where T : class, new()
{
    private static T m_Instance;

    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new T();
            }
            return m_Instance;
        }
    }
}

public class TCoreSystem<T> : ICoreSystem
    where T : class, new()
{
    [global::System.ComponentModel.DefaultValue(SystemLevel.Persistent)]
    public SystemLevel m_SystemLevel
    {
        get; private set;
    }

    private int[] m_ExecuteOrders = new int[(int)SystemModuleType.Count];
    public static T Instance
    {
        get
        {
            return m_Instance;
        }
    }
    private static T m_Instance;
    public TCoreSystem()
    {
        Type t = typeof(T);
        m_Instance = this as T;
    }
    public int GetExecuteOrder(SystemModuleType type)
    {
        return m_ExecuteOrders[(int)type];
    }
    public void SetExecuteOrder(SystemModuleType type, int value)
    {
        m_ExecuteOrders[(int)type] = value;
    }
}


