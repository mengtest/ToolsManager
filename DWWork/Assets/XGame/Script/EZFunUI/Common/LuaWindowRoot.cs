using UnityEngine;
using System.Collections;
using System;

[LuaWrap]
public class LuaWindowRoot : WindowRoot
{
    WindowBaseLua m_windowBaseLua = null;

    public void InitLua(string luaPath, bool isNeedCreate)
    {
        m_windowBaseLua = EZFunTools.GetOrAddComponent<WindowBaseLua>(gameObject);
        var arys = luaPath.Split('.');
        var luaName = luaPath;
        if (arys.Length > 0)
        {
            luaName = arys[arys.Length - 1];
        }
        m_windowBaseLua.InitLuaFile(luaPath, luaName, this, isNeedCreate);
        m_windowName = luaName;
    }

    public sealed override void InitWindow(bool open, int state, bool animated, object param = null)
    {
        m_windowBaseLua.InitParam(param);
		base.InitWindow(open, state, animated, param);
    }

	public sealed override void InitWindow(bool open, int state, object param)
	{
		m_windowBaseLua.InitWindow(open, state, param);
	}
		
    public sealed override void InitWindow(bool open = true, int state = 0)
    {
        m_windowBaseLua.InitWindow(open, state);
    }

    public void BaseIniwWindow(bool Open, int state)
    {
        base.InitWindow(Open, state);
    }

    protected override void OnFucus()
    {
        m_windowBaseLua.OnFocus();
    }

    protected override void PreCreateWindow()
    {
        m_windowBaseLua.PreCreateWindow();
        base.PreCreateWindow();
    }

    protected override void CreateWindow()
    {
        m_windowBaseLua.CreateWindow();
        base.CreateWindow();

    }

    public string GetName()
    {
        return this.name;
    }

    //public void BaseCreateWindow()
    //{
    //	base.BaseCreateWindow();
    //}

    public void TopBarCreateWindow()
    {
        base.CreateWindow();
    }

    protected override void HandleWidgetClick(GameObject gb)
    {
        if (!CheckBtnCanClick(gb))
        {
            return;
        }

        if (!CheckCanClick())
        {
            return;
        }
        m_windowBaseLua.SendWidgetClick(gb);
    }

    protected override void HandleWidgetDrag(Vector2 delta)
    {
        base.HandleWidgetDrag(delta);
        m_windowBaseLua.SendWidgetDrag(delta);
    }

    private bool CheckCanClick()
    {
        return true;
    }

    //public void BaseHandleWidgetClick(GameObject gb)
    //{
    //	base.BaseHandleWidgetClick(gb);
    //}

    public void TopBarHandleWidgetClick(GameObject gb)
    {
        base.HandleWidgetClick(gb);
    }

	public override void OnDestroy ()
	{
		m_windowBaseLua.OnDestroy ();
		base.OnDestroy ();
	}

    public override void SetScrollViewCanDrag(bool canDrag)
    {
        m_windowBaseLua.SetScrollViewCanDrag(canDrag);
    }

    protected override void HandleWidgetPressed(UIButtonMessage.PressObject pressObj)
    {
        base.HandleWidgetPressed(pressObj);
        m_windowBaseLua.SendWidgetPress(pressObj.m_GameObject, pressObj.m_IsPressed);
    }

    protected override void HandleWidgetRelease(UIButtonMessage.PressObject pressObj)
    {
        base.HandleWidgetRelease(pressObj);
        m_windowBaseLua.SendWidgetRelease(pressObj.m_GameObject, pressObj.m_IsPressed);
    }

    public void SetOpenWinFunction(string luaFunctionName)
    {
        if (m_windowBaseLua != null && WindowBaseLua.m_luaMgr != null && !string.IsNullOrEmpty(m_windowBaseLua.m_fileName))
        {
            EZFunWindowMgr.Instance.AddOpenWinFunction(m_currentWindowEnum, m_windowName, () =>
            {
                WindowBaseLua.m_luaMgr.CallLuaFunction(m_windowBaseLua.m_fileName + "." + luaFunctionName);
            });
        }
    }

}
