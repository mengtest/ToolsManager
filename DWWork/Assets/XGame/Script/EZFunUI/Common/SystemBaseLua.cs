using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SystemBaseLua
{
    string m_fileName = "";

    public void InitWithoutLuaFile(string fullPahtfileName, string typeName = "")
    {
        m_fileName = fullPahtfileName;
        if (!string.IsNullOrEmpty(typeName))
        {
            m_fileName = typeName;
        }
    }

    public void InitLuaData()
    {
        LuaRootSys.Instance.LuaMgr.CallLuaFunction(m_fileName + ".InitLuaData");
    }

    public void Reset()
    {
        LuaRootSys.Instance.LuaMgr.CallLuaFunction(m_fileName + ".Reset");
    }
    public void Release()
    {
        LuaRootSys.Instance.LuaMgr.CallLuaFunction(m_fileName + ".Release");
    }
    public void CallLuaFunc(string function_name, params object[] args)
    {
        LuaRootSys.Instance.LuaMgr.CallLuaFunction(m_fileName + "." + function_name, args);
    }

    /// <summary>
    /// 获取lua层table数据   仅支持一个返回数据  用时需谨慎
    /// </summary>

    public Hashtable GetLuaTable(string functionName, params object[] args)
    {
        object[] data = LuaRootSys.Instance.LuaMgr.CallLuaFunction(m_fileName + "." + functionName, args);
        if (data == null)
        {
            Debug.LogError("GetLuaTable data is null!");
            return new Hashtable();
        }
        if (data.Length > 0)
        {
            LuaInterface.LuaTable ob = data[0] as LuaInterface.LuaTable;
            return GetLuaTableData(ob);
        }
        else
        {
            Debug.LogError("no return data");
            return new Hashtable();
        }
    }

    /// <summary>
    /// 获取lua层double数据(int ,long ,float, double)   仅支持一个返回数据  用时需谨慎
    /// </summary>
    public double GetLuaDouble(string functionName, params object[] args)
    {
        object[] data = LuaRootSys.Instance.LuaMgr.CallLuaFunction(m_fileName + "." + functionName, args);
        if (data == null)
        {
            Debug.LogError("GetLuaDouble data is null!");
            return 0;
        }
        if (data.Length > 0)
        {
            if (data[0] is double)
            {
                return (double)data[0];
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
    /// 获取lua层string数据   仅支持一个返回数据  用时需谨慎
    /// </summary>
    public string GetLuaString(string functionName, params object[] args)
    {
        object[] data = LuaRootSys.Instance.LuaMgr.CallLuaFunction(m_fileName + "." + functionName, args);
        if (data == null)
        {
            Debug.LogError("GetLuaString data is null!");
            return "";
        }
        if (data.Length > 0)
        {
            if (data[0] is string)
            {
                return (string)data[0];
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
    /// 获取lua层bool数据   仅支持一个返回数据  用时需谨慎
    /// </summary>
    public bool GetLuaBool(string functionName, params object[] args)
    {
        object[] data = LuaRootSys.Instance.LuaMgr.CallLuaFunction(m_fileName + "." + functionName, args);
        if (data == null)
        {
            Debug.LogError("GetLuaBool data is null!");
            return false;
        }
        if (data.Length > 0)
        {
            if (data[0] is bool)
            {
                return (bool)data[0];
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

    public object GetLuaField(string fieldName)
    {
        var table = LuaRootSys.Instance.LuaMgr.GetLuaTable(m_fileName);
        object value = null;
        if (table != null)
        {
            value = table[fieldName];
            table.Release();
        }
        return value;
    }

    static public bool GetLuaBoolStatic(string functionPath, params object[] args)
    {
        object[] data = WindowBaseLua.m_luaMgr.CallLuaFunction(functionPath, args);
        if (data == null)
        {
            Debug.LogError("GetLuaBoolStatic data is null!");
            return false;
        }
        if (data.Length > 0)
        {
            if (data[0] is bool)
            {
                return (bool)data[0];
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



    //luatable 数据 转换为哈希
    static public Hashtable GetLuaTableData(LuaInterface.LuaTable ob)
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

}

