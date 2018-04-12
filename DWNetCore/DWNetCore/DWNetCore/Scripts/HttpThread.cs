using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Net;
using System.IO;
using System.Text;

public class DWHttpClient
{
    public HttpConnection m_httpConnection;
    //�����������ر���
    public class AsynState
    {
        public HttpWebRequest m_webRequest;
        public Stream m_reqStream;
        public Action<GamePackage> m_sucCallBack;
        public string m_msgKey;
        public int m_seq;
    }

    //同步请求
    public void RequestSync(WebCSPackage csPackage)
    {
        var webRequest = CreateHttpWebRequest(csPackage.m_url, csPackage.m_param, csPackage.m_isPost);

        if (webRequest == null)
        {
            FailedCallback(csPackage.m_msgKey,csPackage.m_seq, csPackage.m_sucCB);
            return;
        }

        Stream reqStream = null;
        Stream responseStream = null;
        HttpWebResponse response = null;
        GamePackage package = null;
        BinaryReader br = null;

        byte[] data = Encoding.GetEncoding("utf-8").GetBytes(csPackage.m_param);

        try
        {
            //上报数据
            reqStream = webRequest.GetRequestStream();
            reqStream.Write(data, 0, data.Length);
            reqStream.Flush();
            reqStream.Dispose();

            //返回数据
            response = (HttpWebResponse)webRequest.GetResponse();

            if (response != null)
            {
                responseStream = response.GetResponseStream();
                br = new BinaryReader(responseStream);
                int contentLength = 1024 * 10;
                if (contentLength > 0)
                {
                    byte[] content = br.ReadBytes(contentLength);
                    package = Unpack(content);
                }
                else
                {
                    package = new GamePackage();
                    package.errno = -1;
                }
            }
            else
            {
                package = new GamePackage();
                package.errno = -1;
            }

        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("DWHttpClient RequestSync  Exception : " + e.Message);
            package = new GamePackage();
            package.errno = -1;

            return;
        }
        finally
        {
            if(br != null)
            {
                br.Close();
                br = null;
            }
            if (reqStream != null)
            {
                reqStream.Close();
                reqStream = null;
            }
            if(responseStream != null)
            {
                responseStream.Close();
                responseStream = null;
            }

            if (response != null)
            {
                response.Close();
                response = null;
            }


            if(webRequest != null)
            {
  //              if (Application.platform != RuntimePlatform.IPhonePlayer)
                {
                    if (webRequest != null)
                        webRequest.Abort();
                }
                webRequest = null;
            }

            WebSCPackage webPackage = new WebSCPackage(csPackage.m_msgKey, package,csPackage.m_seq,csPackage.m_sucCB);

            if (m_httpConnection != null)
                m_httpConnection.EnqueueRespondMsg(webPackage);
        }
    }




    //异步请求
    public void RequestAsync(WebCSPackage csPackage)
    {
        var webRequest = CreateHttpWebRequest(csPackage.m_url,csPackage.m_param, csPackage.m_isPost);
        if (webRequest == null)
        {
            FailedCallback(csPackage.m_msgKey,csPackage.m_seq,csPackage.m_sucCB);
            return;
        }
        UnityEngine.Debug.LogError("CreateHttpWebRequest end");
        Stream reqStream = null;
        if (csPackage.m_isPost)
        {
            byte[] data = Encoding.GetEncoding("utf-8").GetBytes(csPackage.m_param);

            try
            {
                reqStream = webRequest.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Flush();
                reqStream.Dispose();

            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("DWHttpClient Request  Stream  Exception : " + e.Message);

                FailedCallback(csPackage.m_msgKey,csPackage.m_seq,csPackage.m_sucCB);
                return;
            }
            finally
            {
                if (reqStream != null)
                {
                    reqStream.Close();
                    reqStream = null;
                }
            }
        }

        AsynState resultState = new AsynState();
        resultState.m_webRequest = webRequest;
        resultState.m_reqStream = reqStream;
        resultState.m_sucCallBack = csPackage.m_sucCB;
        resultState.m_msgKey = csPackage.m_msgKey;
        resultState.m_seq = csPackage.m_seq;
        UnityEngine.Debug.LogError("GetRequestStream end");
        try
        {
           var ar = webRequest.BeginGetResponse(RequestCallback, resultState);
            UnityEngine.Debug.LogError("BeginGetResponse end");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("DWHttpClient BeginGetResponse   Exception : " + e.Message);
            if (webRequest != null)
            {
  //              if (Application.platform != RuntimePlatform.IPhonePlayer)
                {
                    if (webRequest != null)
                        webRequest.Abort();
                }
                webRequest = null;
            }
            FailedCallback(csPackage.m_msgKey,csPackage.m_seq, csPackage.m_sucCB);
            return;
        }
        finally
        {
            
        }
    }

