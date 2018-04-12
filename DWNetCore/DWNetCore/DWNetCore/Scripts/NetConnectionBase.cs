using System.Collections;
using System.Collections.Generic;

public delegate void ConnectCallBack(bool bConnected);
public delegate void ConnectRecvPack(IPack pack);
public delegate void DisconnectCallBack();



public class NetConnectionBase
{
    public static class NETSTATE
    {
        public const int ns_none = 0;
        public const int ns_Connecting = 1;
        public const int ns_ConnectAuthing = 2;
        public const int ns_ConnectAuthed = 3;
        public const int ns_WaitingLogin = 4;
        public const int ns_Connected = 5;
        public const int ns_DisConnected = 6;
        public const int ns_ConnectServerErro = 7;//服务器连接异常
        public const int ns_SvrIsFull = 8;//服务器爆满
        public const int ns_Waitting = 9;//排队
        public const int ns_reTry = 10;
    }

	public NetCrypte m_crypte;

    public const float HeartBeatTime = 300.0f;
    private float m_heartBeatTime = 0;
    private uint m_dwMsgSeq = 0;
    public uint MsgSeq
    {
        get {
            return m_dwMsgSeq++; 
        }
        set { m_dwMsgSeq = value; }
    }

    protected void SendHello()
    {
        // 心跳包, connect 才发
        //if (Time.realtimeSinceStartup - m_heartBeatTime > HeartBeatTime && IsConnected())
        {
			// todo
            /*CLoginSys loginSys = CGameRoot.GetGameSystem<CLoginSys>();
            bool IsLogined = loginSys != null && loginSys.IsLogined;
            if (IsLogined)
            {
                CSMsgReqBody msg = new CSMsgReqBody();
                msg.construct(csprotoMacros.CS_REQ_HELLO);
                msg.stHello.chReserve = 0;

                SendMsg(msg, csprotoMacros.CS_REQ_HELLO);
                m_heartBeatTime = Time.realtimeSinceStartup;
                //Debug.Log("Send Heart");
            }*/
        }
    }

    protected virtual void OnNotifyNewState()
    {

    }
    public static int TcpClientReceiveBufferSize = 256 * 1024;
    public static int TcpClientReceiveTimeout = 10000;

    public static int TcpClientSendBufferSize = 256 * 1024;
    public static int TcpClientSendTimeout = 10000;
    //发送队列
    //protected List<CSMsgRequest> m_CSMsgCToSend = new List<CSMsgRequest>();
    public virtual bool IsConnected()
    {
		return false;
    }

	public virtual void Disconnect(bool isHandle=false)
    {

    }

	public virtual void ConnectServer(string strAddr, string strIp, int nPort, NetToken token, ConnectCallBack ConnetCallback, ConnectRecvPack connectRecvPack, byte[] pkg)
    {

    }

    //public virtual void SendMsg(CSMsgReqBody msg, int iCmdId)
	public virtual void SendMsg(IPack pack)
	{

    }

    public virtual void Update()
    {

    }
}

public class NetConnectionFactory
{
    /// <summary>
    /// 创建一个网络连接，bUseTGCP为true的时候返回一个tgcp的连接
    /// 为false的时候返回一个c#实现的网络连接
    /// </summary>
    /// <param name="bUseTGCP"></param>
    /// <returns></returns>
    public static NetConnectionBase CreateConnection(string name)
    {
    	return new CNetConnection(name);
    }
}
