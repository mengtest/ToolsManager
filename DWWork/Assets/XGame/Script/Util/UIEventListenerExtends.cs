using UnityEngine;
using System.Collections;
[LuaWrap]
public static class UILuaExtends  {

	public static void SetLuaOnDrag(UIEventListener lisnter, LuaInterface.LuaFunction luaFunc)
    {
        if (lisnter == null)
        {
            return;
        }
        if (luaFunc == null)
        {
            lisnter.onDrag = null;
        }
        lisnter.onDrag = (GameObject go, Vector2 delta) =>
        {
            luaFunc.Call2Args(go, delta);
        };
    }

    public static void SetLuaOnDragEnd(UIEventListener lisnter, LuaInterface.LuaFunction luaFunc)
    {
        if (lisnter == null)
        {
            return;
        }
        if (luaFunc == null)
        {
            lisnter.onDragEnd = null;
        }
        lisnter.onDragEnd = (GameObject go) =>
        {
            luaFunc.Call1Args(go);
        };
    }
}
