/************************************************************
//     文件名      : NetStateConnectBase.cs
//     功能描述    : 网络状态连接基类
//     负责人      : lezen   lezenzeng@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-07-10 17:36:27.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using ProtoBuf;

public delegate bool LuaCMDCheckDelegate(int cmd);
public delegate void LoginSvrDelegate(string netName);
public class NetStateConnectBase
{
    public class StateEqualityComparer : IEqualityComparer<ENETSYSSTATE>
    {
        public bool Equals(ENETSYSSTATE x, ENETSYSSTATE y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(ENETSYSSTATE obj)
        {
            return obj.GetHashCode();
        }
    }

    const int MAX_AUTO_RECONNECT_TIME = 3;

    protected string m_key = "";

    public string NetName
    {
        get { return m_key; }
    }

    protected CNetConnection m_netConnection;
    public virtual CNetConnection NetConnection
    {
        get
        {
            return m_netConnection;
        }
    }

    //网络的状态机
    protected TransferState<ENETSYSSTATE> m_stNetState;
    public TransferState<ENETSYSSTATE> NetState
    {
        get
        {
            return m_stNetState;
        }
    }
    //阻塞回调处理
    public BlockCallBack m_BlockCallBack;
    //block的状态机
    protected TransferState<int> m_stBlockState = null;
    public TransferState<int> BlockState
    {
        get
        {
            return m_stBlockState;
        }
    }
    //重新登陆回调
    protected LoginSvrDelegate m_reLoginCallBack;
    //lua协议检查回调
    protected LuaCMDCheckDelegate m_isLuaCMDCheck;
    //登陆成功回调
    protected NetLoginedCallBack m_LoginedCallBack;
    public NetLoginedCallBack LoginedCallBack
    {
        set
        {
            m_LoginedCallBack = value;
        }
        get
        {
            return m_LoginedCallBack;
        }
    }

    protected LoginSvrDelegate m_reconnectSuccessCB;


    public bool m_isReconnect;
    //网络是否激活
    public bool m_isNetActiveWork = false;

    protected CDealerMap<int, CDealerCB> m_mapDealer = new CDealerMap<int, CDealerCB>();
    protected Dictionary<Int32, CDealerCBOP> m_mapCallBack = new Dictionary<Int32, CDealerCBOP>();

    //等待回包的请求队列
    protected Dictionary<int, CDealerCBOP> m_waitPackDic = new Dictionary<int, CDealerCBOP>();

    //序列号
    public Int32 m_Seq = 0;
    //用户是否从游戏中退出到登陆界面
    public bool m_isForceOut = false;



    public void Init(CNetConnection netConnection, LoginSvrDelegate loginCB,
        LuaCMDCheckDelegate luaCMDCheck, LoginSvrDelegate reconnectSucCB = null)
    {
        m_netConnection = netConnection;
        m_reLoginCallBack = loginCB;
        m_isLuaCMDCheck = luaCMDCheck;
        m_reconnectSuccessCB = reconnectSucCB;
        BaseInit();
        BlockStateInit();
        NetStateInit();
    }

    protected virtual void BaseInit()
    {

    }

    #region Net State
    public virtual void NetStateInit()
    {
        m_stNetState = new TransferState<ENETSYSSTATE>(ENETSYSSTATE.None, new StateEqualityComparer());
    }
    #endregion

    #region Block State

    private class IntEqualityComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            return x == y;
        }

        public int GetHashCode(int obj)
        {
            return obj;
        }
    }
    private void BlockStateInit()
    {
        m_stBlockState = new TransferState<int>(EBLOCKSTATE.normal,  new IntEqualityComparer());
        m_stBlockState.m_transfer = (int pre, TransferStateInput input ,int aft) =>
        {
            if (m_BlockCallBack != null)
            {
                m_BlockCallBack(pre, input, aft);
            }
            return false;
        };
        //正常状态发送消息之后转成锁屏状态
        m_stBlockState.addTransfer(EBLOCKSTATE.normal,CNetSys.DNetSysInputSendMsg, EBLOCKSTATE.wait1, onBlockSendMsg);
        //锁屏状态到时间之后转成菊花状态
        m_stBlockState.addTransfer(EBLOCKSTATE.wait1, CNetSys.DNetSysInputTick, EBLOCKSTATE.wait2, onBlockWait1Tick);
        //菊花状态检查超时onBlockWait2Tick 必然返回false
        m_stBlockState.addTransfer(EBLOCKSTATE.wait2, CNetSys.DNetSysInputTick, EBLOCKSTATE.wait2, onBlockWait2Tick);
        //菊花状态收到消息之后队列为空转成正常状态
        m_stBlockState.addTransfer(EBLOCKSTATE.wait2, CNetSys.DNetSysInputRecvMsg, EBLOCKSTATE.normal, onBlockRecvIsNull);
        //菊花状态收到消息之后队列里面没有到时间的转成锁屏状态
        m_stBlockState.addTransfer(EBLOCKSTATE.wait2, CNetSys.DNetSysInputRecvMsg, EBLOCKSTATE.wait1, onBlockRecvIsTime);
        //锁屏状态收到消息之后队列为空转成正常状态
        m_stBlockState.addTransfer(EBLOCKSTATE.wait1, CNetSys.DNetSysInputRecvMsg, EBLOCKSTATE.normal, onBlockRecvIsNull);
        //强制转成正常状态
        m_stBlockState.addTransferAll(CNetSys.DNetSysInputForceReset, EBLOCKSTATE.normal, onBlockForceReset);
    }

    protected virtual bool onBlockSendMsg(int curState, TransferStateInput input, int dstState)
    {
        var inputMsg = input as TransferStateInputMsg<CDealerCBOP>;
        if (inputMsg.m_sMsg.m_op.m_startBlock < 0)
        {
            return false;
        }
        return true;
    }
    protected virtual bool onBlockWait1Tick(int curState, TransferStateInput input, int dstState)
    {
        var ids = m_mapCallBack.Keys;
        var enm = ids.GetEnumerator();
        while(enm.MoveNext())
        {
            CDealerCBOP dealer = m_mapCallBack[enm.Current];
            DateTime now = DateTime.Now;
            if (dealer.m_op.m_startBlock > 0 && now > dealer.m_sendTime.AddMilliseconds(dealer.m_op.m_startBlock))
            {
                return true;
            }
        }
        return false;
    }
    protected virtual bool onBlockWait2Tick(int curState, TransferStateInput input, int dstState)
    {
        //if (IsInLoading)//加载不处理阻塞
        //{
        //    return false;
        //}
        int[] ids = new int[m_mapCallBack.Count];
        m_mapCallBack.Keys.CopyTo(ids, 0);
        for (int i = 0; i < ids.Length; ++i)
        {
            var id = ids[i];
            if (m_mapCallBack.ContainsKey(id))
            {
                CDealerCBOP dealer = m_mapCallBack[id];
                DateTime now = DateTime.Now;
                if (!dealer.m_isAddWaitQueue && now > dealer.m_sendTime.AddMilliseconds(dealer.m_op.m_timeout))
                {
                    //                if (!CAccMgr.Instance.m_isKickedOff)
                    {
                        if (!dealer.m_canAbandon)
                        {

                            ResetAutoConnect();
#if UNITY_EDITOR
                            NetDebug(("onBlockWait2Tick" + dealer.m_req.cmd).ToString());
#endif
                            dealer.m_isAddWaitQueue = true;

                            return false;
                        }
                        //else
                        //{
                        //    var msg = new GamePackage();
                        //    msg.cmd = dealer.m_req.cmd;
                        //    msg.seq = dealer.m_req.seq;
                        //    msg.errno = (int)ezfun.CS_ERRNO.COMMON_CLIENT_TIMEOUT_FAILCALLBACK;
                        //    LogicRecvPack(msg);
                        //}
                    }
                }
            }
        }
        return false;
    }
    protected virtual bool onBlockRecvIsNull(int curState, TransferStateInput input, int dstState)
    {
        return m_mapCallBack.Count == 0;
    }
    protected virtual bool onBlockRecvIsTime(int curState, TransferStateInput input, int dstState)
    {
        var entry = m_mapCallBack.GetEnumerator();
        while(entry.MoveNext())
        {
            if (entry.Current.Value.m_op.m_startBlock > 0 && entry.Current.Value.m_sendTime.AddMilliseconds(entry.Current.Value.m_op.m_startBlock) > DateTime.Now)
            {
                return false;
            }
        }
        return true;
    }
    protected virtual bool onBlockForceReset(int curState, TransferStateInput input, int dstState)
    {
        m_mapCallBack.Clear();
        return true;
    }

    #endregion


    #region logic
    public virtual void LogicRecvPack(GamePackage msg)
    {
        if(msg == null)
        {
            return;
        }
        var list = m_mapDealer.GetDealer(msg.cmd);
        bool isCalled = false;
        //目前C#层不发送消息
        bool isLuaCmd = true;
        if (list != null)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                var entry = list[i];
                var cb = entry.cb as NetMsgCallBack;
                try
                {
                    if (cb != null)
                    {
                        isCalled = true;
                        cb(msg);
                    }
                }
                catch (Exception e)
                {
                    CNetSys.Instance.SendImportantLog("[Exception]" + e.ToString());
                    //Debug.LogError("[Exception]" + e.ToString());
					Debug.LogException(e);
                }
            }
        }
        CDealerCBOP clist = null;

        if (m_mapCallBack.TryGetValue(msg.seq, out clist))
        {
            var cb = clist.cb as NetMsgCallBack;
            try
            {
                if (cb != null)
                {
                    isCalled = true;
                    cb(msg);
                }
            }
            catch (Exception e)
            {
                CNetSys.Instance.SendImportantLog("[Exception]" + e.ToString());
                Debug.LogError("[Exception]" + e.ToString());
            }
        }
        else if (m_waitPackDic.ContainsKey(msg.seq))
        {
            CDealerCBOP entry = m_waitPackDic[msg.seq];
            var cb = entry.cb as NetMsgCallBack;
            try
            {
                if (cb != null)
                {
                    cb(msg);
                }
            }
            catch (Exception e)
            {
                CNetSys.Instance.SendImportantLog("[Exception]" + e.ToString());
                Debug.LogError("[Exception]" + e.ToString());
            }
        }
        if (isLuaCmd && !isCalled)
        {
            try
            {
                LuaRootSys.Instance.HandleLuaSuccCallBack(NetName,msg);
            }
            catch (Exception e)
            {
                CNetSys.Instance.SendImportantLog("[Exception]" + e.ToString());
                Debug.LogError("[Exception]" + e.ToString());
            }
        }
        //}

        m_mapCallBack.Remove((Int32)msg.seq);

        if (msg.errno != 3 && m_waitPackDic.ContainsKey((Int32)msg.seq))
        {
            m_waitPackDic.Remove((Int32)msg.seq);
        }
        m_stBlockState.InputData(new TransferStateInputMsg<GamePackage>(msg));
    }

    private void SendMsg(int cmd, GamePackage body, NetMsgCallBack dCallBack = null,
       CNetSendOption op = null, bool canAbandon = false, int seq = 0,object parentTarget = null)
    {

        //Debug.Log(DateTime.Now.TimeOfDay.ToString() +  "send msg cmd:" + cmd + " seq:" + (m_Seq+1));
#if WITHOUT_NETWORK
        LocalNetSimulator.OnSendMsg(msg, iCmdId);
        return;
#endif
        if (op == null)
        {
            op = new CNetSendOption(-1,false);
        }

        if (seq == 0)
        {
            m_Seq += 2;
            seq = m_Seq;
        }
        body.seq = seq;

        if (!canAbandon)
        {
            var cb = new CDealerCBOP(dCallBack, op, body, canAbandon, parentTarget);

            if (!m_mapCallBack.ContainsKey(body.seq))
            {
                m_mapCallBack.Add(body.seq, cb);
            }
            else
            {
                m_mapCallBack[body.seq] = cb;
            }
            if (m_waitPackDic.ContainsKey(body.seq))
            {
                m_waitPackDic[body.seq] = cb;
            }
            else
            {
                m_waitPackDic.Add(body.seq, cb);
            }
            m_stBlockState.InputData(new TransferStateInputMsg<CDealerCBOP>(cb));
        }

        if (CheckConnected())
        {
            SendMsg(body);
        }
    }

    public void SendNetMsg(int cmd, GamePackage body, NetMsgCallBack dCallBack, bool is_special_cbk, NetMsgSpecialFailCallBack failCallBack,
                          bool canAbandon = false, bool is_cbk_after_error_window = false, CNetSendOption op = null, bool needPopErrWindow = true,
                          bool needCheckNetConnect = false, NetMsgFailCallBack notConnectCB = null, int seq = 0)
    {
        if(!canAbandon && op == null)
        {
            op = new CNetSendOption();
        }

        SendMsg(cmd, body, (GamePackage msg) =>
        {
            HandleNetErrInfo(msg, dCallBack, failCallBack, canAbandon, is_cbk_after_error_window, needPopErrWindow);
        }, op, canAbandon, seq, (dCallBack != null) ? dCallBack.Target : null);
    }

    protected virtual void HandleNetErrInfo(GamePackage msg, NetMsgCallBack dCallBack, NetMsgSpecialFailCallBack failCallBack, bool canAbandon,
                          bool is_cbk_after_error_window, bool needPopErrWindow)
    {

        switch (msg.errno)
        {
            case 0:
                if (dCallBack != null)
                    dCallBack(msg);
                break;
            case -1:
                if (dCallBack != null)
                    dCallBack(msg);
                break;
            default: // TODO handle different errno
				Debug.LogError("receive errno:" + msg.errno);

                if (needPopErrWindow)
                {

                    WindowRoot.ShowTips((int)msg.errno);
                       
                }

                if (is_cbk_after_error_window)
                {
                    HandleErrorWindow.m_okCallBack = (object p1, object p2) => { failCallBack(msg); };
                    return;
                }
                //else if (failCallBack != null)
                //{
                //    failCallBack(msg);
                //}
                break;
        }

        //有写错误处理回调 应该都调用
        if (msg.errno != 0 && failCallBack != null)
        {
            failCallBack(msg);
        }
    }

    public bool CheckConnected()
    {
        bool connect = m_netConnection != null && m_netConnection.State == NetConnectionBase.NETSTATE.ns_Connected;
        return connect;
    }


    protected virtual void SendMsg(GamePackage msg)
    {

    }

    public void RegisterMsgHandler(int Cmd, NetMsgCallBack dCallBack)
    {
        m_mapDealer.AddHandle(Cmd, new CDealerCB(dCallBack));
    }

    public void RemoveMsgHandle(int Cmd, NetMsgCallBack dCallBack)
    {
        m_mapDealer.RemoveHandleById(Cmd, new CDealerCB(dCallBack));
    }

    private static List<int> m_rmList = new List<int>();
    public void RemoveHander(object target)
    {
        m_mapDealer.RemoveHandleByTarget(target);
        m_rmList.Clear();
        var mapEnum = m_mapCallBack.GetEnumerator();
        while(mapEnum.MoveNext())
        {
            var cur = mapEnum.Current;
            if (cur.Value == null || cur.Value.EqualsTarget(target))
            {
                m_rmList.Add(cur.Key);
            }
        }

        for (int i = 0,len = m_rmList.Count; i < m_rmList.Count; i ++)
        {
            m_mapCallBack.Remove(m_rmList[i]);
            m_waitPackDic.Remove(m_rmList[i]);
        }
    }

    public void DisConnect()
    {
        if (m_netConnection == null)
        {
            return;
        }

        m_netConnection.Disconnect(true);
        m_stBlockState.InputData(CNetSys.DNetSysInputForceReset);
    }

    public void ResetToken()
    {
        m_isForceOut = true;
        m_netConnection.Disconnect(true);
        m_stBlockState.InputData(CNetSys.DNetSysInputForceReset);
#if UNITY_EDITOR
        NetDebug("rest close main connect");
#endif
    }

    #endregion


    #region Auto Reconnect
    private int m_mainConnectRetryTimes = 0;
    private int m_lastRetryTimes = 0;
    private bool m_isRetry = false;
    private const float DELTA_RETRY_TIME = 8f;
    public delegate void ReConnectSucCallBack();
    public class ReconnectPara
    {
        public ReConnectSucCallBack m_cb;
        public object m_target;
        public ReconnectPara(object target,ReConnectSucCallBack cb)
        {
            m_target = target;
            m_cb = cb;
        }
    }
    public Dictionary<int, ReconnectPara> m_reConnectSucCallBack = new Dictionary<int, ReconnectPara>();

    public bool IsRetryConnection
    {
        set { m_isRetry = value; }
    }
    public bool IsRetry()
    {
        return m_isRetry;
    }

    public void ResetAutoConnect()
    {
        if (m_isRetry == true || m_isForceOut)//|| CAccMgr.Instance.m_isKickedOff 
        {
            return;
        }
#if UNITY_EDITOR
        NetDebug( NetName +" ResetAutoConnect success");
#endif
        m_mainConnectRetryTimes = 0;
        m_isRetry = true;
        m_lastRetryTimes = TimerSys.Instance.GetCurrentDateTime() - (int)DELTA_RETRY_TIME;
        DisConnect();

        CacheCallBackFail();
        Clear();
    }
    //让缓存的回调走失败，有一条消息失败会触发重连，清理当前缓存的消息回调
    private void CacheCallBackFail()
    {
        int len = m_mapCallBack.Count;
        if(len > 0)
        {
            int[] ids = new int[len];
            m_mapCallBack.Keys.CopyTo(ids, 0);
            for (int i = 0; i < ids.Length; ++i)
            {
                var id = ids[i];
                if (m_mapCallBack.ContainsKey(id))
                {
                    CDealerCBOP dealer = m_mapCallBack[id];
                    var msg = new GamePackage();
                    msg.cmd = dealer.m_req.cmd;
                    msg.seq = dealer.m_req.seq;
                    msg.areaID = dealer.m_req.areaID;
                    msg.errno = -1;
                    LogicRecvPack(msg);
                }
            }
        }
    
    }


    public void GiveUpAutoConnect()
    {
        m_isNetActiveWork = false;

        m_isRetry = false;
        m_isReconnect = false;
        DisConnect();
    }




    private void UpdateRetryTimes()
    {
        if (m_isRetry && TimerSys.Instance.GetCurrentDateTime() - m_lastRetryTimes >= DELTA_RETRY_TIME)
        {
            TryAutoConnect();
        }
    }

    protected virtual void TryAutoConnect()
    {
        //自动重连超过3次 并且不在游戏中
        if (m_mainConnectRetryTimes > MAX_AUTO_RECONNECT_TIME && GameStateMgr.Instance.CheckNeedNet())
        {
            m_isRetry = false;//自动连接失败，置位标志，弹窗提示玩家断网
            //EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.wait_ui_window, false);
            EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window");
            ShowErrorWindow();
        }
        else
        {
            //auto connect 
            //            Debug.LogError("[Retry Times]" + m_mainConnectRetryTimes);
            //EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.wait_ui_window, RessType.RT_CommonWindow, true,2);
            EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, true, 20000+10, "wait_ui_window");
            ++m_mainConnectRetryTimes;
            m_lastRetryTimes = TimerSys.Instance.GetCurrentDateTime();
            m_netConnection.Disconnect(true);
            m_isReconnect = true;
            //重新登陆
            m_reLoginCallBack(NetName);
        }
    }

    public void RetryLoginSuccess()
    {
        retrySeq = 0;
        m_isRetry = false;
//        var kv = m_reConnectSucCallBack.GetEnumerator();
//        while(kv.MoveNext())
//        {
//#if UNITY_EDITOR
//            NetDebug("reConnectSucCallBack " + kv.Current.Key.ToString());
//#endif
//            kv.Current.Value.m_cb();
//        }
        ClearReconnectCallBack();
        m_stBlockState.InputData(CNetSys.DNetSysInputForceReset);
    }

    public void ShowErrorWindow(bool needCheck = true)
    {
        if(!m_isNetActiveWork)//网络已经不激活工作，不需要弹窗了
        {
            return;
        }
        if (m_isRetry)//正在自动连接，不需要弹窗
            return;
#if UNITY_EDITOR
        NetDebug("try ShowErrorWindow");
#endif
        if (!GameStateMgr.Instance.CheckNeedNet() && needCheck)
        {
            return;
        }
            HandleNetTimeOutWindow.AddOkCallBack(m_key, () =>
        {
            HandRetry(0, needCheck);
        });

        if (EZFunWindowMgr.Instance.CheckWindowOpen(EZFunWindowEnum.reconnect_ui_window) && GameStateMgr.Instance.GetCurStateType() != EGameStateType.LoginState)//已经弹出重连窗，不需要弹
        {
            return;
        }
        m_isRetry = false;
        m_netConnection.Disconnect(true);

        HandleNetTimeOutWindow.m_noCallBack = () =>
        {
            EventSys.Instance.AddEvent(EEventType.UI_Msg_LoginOut);
        };
        int state = 1;
        if (GameStateMgr.Instance.GetCurStateType() == EGameStateType.LoginState)
        {
            state = 2;
        }
#if UNITY_EDITOR
        NetDebug("ShowErrorWindow success");
#endif
        EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.reconnect_ui_window, RessType.RT_Window, true, state);
    }


    int retrySeq = 0;
    private void HandRetry(int times, bool needCheck = true)
    {
        retrySeq++;
        int m_retrySeq = retrySeq;
        //        Debug.LogError("Retry:" + times);
        m_isReconnect = true;
        m_reLoginCallBack(NetName);

        if (!GameStateMgr.Instance.CheckNeedNet() && needCheck)
        {
        }
        else
        {
            //EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.wait_ui_window, RessType.RT_CommonWindow, true, 2);
            EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, true, 20000+10, "wait_ui_window");
        }

        TimerSys.Instance.AddTimerEventByLeftTime(() =>
        {
//            Debug.LogError("m_retrySeq=" + m_retrySeq + ",retrySeq=" + retrySeq);
            if (m_retrySeq != retrySeq)
            {
                return;
            }

            if (!m_netConnection.IsConnected() && m_isNetActiveWork)
            {
                if (times == 0)
                {
                    HandRetry(1, needCheck);
                }
                else if (times == 1)
                {
                    //EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.wait_ui_window, false);
                    EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window");
                    ShowErrorWindow(needCheck);
                }
            }
            else
            {
                //EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.wait_ui_window, false);
                EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window");
            }
        }, DELTA_RETRY_TIME);
    }

    public void HandleTimeOut(GamePackage msg, NetMsgCallBack dCallBack, NetMsgSpecialFailCallBack failCallBack, bool canAbandon, bool needPopErrWindow)
    {
        if (needPopErrWindow)
        {
            HandleNetTimeOutWindow.AddOkCallBack(m_key, () =>
             {
                 if (failCallBack != null)
                 {
                     failCallBack(msg);
                 }
                 HandleTimeOutRetry();
             });

            EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.reconnect_ui_window, RessType.RT_Window, true);
        }
        else
        {
            if (failCallBack != null)
            {
                failCallBack(msg);
            }

            HandleTimeOutRetry();
        }
    }

    private void HandleTimeOutRetry()
    {
        if (NetConnection != null && NetConnection.State == NetConnectionBase.NETSTATE.ns_Connected)
        {
            NetConnection.Disconnect(true);
            HandRetry(0);
        }
    }



#endregion

    public virtual void ConnectServer(string strAddr, string strIp, int nPort, NetToken token, ConnectCallBack ConnetCallback, ConnectRecvProtoMsg<GamePackage> connectRecvPack, GamePackage cmsg)
    {

    }
    public void Update()
    {
        m_netConnection.Update();

        if (null != m_stBlockState)
        {
            m_stBlockState.InputData(CNetSys.DNetSysInputTick);
        }
        UpdateRetryTimes();
    }


    public void Clear()
    {
        m_mapCallBack.Clear();
        m_waitPackDic.Clear();
    }


    public void OnApplicationQuit()
    {
        m_netConnection.CloseThread();
    }


    public void Reset()
    {
        ClearReconnectCallBack();
    }


#region reconnect callback
    bool m_isHaveChangeMapCmd = false;
    public void AddReconnectCallBack(int cmd,ReconnectPara reconnectPara,int seq = -1)
    {
        if(!m_reConnectSucCallBack.ContainsKey(cmd))
        {
#if UNITY_EDITOR
            NetDebug("AddReconnectCallBack: " + cmd.ToString());
#endif
            m_reConnectSucCallBack.Add(cmd,reconnectPara);
        }
    }

    public void RemoveReconnectCallBack(int cmd)
    {
        m_reConnectSucCallBack.Remove(cmd);
    }

    public void ClearReconnectCallBack()
    {
        m_isHaveChangeMapCmd = false;
        m_reConnectSucCallBack.Clear();
    }

#endregion

    public bool isEnableDebug = true;
    public void NetDebug(string log)
    {
        if(isEnableDebug)
            Debug.LogError(log);
    }
}
