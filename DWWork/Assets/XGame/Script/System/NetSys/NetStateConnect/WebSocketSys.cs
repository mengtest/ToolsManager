using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using LitJson;
using System.Text.RegularExpressions;
using LuaInterface;


[RegisterSystem(typeof(WebSocketSys), true)]
public class WebSocketSys : TCoreSystem<WebSocketSys>, IInitializeable, IUpdateable, IResetable
{
    public enum HttpBlockState
    {
        Block1,
        Block2,
        Normal
    }
    public DWHttpConnection m_httpConnection;

    private bool m_hasInit = false;

    protected Dictionary<string, CHttpDealerOP> m_sendRecordMap = new Dictionary<string, CHttpDealerOP>();
    private HttpBlockState m_blockState = HttpBlockState.Normal;

    public void Init()
    {
        if (m_hasInit)
        {
            return;
        }
        System.Net.ServicePointManager.DefaultConnectionLimit = 50;
        m_hasInit = true;
        m_httpConnection = new DWHttpConnection(LogicRev);
        m_httpConnection.StartWork();
    }

    public void ReStartWork()
    {
        if(m_httpConnection != null)
        {
            m_httpConnection.ReStartWork();
        }
    }

    public void Reset()
    {
        m_sendRecordMap.Clear();
    }

    public void Release()
    {
//        Debug.LogError("WebSocket Quit");
        m_httpConnection.CloseThread();
    }

    #region wifi
    bool m_bWifi = false;
    public bool CheckWifiEnv
    {
        get
        {
            return m_bWifi;
        }
    }

    public void NotifyWifiEnv()
    {
        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            m_bWifi = true;
        }
    }
    #endregion

    public void Update()
    {
        m_httpConnection.Update();

  //      onBlockWaitTick();
    }


    public void SendMsg(string msgKey,string url,string param,bool isPost,int seq,int timeOut)
    {
        WebCSPackage csPackage = new WebCSPackage();
        csPackage.m_msgKey = msgKey;
        csPackage.m_url = url;
        csPackage.m_param = param;
        csPackage.m_seq = seq;
        csPackage.m_isPost = isPost;
        csPackage.m_timeOut = timeOut;
        //if (!m_sendRecordMap.ContainsKey(msgKey))
        //{
        //    m_sendRecordMap.Add(msgKey, new CHttpDealerOP(new CNetSendOption()));
        //}
        m_httpConnection.SendMsg(csPackage);
    }

    public void SendMsg(string msgKey,string url,string param, bool isPost,int seq ,Action<GamePackage> succCB)
    {
        WebCSPackage csPackage = new WebCSPackage();
        csPackage.m_msgKey = msgKey;
        csPackage.m_url = url;
        csPackage.m_param = param;
        csPackage.m_isPost = isPost;
        csPackage.m_seq = seq;
        csPackage.m_sucCB = succCB;

 //       if (!m_sendRecordMap.ContainsKey(msgKey))
 //       {
 ////           m_sendRecordMap.Add(msgKey, new CHttpDealerOP(new CNetSendOption()));
 //       }

        m_httpConnection.SendMsg(csPackage);
    }

    public void LogicRev(WebSCPackage package)
    {
        if(package != null)
        {
            //移除监听
 //           m_sendRecordMap.Remove(package.m_msgKey);

            //if(m_blockState == HttpBlockState.Block1)
            //{
            //    //EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.wait_ui_window, false);
            //    EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window");
            //}

            m_blockState = HttpBlockState.Normal;

            if (package.m_pack != null)
            {
                if(package.m_sucCB != null)//如果C#层注册了回调
                    package.m_sucCB(package.m_pack);
                else
                {
                    LuaRootSys.Instance.HandleHttpLuaSuccCallBack(package);
                }
            }
        }
    }

    protected void onBlockWaitTick()
    {
        if(m_blockState == HttpBlockState.Normal)
        {
            var ids = m_sendRecordMap.Keys;
            var enm = ids.GetEnumerator();
            while (enm.MoveNext())
            {
                var dealer = m_sendRecordMap[enm.Current];
                DateTime now = DateTime.Now;
                if (dealer.m_op.m_startBlock > 0 && now > dealer.m_sendTime.AddMilliseconds(dealer.m_op.m_startBlock))
                {
                    m_blockState = HttpBlockState.Block1;
                    //EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.wait_ui_window, RessType.RT_CommonWindow, true, 2);
                    EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, true, 20000+10, "wait_ui_window");
                    return;
                }
            }
        }
        else if(m_blockState == HttpBlockState.Block1)
        {
            //var ids = m_sendRecordMap.Keys;
            //var enm = ids.GetEnumerator();
            //while (enm.MoveNext())
            //{
            //    var dealer = m_sendRecordMap[enm.Current];
            //    DateTime now = DateTime.Now;
            //    if (dealer.m_op.m_startBlock > 0 && now > dealer.m_sendTime.AddMilliseconds(dealer.m_op.m_timeout))
            //    {
            //        m_blockState = HttpBlockState.Block2;
            //        if (!CNetSys.Instance.NetStateConnect.m_isReconnect)
            //        {
            //            //EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.wait_ui_window, false);
            //            EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window");
            //        }
            //        LuaRootSys.Instance.HttpReconnect();
            //        break;
            //    }
            //}
            //if(m_blockState == HttpBlockState.Block2)
            //{
            //    m_blockState = HttpBlockState.Normal;
            //    enm = ids.GetEnumerator();
            //    while (enm.MoveNext())
            //    {
            //        WebSCPackage webPackage = new WebSCPackage();
            //        webPackage.m_pack = new GamePackage();
            //        webPackage.m_msgKey = enm.Current;
            //        webPackage.m_pack.errno = -1;
            //        LuaRootSys.Instance.HandleHttpLuaSuccCallBack(webPackage);
            //    }
            //    m_sendRecordMap.Clear();
            //}
        }
    }

}