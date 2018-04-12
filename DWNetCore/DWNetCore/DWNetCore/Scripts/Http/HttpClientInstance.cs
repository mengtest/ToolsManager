//--------------------------------------------------------------------------------
//   File      : HttpClientInstance.cs
//   author    : guoliang
//   function   : 单个http请求实例
//   date      : 2018-2-6
//   copyright : Copyright 2017 DW Inc.
//--------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Net;
using System.IO;
using System.Text;


public class HttpClientInstance
{
    //超时时间阈值
    private float m_timeOut = 8;
    //请求开始时间
    private double m_requestStartTime = 0;
    //请求超时时间
    private double m_requsetEndTime = 0;
    //是否异常
    private bool m_isExcept = false;
    //是否已经被处理
    private bool m_isHandled = false;

    public WebCSPackage m_csPackage = null;

    public AsynState m_cacheIR = null;

    //连接管理类
    public DWHttpConnection m_httpConnection;

    //异步状态消息
    public class AsynState
    {
        public HttpWebRequest m_webRequest;
        public Stream m_reqStream;
        public Action<GamePackage> m_sucCallBack;
        public string m_msgKey;
        public int m_seq;

        public HttpWebResponse m_response = null;
        public Stream m_responseStream = null;
    }

    public HttpClientInstance()
    { }
    public HttpClientInstance(DWHttpConnection httpConnection,WebCSPackage csPackage, double startTime)
    {
        m_httpConnection = httpConnection;
        //缓存本次消息的包头和seq
        m_requestStartTime = startTime;
        m_timeOut = csPackage.m_timeOut;
        m_requsetEndTime = startTime + m_timeOut;
        m_csPackage = csPackage;
    }

