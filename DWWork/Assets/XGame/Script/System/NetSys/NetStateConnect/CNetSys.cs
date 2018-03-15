using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ezfun_resource;


public delegate void NetMsgByNetNameCallBack(string netName,GamePackage msg);
public delegate void ConnectByNetNameCallBack(string netName,bool isConnected);

[RegisterSystem(typeof(CNetSys), true)]
public class CNetSys : TCoreSystem<CNetSys>, IInitializeable, IResetable, IUpdateable
{
    public const string GAME_SVR_NAME = "GameLogicNet";

    public static CNetSys TInstance
    {
        get { return Instance; }
    }
    public static readonly TransferStateInput DNetSysInputDisconnected = new TransferStateInputEnum<ENNetSysInput>(ENNetSysInput.Disconnected);
    public static readonly TransferStateInput DNetSysInputSendMsg = new TransferStateInputMsg<CDealerCBOP>();
    public static readonly TransferStateInput DNetSysInputTick = new TransferStateInput();
    public static readonly TransferStateInput DNetSysInputForceReset = new TransferStateInput();
    public static readonly TransferStateInput DNetSysInputRecvMsg = new TransferStateInputMsg<GamePackage>();
    public static readonly TransferStateInput DNetSysInputUseOldToken = new TransferStateInput();
    public static readonly TransferStateInput DNetSysInputConnect = new TransferStateInputEnum<ENNetSysInput>(ENNetSysInput.LogicConnect);
    public static readonly TransferStateInput DNetSysInputConnected = new TransferStateInputEnum<ENNetSysInput>(ENNetSysInput.LogicConnected);


    //网络实例池
    private EZFunDictionary<string, LogicNetStateConnect> m_netStateConnectDic = new EZFunDictionary<string, LogicNetStateConnect>();

    //游戏服 网络连接
    private LogicNetStateConnect m_gameNetStateConnect = null;
    public LogicNetStateConnect GameNetStateConnect
    {
        get
        {
            if(m_gameNetStateConnect == null)
            {
                m_gameNetStateConnect = GetNetConnectInstance(GAME_SVR_NAME);
            }
            return m_gameNetStateConnect;
        }
    }

    //游戏服阻塞回调处理
    public BlockCallBack m_BlockCallBack
    {
        set
        {
            m_gameNetStateConnect.m_BlockCallBack = value;
        }
    }

    //  public bool m_isInWar = false;

    private HashSet<int> m_luaCmdSet = new HashSet<int>();

    public NetToken m_Token = new NetToken ();

    #region reflect test
    public string m_testStr = "null";
    public void TestRSet(string s)
    {
        m_testStr = s;
    }
    public void PrintR()
    {
        Debug.LogError("PrintR :" + m_testStr);
    }
    public string GetR()
    {
        return m_testStr;
    }

    #endregion

    public void Init()
    {
        //游戏服连接
        m_gameNetStateConnect = AddNetConnectInstance(GAME_SVR_NAME,false);

        SetEventListener();
    }

    //添加一个网络连接实例
    public LogicNetStateConnect AddNetConnectInstance(string netName,bool needSetBlockCB = false)
    {
        if (netName == null)
        {
            Debug.LogError("netName is null");
            return null;
        }

        if (!m_netStateConnectDic.ContainsKey(netName))
        {
            LogicNetStateConnect netStateConnect = new LogicNetStateConnect(netName);
            var netConnection = new CDWNetConnection(netName);
            netConnection.m_fnDisconnectedCB = () =>
            {
                netStateConnect.NetState.InputData(CNetSys.DNetSysInputDisconnected);
                //netStateConnect.ResetAutoConnect();
                //改由业务层发起重连
                EventSys.Instance.AddEvent(EEventType.NetResetAutoConnect, netName);
            };
            netStateConnect.Init(netConnection, Reconnect, CNetSys.Instance.IsLuaCMD, OnReconnectSuccess);

            if (needSetBlockCB)
            {
                netStateConnect.m_BlockCallBack = EZFunWindowMgr.BlockFunc;
            }


            m_netStateConnectDic.Add(netName, netStateConnect);
            return netStateConnect;
        }
        else
        {
            return m_netStateConnectDic[netName];
        }
    }
    //移除一个网络连接实例
    public void RemoveNetConnectInstance(string netName)
    {
        if (netName == null)
        {
            Debug.LogError("netName is null");
            return ;
        }

        if (!m_netStateConnectDic.ContainsKey(netName))
        {
            CloseConnect(netName);
            m_netStateConnectDic.Remove(netName);
        }
    }

    //通过网络连接名称获取网络连接实例
    public LogicNetStateConnect GetNetConnectInstance(string netName)
    {
        if(netName == null)
        {
            Debug.LogError("netName is null");
            return null;
        }
        LogicNetStateConnect netConnect = null;
        if (!m_netStateConnectDic.TryGetValue(netName,out netConnect))
        {
            Debug.LogError(netName + " is not find");
        }

        return netConnect;
    }

    public void Reset()
    {
        for(int i= 0,len = m_netStateConnectDic.Count;i < len; i ++)
        {
            CloseConnect(m_netStateConnectDic.GetKey(i));
        }
    }

    public void Update()
    {
        for (int i = 0, len = m_netStateConnectDic.Count; i < len; i++)
        {
            m_netStateConnectDic.GetValue(i).Update();
        }

        //        UpdateHeartBeatTime();
    }
    public void Release()
    {
        for (int i = 0, len = m_netStateConnectDic.Count; i < len; i++)
        {
            CloseConnect(m_netStateConnectDic.GetKey(i));
        }
    }

