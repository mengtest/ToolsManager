using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LuaInterface;

public class WindowBaseLua : MonoBehaviour
{
    public static LuaScriptMgr m_luaMgr;

    static bool m_hasInitTool = false;
    public string m_fileName = "";

    private LuaTable m_luaObj = null;

    private WindowRoot m_luaWindowRoot;
    //static HashSet<string> m_hasDoFileDic = new HashSet<string>();

    private LuaFunction m_luaUpdate = null;
    private LuaFunction m_luaHandleClick = null;
    private LuaFunction m_luaHandleDrag = null;


    public void InitLuaFile(string fileName, string name, WindowRoot luaWindowRoot, bool isNeedCreate)
    {
        if (!m_hasInitTool)
        {
            m_hasInitTool = true;
        }

        m_fileName = name;
        TimeProfiler.BeginTimer("LoadLua Lua " + fileName);
        m_luaObj = LuaRootSys.Instance.RequireAndNew(fileName, luaWindowRoot) as LuaTable;
        TimeProfiler.EndTimerAndLog("LoadLua Lua " + fileName);
        m_luaWindowRoot = luaWindowRoot;
        if (m_luaObj != null)
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".BaseInit", m_luaObj, luaWindowRoot);
        }
        else
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".Init", luaWindowRoot);
        }
        //直接持有函数，提高性能
        m_luaUpdate = m_luaMgr.GetLuaFunction(m_fileName + ".Update");
        if (m_luaObj != null)
        {
            m_luaHandleClick = m_luaMgr.GetLuaFunction(m_fileName + ".BaseHandleWidgetClick");
        }
        else
        {
            m_luaHandleClick = m_luaMgr.GetLuaFunction(m_fileName + ".HandleWidgetClick");
        }

        if (m_luaObj != null)
        {
            m_luaHandleDrag = m_luaMgr.GetLuaFunction(m_fileName + ".BaseHandleWidgetDrag");
        }
        else
        {
            m_luaHandleDrag = m_luaMgr.GetLuaFunction(m_fileName + ".HandleWidgetDrag");
        }
    }

    public void InitWindow(bool open = true, int state = 0, object param = null)
    {
        if (m_luaObj != null)
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".BaseInitWindow", m_luaObj, open, state, param);
        }
        else
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".InitWindow", open, state, param);
        }
    }

    public void InitParam(object param)
    {
        if (m_luaObj != null)
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".InitParam", m_luaObj, param);
        }
        else
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".InitParam", param);
        }
    }

    public void OnFocus(bool open = true, int state = 0)
    {
        if (m_luaObj != null)
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".OnFocus", m_luaObj);
        }
        else
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".OnFocus");
        }
    }

    public void PreCreateWindow()
    {
        if (m_luaObj != null)
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".BasePreCreateWindow", m_luaObj);
        }
        else
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".PreCreateWindow");
        }
    }

    public void CreateWindow()
    {
        if (m_luaObj != null)
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".BaseCreateWindow", m_luaObj);
        }
        else
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".CreateWindow");
        }
        EventSys.Instance.AddHandler(EEventType.LuaWindowEv, (EEventType evendID, object p1, object p2) =>
        {
            if (m_luaObj != null)
            {
                m_luaMgr.CallLuaFunction(m_fileName + ".BaseHandleLuaWindowEv", m_luaObj, gameObject);
            }
            else
            {
                m_luaMgr.CallLuaFunction(m_fileName + ".HandleLuaWindowEv", p1, p2);
            }
        });
    }

    public void SendWidgetClick(GameObject gb)
    {
        if (!m_luaWindowRoot.CheckBtnCanClickForLua(gb, false))
        {
            return;
        }
        if (m_luaHandleClick != null)
        {
            if (m_luaObj != null)
            {
                m_luaHandleClick.Call(m_luaObj, gb);
            }
            else
            {
                m_luaHandleClick.Call(gb);
            }
        }
    }

    public void SendWidgetDrag(Vector2 delta) 
    {
        if (m_luaHandleClick != null)
        {
            if (m_luaObj != null)
            {
                m_luaHandleDrag.Call(m_luaObj, delta.x, delta.y);
            }
            else
            {
                m_luaHandleDrag.Call(delta.x, delta.y);
            }
        }
    }

    public void SendWidgetPress(GameObject gameObject, bool pressed)
    {
        if (!m_luaWindowRoot.CheckBtnCanClickForLua(gameObject, false))
        {
            return;
        }
        if (m_luaObj != null)
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".HandleWidgetPressed", m_luaObj, gameObject);
        }
        else
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".HandleWidgetPressed", gameObject);
        }
    }

    public void SendWidgetRelease(GameObject gameObject, bool pressed)
    {
        if (!m_luaWindowRoot.CheckBtnCanClickForLua(gameObject, false))
        {
            return;
        }
        if (m_luaObj != null)
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".HandleWidgetRelease", m_luaObj, gameObject);
        }
        else
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".HandleWidgetRelease", gameObject);
        }
    }

    private double m_timeSecond = 0;

    public void Update()
    {
        if (m_luaUpdate != null)
            m_luaUpdate.Call();
        m_timeSecond += Time.deltaTime;
        if (m_timeSecond >= 1)
        {
            m_timeSecond = 0;
            if (m_luaObj != null)
            {
                m_luaMgr.CallLuaFunction(m_fileName + ".UpdatePerSecond", m_luaObj);
            }
            else
            {
                m_luaMgr.CallLuaFunction(m_fileName + ".UpdatePerSecond");
            }
        }
    }

    public void OnDestroy()
    {
        if (m_luaMgr.lua != null)
        {

            if (m_luaObj != null)
            {
                m_luaMgr.CallLuaFunction(m_fileName + ".BaseOnDestroy", m_luaObj);
            }
            else
            {
                m_luaMgr.CallLuaFunction(m_fileName + ".OnDestroy");
            }
            if (m_luaUpdate != null)
            {
                m_luaUpdate.Release();
            }
            if (m_luaHandleClick != null)
            {
                m_luaHandleClick.Release();
            }
        }
    }

    public void SetScrollViewCanDrag(bool canDrag)
    {
        if (m_luaObj != null)
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".SetScrollViewCanDrag", m_luaObj, canDrag);
        }
        else
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".SetScrollViewCanDrag", canDrag);
        }
    }

    public static T GetValueFrom<T>(string luaFile, string methodName, bool localMethod, params object[] obArray)
    {
        object[] value = GetValueFromLua(luaFile, methodName, localMethod, obArray);
        if (value != null && value.Length > 0 && value[0] is T)
        {
            return (T)(value[0]);
        }
        else
        {
            return default(T);
        }
    }

    public static object[] GetValueFromLua(string luaFile, string methodName, bool localMethod, params object[] obArray)
    {
        if (localMethod)
        {
            return m_luaMgr.CallLuaFunction(luaFile + "." + methodName, obArray);
        }
        else
        {
            return m_luaMgr.CallLuaFunction(methodName, obArray);
        }
    }
}
