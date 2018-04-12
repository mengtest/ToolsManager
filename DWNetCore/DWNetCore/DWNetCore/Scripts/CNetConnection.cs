//#define LOG_NETMSG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;


public class CNetConnection : NetConnectionBase
{
    public class CQueueEnry
    {
        public enum EQuequeEnetry
        {
            data,
            close,
        };
        public EQuequeEnetry m_eType;
        public IPack data;
    };
    //收到的数据包队列
    private Queue<CQueueEnry> m_queRecvPack;
    //准备发送的数据包队列
    private Queue<IPack> m_queSendPack;


    //连接成功的回调接口
    private ConnectCallBack m_fnConnectedCB;
    //断线时回调
    public DisconnectCallBack m_fnDisconnectedCB;
    //收到数据的回调接口
    private ConnectRecvPack m_fnRecvCB;

    //接收线程
    private ReceiverThread m_thRecv;
    //发送线程 
    private SenderThread m_thSend;

    //用于连接验证的token 
    private NetToken m_stToken;

    public string m_name;
    public int m_seq = 0;

    public bool m_isAuthed = true;

    public string Name
    {
        get
        {
            return m_name + "_" + m_seq;
        }
    }

    //验证包带的数据
    private byte[] m_cpkg;

    //该连接的状态机
    private TransferState<int> m_stNetState;
    //状态机的输入
    enum ENNetStateInput
    {
        Connect,
        Auth,
        Authed,
        Disconnect,
        HandleDisconnect,
    }

    static readonly TransferStateInput DNetStateInputConnect = new TransferStateInputEnum<ENNetStateInput>(ENNetStateInput.Connect);
    static readonly TransferStateInput DNetStateInputAuth = new TransferStateInputEnum<ENNetStateInput>(ENNetStateInput.Auth);
    static readonly TransferStateInput DNetStateInputAuthed = new TransferStateInputEnum<ENNetStateInput>(ENNetStateInput.Authed);
    static readonly TransferStateInput DNetStateInputDisconnect = new TransferStateInputEnum<ENNetStateInput>(ENNetStateInput.Disconnect);
    static readonly TransferStateInput DNetStateInputHandleDisconnect = new TransferStateInputEnum<ENNetStateInput>(ENNetStateInput.HandleDisconnect);
    static readonly TransferStateInput DNetStateInputData = new TransferStateInputData(null);


    //连接的tcp接口

    private NetTcpClient m_tcpClient = null;

    //连接的ip端口
    private int m_nPort = 0;
    private string m_sNetAddr = "10.12.17.143";
    private string m_sNetIp = "10.12.17.143";

    //当前重试次数
    private int m_iReTryTimes = 1;

    public int State
    {
        get
        {
            return m_stNetState.m_curState;
        }
    }

    //发送数据, 数据会先放到发送队列， 由发送线程定时发送
    public override void SendMsg(IPack pack)
    {
        //Debug.Log (Name + ":send msg:" + BitConverter.ToString(pack));
        lock (m_queSendPack)
        {
            m_queSendPack.Enqueue(pack);
        }
    }
    public void clearQueue()
    {
        lock (m_queRecvPack)
        {
            m_queRecvPack.Clear();
        }
        lock (m_queSendPack)
        {
            m_queSendPack.Clear();
        }
    }

    //尝试重连
    void TryReConnect()
    {
        m_stNetState.InputData(DNetStateInputConnect);
    }

    //public void EnqueueRespondMsg(CSMsgResponse msg)
    public void EnqueueRespondMsg(CQueueEnry msg, int seq)
    {
#if LOG_NETMSG
        Debug.Log(string.Format("CNetSys: RespondMsg {0}", msg.stHead.iCmdID));
#endif
        if (seq != m_seq)
        {
            Debug.Log("enqueue msg but seq not match," + seq + "," + m_seq);
            return;
        }

        lock (m_queRecvPack)
        {
            m_queRecvPack.Enqueue(msg);
        }
    }

    public byte[] DequeueRequestMsg(int seq)
    {
#if LOG_NETMSG
		Debug.Log(string.Format("CNetSys: RespondMsg {0}", msg.stHead.iCmdID));
#endif
        if (seq != m_seq)
        {
            return null;
        }
        byte[] pkg = null;
        IPack pck = null;
        lock (m_queSendPack)
        {
            if (m_queSendPack.Count > 0)
            {
                pck = m_queSendPack.Dequeue();
            }
        }
        if (pck != null)
        {
            pkg = pck.PackMsg();
        }
        return pkg;
    }