    private void SetEventListener()
    {
        EventSys.Instance.AddHandler(EEventType.ShowWaitWindow, (EEventType eventID, object p1, object p2) => {
           if(p1 != null)
            {
                bool open = (bool)p1;
                if(open)
                {
                    if (p2 is int)
                    {
                        //EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.wait_ui_window, RessType.RT_CommonWindow, true, (int)p2);
                        EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, true, (int)p2 * 10000+10, "wait_ui_window");
                    }
                    else
                    {
                        //EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.wait_ui_window, RessType.RT_CommonWindow, true, 2);
                        EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, true, 20000+10, "wait_ui_window");
                    }
                }
                else
                {
                    //EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.wait_ui_window, false);
                    EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window");
                }
            }
        });

    }

    #region  Net Core
    public void RegisterMsgHandler(string netName,int cmd, NetMsgCallBack cb)
    {
        LogicNetStateConnect netConnect = GetNetConnectInstance(netName);
        if (netConnect != null)
            netConnect.RegisterMsgHandler(cmd, cb);
    }

    public void RemoveByTarget(string netName,object target)
    {
        LogicNetStateConnect netConnect = GetNetConnectInstance(netName);
        if (netConnect != null)
            netConnect.RemoveHander(target);
    }

    /// <summary>
    /// 建立一个链接
    /// </summary>
    /// <param name="ip">IP</param>
    /// <param name="port">端口号</param>
    /// <param name="connectCallBack">链接结果回调</param>
    public void CreateConnect(string netName,string ip, int port, ConnectByNetNameCallBack connectCallBack, System.Action reqNetMsg)
    {
        LogicNetStateConnect netConnect = GetNetConnectInstance(netName);
        if(netConnect != null)
        {
             netConnect.ConnectToServer(ip, port, (bool isConnected)=> {
                if (isConnected)
                {
     //               m_isInWar = true;
                }
                connectCallBack(netName,isConnected);
            });
      
            if (reqNetMsg != null)
            {
                reqNetMsg();
            }
        }
       
    }

    /// <summary>
    /// 关闭链接
    /// </summary>
    public void CloseConnect(string netName)
    {
        LogicNetStateConnect netConnect = GetNetConnectInstance(netName);
        if (netConnect != null)
        {
            netConnect.DisConnect();
            netConnect.Clear();
        }
    }


    public void GiveUpAutoConnect(string netName)
    {
        LogicNetStateConnect netConnect = GetNetConnectInstance(netName);
        if (netConnect != null)
        {
            netConnect.GiveUpAutoConnect();
            netConnect.Clear();
        }
    }

    public void ResetAutoConnect(string netName)
    {
        LogicNetStateConnect netConnect = GetNetConnectInstance(netName);
        if (netConnect != null)
        {
            if (netConnect != null)
            {
                netConnect.ResetAutoConnect();
            }
        }

    }

    public void SendNetMsg(string netName, int cmd, GamePackage body, NetMsgByNetNameCallBack dCallBack, bool is_special_cbk, NetMsgByNetNameCallBack failCallBack,
                          bool canAbandon = false, bool is_cbk_after_error_window = false, CNetSendOption op = null, bool needPopErrWindow = true,
                          bool needCheckNetConnect = false, NetMsgFailCallBack notConnectCB = null, int seq = 0)
    {
        LogicNetStateConnect netConnect = GetNetConnectInstance(netName);
        if (netConnect != null)
        {
            netConnect.SendNetMsg(cmd, body, (GamePackage msg) =>
            {
                dCallBack(netName, msg);
            }, is_special_cbk, (GamePackage msg) =>
            {
                failCallBack(netName, msg);
            }, canAbandon, is_cbk_after_error_window, op, needPopErrWindow, needCheckNetConnect, notConnectCB, seq);
        }
    }
    #endregion


    #region reconnect
    //重新连接函数
    private void Reconnect(string netName)
    {
        LuaRootSys.Instance.NetReconnectToGame(netName);
    }


    //重连成功回调处理
    public void OnReconnectSuccess(string netName)
    {
        EventSys.Instance.AddEvent(EEventType.NetReconnectSuccess,netName);
    }

    #endregion

    #region lua cmd
    public void AddLuaCMD(int cmd)
    {
        lock (m_luaCmdSet)
        {
            if (!m_luaCmdSet.Contains(cmd))
            {
                m_luaCmdSet.Add(cmd);
            }
        }
    }

    public bool IsLuaCMD(int cmd)
    {
        lock (m_luaCmdSet)
        {
            return m_luaCmdSet.Contains(cmd);
        }
    }

    #endregion

    public void SendImportantLog(params string[] stringArray)
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            for (int i = 0; !Constants.RELEASE && i < stringArray.Length; i++)
            {
                Debug.LogError(stringArray[i]);
            }
            return;
        }
        else
        {
            for (int i = 0; i < stringArray.Length; i++)
            {
                Debug.LogError(stringArray[i]);
            }
        }

        if (stringArray == null || stringArray.Length == 0)
        {
            return;
        }

    }

    public void RemoveAllHandler(object target)
    {
        for (int i = 0, len = m_netStateConnectDic.Count; i < len; i++)
        {
            m_netStateConnectDic.GetValue(i).RemoveHander(target);
        }
    }

    public void RemoveHandlerByNetName(string netName,object target)
    {
        LogicNetStateConnect netConnect = null;
        if(m_netStateConnectDic.TryGetValue(netName,out netConnect))
        {
            netConnect.RemoveHander(target);
        }
    }
}
