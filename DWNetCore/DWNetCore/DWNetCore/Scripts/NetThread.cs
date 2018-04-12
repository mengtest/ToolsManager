using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System;

public class NetTcpClient {
	//private bool bLock = true;
	private Socket m_tcpClient;
	private ConnectCallBack connectCB;
	private bool is_connecting = false;
	private bool is_closed = false;
	
	public IAsyncResult BeginConnect (string host, string bakIp, int port, ConnectCallBack requestCallback) {
		lock (this) {
			if (m_tcpClient != null) {
				m_tcpClient.Close();
				m_tcpClient = null;
			}
			//m_tcpClient = new TcpClient();
			
			IPAddress ip = null;
			IPAddress.TryParse(host, out ip);
			if (ip == null) {
				IPHostEntry hostEntry = null;
				try 
				{
					hostEntry = Dns.GetHostEntry(host);
					if (hostEntry.AddressList.Length > 0) {
						ip = hostEntry.AddressList[0];
					} else { 
						IPAddress.TryParse(bakIp, out ip);
					}
				} 
				catch
				{
					IPAddress.TryParse(bakIp, out ip);
				}
			}
			
			IPEndPoint ipe = new IPEndPoint(ip, port);
			m_tcpClient = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			
			
			m_tcpClient.NoDelay = true;
			m_tcpClient.Blocking = true;
			
			m_tcpClient.ReceiveBufferSize = NetConnectionBase.TcpClientReceiveBufferSize;
			m_tcpClient.ReceiveTimeout = NetConnectionBase.TcpClientReceiveTimeout;
			
			m_tcpClient.SendBufferSize = NetConnectionBase.TcpClientSendBufferSize;
			m_tcpClient.SendTimeout = NetConnectionBase.TcpClientSendTimeout;
			
			connectCB = requestCallback;
			is_connecting = true;
			return m_tcpClient.BeginConnect(ipe, ConnectCallback, m_tcpClient);
			//return m_tcpClient.BeginConnect(host, port, ConnectCallback, m_tcpClient);
		}
	}
	private void ConnectCallback(IAsyncResult asyncresult)
	{
		Debug.Log("net connectCallBackMethod");
		lock (this) {
			var tcpclient = asyncresult.AsyncState as Socket;
			if (m_tcpClient != tcpclient) {
				return;
			}
			is_connecting = false;
			bool success = false;
			try {
				if (tcpclient.Connected) 
				{
					success = true;
					tcpclient.EndConnect(asyncresult);
					if (is_closed) {
						m_tcpClient.Close();
						m_tcpClient = null;
					}
				}
			}
			catch (Exception ex)
			{
				success = false;
				if (ex is SocketException)
				{
					SocketException socketExcept = (SocketException)ex;
					if (socketExcept.ErrorCode == 10055)//No buffer space available. iphone4上会出这个异常，但实际连接还是有效
					{
						success = true;
					}
				}
				Debug.Log("net connect callback: exception " + ex.Message);
			}
			finally
			{
				connectCB(success);
			}
		}
	}
	public void Close() {
		lock (this) {
			if (is_connecting) {
				is_closed = true;
				return;
			}
			if (m_tcpClient != null) {
				m_tcpClient.Close();
			}
			m_tcpClient = null;
		}
	}
	
	public void Write(byte[] data, int off, int len) {
		lock (this) {
			if (m_tcpClient != null && m_tcpClient.Connected) {
				//m_tcpClient.Write(data, off, len);
				try
				{
					m_tcpClient.Send (data, off, len, SocketFlags.None);
				}
				catch(System.Exception)
				{
				}
			}
		}
	}
	
	public int Read(byte[] buff, int off, int len) {
		lock(this) {
			int rlen = -1;
			try {
                
				if (m_tcpClient != null && m_tcpClient.Connected && m_tcpClient.Poll(0, SelectMode.SelectRead) && m_tcpClient.Available != 0) {
                    //Debug.Log("start Receive");
                    rlen = m_tcpClient.Receive(buff, off, len, SocketFlags.None);
					//Debug.Log("finished Receive" + rlen);
				}
			} catch (System.Exception ex) {
				Debug.Log("socket close:" + ex);
				rlen = 0;
			}
			return rlen;
		}
	}
}

public class NetThread
{
    private Thread m_thread;
    private volatile bool m_terminateFlag;
    private System.Object m_terminateFlagMutex;
	protected CNetConnection m_NetConnection;
	protected NetTcpClient m_tcpClient;
	protected int m_seq;

    public void Run()
    {
        m_thread.Start(this);
    }

	public void Close()
	{
        SetTerminateFlag();
		if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			//
		}
		else
		{
			m_thread.Abort();
		}
	}

    protected static void ThreadProc(object obj)
    {
        NetThread me = (NetThread)obj;
        me.Main();
    }

    protected virtual void Main()
    {

    }

    public void WaitTermination()
    {
        m_thread.Join();
    }

    public void SetTerminateFlag()
    {
        lock (m_terminateFlagMutex)
        {
            m_terminateFlag = true;
        }
    }

    protected bool IsTerminateFlagSet()
    {
        lock (m_terminateFlagMutex)
        {
            return m_terminateFlag;
        }
    }

	public NetThread(CNetConnection net, NetTcpClient client, int seq)
    {
		m_thread = new Thread(ThreadProc);
        m_terminateFlag = false;
		m_NetConnection = net;
		m_tcpClient = client;
		m_seq = seq;
        m_terminateFlagMutex = new System.Object();
    }
}
