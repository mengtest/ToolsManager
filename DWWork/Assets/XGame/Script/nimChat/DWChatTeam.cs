using UnityEngine;
using System.IO;
using NIM;
using NIM.DataSync;
using NimUtility;
using Newtonsoft.Json;
using LuaInterface;
using System.Collections.Generic;
using NIM.Team;
using NIM.SysMessage;

public class DWChatTeam {

    public string _teamID = "";
    public string _TeamOwnerID = "";
    public List<string> _teamMenbers {  set; get; }

    private volatile static DWChatTeam _instance = null;
    private static readonly object lockHelper = new object();

    private DWChatTeam() { }
    public static DWChatTeam getInstance()
    {
        if (_instance == null)
        {
            lock (lockHelper)
            {
                if (_instance == null)
                {
                    _instance = new DWChatTeam();
                    _instance._teamID = "";
                    _instance._teamMenbers = new List<string>();
                }
            }
        }
        return _instance;
    }

    public void DW_initTeam()
    {
        DWChatUtils.DebugLog("DW_initTeam=================================");
        TeamAPI.TeamEventNotificationHandler = OnTeamEventNotification;
        SysMsgAPI.ReceiveSysMsgHandler = OnReceiveSysMsgHandler;
        //DWLoom.Initialize();
    }

    public void DW_releaseTeam()
    {
        TeamAPI.TeamEventNotificationHandler = null;
        SysMsgAPI.ReceiveSysMsgHandler = null;
    }

    public void DW_createChatTeam(string teamName, string[] idList)
    {
        NIM.Team.NIMTeamInfo info = new NIM.Team.NIMTeamInfo();

        NIMTeamInfo ti = new NIMTeamInfo
        {
            Name = (teamName != "") ? teamName : "dwGame",
            Announcement = "",
            Introduce = teamName,
            TeamType = NIMTeamType.kNIMTeamTypeAdvanced,
            BeInvitedMode = NIM.Team.NIMTeamBeInviteMode.kNIMTeamBeInviteModeNotNeedAgree,
            CreatedTimetag = System.DateTime.Now.Ticks,
            InvitedMode = NIM.Team.NIMTeamInviteMode.kNIMTeamInviteModeEveryone,
            JoinMode = NIM.Team.NIMTeamJoinMode.kNIMTeamJoinModeNoAuth,
            MembersCount = 4,
            OwnerId = DWChatLogin.getInstance().DW_getAccountInfo().Account,
        };
        DWChatUtils.DebugLog("create team" + ti.Dump());
        DWChatUtils.DebugLog("create team list" + idList.Dump());
        NIM.Team.TeamAPI.CreateTeam(ti, idList, "", onTeamChangedNotify);
    }

    public void DW_queryTeamMembersInfo()
    {
        if(_teamID.CompareTo("") != 0)
            TeamAPI.QueryTeamMembersInfo(_teamID, true, true, OnQueryTeamMembersInfoResult);
    }

    public void DW_queryMyTeamInfos(bool searchInvalid)
    {
        if (searchInvalid)
            TeamAPI.QueryAllMyTeamsInfo(OnQueryMyTeamsInfoResult);
        else
            TeamAPI.QueryMyValidTeamsInfo(OnQueryMyTeamsInfoResult);
    }

    public void DW_queryMyTeams()
    {
        TeamAPI.QueryAllMyTeams(OnQueryMyTeamsResult);
    }

