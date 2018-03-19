using NIM;
using NIM.Session;
using NIM.SysMessage;
using NIM.SysMessage.Delegate;
using NimUtility;
using NimUtility.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DWChatTalk
{
    private NIMAudioAttachment _audioAttachment = null;
    private System.Timers.Timer capture_timer_ = null;
    private System.Timers.Timer play_timer_ = null;
    //private bool speaker_ = false;
    private string _senderid;
    private bool is_capture_audio = false;

    //记录的语音信息
    private struct DicInfo
    {
        public string senderID;
        public int dur;
        public NIMAudioMessage nimImMsgContent;
        public DicInfo(string _senderID, int _dur, NIMAudioMessage _nimImMsgContent) 
        {
            senderID = _senderID;
            dur = _dur;
            nimImMsgContent = _nimImMsgContent;
        }
    }
    private Dictionary<string, DicInfo> _callID_senderID_DIC = new Dictionary<string, DicInfo>();
    List<string> capture_devices = null;
    AndroidJavaObject context;

    private volatile static DWChatTalk _instance = null;
    private static readonly object lockHelper = new object();
    private DWChatTalk() { }
    public static DWChatTalk getInstance()
    {
        if (_instance == null)
        {
            lock (lockHelper)
            {
                if (_instance == null)
                    _instance = new DWChatTalk();
            }
        }
        return _instance;
    }

    public void DW_initTalk()
    {
        DWChatUtils.DebugLog("DW_initTalk=========================");
        TalkAPI.OnSendMessageCompleted += OnSendMessageResult;
	    TalkAPI.OnReceiveMessageHandler += OnMessageReceived;

        NIM.Nos.NosAPI.RegDownloadCb(onMedioDownload);
        NIMAudio.AudioAPI.RegStartPlayCb(StartPlayCallback);
        NIMAudio.AudioAPI.RegGetPlayCurrentPositionCb(AudioPlayCurrentPositionCallback);
        NIMAudio.AudioAPI.RegPlayEndCb(EndPlayCallback);
        InitAudio();
    }

    //语音自动下载 下载完成回调
    private void onMedioDownload(int rescode, string filePath, string callId, string resId)
    {
        DWChatUtils.DebugLog("rescode = " + rescode.ToString() + " callid = " + callId.ToString() + " resid = " + resId.ToString());
        if(rescode == 200)
        {
            //坑 sdk 返回成功 实际上是没有文件的！！ 重新调用下载接口 重下！！！
            if (!System.IO.File.Exists(filePath))
            {
                DWChatUtils.DebugLog("filePath : " + filePath + ", is not Exists!!!!!!!!!");
           
                    DWLoom.QueueOnMainThread(() =>
                    {     if (_callID_senderID_DIC.ContainsKey(resId))
                            {
                                DicInfo dicInfo = _callID_senderID_DIC[resId];
                                NIMAudioMessage nimImMsgContent = dicInfo.nimImMsgContent;

                                //通过url下载 参数变化了
                                _callID_senderID_DIC.Add(nimImMsgContent.AudioAttachment.RemoteUrl, dicInfo);
                                _callID_senderID_DIC.Remove(resId);

                                NIM.Nos.NosAPI.Download(nimImMsgContent.AudioAttachment.RemoteUrl, OnDownloadCompleted, DwonloadProgressCallback);
                            }
                    });
                return;
            }
            onDownloadSuc(rescode, filePath, callId, resId);
        }
        else
        {
            DWChatUtils.DebugLog("download mediofailed==========");
        }
    }

    //url下载回掉 注意 和上面的 返回的参数不一致
    private void OnDownloadCompleted(int rescode, string filePath, string callId, string resId)
    {
        //DWChatUtils.DebugLog("rescode = " + rescode.ToString()  + " resid = " + resId.ToString());
        onDownloadSuc(rescode, filePath, callId, resId);
    }

    //下载进度 目前不用
    private void DwonloadProgressCallback(NIM.Nos.ProgressData prgData)
    {

    }

    //下载成功回调
    private void onDownloadSuc(int rescode, string filePath, string callId, string resId) 
    {
        DWLoom.QueueOnMainThread(() =>
        {
            //DWChatUtils.DebugLog("download audio finish=======path = " + filePath);
            if (!_callID_senderID_DIC.ContainsKey(resId))
            {
                Debug.LogError("download voice can not find resId : " + resId);
                return;
            }
            //OnPlayAudioStart(filePath);
            string senderid = _callID_senderID_DIC[resId].senderID;
            int dur = _callID_senderID_DIC[resId].dur;
            //DWChatUtils.DebugLog("download audio finish=======senderid = " + senderid);
            ChatInterface.getInstance().receiveVoiceMsg(senderid, filePath, dur);
            _callID_senderID_DIC.Remove(resId);
        });
    }

     private void OnSendMessageResult(object sender, MessageArcEventArgs args)
    {
        if (args != null && args.ArcInfo != null) 
        {
            DWChatUtils.DebugLogFormat("OnSendMessageResult==={0}", args.ArcInfo.Dump());
            DWChatUtils.DebugLog(args.ArcInfo.Dump());
        }
    }

    private void OnMessageReceived(object sender, NIMReceiveMessageEventArgs args)
    {
        DWChatUtils.DebugLog("=============================OnMessageReceived");
        if (args == null || args.Message == null || args.Message.MessageContent == null)
            return;

        //DWChatUtils.DebugLogFormat("OnMessageReceived===={0}", args.Message.Dump());
        DWChatUtils.DebugLogFormat("msg content ==={0}", args.Message.MessageContent);
        DWChatUtils.DebugLog(args.Message.Serialize());

        NIMMessageFeature feature = args.Message.Feature;
        NIMIMMessage nimImMsgContent = args.Message.MessageContent;
        string senderID = nimImMsgContent.SenderID;
        string receiverID = nimImMsgContent.ReceiverID;
        
        DWChatUtils.DebugLog("OnMessageReceived ReceiverID ==== " + receiverID);
        DWChatUtils.DebugLog("OnMessageReceived senderID ==== " + senderID);
        //DWChatUtils.DebugLog("OnMessageReceived resID ==== " + resID);

        DWLoom.QueueOnMainThread(() =>
        {
            if (feature == NIMMessageFeature.kNIMMessageFeatureDefault)
                sendMsgRecipt(nimImMsgContent);
        });

        //屏蔽离线消息
        if (feature == NIMMessageFeature.kNIMMessageFeatureLeaveMsg || feature == NIMMessageFeature.kNIMMessageFeatureRoamMsg)
        {
            DWChatUtils.DebugLog("receive message is leave msg or sync msg=================");
        }
        // 文本消息
        else if (nimImMsgContent.MessageType == NIMMessageType.kNIMMessageTypeText)
        {
            string msgcontext = ((NIMTextMessage)nimImMsgContent).TextContent;
            //DWChatUtils.DebugLog("OnMessageReceived======context = " + msgcontext);
            //DWChatUtils.DebugLog("receiverID : " + receiverID + ", DWChatTeam.getInstance()._teamID :" + DWChatTeam.getInstance()._teamID);

            if (receiverID == DWChatTeam.getInstance()._teamID)
                parseMessage(senderID, msgcontext);
        }
        // 音频消息
        else if (nimImMsgContent.MessageType == NIMMessageType.kNIMMessageTypeAudio)
        {
            //DWChatUtils.DebugLog("receive audio message ======= " + DWChatTeam.getInstance()._teamID);
            if (receiverID == DWChatTeam.getInstance()._teamID)
            {
                _audioAttachment = ((NIMAudioMessage)nimImMsgContent).AudioAttachment;
                string resID = ((NIMAudioMessage)nimImMsgContent).ResourceId;

                //DWChatUtils.DebugLog("receive audio message=" + resID);
                //DWChatUtils.DebugLog("receive audio message path=" + ((NIMAudioMessage)nimImMsgContent).LocalFilePath);

                DWLoom.QueueOnMainThread(() =>
                {
                    if (_audioAttachment != null && !_callID_senderID_DIC.ContainsKey(resID))
                        _callID_senderID_DIC.Add(resID, new DicInfo(senderID, _audioAttachment.Duration, (NIMAudioMessage)nimImMsgContent));
                });
            }
        }
    }

    public void DW_sendTeamMessage(string teamid, string msgcontext)
    {
        if (!DWChatLogin._loginSuccess)
            return;

        DWChatUtils.DebugLog("send message in c# ======== teamid :" + teamid + ",msgcontext" + msgcontext);
        if (msgcontext == null)
            return;
        var msg = createIMMessage(0, msgcontext, "", teamid);
        if (msg == null)
            return;
        DWChatUtils.DebugLog(msg.Dump());
        TalkAPI.SendMessage(msg, OnReportUploadProgress2);
    }

    //发送语音消息
    public void DW_sendAudioMessage(string teamid, string filepath)
    {
        if (!DWChatLogin._loginSuccess)
            return;
        if (string.IsNullOrEmpty(teamid))
            return;

        DWChatUtils.DebugLog("audio path = " + filepath);
        var msg = createIMMessage(2 , "", filepath, teamid);
        if (msg == null)
            return;
        DWChatUtils.DebugLog(msg.Dump());

        TalkAPI.SendMessage(msg, OnReportUploadProgress2);
    }

    //发送已读
    void sendMsgRecipt(NIMIMMessage msg)
    {
        if (msg != null)
            NIM.Messagelog.MessagelogAPI.SendReceipt(msg, OnMsglogStatusChangedResult);
    }

    void OnMsglogStatusChangedResult(ResponseCode res, string result)
    {
        DWChatUtils.DebugLog(string.Format("code : {0},result:{1}", res, result));
    }

    [AOT.MonoPInvokeCallback(typeof(ReportUploadProgressDelegate))]
    private void OnReportUploadProgress2(long uploadedSize, long totalSize, object progressData)
    {
        DWChatUtils.DebugLog(string.Format("uploadProgress:uploadSize:{0},totalSize:{1}", uploadedSize, totalSize));
    }

    //已读所有消息
    public void DW_readAllMsg()
    {
        SysMsgAPI.SetAllMsgRead(HandleSetAllMsgReadResult);
    }
    [AOT.MonoPInvokeCallback(typeof(OperateSysMsgDelegate))]
    private void HandleSetAllMsgReadResult(int res_code, int unread_count, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaler))] string json_extension, IntPtr user_data)
    {
        DWChatUtils.DebugLog("Call Back SetAllMsgRead!");
        DWLoom.QueueOnMainThread(() =>
        {
            string text = string.Format("unread count = {0}", unread_count);
            text += "   AllMsgRead " + res_code;
            DWChatUtils.DebugLog(text);
        });
    }

    //删除所有
    public void DW_deleteall()
    {
        SysMsgAPI.DeleteAll(HandleDeleteAllResult);
    }
    [AOT.MonoPInvokeCallback(typeof(OperateSysMsgDelegate))]
    private void HandleDeleteAllResult(int res_code, int unread_count, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaler))] string json_extension, IntPtr user_data)
    {
        DWChatUtils.DebugLog("Call Back DeleteAll!");
        DWLoom.QueueOnMainThread(() =>
        {
            string text = string.Format("unread_count = {0}", unread_count);
            text += "  DeleteAll " + res_code;
        });
    }

    public void DW_delByTypeClick(int result_sys_msg_type)
    {
            SysMsgAPI.DeleteMsgByType((NIMSysMsgType)result_sys_msg_type, HandleDeleteMsgByTypeResult);
    }
    [AOT.MonoPInvokeCallback(typeof(OperateSysMsgDelegate))]
    private void HandleDeleteMsgByTypeResult(int res_code, int unread_count, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaler))] string json_extension, IntPtr user_data)
    {
        DWChatUtils.DebugLog("Call Back DeleteAll!");
        DWLoom.QueueOnMainThread(() =>
        {
            string text = string.Format("unread_count = {0}", unread_count);
            text += " DelByType " + res_code;
        });
    }
    //设置消息状态
    public void DW_setMsgByType(int result_sys_msg_type, int result_msg_status)
    {
            SysMsgAPI.SetMsgStatusByType((NIMSysMsgType)result_sys_msg_type, (NIMSysMsgStatus)result_msg_status, HandleSetMsgStatusByTypeResult);
    }
    [AOT.MonoPInvokeCallback(typeof(OperateSysMsgDelegate))]
    private void HandleSetMsgStatusByTypeResult(int res_code, int unread_count, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaler))] string json_extension, IntPtr user_data)
    {
        DWChatUtils.DebugLog("Call Back SetMsgStatusByType!");
        DWLoom.QueueOnMainThread(() =>
        {
            string text = string.Format("unread_count = {0}", unread_count);
            text += " SetByType " + res_code;
        });
    }
    //查询本地消息
    public void DW_queryMessage()
    {
        SysMsgAPI.QueryMessage(20, 0, HandleQueryMessageResult);
    }
    [AOT.MonoPInvokeCallback(typeof(QuerySysMsgResult))]
    private void HandleQueryMessageResult(NIMSysMsgQueryResult result)
    {
        DWChatUtils.DebugLog("Call Back QueryMessage!");
        DWLoom.QueueOnMainThread(() =>
        {
            string text = string.Format("call backed! {0}", result.Dump());
        });
    }
    //查询未读消息
    public void DW_queryUnreadCount()
    {
        SysMsgAPI.QueryUnreadCount(HandleQueryUnreadCountResult);
    }
    [AOT.MonoPInvokeCallback(typeof(CommomOperateResult))]
    private void HandleQueryUnreadCountResult(ResponseCode response, int count)
    {
        DWLoom.QueueOnMainThread(() =>
        {
            string text = string.Format("unread count = {0}", count);
        });
    }
    //根据消息ID删除消息
    public void OnDelByMsgIdClick(long msgID)
    {
        SysMsgAPI.DeleteByMsgId(msgID, HandleDeleteByMsgIdResult);
    }
    [AOT.MonoPInvokeCallback(typeof(OperateSysMsgExternDelegate))]
    private void HandleDeleteByMsgIdResult(int res_code, long msg_id, int unread_count, string json_extension, IntPtr user_data)
    {
        DWLoom.QueueOnMainThread(() =>
        {
            string text = string.Format("call backed! {0},msg_id is{1}", res_code, msg_id);
            text += string.Format("{0}", unread_count);
        });
    }

    private NIMIMMessage createIMMessage(NIMMessageType type)
    {
        NIMIMMessage msg = null;
        switch (type)
        {
            case NIMMessageType.kNIMMessageTypeText:
                msg = new NIMTextMessage();
                break;
            case NIMMessageType.kNIMMessageTypeImage:
                msg = new NIMImageMessage();
                break;
            case NIMMessageType.kNIMMessageTypeAudio:
                msg = new NIMAudioMessage();
                break;
            case NIMMessageType.kNIMMessageTypeVideo:
                msg = new NIMVideoMessage();
                break;
            case NIMMessageType.kNIMMessageTypeLocation:
                msg = new NIMLocationMessage();
                break;
            case NIMMessageType.kNIMMessageTypeNotification:
                msg = new NIMTeamNotificationMessage();
                break;
            case NIMMessageType.kNIMMessageTypeFile:
                msg = new NIMFileMessage();
                break;
            case NIMMessageType.kNIMMessageTypeTips:
                msg = new NIMTipMessage();
                break;
            case NIMMessageType.kNIMMessageTypeCustom:
                msg = new NIMCustomMessage<string>();
                break;
            case NIMMessageType.kNIMMessageTypeUnknown:
                //msg = new NIMUnknownMessage ();
                break;
            default:
                break;
        }
        return msg;
    }
    private NIMIMMessage createIMMessage(int msgtype, string sendContent, string audiopath, string teamid)
    {
        NIMMessageType type = (NIMMessageType)Enum.Parse(typeof(NIMMessageType), Convert.ToString(msgtype));
        NIMIMMessage msg = createIMMessage(type);
        if (msg == null)
        {
            DWChatUtils.DebugLog(string.Format("createIMMessage Faild:{0}", type));
            return null;
        }
        if(string.IsNullOrEmpty(teamid))
        {
            DWChatUtils.DebugLog("createIMMessage but team id is empty =====");
            return null;
        }
        setMessageSetting(msg);
        msg.SenderID = DWChatLogin.getInstance().DW_getAccountInfo().Account;
        msg.ReceiverID = teamid;
        msg.SessionType = (NIMSessionType)Enum.Parse(typeof(NIMSessionType), "1");
        msg.TimeStamp = DWChatUtils.toTicks(DateTime.Now);
        msg.ClientMsgID = DWChatUtils.createUniqueID().ToString();
        msg.LocalFilePath = audiopath; 
        msg.LocalExtension = "";
        msg.ServerExtension = "";
        msg.PushPayload = new JsonExtension();
        msg.PushPayload.AddItem("PushPayload", "");
        msg.PushContent = "";
        msg.AntiSpamContent = "";
        DWChatUtils.DebugLog("create message    team id = " + DWChatTeam.getInstance()._teamID + "meg type = " + msgtype + " msg content = " + sendContent);
        switch (type)
        {
            case NIMMessageType.kNIMMessageTypeText:
                if (string.IsNullOrEmpty(sendContent))
                    return null;
                ((NIMTextMessage)msg).TextContent = sendContent;
                break;
            case NIMMessageType.kNIMMessageTypeImage:
                ((NIMImageMessage)msg).ImageAttachment = new NIMImageAttachment
                {
                    Width = 100,
                    Height = 100
                };
                break;
            case NIMMessageType.kNIMMessageTypeAudio:
                ((NIMAudioMessage)msg).AudioAttachment = createAudioAttachment(audiopath);
                if (((NIMAudioMessage)msg).AudioAttachment == null)
                    return null;
                break;
            case NIMMessageType.kNIMMessageTypeVideo:
//                 ((NIMVideoMessage)msg).VideoAttachment = createVideoAttachment();
//                 if (((NIMVideoMessage)msg).VideoAttachment == null)
//                     return null;
                break;
            case NIMMessageType.kNIMMessageTypeLocation:
                //((NIMLocationMessage)msg).LocationInfo = createLocationMsgInfo();
                break;
            //case NIMMessageType.kNIMMessageTypeNotification:
            //	((NIMTeamNotificationMessage)msg).NotifyMsgData = createLocationMsgInfo ();
            //	break;
            case NIMMessageType.kNIMMessageTypeFile:
                //((NIMFileMessage)msg).FileAttachment = createFileAttachment();
                //if (((NIMFileMessage)msg).FileAttachment == null)
                //    return null;
                break;
            case NIMMessageType.kNIMMessageTypeTips:
                //((NIMTipMessage)msg).Attachment = _attachInputField.text;
                //((NIMTipMessage)msg).TextContent = _bodyInputField.text;
                break;
            case NIMMessageType.kNIMMessageTypeCustom:
                //((NIMCustomMessage<string>)msg).CustomContent = _attachInputField.text;
                //((NIMCustomMessage<string>)msg).Extention = _bodyInputField.text;
                //setCustomMessageSetting((NIMCustomMessage<string>)msg);
                break;
            default:
                msg = null;
                break;
        }
        return msg;
    }

    private void setMessageSetting(NIMIMMessage msg)
    {
        msg.Routable = false;
        msg.SavedOffline = true;
        msg.NeedCounting = false;
        msg.NeedPush = false;
        msg.NeedPushNick = false;
        msg.ServerSaveHistory = true;
        msg.Roaming = false;
        msg.MultiSync = false;
        msg.AntiSpamEnabled = true;
    }

    private NIMAudioAttachment createAudioAttachment(string audiofile)
    {
        string filePath = audiofile;
        if (string.IsNullOrEmpty(filePath))
            return null;

        NIMAudioAttachment attch = new NIMAudioAttachment();
        if (_audioAttachment != null)
        {
            attch.Duration = _audioAttachment.Duration;
            attch.DisplayName = _audioAttachment.DisplayName;
            attch.FileExtension = _audioAttachment.FileExtension;
            attch.Size = _audioAttachment.Size;
        }
        return attch;
    }

    //语音================================================================================================
    [AOT.MonoPInvokeCallback(typeof(NIMAudio.StatusCallbackDelegate))]
    public void StartCaptureCallback(int rescode)
    {
        DWChatUtils.DebugLog("Start Capture:" + rescode.ToString());
        if (rescode == 200)
        {
            DWLoom.QueueOnMainThread(() =>
            {
                if (capture_timer_ != null)
                    capture_timer_.Start();
            });
        }

        ChatInterface.getInstance().StartRecordAudioCallBack(rescode);
    }

    [AOT.MonoPInvokeCallback(typeof(NIMAudio.StopCaptureCallbackDelegate))]
    public void StopCaptureCallback(int rescode, string file_path, string file_ext, int file_size, int audio_duration)
    {
        if(file_path.CompareTo("") == 0)
        {
            ChatInterface.getInstance().recoredVoiceFaled();
            return;
        }

        DWLoom.QueueOnMainThread(() =>
        {
            _audioAttachment = new NIMAudioAttachment();
            _audioAttachment.FileExtension = file_ext;
            _audioAttachment.Size = file_size;
            _audioAttachment.Duration = audio_duration;
            if (capture_timer_ != null)
                capture_timer_.Stop();
        });

        if (audio_duration < 800)
        {
            ChatInterface.getInstance().CaptureVoiceTimeIsShort();
            return;
        }

        ChatInterface.getInstance().recordAudioResult(file_path, audio_duration);
    }

    [AOT.MonoPInvokeCallback(typeof(NIMAudio.StatusCallbackDelegate))]
    public void CancelCaptureCallback(int resCode)
    {
        DWChatUtils.DebugLog("Cancel Capture:" + resCode.ToString());
        DWLoom.QueueOnMainThread(() =>
        {
            if (capture_timer_ != null)
                capture_timer_.Stop();
        });

    }

    [AOT.MonoPInvokeCallback(typeof(NIMAudio.PlayCallbackDelegate))]
    public void StartPlayCallback(int resCode, string filepath)
    {
        if (resCode == 200)
        {
            int duration = NIMAudio.AudioAPI.GetPlayTime();
            DWChatUtils.DebugLog("Start Play:" + duration.ToString());
            DWLoom.QueueOnMainThread(() =>
            {
                if (play_timer_ != null)
                    play_timer_.Start();

            });
        }
        else
        {
            DWChatUtils.DebugLog("rescode is not 200, file path ======== " + filepath.ToString());
            ChatInterface.getInstance().audioPlayEnd(resCode, filepath.ToString(),1);
        }
        if (filepath == null)
            filepath = "";
        DWChatUtils.DebugLog("Start Play resCode:" + resCode.ToString() + " filepath:" + filepath.ToString());
    }

    [AOT.MonoPInvokeCallback(typeof(NIMAudio.PlayCallbackDelegate))]
    public void StopPlayCallback(int resCode, string filepath)
    {
        DWLoom.QueueOnMainThread(() =>
        {
            if (play_timer_ != null)
                play_timer_.Stop();
        });

        ChatInterface.getInstance().audioPlayEnd(resCode,filepath,3);
        DWChatUtils.DebugLog("Stop Play resCode:" + resCode.ToString() + " filepath:" + filepath.ToString());
    }

    [AOT.MonoPInvokeCallback(typeof(NIMAudio.PlayCallbackDelegate))]
    public void EndPlayCallback(int resCode, string filepath)
    {
        DWChatUtils.DebugLog("Play End resCode:" + resCode.ToString() + " filepath:" + filepath.ToString());
        DWLoom.QueueOnMainThread(() =>
        {
            if (play_timer_ != null)
                play_timer_.Stop();
        });

        ChatInterface.getInstance().audioPlayEnd(resCode,filepath,2);
    }


    [AOT.MonoPInvokeCallback(typeof(NIMAudio.StatusCallbackDelegate))]
    public void AudioCaptureTimeCallback(int time)
    {
        //DWLoom.QueueOnMainThread(() =>
        //{
            
        //});
    }

    [AOT.MonoPInvokeCallback(typeof(NIMAudio.StatusCallbackDelegate))]
    public void AudioPlayCurrentPositionCallback(int time)
    {
        //DWLoom.QueueOnMainThread(() =>
        //{
        //    //_playTimeText.text = time.ToString();
        //});
    }

    [AOT.MonoPInvokeCallback(typeof(NIMAudio.GetCaptureDevicesCallbackDelegate))]
    public void AudioCaptureDevicesCallback(int resCode, List<string> device)
    {
        capture_devices = device;
        if (device == null || device.Count <= 0)
        {
            DWChatUtils.DebugLog("没有采集设备");
            return;
        }

        for (int i = 0; i < device.Count; i++)
        {
            DWChatUtils.DebugLog("采集设备======" + device[i]);
        }
    }

    private void InitAudio()
    {

        capture_timer_ = new System.Timers.Timer(1000);//1s刷新一次
        capture_timer_.Elapsed += Capture_timer__Elapsed;
        play_timer_ = new System.Timers.Timer(1000);//1s
        play_timer_.Elapsed += Play_timer__Elapsed;

#if UNITY_ANDROID
        /*AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
         if(jc != null)
         {
             AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
             if (jo != null)
             {
                 context = jo.Call<AndroidJavaObject>("getApplicationContext");
             }
             else
             {
                 DWChatUtils.DebugLog("java object is null======================");
             }
         }
         else
         {
             DWChatUtils.DebugLog("javaclass is null=====================");
         }
         */
#endif
        NIMAudio.AudioAPI.RegGetCaptureDevices(AudioCaptureDevicesCallback);
        NIMAudio.AudioAPI.GetCaptureDevices();
    }

    public void ClearAudio()
    {
        if (capture_timer_ != null)
        {
            capture_timer_.Stop();
            capture_timer_.Dispose();
            capture_timer_ = null;
        }
        if (play_timer_ != null)
        {
            play_timer_.Stop();
            play_timer_.Dispose();
            play_timer_ = null;
        }
        NIMAudio.AudioAPI.CancelCapture();
        NIMAudio.AudioAPI.StopPlayAudio();

    }

    private void Play_timer__Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        NIMAudio.AudioAPI.GetPlayCurrentPosition();
    }

    private void Capture_timer__Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        NIMAudio.AudioAPI.GetCaptureTime();
    }

    public void DW_RecordAudioStart()
    {
        //开始录音
        NIMAudio.AudioAPI.RegStartCaptureCb(StartCaptureCallback);
        NIMAudio.AudioAPI.RegGetCaptureTimeCb(AudioCaptureTimeCallback);

        int audio_type = 0;
//         if (_audio_AMR_Toggle.isOn)
//         {
//             audio_type = Convert.ToInt32(NIMAudio.NIMAudioType.kNIMAudioAMR);
//         }
//         else
//         {
            audio_type = Convert.ToInt32(NIMAudio.NIMAudioType.kNIMAudioAAC);
        //}

        //选择麦克风采集，填空则为默认
        if (capture_devices != null && capture_devices.Count > 1)
        {
            NIMAudio.AudioAPI.StartCapture(audio_type, 255, 0, capture_devices[1]);
        }
        else 
        {
            NIMAudio.AudioAPI.StartCapture(audio_type, 180, 0, "");
        }
        is_capture_audio = true;
        DWChatUtils.DebugLog("OnRecordAudioStart");
    }

    public bool CheckCaptureDevice()
    {
        return capture_devices != null && capture_devices.Count > 1;
    }

    public void DW_RecordAudioStop()
    {
        if (is_capture_audio)
        {
            NIMAudio.AudioAPI.RegStopCaptureCb(StopCaptureCallback);
            NIMAudio.AudioAPI.StopCapture();
            is_capture_audio = false;
            DWChatUtils.DebugLog("OnRecordAudioStop1");
        }

        DWChatUtils.DebugLog("OnRecordAudioStop2");
    }
    public void OnPlayAudioStart(string audiopath)
    {
        if (!System.IO.File.Exists(audiopath)) {
            ChatInterface.getInstance().audioPlayEnd(0,audiopath,1);
            return;
        }
        DWChatUtils.DebugLog("OnPlayAudioStart");
//         NIMAudio.AudioAPI.RegStartPlayCb(StartPlayCallback);
//         NIMAudio.AudioAPI.RegGetPlayCurrentPositionCb(AudioPlayCurrentPositionCallback);
//         NIMAudio.AudioAPI.RegPlayEndCb(EndPlayCallback);
        NIMAudio.NIMAudioType audiotype = NIMAudio.NIMAudioType.kNIMAudioAAC;
        DWChatUtils.DebugLog("OnPlayAudioStart ===== audiopath= " + audiopath + "   type = " + audiotype);
        NIMAudio.AudioAPI.PlayAudio(audiopath, audiotype);
        if (play_timer_ != null)
        {
            play_timer_.Stop();
            play_timer_.Start();
        }
    }

    public void OnRecordAudioCancel()
    {
        NIMAudio.AudioAPI.RegCancelCaptureCb(CancelCaptureCallback);
        NIMAudio.AudioAPI.CancelCapture();
    }


    public void OnPlayAudioStop()
    {
        NIMAudio.AudioAPI.RegStopPlayCb(StopPlayCallback);
        NIMAudio.AudioAPI.StopPlayAudio();
    }

    public void OnAudioSetSpeaker(bool speaker_)
    {
        //speaker_ = !speaker_;
        IntPtr ptr = IntPtr.Zero;
#if UNITY_ANDROID
		ptr=context.GetRawObject();
#endif
        NIMAudio.AudioAPI.SetPlaySpeaker(speaker_, ptr);
    }

    public bool OnAudioGetSpeaker()
    {
        IntPtr ptr = IntPtr.Zero;
#if UNITY_ANDROID
		ptr=context.GetRawObject();
#endif
        bool speaker = NIMAudio.AudioAPI.GetPlaySpeaker(ptr);
        return speaker;
        DWChatUtils.DebugLog("speaker status:" + speaker.ToString());
    }
    //==============================================================语音 end

    

    private void parseMessage(string senderID, string msg)
    {
        if (msg.CompareTo("") == 0)
            return;

        ChatInterface.getInstance().receiveTextMsg(senderID, msg);
    }

}