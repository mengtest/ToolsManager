
using UnityEngine;
using System.Collections;
using LuaInterface;

public class ChatInterface
{
    LuaFunction call_IMLoginCallBack;
    LuaFunction call_IMLogoutCallBack;
    LuaFunction call_StartRecordAudioCallBack;
    LuaFunction call_CreateTeamCallBack;
    LuaFunction call_ReceiveText;
    LuaFunction call_ReceiveVoice;
    LuaFunction call_RecordResult;
    LuaFunction call_audioPlayEnd;

    private volatile static ChatInterface _instance = null;
    private static readonly object lockHelper = new object();
    private ChatInterface() { }

    public static ChatInterface getInstance()
    {
        if (_instance == null)
        {
            lock (lockHelper)
            {
                if (_instance == null)
                    _instance = new ChatInterface();
            }
        }
        return _instance;
    }
    /// <summary>
    /// 初始化lua方法
    /// </summary>
    public void initLuaFunc()
    {
        if (LuaRootSys.Instance != null && LuaRootSys.Instance.LuaMgr != null)
        {
            call_IMLoginCallBack = LuaRootSys.Instance.LuaMgr.GetLuaFunction("NimChatSys.IMLoginCallBack");
            call_IMLogoutCallBack = LuaRootSys.Instance.LuaMgr.GetLuaFunction("NimChatSys.IMLogoutCallBack");
            call_CreateTeamCallBack = LuaRootSys.Instance.LuaMgr.GetLuaFunction("NimChatSys.CreateTeamCallBack");
            call_StartRecordAudioCallBack = LuaRootSys.Instance.LuaMgr.GetLuaFunction("NimChatSys.StartRecordAudioCallBack");
            call_ReceiveText = LuaRootSys.Instance.LuaMgr.GetLuaFunction("NimChatSys.ReceiveText");
            call_ReceiveVoice = LuaRootSys.Instance.LuaMgr.GetLuaFunction("NimChatSys.ReceiveVoice");
            call_RecordResult = LuaRootSys.Instance.LuaMgr.GetLuaFunction("NimChatSys.RecordResult");
            call_audioPlayEnd = LuaRootSys.Instance.LuaMgr.GetLuaFunction("NimChatSys.AudioPlayEnd");
        }
    }

    public void InitIMSDK(string nimAppKey, string nimAppSecret)
    {
        DWChatLogin.getInstance().initSDK(nimAppKey,nimAppSecret);
    }

    /// <summary>
    /// 登录im服务
    /// </summary>
    public void loginIm(string nimAppKey, string nimAppSecret, string imID, string token)
    {
        DWChatLogin.getInstance().DW_setAccountInfo(imID, token);
        DWChatLogin.getInstance().DW_login(nimAppKey, nimAppSecret);
    }

    public void Relogin()
    {
        NIM.ClientAPI.Relogin();
    }

    public void SDKCleanup() 
    {
        NIM.ClientAPI.Cleanup();
    }

    /// <summary>
    /// 注销登陆
    /// </summary>
    public void logoutIm()
    {
        DWChatLogin.getInstance().DW_logout();
    }
    /// <summary>
    /// 设置im帐号密码(保留接口)
    /// </summary>
    /// <param name="imID"></param>
    /// <param name="token"></param>
    public void SetImCount(string imID, string token)
    {
        DWChatLogin.getInstance().DW_setAccountInfo(imID, token);
    }




    /// <summary>
    /// 查询群信息并且解散群
    /// </summary>
    public void queryAllMyTeamAndDismiss()
    {
        if (!DWChatLogin._loginSuccess)
            return;

        DWChatTeam.getInstance().DW_queryMyTeamInfos(false);
    }
    /// <summary>
    /// 查询自己的群
    /// </summary>
    public void queryMyTeams()
    {
        if (!DWChatLogin._loginSuccess)
            return;

        DWChatTeam.getInstance().DW_queryMyTeams();
    }

