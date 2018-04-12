using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Text.RegularExpressions;


public delegate void NetLoginedCallBack(bool bConnected, UInt64 UID);

public delegate void NetMsgCallBack(GamePackage msg);

public delegate void BlockCallBack(int preState, TransferStateInput input,int curState);

public delegate void NetMsgSPCallBackLua(int err);

public delegate void NetMsgSpecialFailCallBack(GamePackage msg);

public delegate void NetMsgFailCallBack();

public delegate void ConnectRecvProtoMsg<SCT>(SCT msg);

public delegate void HttpConnectRecvPack(WebSCPackage pack);

public class GamePackage
{
    public int areaID;
    public int cmd;
    public int seq;
    public int errno;
    public bool needLockScreen = true;
    public byte[] body;
    public GamePackage()
    {

    }
}

public class WebCSPackage
{
    public string m_url;
    public string m_param;
    public bool m_isPost;
    public string m_msgKey;
    public int m_seq;
    public int m_timeOut = 8;
    public Action<GamePackage> m_sucCB;
    public WebCSPackage()
    {
        
    }
}

public class WebSCPackage
{
    public string m_msgKey;
    public int m_seq;
    public GamePackage m_pack;
    public Action<GamePackage> m_sucCB;
    public WebSCPackage()
    {

    }
    public WebSCPackage(string msgKey,GamePackage pack, Action<GamePackage> cb)
    {
        m_msgKey = msgKey;
        m_pack = pack;
        m_sucCB = cb;
    }
    public WebSCPackage(string msgKey, GamePackage pack,int seq, Action<GamePackage> cb)
    {
        m_msgKey = msgKey;
        m_pack = pack;
        m_sucCB = cb;
        m_seq = seq;
    }
}


public class NetToken
{
    public UInt64 m_UID;
    public string m_TokenStr;
    public DateTime m_RecvTime;
    public NetToken() { }

    public void SetAccessToken(string accessToken)
    {
        m_TokenStr = accessToken;
    }
    public void Pack(NetBinaryWriter bw)
    {
        bw.PushUInt64(m_UID);
        bw.PushString(m_TokenStr);
    }

    public bool isExpired()
    {
        var dt = DateTime.Now - m_RecvTime;
        return dt.TotalMinutes > 60 * 24 * 1; // token有效期为1天
    }
}



public class CNetSendOption
{
    public Int32 m_startBlock = 3000;
    public Int32 m_timeout = 8000;
    public bool m_hasRspMsg = true;

    public CNetSendOption(int startBlock, bool hasRspMsg = true, int timeOut = 8000)
    {
        m_startBlock = startBlock;
        m_hasRspMsg = hasRspMsg;
        m_timeout = timeOut;
    }

    public CNetSendOption() { }
}

public class CDealerCBOP : CDealerCB
{
    public CNetSendOption m_op;
    public DateTime m_sendTime;
    public GamePackage m_req;
    public bool m_canAbandon = true;
    public int m_autoRetryTimes = 0;
    public object m_cbTarget = null;
    public bool m_isAddWaitQueue = false;
    public CDealerCBOP(System.Delegate cb, CNetSendOption op, GamePackage req, bool canAbandon = true, object parentTarget = null) : base(cb)
    {
        m_op = op;
        m_req = req;
        m_sendTime = DateTime.Now;
        m_canAbandon = canAbandon;
        m_autoRetryTimes = 0;
        if (parentTarget != null)
            m_cbTarget = parentTarget;
        else
            m_cbTarget = cb.Target;
    }

    public override bool EqualsTarget(object target)
    {
        return m_cbTarget == target;
    }
    public override bool EqualsType<T>()
    {
        return m_cbTarget.GetType() == typeof(T);
    }
}

public class CHttpDealerOP 
{
    public CNetSendOption m_op;
    public DateTime m_sendTime;
    public object m_cbTarget = null;
    public CHttpDealerOP(CNetSendOption op) 
    {
        m_op = op;
        m_sendTime = DateTime.Now;
    }
}


public enum ENETSYSSTATE : int
{
    None = 0,
    LogicConnecting,
    LogicConnected,
    Disconnected,


};

public static class EBLOCKSTATE
{
    public const int normal = 0;
    public const int wait1 = 1;
    public const int wait2 = 2;
}

public enum ENNetSysInput : int
{
    LogicConnect,
    LogicConnected,
    Disconnected,
}

public class NetCommon{

}
