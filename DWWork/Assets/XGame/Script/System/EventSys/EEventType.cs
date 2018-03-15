//#define WITHOUT_NETWORK

using UnityEngine;
using System.Collections;

public enum EEventType
{
    EventTypeNull = 0,
    #region common events
    GamePause,                      //游戏停顿消息
    ChangeGameState,                //改变游戏状态消息
    AccGuestLogin,                  //点击快速登录 accountName
    LuaNetCallBack,                 //lua net call back
    GmExecute,                      //GM操作
    ChangeGameStateForce,           //强制改变游戏状态消息
    RefreshServerList,              //刷新服务器列表
    Msg_Quit_Fuben,                 //退出副本
    ActorRest,                //让怪物和玩家处于idle状态,仅对单人地图有效
    #endregion

    OpenNewWin,
    UI_Msg_ClearCamera,
    UI_Msg_CloseWindow,
    CloseWin,
    UI_Msg_RefreshErrorContent,
    LoadingStateFinish,
    LuaWindowEv,
    EnterStateFinish,
    ChangeScene,
    ShowWaitWindow,
    OpenOrCloseWindow,
    ScreenShotEnd,  //截屏完成
    WeiXinLoginEnd,
    ShareFail,

    GameSvrConnectSuc,
    NetReconnectStart,//断线重连开始
    NetReconnectSuccess,
    NetResetAutoConnect,

    UI_Msg_LoginOut,

    NetWorkTypeRefresh,
    NetWorkStrengthRefresh,
    LocationRefresh,
    BatteryRefreh,
    DeviceIDRefresh,

    NimIsNotInTeam, //当前不在语音聊天群

    ApplicationAwakeEvent,
    ApplicationPauseEvent,
    
    Max,
}