    [AOT.MonoPInvokeCallback(typeof(NIM.Team.TeamChangedNotificationDelegate))]
    private void onTeamChangedNotify(NIM.Team.NIMTeamEventData data)
    {
        string msg = string.Format("code : {0},tid: {1},notificationtype : {2},data:{3}",
            data.TeamEvent.ResponseCode, data.TeamEvent.TeamId, data.TeamEvent.NotificationType, data.Dump());

        DWChatUtils.DebugLogFormat("onTeamChanged======{0}", msg);
        if(data.TeamEvent.ResponseCode == ResponseCode.kNIMResTeamENAccess || data.TeamEvent.ResponseCode == ResponseCode.kNIMResTeamErrType || data.TeamEvent.ResponseCode == ResponseCode.kNIMResTeamLimit)
        {
            string teamName = "";
            if (data.TeamEvent.TeamInfo != null && data.TeamEvent.TeamInfo.Name != null)
                teamName = data.TeamEvent.TeamInfo.Name;

            ChatInterface.getInstance().createTeamFailed((int)data.TeamEvent.ResponseCode, teamName);
        }

        if (data.TeamEvent.ResponseCode == ResponseCode.kNIMResTeamAlreadyIn || data.TeamEvent.ResponseCode == ResponseCode.kNIMResSuccess)
        {
            _teamID = data.TeamEvent.TeamId;
        }

        switch (data.TeamEvent.NotificationType)
        {
            case NIM.Team.NIMNotificationType.kNIMNotificationIdTeamSyncCreate:
                DWChatUtils.DebugLogFormat("Sync create team ======= {0}", data.TeamEvent.TeamId);
                _teamID = data.TeamEvent.TeamId;
                _TeamOwnerID = DWChatLogin.getInstance().DW_getAccountInfo().Account;
                break;
            case NIM.Team.NIMNotificationType.kNIMNotificationIdTeamDismiss:
                DWChatUtils.DebugLogFormat("dismiss team ======= {0}", data.TeamEvent.TeamId);
                _teamID = "";
                _TeamOwnerID = "";
                _teamMenbers.Clear();
                break;
            case NIM.Team.NIMNotificationType.kNIMNotificationIdLocalCreateTeam:
                DWChatUtils.DebugLog("local create team =======");
                _teamID = data.TeamEvent.TeamId;
                _TeamOwnerID = DWChatLogin.getInstance().DW_getAccountInfo().Account;
                DWChatUtils.DebugLog("teamid = " + _teamID + "ownerid = " + _TeamOwnerID);
                _teamMenbers.Clear();
                for(int i = 0; i < data.TeamEvent.IdCollection.Count; i++)
                {
                    DWChatUtils.DebugLog("member id = " + data.TeamEvent.IdCollection[i]);
                    _teamMenbers.Add(data.TeamEvent.IdCollection[i]);
                }
                ChatInterface.getInstance().createChatTeamSuccess(_TeamOwnerID, _teamID);
                DWChatUtils.DebugLog("team members num = " + _teamMenbers.Count.ToString());
                break;

            case NIM.Team.NIMNotificationType.kNIMNotificationIdTeamMemberChanged:
                DWChatUtils.DebugLogFormat("members changed ======={0}", data.TeamEvent.MemberInfo.Dump());
                DWChatUtils.DebugLog("kNIMNotificationIdTeamMemberChanged " + data.TeamEvent.TeamId);
                if (data.TeamEvent.TeamId == _teamID)
                {
                    _teamMenbers.Clear();
                    foreach (var id in data.TeamEvent.IdCollection)
                    {
                        _teamMenbers.Add(id);
                    }
                }
                else
                {
//                     _teamID = "";
//                     _TeamOwnerID = "";
//                     _teamMenbers.Clear();
                }
                break;

            case NIM.Team.NIMNotificationType.kNIMNotificationIdTeamInvite:
                TeamAPI.AcceptTeamInvitation(data.TeamEvent.TeamId, "", onTeamChangedNotify);
                DWChatUtils.DebugLog("kNIMNotificationIdTeamInvite " + data.TeamEvent.TeamId);
               
                _teamID = data.TeamEvent.TeamId;
                if(data.TeamEvent.TeamInfo != null)
                {
                    _TeamOwnerID = data.TeamEvent.TeamInfo.OwnerId;
                    DWChatUtils.DebugLog("kNIMNotificationIdTeamInvite    teamid = " + _teamID + " ownerid = " + _TeamOwnerID);
                }
                else
                {
                    DWChatUtils.DebugLog("kNIMNotificationIdTeamInvite  but team owner id is null======");
                }

                DWChatUtils.DebugLog("teamid = " + _teamID + "ownerid = " + _TeamOwnerID);
                break;
            case NIM.Team.NIMNotificationType.kNIMNotificationIdTeamInviteAccept:
                DWChatUtils.DebugLog("kNIMNotificationIdTeamInviteAccept===============");
                break;

            case NIM.Team.NIMNotificationType.kNIMNotificationIdTeamLeave:
                DWChatUtils.DebugLog("kNIMNotificationIdTeamLeave================");
//              _teamID = "";
//              _TeamOwnerID = "";
                break;
            case NIM.Team.NIMNotificationType.kNIMNotificationIdLocalApplyTeam:
                DWChatUtils.DebugLog("kNIMNotificationIdLocalApplyTeam=================");
                //TeamAPI.AgreeJoinTeamApplication(_teamID, )
                break;

            case NIMNotificationType.kNIMNotificationIdTeamApplyPass:
                DWChatUtils.DebugLog("kNIMNotificationIdTeamApplyPass=================");
                break;
            default:
                break;
        }
    }

