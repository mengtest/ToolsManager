/************************************************************
     File      : LuaObj.cs
     author    : shandong   shandong@ezfun.cn
     version   : 1.0
     date      : 2016-07-27 11:15:46.
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;
using LuaInterface;

public class LuaObj
{
    private LuaTable m_self;

    private string m_tableName;

    public LuaObj(LuaTable luaTable, string tableName)
    {
        this.m_self = luaTable;
        this.m_self.AddRef();
        this.m_tableName = tableName;
    }

    ~LuaObj()
    {
        if (m_self != null)
        {
            m_self.Release();
        }
    }

    public LuaTable GetLua()
    {
        return m_self;
    }

    #region  lua调用

    /// <summary>
    /// 这个方法需要注意: 很容易被系统认为前面的m_self是一个参数，后面的args当成一个参数了
    /// </summary>
    /// <param name="funcName"></param>
    /// <param name="args"></param>
    /// <returns></returns>
  //  public object[] CallFunction(string funcName, params object[] args)
  //  {
		//LuaFunction luaFunc = m_self.RawGetFunc (funcName) as LuaFunction;
  //      if (luaFunc != null)
  //      {
  //          return luaFunc.Call(m_self, args);
  //      }
  //      return null;
  //  }

    public object[] CallFunction(string funcName, object param1, object param2)
    {
        LuaFunction luaFunc = m_self.RawGetFunc(funcName) as LuaFunction;
        if (luaFunc != null)
        {
            return luaFunc.Call3Args(m_self, param1, param2);
        }
        return null;
    }

    public object[] CallFunction(string funcName, object param1)
    {
        LuaFunction luaFunc = m_self.RawGetFunc(funcName) as LuaFunction;
        if (luaFunc != null)
        {
            return luaFunc.Call2Args(m_self, param1);
        }
        return null;
    }

    public object[] CallFunction(string funcName)
    {
        LuaFunction luaFunc = m_self.RawGetFunc(funcName) as LuaFunction;
        if (luaFunc != null)
        {
            return luaFunc.Call1Args(m_self);
        }
        return null;
    }


    public object GetField(string fieldName)
    {
        return m_self[fieldName];
    }

    public object this[string field]
    {
        get
        {
            return m_self[field];
        }
        set
        {
            m_self[field] = value;
        }
    }

    #endregion


    #region 属性获取
    public Hashtable GetFieldTable(string fieldName)
    {
        object data = m_self[fieldName];
        if (data is LuaInterface.LuaTable)
        {
            LuaInterface.LuaTable ob = data as LuaInterface.LuaTable;
            return GetLuaTableData(ob);
        }
        else
        {
            Debug.LogError("no return data");
            return new Hashtable();
        }
    }

    /// <summary>
    /// 获取double属性
    /// </summary>
    private double GetLuaDouble(string fieldName, params object[] args)
    {
        object data = m_self[fieldName];
        if (data != null)
        {
            if (data is double)
            {
                return (double)data;
            }
            else
            {
                Debug.LogError("type is not double");
                return 0;
            }
        }
        else
        {
            Debug.LogError("no return data");
            return 0;
        }
    }

    /// <summary>
    /// 获取string
    /// </summary>
    private string GetLuaString(string fieldName)
    {
        object data = m_self[fieldName];
        if (data != null)
        {
            if (data is string)
            {
                return (string)data;
            }
            else
            {
                Debug.LogError("type is not string");
                return "";
            }
        }
        else
        {
            Debug.LogError("no return data");
            return "";
        }
    }

    /// <summary>
    /// 获取bool
    /// </summary>
    private bool GetLuaBool(string fieldName)
    {
        object data = m_self[fieldName];
        if (data != null)
        {
            if (data is bool)
            {
                return (bool)data;
            }
            else
            {
                Debug.LogError("type is not bool");
                return false;
            }
        }
        else
        {
            Debug.LogError("no return data");
            return false;
        }
    }

    private bool GetLuaBoolStatic(string fieldName)
    {
        object data = m_self[fieldName];
        if (data != null)
        {
            if (data is bool)
            {
                return (bool)data;
            }
            else
            {
                Debug.LogError("type is not bool");
                return false;
            }
        }
        else
        {
            Debug.LogError("no return data");
            return false;
        }
    }



    /// <summary>
    /// 将luatable转变成hashTable，还是比较消耗的
    /// </summary>
    /// <param name="ob"></param>
    /// <returns></returns>
    public static Hashtable GetLuaTableData(LuaInterface.LuaTable ob)
    {
        if (ob == null)
        {
            return null;
        }

        Hashtable rootTable = new Hashtable();
        IEnumerator itr = ob.Keys.GetEnumerator();
        while (itr.MoveNext())
        {
            var okey = itr.Current;
            var ovalue = ob[okey];
            var okeyStr = GetKeyString(okey);
            if (ovalue is LuaInterface.LuaTable)
            {
                rootTable.Add(okeyStr, GetLuaTableData(ovalue as LuaInterface.LuaTable));
            }
            else if (ovalue is double)
            {
                rootTable.Add(okeyStr, (double)ovalue);
            }
            else if (ovalue is bool)
            {
                rootTable.Add(okeyStr, (bool)ovalue);
            }
            else if (ovalue is string)
            {
                rootTable.Add(okeyStr, (string)ovalue);
            }
        }
        return rootTable;
    }

    static string GetKeyString(object key)
    {
        LuaInterface.LuaTable ob = key as LuaInterface.LuaTable;
        string tempStr = "";
        if (ob != null)
        {
            foreach (var k in ob.Keys)
            {
                tempStr += k.ToString();
            }
        }
        else
        {
            tempStr = key.ToString();
        }
        return tempStr;
    }
    #endregion
}
