/************************************************************
     File      : ComponentBaseLua.cs
     author    : xzs
     version   : 1.0
     date      : 2017年11月6日
     copyright : Copyright 2017 DW Inc.
**************************************************************/
using UnityEngine;
using System.Collections;
using LuaInterface;
public class ComponentBaseLua : BaseUI
{
	public static LuaScriptMgr m_luaMgr;
	public string m_fileName = "";

	private LuaTable m_luaObj = null;

	public void Init(string luaPath, bool is_instance)
	{
		m_luaMgr = LuaRootSys.Instance.LuaMgr;

		var arys = luaPath.Split('.');
		if (arys.Length > 0)
		{
			m_fileName = arys[arys.Length - 1];
		}

		if (is_instance)
		{
			m_luaObj = LuaRootSys.Instance.RequireAndNew(luaPath, gameObject) as LuaTable;
		}
		if (m_luaObj != null)
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".Init", m_luaObj, this);
		}
		else 
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".Init", this);
		}
	}

    public LuaTable GetLuaObj()
    {
        return m_luaObj;
    }

	void Start ()
	{
		if(m_luaMgr != null)
		{
			if (m_luaObj != null)
			{
				m_luaMgr.CallLuaFunction(m_fileName + ".Start", m_luaObj);
			}
			else 
			{
				m_luaMgr.CallLuaFunction(m_fileName + ".Start");
			}
		}
		else 
		{
			Debug.LogError("脚本不能直接挂在gameobject上，要AddComponent加上去！");
		}
	}

	void OnClick()
	{
		if (m_luaObj != null)
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnClick", m_luaObj);			
		}
		else 
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnClick");
		}
	}

	void OnDoubleClick()
	{
		if (m_luaObj != null)
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnDoubleClick", m_luaObj);
		}
		else 
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnDoubleClick");
		}
	}

	void OnHover(bool flag)
	{
		if (m_luaObj != null)
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnHover", m_luaObj, flag);
		}
		else 
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnHover", flag);
		}
	}

	void OnPress(bool flag)
	{
		if (m_luaObj != null)
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnPress", m_luaObj, flag);
		}
		else 
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnPress", flag);
		}
	}

	void OnSelect(bool flag)
	{
		if (m_luaObj != null)
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnSelect", m_luaObj, flag);
		}
		else 
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnSelect", flag);
		}
	}

	void OnScroll(float delta)
	{
		if (m_luaObj != null)
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnScroll", m_luaObj, delta);
		}
		else 
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnScroll", delta);
		}
	}
		
	void OnDrag(Vector2 delta)
	{
		if (m_luaObj != null)
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnDrag", m_luaObj, delta);
		}
		else 
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnDrag", delta);
		}
	}

	void OnDragStart()
	{
		if (m_luaObj != null)
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnDragStart", m_luaObj);
		}
		else 
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnDragStart");
		}
	}

	void OnDragOver(GameObject obj)
	{
		if (m_luaObj != null)
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnDragOver", m_luaObj, obj);
		}
		else 
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnDragOver", obj);
		}
	}

	void OnDragOut(GameObject obj)
	{
		if (m_luaObj != null)
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnDragOut", m_luaObj, obj);
		}
		else 
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnDragOut", obj);
		}
	}

	void OnDragEnd()
	{
		if (m_luaObj != null)
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnDragEnd", m_luaObj);
		}
		else 
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnDragEnd");
		}
	}

	void OnDrop(GameObject obj)
	{
		if (m_luaObj != null)
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnDrop", m_luaObj, obj);
		}
		else 
		{
			m_luaMgr.CallLuaFunction(m_fileName + ".OnDrop", obj);
		}
	}

    void OnDestory() 
    {
        if (m_luaObj != null)
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".OnDestroy", m_luaObj);
        }
        else
        {
            m_luaMgr.CallLuaFunction(m_fileName + ".OnDestroy");
        }
    }

}