    void RequestCallback(IAsyncResult ar)
    {
        UnityEngine.Debug.LogError("RequestCallback start");
        GamePackage package = null;
        AsynState resultState = null;
        HttpWebResponse response = null;
        Stream responseStream = null;
        BinaryReader br = null;
        if (ar != null)
        {
            resultState = (AsynState)ar.AsyncState;
        }

        try
        {
            if (resultState != null)
            {
                response = (HttpWebResponse)resultState.m_webRequest.EndGetResponse(ar);
                UnityEngine.Debug.LogError("EndGetResponse end");
                if (response != null)
                {
                    responseStream = response.GetResponseStream();
                    UnityEngine.Debug.LogError("GetResponseStream end");
                    br = new BinaryReader(responseStream);
                    int contentLength = 1024 * 10;
                    if(contentLength > 0)
                    {
                        byte[] content = br.ReadBytes(contentLength);
                        package = Unpack(content);
                    }
                    else
                    {
                        package = new GamePackage();
                        package.errno = -1;
                    }
                }
                else
                {
                    package = new GamePackage();
                    package.errno = -1;
                }
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("DWHttpClient.RequestCallback EndGetResponse  Exception : " + e.Message);
        }
        finally
        {
            if(br != null)
            {
                br.Close();
                br = null;
            }
            if(responseStream != null)
            {
                responseStream.Close();
                responseStream = null;
            }
            if(response != null)
            {
                response.Close();
                response = null;
            }
            if (resultState != null)
            {
                WebSCPackage webPackage = new WebSCPackage(resultState.m_msgKey, package, resultState.m_sucCallBack);

                if (m_httpConnection != null)
                    m_httpConnection.EnqueueRespondMsg(webPackage);
            }
            else
            {
                UnityEngine.Debug.LogError("DWHttpClient.RequestCallback IAsyncResult resultState is null ");
            }
        }
    }

    private GamePackage Unpack(byte[] content)
    {
        GamePackage package = new GamePackage();
        package.body = content;
        return package;
    }


    void FailedCallback(string msgKey,int seq,Action<GamePackage> callBack)
    {
        GamePackage package = new GamePackage();
        package.errno = -1;
        WebSCPackage webPackage = new WebSCPackage(msgKey,package,seq,callBack);
        if (m_httpConnection != null)
        {
            m_httpConnection.EnqueueRespondMsg(webPackage);
        }
    }


    //����http����
    protected HttpWebRequest CreateHttpWebRequest(string url, string param, bool post,int range = 0)
    {
        if (!post && !string.IsNullOrEmpty(param))
            url = string.Format("{0}?{1}", url, param);
        // ���� URI �����쳣
        HttpWebRequest webRequest = null;
        try
        {
            webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.ProtocolVersion = HttpVersion.Version11;
            webRequest.Accept = "text/html";
            webRequest.Method = post ? "POST" : "GET";
            webRequest.ReadWriteTimeout = 8 * 1000;
            webRequest.Timeout = 8 * 1000;
            webRequest.ContentType = "application/x-www-form-urlencoded";

            if (range > 0)
                webRequest.AddRange(range); //����Rangeֵ
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("DWHttpClient CreateHttpWebRequest Exception : " + e.Message);
            if(webRequest != null)
            {
 //               if (Application.platform != RuntimePlatform.IPhonePlayer)
                {
                    if (webRequest != null)
                        webRequest.Abort();
                }

                webRequest = null;

            }

        }
        return webRequest;
    }


   
    //�ر�http����
    protected void CloseHttpWepRequest(AsynState state)
    {
        if (state.m_reqStream != null)
            state.m_reqStream.Close();

 //       if (Application.platform != RuntimePlatform.IPhonePlayer)
        {
            if (state.m_webRequest != null)
                state.m_webRequest.Abort();
        }
    }

}


public class HttpThread{

    private Thread m_thread;
    private volatile bool m_terminateFlag;
    private System.Object m_terminateFlagMutex;
    protected DWHttpClient m_httpClient;
    protected HttpConnection m_httpConnection;
    protected int m_seq;

    public void Run()
    {
        m_thread.Start(this);
    }

    public bool CheckIsRunning()
    {
        Debug.LogError("m_thread.ThreadState " + m_thread.ThreadState);
        return m_thread.ThreadState == ThreadState.Running || m_thread.ThreadState == ThreadState.WaitSleepJoin;
    }

    public void Close()
    {
        SetTerminateFlag();
        if (Application.platform == RuntimePlatform.IPhonePlayer)
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
        HttpThread me = (HttpThread)obj;
        me.Main();
    }

    protected virtual void Main()
    {
        while (!IsTerminateFlagSet())
        {
            bool sleep = true;
            WebCSPackage pkg = null;
           
            pkg = m_httpConnection.DequeueRequestMsg();
            if (pkg != null)
            {
                sleep = false;
                UnityEngine.Debug.Log(DateTime.Now.TimeOfDay.ToString() +  ":http send pkg");
                try
                {
                    m_httpClient.RequestAsync(pkg);
                }
                catch (IOException e)
                {
                    Debug.LogError("HttpThread.SenderThread Main " + e.Message);
                    Debug.LogError("HttpThread.SenderThread Main " + e.StackTrace);
                    Debug.LogError("HttpThread.SenderThread Main" + e.InnerException.Message);
                }
            }
            if (sleep)
            {
                Thread.Sleep(10);
            }
        }
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

    public HttpThread(DWHttpClient client,HttpConnection connection)
    {
        m_thread = new Thread(ThreadProc);
        m_terminateFlag = false;
        m_httpClient = client;
        m_httpConnection = connection;
        m_terminateFlagMutex = new System.Object();
    }
}
