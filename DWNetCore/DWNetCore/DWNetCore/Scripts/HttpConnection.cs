using System.Collections;
using System.Collections.Generic;




public class HttpConnection {

    //�յ������ݰ�����
    private Queue<WebSCPackage> m_queRecvPack;
    //׼�����͵����ݰ�����
    private Queue<WebCSPackage> m_queSendPack;

    //�յ����ݵĻص��ӿ�
    private HttpConnectRecvPack m_fnRecvCB;

    //http�ͻ���
    private DWHttpClient m_httpClient;


    //�����߳� 
    private HttpThread m_thSend;

    public NetCrypte m_crypte;
    public HttpConnection(HttpConnectRecvPack recvCB)
    {
        m_fnRecvCB = recvCB;
        m_queRecvPack = new Queue<WebSCPackage>();
        m_queSendPack = new Queue<WebCSPackage>();
        m_crypte = new NetCrypte();
    }

    //��������, ���ݻ��ȷŵ����Ͷ��У� �ɷ����̶߳�ʱ����
    public void SendMsg(WebCSPackage pack)
    {
        UnityEngine.Debug.Log ("http :send msg: " + pack.m_url);
//        if(!m_thSend.CheckIsRunning())
        {
            UnityEngine.Debug.LogError("http :m_thSend is not work ");
            ReStartWork();
        }
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

    public void EnqueueRespondMsg(WebSCPackage package)
    {
        lock (m_queRecvPack)
        {
            m_queRecvPack.Enqueue(package);
        }
    }

    public WebCSPackage DequeueRequestMsg()
    {
        WebCSPackage pkg = null;
        lock (m_queSendPack)
        {
            if (m_queSendPack.Count > 0)
            {
                pkg = m_queSendPack.Dequeue();
            }
        }
        return pkg;
    }


    public void Update()
    {
        TriggerEvent();
    }

    void TriggerEvent()
    {
        lock (m_queRecvPack)
        {
            while (m_queRecvPack.Count > 0)
            {
                WebSCPackage entry = m_queRecvPack.Dequeue();
                if (m_fnRecvCB != null)
                    m_fnRecvCB(entry);
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
    }

    public void WaitTermination()
    {
        if (m_thSend != null)
            m_thSend.WaitTermination();
    }

    public void CloseThread()
    {
        if (m_thSend != null)
        {
            m_thSend.Close();
        }
    }

    protected void CloseConnect()
    { 
        SetTerminateFlag();
    }

    public void StartWork()
    {
        if (m_httpClient == null)
        {
            m_httpClient = new DWHttpClient();
            m_httpClient.m_httpConnection = this;
        }

        // todo
        m_thSend = new HttpThread(m_httpClient, this);
        m_thSend.Run();
    }

    public void ReStartWork()
    {
        UnityEngine.Debug.LogError("http :m_thSend is restart work ");
        if (m_httpClient == null)
        {
            m_httpClient = new DWHttpClient();
            m_httpClient.m_httpConnection = this;
        }
        CloseThread();
        // todo
        m_thSend = new HttpThread(m_httpClient, this);
        m_thSend.Run();
    }

}