    public bool StartConnect(int curState, TransferStateInput input, int dstState)
    {
        if (m_tcpClient != null)
        {
            m_tcpClient.Close();
            m_tcpClient = null;
        }
        m_tcpClient = new NetTcpClient();
        try
        {
            m_tcpClient.BeginConnect(m_sNetAddr, m_sNetIp, m_nPort, (bool success) =>
            {
                if (success)
                {
                    m_stNetState.InputData(DNetStateInputAuth);
                }
                else
                {
                    m_stNetState.InputData(DNetStateInputDisconnect);
                }
            });
        }
        catch
        {
        }

        return true;
    }
    public bool DealTcpConnected(int curState, TransferStateInput input, int dstState)
    {
        ////成功, 设置属性
        //NetBinaryWriter bw = new NetBinaryWriter();
        //m_stToken.Pack(bw);
        //if (m_cpkg != null)
        //{
        //    bw.PushData(m_cpkg);
        //    m_cpkg = null;
        //}
        //byte[] data = bw.getBytes();
        //Debug.Log("send token:" + BitConverter.ToString(data));
        //m_tcpClient.Write(data, 0, data.Length);
        //m_isAuthed = false;
        //去掉验证

        m_stNetState.InputData(DNetStateInputAuthed);
        StartSendThread();
        StartReceiveThread();
        return true;
    }


    public bool DealDisconnect(int curState, TransferStateInput a_input, int dstState)
    {
        CloseConnect();
        if (m_stNetState != NETSTATE.ns_DisConnected && m_fnDisconnectedCB != null)
        {
            m_fnDisconnectedCB();
        }
        /*
		m_iReTryTimes = m_iReTryTimes + 1;
		if (m_iReTryTimes == 0) {
			m_stNetState.InputData(DNetStateInputConnect);
		} else {
			//TODO 没有定时器， 需要延迟重连
		}
		*/
        return true;
    }

    public bool DealHandleDisconnect(int curState, TransferStateInput a_input, int dstState)
    {
        CloseConnect();
        return true;
    }

    //public bool DealAuthResult(int curState, TransferStateInput a_input, int dstStat)
    //{
    //    TransferStateInputData input = a_input as TransferStateInputData;
    //    //short errno = System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(input.m_sData, 0));
    //    if (input.m_sData.ErrorID == 0)
    //    {
    //        StartSendThread();
    //        m_iReTryTimes = 0;
    //        m_stNetState.InputData(DNetStateInputAuthed);
    //        if (input.m_sData.HasData)
    //        {
    //            m_stNetState.InputData(new TransferStateInputData(input.m_sData));
    //        }
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    public bool NotifyConnected(int curState, TransferStateInput a_input, int dstStat)
    {
        if (m_fnConnectedCB != null)
        {
            m_fnConnectedCB(true);
        }
        return true;
    }

    public bool DealRecvPack(int curState, TransferStateInput a_input, int dstStat)
    {
        TransferStateInputData input = a_input as TransferStateInputData;
        //byte[] pack = m_crypte.Decrypte(input.m_sData);
        m_fnRecvCB(input.m_sData);
        return true;
    }

    public CNetConnection(string name)
    {
        // todo
        m_queRecvPack = new Queue<CQueueEnry>();
        m_queSendPack = new Queue<IPack>();
        m_crypte = new NetCrypte();
        m_name = name;

        //使用状态机管理网络
        m_stNetState = new TransferState<int>(NETSTATE.ns_none);
        //所以状态在收到连接请求时都开始进行连接
        m_stNetState.addTransferAll(DNetStateInputConnect, NETSTATE.ns_Connecting, StartConnect);
        ////正在连接的状态收到tcp连接成功时开始进行验证
        //m_stNetState.addTransfer(NETSTATE.ns_Connecting, DNetStateInputAuth, NETSTATE.ns_ConnectAuthing, DealTcpConnected);

        ////验证状态收到第一个包， 验证成功时，切换到连接成功状态
        //m_stNetState.addTransfer(NETSTATE.ns_ConnectAuthing, DNetStateInputData, NETSTATE.ns_Connected, DealAuthResult);

        //跳过验证，用建立连接后通过登录消息来验证
        m_stNetState.addTransfer(NETSTATE.ns_Connecting, DNetStateInputAuth, NETSTATE.ns_Connected, DealTcpConnected);
        //连接成功状态后后续的包为正常消息包
        m_stNetState.addTransfer(NETSTATE.ns_Connected, DNetStateInputAuthed, NETSTATE.ns_Connected, NotifyConnected);
        m_stNetState.addTransfer(NETSTATE.ns_Connected, DNetStateInputData, NETSTATE.ns_Connected, DealRecvPack);
        //任何状态收到断开tcp的请求时， 切换到连接断开状态
        m_stNetState.addTransferAll(DNetStateInputDisconnect, NETSTATE.ns_DisConnected, DealDisconnect);
        //任何状态收到断开tcp的请求时， 切换到连接断开状态
        m_stNetState.addTransferAll(DNetStateInputHandleDisconnect, NETSTATE.ns_DisConnected, DealHandleDisconnect);
        m_stNetState.m_transfer = (int pre,TransferStateInput input ,int aft) =>
        {
            //Debug.Log(Name + ":" + pre + " transfer to " + aft);
            return false;
        };
    }