    //同步请求
    public void RequestSync()
    {
        var csPackage = m_csPackage;
        var webRequest = CreateHttpWebRequest(csPackage.m_url, csPackage.m_param, csPackage.m_isPost);

        if (webRequest == null)
        {
            FailedCallback(csPackage.m_msgKey, csPackage.m_seq, csPackage.m_sucCB);
            return;
        }


        m_cacheIR = new AsynState();
        m_cacheIR.m_webRequest = webRequest;
        m_cacheIR.m_sucCallBack = csPackage.m_sucCB;
        m_cacheIR.m_msgKey = csPackage.m_msgKey;
        m_cacheIR.m_seq = csPackage.m_seq;
        m_cacheIR.m_reqStream = null;
        m_cacheIR.m_response = null;
        m_cacheIR.m_responseStream = null;

        GamePackage package = null;
        BinaryReader br = null;

        byte[] data = Encoding.GetEncoding("utf-8").GetBytes(csPackage.m_param);

        try
        {
            //上报数据
            m_cacheIR.m_reqStream = webRequest.GetRequestStream();
            m_cacheIR.m_reqStream.Write(data, 0, data.Length);
            m_cacheIR.m_reqStream.Flush();
            m_cacheIR.m_reqStream.Dispose();

            //返回数据
            m_cacheIR.m_response = (HttpWebResponse)webRequest.GetResponse();

            if (m_cacheIR.m_response != null)
            {
                m_cacheIR.m_responseStream = m_cacheIR.m_response.GetResponseStream();
                //StreamReader streamRead = new StreamReader(responseStream, Encoding.UTF8);
                //string responseString = streamRead.ReadToEnd();
                //Debug.LogError(responseString);
                br = new BinaryReader(m_cacheIR.m_responseStream);
                int contentLength = 1024 * 10;
                if (contentLength > 0)
                {
                    byte[] content = br.ReadBytes(contentLength);
                    package = Unpack(content);
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
            m_isExcept = true;
            return;
        }
        finally
        {
            if (br != null)
            {
                br.Close();
                br = null;
            }

            CloseHttpWepRequest(m_cacheIR);
            m_cacheIR = null;
            if(package != null)
            {
                m_isHandled = true;
                WebSCPackage webPackage = new WebSCPackage(csPackage.m_msgKey, package, csPackage.m_seq, csPackage.m_sucCB);
                if (m_httpConnection != null)
                    m_httpConnection.EnqueueRespondMsg(webPackage);
            }
        }
    }



    //异步请求
    public void RequestAsync()
    {
        var csPackage = m_csPackage;

        var webRequest = CreateHttpWebRequest(csPackage.m_url, csPackage.m_param, csPackage.m_isPost);
        if (webRequest == null)
        {
            FailedCallback(csPackage.m_msgKey, csPackage.m_seq, csPackage.m_sucCB);
            return;
        }
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
                m_isExcept = true;
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

        try
        {
            var ar = webRequest.BeginGetResponse(RequestCallback, resultState);
        }
        catch (Exception e)
        {
            m_isExcept = true;
            UnityEngine.Debug.LogError("DWHttpClient BeginGetResponse   Exception : " + e.Message);
            return;
        }
        finally
        {

        }
    }

    //异步回调
    void RequestCallback(IAsyncResult ar)
    {
        GamePackage package = null;
        AsynState resultState = null;
        BinaryReader br = null;
        if (ar != null)
        {
            resultState = (AsynState)ar.AsyncState;
        }
        try
        {
            if (resultState != null)
            {
                resultState.m_response = (HttpWebResponse)resultState.m_webRequest.EndGetResponse(ar);
                if (resultState.m_response != null)
                {
                    resultState.m_responseStream = resultState.m_response.GetResponseStream();
                    //StreamReader streamRead = new StreamReader(resultState.m_responseStream, Encoding.UTF8);
                    //string responseString = streamRead.ReadToEnd();
                    br = new BinaryReader(resultState.m_responseStream);
                    int contentLength = 1024 * 10;
                    if (contentLength > 0)
                    {
                        byte[] content = br.ReadBytes(contentLength);
                        //byte[] content = System.Text.Encoding.Default.GetBytes(responseString);
                        package = Unpack(content);
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
            m_isExcept = true;
            UnityEngine.Debug.LogError("DWHttpClient.RequestCallback EndGetResponse  Exception : " + e.Message);
            return;
        }
        finally
        {
            if (br != null)
            {
                br.Close();
                br = null;
            }

            if (resultState != null && package != null)
            {
                m_isHandled = true;
                WebSCPackage webPackage = new WebSCPackage(resultState.m_msgKey, package, resultState.m_sucCallBack);

                if (m_httpConnection != null)
                    m_httpConnection.EnqueueRespondMsg(webPackage);

                CloseHttpWepRequest(resultState);
            }
            else
            {
                m_isExcept = true;
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


    void FailedCallback(string msgKey, int seq, Action<GamePackage> callBack)
    {
        GamePackage package = new GamePackage();
        package.errno = -1;
        WebSCPackage webPackage = new WebSCPackage(msgKey, package, seq, callBack);
        if (m_httpConnection != null)
        {
            m_httpConnection.EnqueueRespondMsg(webPackage);
        }
    }

    //检查是否超时
    public bool CheckIsTimeOut(double nowTime,double offset)
    {
        return (nowTime - offset) >= m_requsetEndTime;
    }

    //检查连接是否异常
    public bool CheckIsExcept()
    {
        return m_isExcept;
    }

    //外部调用失败接口
    public void HanldeFailCB()
    {
        if (m_isHandled)
        {
            UnityEngine.Debug.LogError(m_csPackage.m_msgKey + "is alreday HanldeFailCB");
            return;
        }

        if (m_csPackage != null)
        {
            m_isHandled = true;
            UnityEngine.Debug.LogError(m_csPackage.m_msgKey + " HanldeFailCB");
            FailedCallback(m_csPackage.m_msgKey, m_csPackage.m_seq, m_csPackage.m_sucCB);
            CloseHttpWepRequest(m_cacheIR);
            m_cacheIR = null;
        }
    }

    //创建http连接
    protected HttpWebRequest CreateHttpWebRequest(string url, string param, bool post, int range = 0,float timeOut = 8)
    {
        if (!post && !string.IsNullOrEmpty(param))
            url = string.Format("{0}?{1}", url, param);
        //http连接请求对象
        HttpWebRequest webRequest = null;
        try
        {
            webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.ProtocolVersion = HttpVersion.Version11;
            webRequest.Accept = "text/html";
            webRequest.Method = post ? "POST" : "GET";
            webRequest.ReadWriteTimeout = (int)(timeOut * 1000);
            webRequest.Timeout = (int)(timeOut * 1000);
            webRequest.ContentType = "application/x-www-form-urlencoded";

            if (range > 0)
                webRequest.AddRange(range);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("DWHttpClient CreateHttpWebRequest Exception : " + e.Message);
            m_isExcept = true;
            if (webRequest != null)
            {
                webRequest.Abort();
                webRequest = null;

            }

        }
        return webRequest;
    }


    //关闭连接
    protected void CloseHttpWepRequest(AsynState state)
    {
        if (state == null)
        {
            return;
        }
        if (state.m_reqStream != null)
            state.m_reqStream.Close();

        if (state.m_responseStream != null)
        {
            state.m_responseStream.Close();
            state.m_responseStream = null;
        }
        if (state.m_response != null)
        {
            state.m_response.Close();
            state.m_response = null;
        }

        if (state.m_webRequest != null)
            state.m_webRequest.Abort();

    }
}