    /// <summary>
    /// 创建聊天群
    /// </summary>
    public void createChatTeam(string teamName)
    {
        if (!getIMLoginStatus()) 
        {
            if (call_CreateTeamCallBack == null)
                return;
            DWLoom.QueueOnMainThread(() =>
            {
                call_CreateTeamCallBack.Call(false, 0);
            });
            return;
        }

        //已经有自己的房间了
        if (DWChatTeam.getInstance()._teamID != "" && DWChatTeam.getInstance()._TeamOwnerID == DWChatLogin.getInstance().DW_getAccountInfo().Account) 
        {
            DWChatUtils.DebugLog("has my room ready! _teamID :" + DWChatTeam.getInstance()._teamID);
            createChatTeamSuccess(DWChatTeam.getInstance()._TeamOwnerID, DWChatTeam.getInstance()._teamID);
            return;
        }

        string[] idlist = { DWChatLogin.getInstance().DW_getAccountInfo().Account };
        DWChatTeam.getInstance().DW_createChatTeam(teamName, idlist);
    }

    /// <summary>
    /// 加群
    /// </summary>
    /// <param name="teamid"></param>
    public void applyJoinTeam(string teamid)
    {
        //if (!CheckIsLogin())
        //    return;

        DWChatTeam.getInstance().DW_joinTeam(teamid);
    }

    //发送消息
    public void sendTextMessage(string teamid, string msg)
    {
        //if (!CheckIsLogin() || string.IsNullOrEmpty(teamid))
        //    return;

        if (string.IsNullOrEmpty(teamid))
            return;

        //不在群里面
        if (teamid != getCurrentTeamID()) 
        {
            EventSys.Instance.AddEvent(EEventType.NimIsNotInTeam);
            return;
        }

        DWChatTalk.getInstance().DW_sendTeamMessage(teamid, msg);
    }

    /// <summary>
    /// 开始录音
    /// </summary>
    public void startRecordAudio()
    {
        //if (!CheckIsLogin())
        //    return;

        DWChatTalk.getInstance().DW_RecordAudioStart();
    }
    /// <summary>
    /// 停止录音
    /// </summary>
    public void endRecordAudio()
    {
        DWChatTalk.getInstance().DW_RecordAudioStop();
    }

    //取消录音
    public void OnRecordAudioCancel() 
    {
        DWChatTalk.getInstance().OnRecordAudioCancel();
    }

    /// <summary>
    /// 发送声音
    /// </summary>
    /// <param name="teamid"></param>
    /// <param name="path"></param>
    public void sendAudioMessage(string teamid, string path)
    {
        //if (!CheckIsLogin())
        //    return;

        //不在群里面
        if (teamid != getCurrentTeamID())
        {
            EventSys.Instance.AddEvent(EEventType.NimIsNotInTeam);
            return;
        }

        DWChatTalk.getInstance().DW_sendAudioMessage(teamid, path);
    }

    /// <summary>
    /// 播放声音
    /// </summary>
    public void playVoiceAudio(string localpath)
    {
        //if (!CheckIsLogin())
        //    return;

        DWChatTalk.getInstance().OnPlayAudioStart(localpath);
    }

    /// <summary>
    /// 邀请入群
    /// </summary>
    /// <param name="userChatID"></param>
    public void inviteTeamMember(string teamid, string userChatID)
    {
        //if (!CheckIsLogin())
        //    return;

        DWChatTeam.getInstance().DW_inviteTeamMember(teamid, userChatID);
    }

    /// <summary>
    /// 解散群
    /// </summary>
    public void dismissTeam(string teamid)
    {
        if (!DWChatLogin._loginSuccess)
            return;

        DWChatTeam.getInstance().DW_dismissTeam(teamid);
    }
    /// <summary>
    /// 离开群
    /// </summary>
    public void leaveTeam(string teamid)
    {
        if (!DWChatLogin._loginSuccess)
            return;

        DWChatTeam.getInstance().DW_leaveTeam(teamid);
    }

    /// <summary>
    /// 设置当前群ID
    /// </summary>
    public void setTeamID(string teamid)
    {
        DWChatTeam.getInstance()._teamID = teamid;
    }

    /// <summary>
    /// 获取当前群ID
    /// </summary>
    /// <returns></returns>
    public string getCurrentTeamID()
    {
        return DWChatTeam.getInstance()._teamID;
    }

    /// <summary>
    /// 获取登录状态
    /// </summary>
    /// <returns></returns>
    public bool getIMLoginStatus()
    {
        return DWChatLogin._loginSuccess;
    }

    public bool GetLoginState()
    {
        return NIM.ClientAPI.GetLoginState() == NIM.NIMLoginState.kNIMLoginStateLogin;
    }

