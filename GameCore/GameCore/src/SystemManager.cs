using System;
using System.Collections.Generic;
using System.Reflection;

public class SystemManager : TSingleton<SystemManager>
{
    private List<ICoreSystem> m_CoreSystems = new List<ICoreSystem>();
    private List<IInitializeable> m_Initializeables = new List<IInitializeable>();
    private List<IUpdateable> m_Updateables = new List<IUpdateable>();
    private List<IFixedUpdateable> m_FixedUpdateables = new List<IFixedUpdateable>();
    private List<ILateUpdateable> m_LateUpdateables = new List<ILateUpdateable>();
    private List<IResetable> m_Resetables = new List<IResetable>();
    public FastEvent<Action> m_OnModifySystemModuleExecuteOrder = new FastEvent<Action>();
    private List<Action> m_UpdateList = new List<Action>();

    public ICoreSystem AddSystem(ICoreSystem sys)
    {
        m_CoreSystems.Add(sys);

        IInitializeable initializeable = (sys as IInitializeable);
        if (initializeable != null)
        {
            m_Initializeables.Add(initializeable);
        }

        IUpdateable updateable = (sys as IUpdateable);
        if (updateable != null)
        {
            m_Updateables.Add(updateable);
        }

        ILateUpdateable lateUpdateable = (sys as ILateUpdateable);
        if (lateUpdateable != null)
        {
            m_LateUpdateables.Add(lateUpdateable);
        }

        IFixedUpdateable fixedUpdateable = (sys as IFixedUpdateable);
        if (fixedUpdateable != null)
        {
            m_FixedUpdateables.Add(fixedUpdateable);
        }

        IResetable resetable = (sys as IResetable);
        if (resetable != null)
        {
            m_Resetables.Add(resetable);
        }

        return sys;
    }

    public void ModifySystemModuleExecuteOrder()
    {
        m_OnModifySystemModuleExecuteOrder.Invoke();
        //TableLoader.Instance.SetExecuteOrder(SystemModuleType.Initializeable, 0);
        //EventSys.Instance.SetExecuteOrder(SystemModuleType.Initializeable, 1);

        m_Initializeables.Sort((IInitializeable a1, IInitializeable a2) =>
        {
            ICoreSystem s1 = a1 as ICoreSystem;
            ICoreSystem s2 = a2 as ICoreSystem;
            return s1.GetExecuteOrder(SystemModuleType.Initializeable) - s2.GetExecuteOrder(SystemModuleType.Initializeable);
        });

        m_Resetables.Sort((IResetable a1, IResetable a2) =>
        {
            ICoreSystem s1 = a1 as ICoreSystem;
            ICoreSystem s2 = a2 as ICoreSystem;
            return s1.GetExecuteOrder(SystemModuleType.Resetable) - s2.GetExecuteOrder(SystemModuleType.Resetable);
        });

        m_Updateables.Sort((IUpdateable a1, IUpdateable a2) =>
        {
            ICoreSystem s1 = a1 as ICoreSystem;
            ICoreSystem s2 = a2 as ICoreSystem;
            return s1.GetExecuteOrder(SystemModuleType.Updateable) - s2.GetExecuteOrder(SystemModuleType.Updateable);
        });

        m_LateUpdateables.Sort((ILateUpdateable a1, ILateUpdateable a2) =>
        {
            ICoreSystem s1 = a1 as ICoreSystem;
            ICoreSystem s2 = a2 as ICoreSystem;
            return s1.GetExecuteOrder(SystemModuleType.LateUpdateable) - s2.GetExecuteOrder(SystemModuleType.LateUpdateable);
        });

        m_FixedUpdateables.Sort((IFixedUpdateable a1, IFixedUpdateable a2) =>
        {
            ICoreSystem s1 = a1 as ICoreSystem;
            ICoreSystem s2 = a2 as ICoreSystem;
            return s1.GetExecuteOrder(SystemModuleType.FixedUpdateable) - s2.GetExecuteOrder(SystemModuleType.FixedUpdateable);
        });
    }

    public void OnRelease()
    {
        for (int i = 0; i < m_Initializeables.Count; ++i)
        {
            m_Initializeables[i].Release();
        }
        m_UpdateList.Clear();
    }

    public void LoadingSystem(bool isPreSys)
    {
        Assembly a = Assembly.GetCallingAssembly();
        Type[] types = a.GetTypes();
        Type genericT = typeof(TCoreSystem<>);
        Type systemT = typeof(ICoreSystem);
        for (int i = 0; i < types.Length; ++i)
        {
            Type type = types[i];
            if (type.GetInterface("ICoreSystem") != null)
            {
                RegisterSystemAttribute attr = Attribute.GetCustomAttribute(type, typeof(RegisterSystemAttribute)) as RegisterSystemAttribute;
                if (attr != null && attr.m_isInit == isPreSys)
                {
                    ICoreSystem system = Activator.CreateInstance(attr.m_Type) as ICoreSystem;
                    AddSystem(system);
                }
            }
        }
    }

    public void OnInit(bool isPreSys)
    {
        for (int i = 0; i < m_Initializeables.Count; ++i)
        {
            Type type = m_Initializeables[i].GetType();
            RegisterSystemAttribute attr = Attribute.GetCustomAttribute(type, typeof(RegisterSystemAttribute)) as RegisterSystemAttribute;
            if (attr != null)
            {
                if (attr.m_isInit == isPreSys)
                {
                    TimeProfiler.BeginTimer(attr.m_Type.Name + ".Init");
                    m_Initializeables[i].Init();
                    TimeProfiler.EndTimerAndLog(attr.m_Type.Name + ".Init");
                }
            }
        }
    }

    public void OnReset()
    {
        for (int i = 0; i < m_Resetables.Count; ++i)
        {
            m_Resetables[i].Reset();
        }
    }


    public void OnFixedUpdate()
    {
        for (int i = 0; i < m_FixedUpdateables.Count; i++)
        {
            m_FixedUpdateables[i].FixedUpdate();
        }
    }

    public void OnUpdate()
    {
        for (int i = 0; i < m_Updateables.Count; i++)
        {
            m_Updateables[i].Update();
        }

        for (int i = 0; i < m_UpdateList.Count; i++)
        {
            m_UpdateList[i]();
        }
    }

    public void OnLateUpdate()
    {
        for (int i = 0; i < m_LateUpdateables.Count; i++)
        {
            m_LateUpdateables[i].LateUpdate();
        }
    }

    public void RegisterUpdate(Action callBack)
    {
        if (!m_UpdateList.Contains(callBack))
        {
            m_UpdateList.Add(callBack);
        }
    }

    public void UnRegisterUpdate(Action callBack)
    {
        m_UpdateList.Remove(callBack);
    }
}