    public override bool IsConnected()
    {
        return m_stNetState.m_curState == NETSTATE.ns_Connected;
    }

    public override void ConnectServer(string strAddr, string strIP, int nPort, NetToken token, ConnectCallBack ConnetCallback, ConnectRecvPack connectRecvPack, byte[] pkg)
    {
        //Debug.Log(string.Format("ConnectServer IP = {0}; Port ={1}", m_NetIp, m_Port));
        //        Disconnect();
        ++m_seq;
        Debug.Log(string.Format(Name + ":ConnectServer IP = {0}; Port ={1}", strAddr, nPort));
        m_fnConnectedCB = ConnetCallback;
        m_fnRecvCB = connectRecvPack;
        m_sNetAddr = strAddr;
        m_sNetIp = strIP;
        m_nPort = nPort;
        m_iReTryTimes = 3;
        m_stToken = token;
        if (m_stToken == null)
        {
            m_stToken = new NetToken();
        }
        m_cpkg = pkg;
        m_stNetState.clearDelayInput();
        m_stNetState.InputData(DNetStateInputConnect);
    }

    public override void Update()
    {
        TriggerEvent();
        SendHello();
        //Debug.Log("connect update finished");
    }

    void TriggerEvent()
    {
        lock (m_queRecvPack)
        {
            while (m_queRecvPack.Count > 0)
            {
                CQueueEnry entry = m_queRecvPack.Dequeue();
                if (entry.m_eType == CQueueEnry.EQuequeEnetry.data)
                {
                    //Debug.Log(Name + ": net recv a pkg");
                    m_stNetState.InputData(new TransferStateInputData(entry.data));
                }
                else if (entry.m_eType == CQueueEnry.EQuequeEnetry.close)
                {
                    Disconnect();
                }
            }
        }
    }


    public void SetTerminateFlag()
    {
        if (m_thSend != null)
        {
            m_thSend.SetTerminateFlag();
            m_thSend = null;
        }

        if (m_thRecv != null)
        {
            m_thRecv.SetTerminateFlag();
            m_thRecv = null;
        }
    }

    public void WaitTermination()
    {
        if (m_thSend != null)
            m_thSend.WaitTermination();
        if (m_thRecv != null)
            m_thRecv.WaitTermination();
    }

    protected void StartReceiveThread()
    {
        if (m_tcpClient == null)
        {
            return;
        }

        m_thRecv = new ReceiverThread(this, m_tcpClient, m_seq);
        m_thRecv.Run();
    }

    protected void StartSendThread()
    {
        if (m_tcpClient == null)
        {
            return;
        }

        // todo
        m_thSend = new SenderThread(this, m_tcpClient, m_seq);
        m_thSend.Run();
    }

    public override void Disconnect(bool isHandle = false)
    {
        if (isHandle)
        {
            m_stNetState.InputData(DNetStateInputHandleDisconnect);
        }
        else
        {
            m_stNetState.InputData(DNetStateInputDisconnect);
        }
    }

    protected void CloseConnect()
    {
        Debug.Log(Name + ": close connect");
        if (m_tcpClient != null)
        {
            m_tcpClient.Close();
        }

        Debug.Log(Name + ": close connect2");
        SetTerminateFlag();
    }

    public void CloseThread()
    {
        if (m_thSend != null)
        {
            m_thSend.Close();
        }
        if (m_thRecv != null)
        {
            m_thRecv.Close();
        }
    }

    class ReceiverThread : NetThread
    {
        const uint MaxPacketSize = 1024 * 256;
        const uint MinPacketSize = 2;

        private byte[] m_recBuf;
        private int m_recBufOffset;

        public int GetPackLen()
        {
            UInt16 Len = (UInt16)System.Net.IPAddress.NetworkToHostOrder((short)System.BitConverter.ToUInt16(m_recBuf, 0));
            return Len;
        }

        public ReceiverThread(CNetConnection netSys, NetTcpClient netClient, int seq)
            : base(netSys, netClient, seq)
        {
            m_recBuf = new byte[MaxPacketSize];
            m_recBufOffset = 0;
        }

        protected override void Main()
        {
            Debug.Log(m_NetConnection.Name + ":CNetSys:ReceiverThread.Main: Begin");

            while (!IsTerminateFlagSet())
            {
                try
                {
                    bool sleep = true;
                    if (ReadFromStream())
                    {
                        sleep = false;
                    }
                    if (ScanPackets())
                    {
                        sleep = false;
                    }
                    if (sleep)
                    {
                        Thread.Sleep(10);
                    }
                }
                catch (Exception)
                {
                    Thread.Sleep(10);
                }
            }

            Debug.Log(m_NetConnection.Name + ":CNetSys:ReceiverThread.Main: End");
        }