    public void OnPlayAudioStop() 
    {
        DWChatTalk.getInstance().OnPlayAudioStop();
    }

    public void OnAudioSetSpeaker(bool speaker) 
    {
        DWChatTalk.getInstance().OnAudioSetSpeaker(speaker);
    }
    public bool OnAudioGetSpeaker()
    {
        return DWChatTalk.getInstance().OnAudioGetSpeaker();
    }

    public void ClearAudio()
    {
        DWChatTalk.getInstance().ClearAudio();
    }

    //清理资源 留个接口给lua
    public void ClearVoice()
    {
        DWChatLogin.getInstance().ClearVoice();
    }
    ///---------------------------c#调用lua---------------------///
    /// <summary>
    /// c# to lua
    /// </summary>
    /// <param name="timeLength"></param>
    [NoToLua]
    public void receiveVoiceMsg(string senderid, string localpath, int timeLength)
    {
        if (call_ReceiveVoice == null)
            return;
        DWLoom.QueueOnMainThread(() =>
        {
            call_ReceiveVoice.Call(senderid, localpath, timeLength);
            //call.Dispose();
        });
    }
    /// <summary>
    /// c# to lua
    /// </summary>
    /// <param name="text"></param>
    [NoToLua]
    public void receiveTextMsg(string senderid, string text)
    {
        if (call_ReceiveText == null)
            return;
        DWLoom.QueueOnMainThread(() =>
        {
            call_ReceiveText.Call(senderid, text);
        });
    }

    [NoToLua]
    public void recoredVoiceFaled()
    {
        if (call_RecordResult == null)
            return;
        DWLoom.QueueOnMainThread(() =>
        {
            call_RecordResult.Call(2,"",0);
        });
    }

    /// <summary>
    /// 录音结果通知lua
    /// </summary>
    /// <param name="path"></param>
    /// <param name="timelength"></param>
    [NoToLua]
    public void recordAudioResult(string path, int timelength)
    {
        DWChatUtils.DebugLog("recordAudioResult path :" + path + ",timelength :" + timelength);
        if (call_RecordResult == null)
            return;
        DWLoom.QueueOnMainThread(() =>
        {
            call_RecordResult.Call(1,path, timelength);
        });
    }

    [NoToLua]
    public void CaptureVoiceTimeIsShort()
    {
        if (call_RecordResult == null)
            return;
        DWLoom.QueueOnMainThread(() =>
        {
            call_RecordResult.Call(3, "", 0);
        });
    }


    /// <summary>
    /// 录音结果返回
    /// </summary>
    [NoToLua]
    public void StartRecordAudioCallBack(int rescode)
    {
        if (call_StartRecordAudioCallBack == null)
            return;
        DWLoom.QueueOnMainThread(() =>
        {
            call_StartRecordAudioCallBack.Call(rescode);
        });
    }

    /// <summary>
    /// 语音播放完成
    /// state 1 开始 2完成 3取消
    /// </summary>
    [NoToLua]
    public void audioPlayEnd(int resCode,string filepath,int state)
    {
        if (call_audioPlayEnd == null)
            return;
        DWLoom.QueueOnMainThread(() =>
        {
            call_audioPlayEnd.Call(resCode, filepath, state);
        });
    }

    [NoToLua]
    public void imLoginCallBack(bool isSucess)
    {
        if (call_IMLoginCallBack == null)
            return;

        DWLoom.QueueOnMainThread(() =>
        {
            call_IMLoginCallBack.Call(isSucess);
        });
    }

    [NoToLua]
    public void imLoginOutCallBack()
    {
        if (call_IMLogoutCallBack == null)
            return;
        DWLoom.QueueOnMainThread(() =>
        {
            call_IMLogoutCallBack.Call();
        });
    }

    [NoToLua]
    public void createChatTeamSuccess(string owerID, string teamID)
    {
        if (call_CreateTeamCallBack == null)
            return;

        DWLoom.QueueOnMainThread(() =>
        {
            call_CreateTeamCallBack.Call(true,owerID, teamID);
        });
    }

    [NoToLua]
    public void createTeamFailed(int rescode,string TeamName)
    {
        if (call_CreateTeamCallBack == null)
            return;

        DWLoom.QueueOnMainThread(() =>
        {
            call_CreateTeamCallBack.Call(false,rescode);
        });
    }

}