    private void OnTeamEventNotification(object sender, NIMTeamEventArgs args)
    {
        if (args != null && args.Data != null)
            onTeamChangedNotify(args.Data);
        else
            DWChatUtils.DebugLog("OnTeamEventNotification error");
    }

    private void OnReceiveSysMsgHandler(object sender, NIMSysMsgEventArgs args)
    {
        DWChatUtils.DebugLog(args.Message.Dump());
    }

    public void DW_dismissTeam(string teamID)
    {
        if (teamID.CompareTo("") != 0)
        {
            DWChatUtils.DebugLog("leave team ====" + teamID);
            TeamAPI.LeaveTeam(teamID, onTeamChangedNotify);
        }
    }

    public void DW_inviteTeamMember(string teamid, string userChatID)
    {
        DWChatUtils.DebugLog("DW_inviteTeamMember==== " + userChatID);
        string[] idlist = { userChatID };
        TeamAPI.Invite(teamid, idlist, "", onTeamChangedNotify);
    }

    [NIM.MonoPInvokeCallback(typeof(QueryMyTeamsResultDelegate))]
    private void OnQueryMyTeamsResult(int count, string[] accountIdList)
    {
        //DWChatUtils.DebugLog("OnQueryMyTeamsResult=======================count====" + count.ToString());
        //DWChatUtils.DebugLog(accountIdList.Dump());

        //foreach(var teamid in accountIdList)
        //{
        //    //DWChatUtils.DebugLog("OnQueryMyTeamsResult     DismissTeam=======" + teamid);
        //    //TeamAPI.DismissTeam(teamid, onTeamChangedNotify);
        //    //TeamAPI.LeaveTeam(teamid, onTeamChangedNotify);
        //}
    }

    [NIM.MonoPInvokeCallback(typeof(QueryTeamMembersInfoResultDelegate))]
    private void OnQueryTeamMembersInfoResult(string tid, int memberCount, bool includeUserInfo, NIMTeamMemberInfo[] infoList)
    {
        DWChatUtils.DebugLog("OnQueryTeamMembersInfoResult=====================");
        if (infoList != null && infoList.Length > 0)
        {
            foreach (NIMTeamMemberInfo v in infoList)
                DWChatUtils.DebugLog(v.Dump());

            DWChatUtils.DebugLog(string.Format("total: {0}", infoList.Length));
        }
        else
        {
            DWChatUtils.DebugLog(string.Format("tid {0} not found, total: {1}", tid, memberCount));
        }
    }

    [NIM.MonoPInvokeCallback(typeof(QueryMyTeamsInfoResultDelegate))]
    private void OnQueryMyTeamsInfoResult(NIMTeamInfo[] infoList)
    {
        DWChatUtils.DebugLog("OnQueryMyTeamsInfoResult===" + infoList.Dump());
        DWChatUtils.DebugLog("account === " + DWChatLogin.getInstance().DW_getAccountInfo().Account);
        foreach(var info in infoList)
        {
            if(info.OwnerId == DWChatLogin.getInstance().DW_getAccountInfo().Account)
            {
                DWChatUtils.DebugLog("DismissTeam=======" + info.TeamId);
                TeamAPI.DismissTeam(info.TeamId, onTeamChangedNotify);
            }
            else
            {
                DWChatUtils.DebugLog("LeaveTeam=======" + info.TeamId);
                TeamAPI.LeaveTeam(info.TeamId, onTeamChangedNotify);
            }
        }
        //_teamID = "";
        //_TeamOwnerID = "";
    }

    public void DW_leaveTeam(string teamid)
    {
        if (DWChatLogin.getInstance().DW_getAccountInfo().Account.CompareTo(_TeamOwnerID) != 0 && _teamID.CompareTo("") != 0)
        {
            DWChatUtils.DebugLog("DW_leaveTeam======= team id = " + _teamID + " user id = " + DWChatLogin.getInstance().DW_getAccountInfo().Account);
            TeamAPI.LeaveTeam(teamid, onTeamChangedNotify);
        }
    }

    public void DW_joinTeam(string teamid)
    {
        TeamAPI.ApplyForJoiningTeam(teamid, "", onTeamChangedNotify);
    }

}