        protected bool ReadFromStream()
        {
            int len = m_tcpClient.Read(m_recBuf, m_recBufOffset, m_recBuf.Length - m_recBufOffset);
            if (len == 0)
            {
                //Debug.Log(m_NetConnection.Name + ":socket close");
                CQueueEnry entry = new CQueueEnry();
                entry.m_eType = CQueueEnry.EQuequeEnetry.close;
                this.m_NetConnection.EnqueueRespondMsg(entry, m_seq);
                return false;
            }
            else if (len < 0)
            {
                return false;
            }
            else
            {
                m_recBufOffset += len;
                return true;
            }
        }
        protected bool ScanPackets()
        {
            bool packetFound = false;
            bool has = false;
            do
            {
                packetFound = false;
                if (m_recBufOffset >= MinPacketSize)
                {
                    int Len = GetPackLen();
                    if (Len <= m_recBufOffset - MinPacketSize)
                    {
                        CQueueEnry entry = new CQueueEnry();
                        entry.data = m_NetConnection.GetPack();
                        entry.m_eType = CQueueEnry.EQuequeEnetry.data;
                        int curOffset = (int)MinPacketSize;
                        //if (!m_NetConnection.m_isAuthed)
                        //{
                        //    //第一次授权包
                        //    entry.data.ErrorID = System.Net.IPAddress.NetworkToHostOrder(
                        //        System.BitConverter.ToInt16(m_recBuf, curOffset));
                        //    m_NetConnection.m_isAuthed = true;
                        //    curOffset += 2;
                        //    int lastLen = Len - 2;
                        //    if (Len - curOffset > MinPacketSize)
                        //    {
                        //        entry.data.UnPack(m_recBuf, curOffset, lastLen);
                        //    }
                        //}
                        //else
                        {
                            entry.data.UnPack(m_recBuf, curOffset, Len);
                        }

                        //Buffer.BlockCopy(m_recBuf, (int)MinPacketSize, entry.data, 0, Len);
                        //						Debug.Log(DateTime.Now.TimeOfDay.ToString() + m_NetConnection.Name + ": recv a pkg");
                        m_NetConnection.EnqueueRespondMsg(entry, m_seq);
                        Buffer.BlockCopy(m_recBuf, (int)(Len + MinPacketSize), m_recBuf, 0, (int)(m_recBufOffset - Len - MinPacketSize));
                        m_recBufOffset -= (int)(Len + MinPacketSize);
                        packetFound = true;
                        has = true;
                    }
                }
            }
            while (packetFound && !IsTerminateFlagSet());
            return has;
        }
    }

    class SenderThread : NetThread
    {
        const int MaxPacketSize = 1024 * 256;
        public SenderThread(CNetConnection netconnection, NetTcpClient tcpClient, int seq)
            : base(netconnection, tcpClient, seq)
        {
        }

        protected override void Main()
        {
            //Debug.Log(m_NetConnection.Name + ":CNetSys:SenderThread.Main: Begin");
            while (!IsTerminateFlagSet())
            {
                bool sleep = true;
                byte[] pkg = null;
                if (m_NetConnection.m_stNetState == NETSTATE.ns_Connected)
                {
                    pkg = m_NetConnection.DequeueRequestMsg(m_seq);
                    if (pkg != null)
                    {
                        sleep = false;
                        //						Debug.Log(DateTime.Now.TimeOfDay.ToString() + m_NetConnection.Name + ":net send pkg");
                        try
                        {
                            UInt16 NetLen = (UInt16)System.Net.IPAddress.NetworkToHostOrder((short)pkg.Length);
                            byte[] netPkg = new byte[pkg.Length + 2];
                            System.BitConverter.GetBytes(NetLen).CopyTo(netPkg, 0);
                            pkg.CopyTo(netPkg, 2);
                            //m_tcpClient.Write(System.BitConverter.GetBytes(NetLen), 0, 2);
                            m_tcpClient.Write(netPkg, 0, (int)netPkg.Length);
                        }
                        catch (IOException)
                        {
                            //ProgressDlg.StopProgress();
                            //Debug.LogError(m_NetConnection.Name + ":CNetSys:SenderThread, Main: " + e.Message);
                            //Debug.LogError("CNetSys:SenderThread, Main: " + e.StackTrace);
                            //Debug.LogError("CNetSys:SenderThread, Main: " + e.InnerException.Message);
                        }
                    }
                }
                if (sleep)
                {
                    Thread.Sleep(10);
                }
            }
           // Debug.Log(m_NetConnection.Name + ":CNetSys:SenderThread.Main: End");
        }
    }

    public virtual IPack GetPack()
    {
        return null;
    }
}
