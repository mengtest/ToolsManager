/************************************************************
//     文件名      : LogicNetStateConnect.cs
//     功能描述    : 逻辑网络状态连接
//     负责人      : lezen   lezenzeng@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-07-11 09:45:58.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;
public class LogicNetStateConnect : NetStateConnectBase
{
    public LogicNetStateConnect(string key)
    {
        m_key = key;
    }
    private string m_ip;

    private int m_port;
    private CDWNetConnection m_logicConnection;
    private bool m_initiativePVPDisConnect = false;

    protected override void BaseInit()
    {
        m_logicConnection = m_netConnection as CDWNetConnection;
    }
    public void ConnectToServer(string ip, int port, ConnectCallBack cb = null)
    {
        m_isNetActiveWork = true;
        m_ip = ip;
        m_port = port;
        m_LoginedCallBack = (bool bConnected, UInt64 UID) =>
        {
            if (cb != null)
            {
                cb(bConnected);
            }
        };
        m_stNetState.InputData(CNetSys.DNetSysInputConnect);
    }

    public void ReconnectToPVPServer()
    {
       
    }

    //public override void ConnectServer(string strAddr, string strIp, int nPort, NetToken token, ConnectCallBack ConnetCallback, ConnectRecvProtoMsg<GamePackage> connectRecvPack, GamePackage cmsg)
    //{
    //    m_logicConnection.ConnectServer(strAddr, strIp, nPort, token, ConnetCallback, connectRecvPack, cmsg);
    //}

    protected override void SendMsg(GamePackage msg)
    {
        m_logicConnection.SendMsg(msg);
    }

    public override void NetStateInit()
    {
        m_stNetState = new TransferState<ENETSYSSTATE>(ENETSYSSTATE.None, new StateEqualityComparer());
        m_stNetState.addTransferAll(CNetSys.DNetSysInputConnect, ENETSYSSTATE.LogicConnecting, onConnect);
        m_stNetState.addTransfer(ENETSYSSTATE.LogicConnecting, CNetSys.DNetSysInputConnected, ENETSYSSTATE.LogicConnected, onConnected);
        m_stNetState.addTransferAll(CNetSys.DNetSysInputDisconnected, ENETSYSSTATE.Disconnected, onDisconnected);
        m_stNetState.m_transfer = (ENETSYSSTATE pre, TransferStateInput input,ENETSYSSTATE aft) =>
        {
            Debug.Log("NetSys:" + pre + " transfer to " + aft);
            return false;
        };
    }


    bool onConnect(ENETSYSSTATE curState, TransferStateInput input, ENETSYSSTATE dstState)
    {
        m_logicConnection.ConnectServer(m_ip, null, m_port, CNetSys.Instance.m_Token, (bool IsConnected) =>
        {
            if (CheckConnected())
            {
                m_initiativePVPDisConnect = false;
                m_stNetState.InputData(CNetSys.DNetSysInputConnected);
                EventSys.Instance.AddEvent(EEventType.ShowWaitWindow, false);
                if (m_isReconnect)
                {
                    m_isReconnect = false;
                    if (m_reconnectSuccessCB != null)
                        m_reconnectSuccessCB(NetName);

                }
            }
            else
            {
                m_stNetState.InputData(CNetSys.DNetSysInputDisconnected);
            }
        }, LogicRecvPack,null);
        return true;
    }
    bool onConnected(ENETSYSSTATE curState, TransferStateInput input, ENETSYSSTATE dstState)
    {
//        UInt64 UID = CNetSys.Instance.m_Token.m_UID;
        if (m_LoginedCallBack != null)
        {
            m_LoginedCallBack(true, 0);
        }
        RetryLoginSuccess();
        return true;
    }

    bool onDisconnected(ENETSYSSTATE curState, TransferStateInput input, ENETSYSSTATE dstState)
    {
        //CDebug.Log("sys net disconnected");
        if (m_LoginedCallBack != null)
        {
            m_LoginedCallBack(false, 0);
        }
        return true;
    }



    private void OnSvrDisConnect()
    {
#if UNITY_EDITOR
        NetDebug("OnPVPSvrDisConnect");
#endif

    }

    protected override void HandleNetErrInfo(GamePackage msg, NetMsgCallBack dCallBack, NetMsgSpecialFailCallBack failCallBack, bool canAbandon, bool is_cbk_after_error_window, bool needPopErrWindow)
    {
        base.HandleNetErrInfo(msg, dCallBack, failCallBack, canAbandon, is_cbk_after_error_window, needPopErrWindow);
        
    }
}
