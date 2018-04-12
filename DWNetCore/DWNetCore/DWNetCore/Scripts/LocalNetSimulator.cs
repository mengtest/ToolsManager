//#define WITHOUT_NETWORK

using System.Collections;

#if WITHOUT_NETWORK
public static class LocalNetSimulator
{

    public static void OnSendMsg(csproto.CSMsgReqBody msg, int iCmdId)
    {
        switch (iCmdId)
        {
            case csprotoMacros.CS_REQ_ACCOUNT_LOGIN:
                OnAccoutLogin(msg);
                break;
            case csprotoMacros.CS_REQ_GET_RHYTHM_FLOOR_INFO:
                OnRhythmFloorInfo(msg);
                break;
            case csprotoMacros.CS_REQ_START_RHYTHM_FLOOR:
                ReturnSimpleRes(csprotoMacros.CS_RES_START_RHYTHM_FLOOR);
                break;
            case csprotoMacros.CS_REQ_FINISH_RHYTHM_FLOOR:
                OnFinishRhythmFloor(msg);
                break;
            default:
                Debug.LogError(string.Format("LocalNetSimulator UnHandled Msg: {0}", iCmdId));
                break;
        }
    }

    private static void OnFinishRhythmFloor(CSMsgReqBody msg)
    {
        CSMsgResponse response = GetMsgResponse(csprotoMacros.CS_RES_FINISH_RHYTHM_FLOOR);
        response.stBody.stFinishRhythmFloor.wFloorID = 1;
        response.stBody.stFinishRhythmFloor.bIsPass =0;
        response.stBody.stFinishRhythmFloor.wGrade = csprotoMacros.CS_GAME_GRADE_D;

        CNetSysEvent evt = new CNetSysEvent(csprotoMacros.CS_RES_FINISH_RHYTHM_FLOOR, response);
        CGameSystem.EventMgr.QueueEvent(evt);
    }

    private static void ReturnSimpleRes(int msgId)
    {
        CSMsgResponse response = GetMsgResponse(msgId);
        CNetSysEvent evt = new CNetSysEvent(msgId, response);
        CGameSystem.EventMgr.QueueEvent(evt);
    }

    private static void OnRhythmFloorInfo(CSMsgReqBody msg)
    {
        CSMsgResponse response = GetMsgResponse(csprotoMacros.CS_RES_GET_RHYTHM_FLOOR_INFO);
        response.stBody.stGetRhythmFloorInfo.iNowFloorID = 1;

        CNetSysEvent evt = new CNetSysEvent(csprotoMacros.CS_RES_GET_RHYTHM_FLOOR_INFO, response);
        CGameSystem.EventMgr.QueueEvent(evt);
    }

    private static void OnAccoutLogin(CSMsgReqBody msg)
    {
        CSMsgResponse response = GetMsgResponse(csprotoMacros.CS_RES_ACCOUNT_LOGIN);
        response.stBody.stAccLogin.dwResult = 0;
        response.stBody.stAccLogin.stDetail.construct(0);
        response.stBody.stAccLogin.stDetail.stAccInfo.ullAccountGID = 1111111L;
        response.stBody.stAccLogin.stDetail.stAccInfo.stBasic.wLevel = 1;
        response.stBody.stAccLogin.stDetail.stAccInfo.stBasic.chGender = 1;

        CNetSysEvent evt = new CNetSysEvent(csprotoMacros.CS_RES_ACCOUNT_LOGIN, response);
        CGameSystem.EventMgr.QueueEvent(evt);
    }

    private static CSMsgResponse GetMsgResponse(int cmdID)
    {
        CSMsgResponse response = new CSMsgResponse();
        CSMsgHead msgHead = new CSMsgHead();
        CSMsgResBody msgBody = new CSMsgResBody();
        msgHead.iCmdID = cmdID;
        msgBody.construct(cmdID);

        response.stHead = msgHead;
        response.stBody = msgBody;

        return response;
    }
}
#endif