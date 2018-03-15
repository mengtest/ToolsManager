using UnityEngine;
using System.Collections;

public class LuaSystemRoot {
	public SystemBaseLua m_systemBaseLua = new SystemBaseLua();
	public string m_systemName = "";
	public string SystemName
	{
		set
		{
			m_systemName = value;
		}
		get
		{
			return m_systemName;
		}
	}

    public void InitByLua()
    {
        m_systemBaseLua.InitWithoutLuaFile(SystemName);
    }

    public void InitLuaData()
    {
        m_systemBaseLua.InitLuaData();
    }
	
	public  void Reset()
	{
		m_systemBaseLua.Reset();
	}
	public  void Release()
	{
		m_systemBaseLua.Release();
	}

    public void CallLuaFunc(string function_name,params object[] args)
    {
        m_systemBaseLua.CallLuaFunc(function_name, args);
    }

    #region get Lua Data
    public Hashtable GetLuaTableData(string funName,params object[] args)
	{
		return m_systemBaseLua.GetLuaTable(funName,args);
	}
	public double GetLuaDoubleData(string funName,params object[] args)
	{
		return m_systemBaseLua.GetLuaDouble(funName,args);
	}
	public string GetLuaStringData(string funName,params object[] args)
	{
		return m_systemBaseLua.GetLuaString(funName,args);
	}

	public bool GetLuaBoolData(string funName,params object[] args)
	{
		return m_systemBaseLua.GetLuaBool(funName,args);
	}


    public object GetLuaData(string fieldName)
    {
        return m_systemBaseLua.GetLuaField(fieldName);
    }


    #endregion get Lua Data

    public void SendNetRequest(string netName,GamePackage pack, NetMsgByNetNameCallBack dCallBack, NetMsgByNetNameCallBack failCallBack = null, bool canAbandon = true, bool is_cbk_after_error_window = false, bool needErrWin = true)
    {
        CNetSys.Instance.SendNetMsg(netName,(int)pack.cmd, pack,
        dCallBack, 
        false,
        failCallBack,
        canAbandon, is_cbk_after_error_window, null, needErrWin);
    }


    public void SetLuaNetRequest(string netName, int cmd, GamePackage luaBody, NetMsgCallBack dCallBack, NetMsgFailCallBack failCallBack = null, bool canAbandon = true, bool is_cbk_after_error_window = false, bool needErrWin = true)
    {

    }

    public void registerNetMsgDeal(string netName,int Cmd, NetMsgCallBack dCallBack)
    {
        CNetSys.Instance.RegisterMsgHandler(netName,Cmd, dCallBack);
    }

    protected void OnDestroy()
	{
	}
}
