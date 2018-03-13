/************************************************************
     File      : NewDownLoadSys.cs
     author    : lenzen  lezen@ezfun.cn
     function  : 下载系统
     version   : 1.0
     date      : 11/29/2016.
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using LitJson;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System;
using ThrityTools;

public enum AssetsPackageState
{
    None,
    ReadFileEnd,
    DownLoadStart,
    DownLoadStop,
    DownLoadComplete,
    DecompressStart,
    DecompresFail,
    AssetsReady,
    AssetsCheck,
}

public class DownLoadSys : ISystem
{
    public static DownLoadSys Instance = null;
    public AssetsPackageState m_curPackageState = AssetsPackageState.None;
    public List<string> m_unClickBtnList = new List<string>();   //不可以点击的按钮
    public List<int> m_clickCardList = new List<int>();        //可以点击的副将id

    private XGameUpdateData.UpdateStruct m_updateStruct = null;
    private List<XGameUpdateData.UpdateInfo> m_updateInfo = new List<XGameUpdateData.UpdateInfo>();

    private List<string> m_needDeleteFileList = new List<string>();    //需要删除的包
    public List<string> m_abMapList = new List<string>();

    private string m_downloadPath = null;
    private string m_availablePath = null;

    private long m_totalSize = 0;             //下载最大长度
    private long m_curSize = 0;               //下载当前长度
    private long m_childDownSize = 0;//当前子文件下载长度
    private long m_curDecompressSize = 0;     //解压当前长度
    private long m_totalDecompressSize = 0;   //解压最大长度

    private bool m_isInit = false;
    public bool m_isDownLoadSuc = false;
    public bool m_isDecompressSuc = false;

    private const string compressName = ".zip";
    private const string DownloadConfigKey = "DownLoadConfig";

    private AsyncTaskManager m_AsyncTaskMgr = null;

    public bool IsGetReward
    {
        private set;
        get;
    }

    public float DownLoadProgress
    {
        get
        {
            if (m_isDownLoadSuc || m_curSize > m_totalSize)
            {
                return 1.0f;
            }
            if (m_totalSize == 0)
            {
                return 0.0f;
            }
            if (m_curSize < 0)
            {
                m_curSize = 0;
            }
            return ((m_curSize + m_childDownSize) * 1f / m_totalSize);
        }
    }
    public float DecompressProgress
    {
        get
        {
            if (m_isDecompressSuc || m_curDecompressSize > m_totalDecompressSize)
            {
                return 1.0f;
            }
            if (m_totalDecompressSize == 0)
            {
                return 0.0f;
            }
            return (m_curDecompressSize + m_AsyncTaskMgr.CurrentDecompressTaskDecompressSize) * 1f / m_totalDecompressSize;
        }
    }

    public override void Init()
    {
        if (m_isInit)
        {
            return;
        }
        m_isInit = true;
        Instance = this;
        m_updateInfo.Clear();
        m_abMapList.Clear();

        m_curPackageState = AssetsPackageState.None;
        m_isDownLoadSuc = false;
        m_isDecompressSuc = false;
        IsGetReward = false;
        InitCanNotClickBtn();

        InitAsyncTaskManager();

        EventSys.Instance.AddHander(EEventType.DecompressDownFileSuc, (EEventType eventID, object p1, object p2) =>
        {
            DecompressAssetsSuccess((int)p1);
        });
        EventSys.Instance.AddHander(EEventType.CheckWifiOpenToDownload, (EEventType eventID, object p1, object p2) =>
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
//                || Application.platform == RuntimePlatform.WindowsEditor)  //wifi 不能在线程里调用
            {
                if (CheckEnoughDisk())
                {
                    StartDownLoadAssets();
                    //通知窗口刷新
                    EventSys.Instance.AddEvent(EEventType.RefreshDownloadWindow);
                }
            }
        });
        EventSys.Instance.AddHander(EEventType.DownloadFileSuc, (EEventType eventID, object p1, object p2) =>
        {
            StartDecompressAssets();
            DataEyeSys.Instance.OnEndDownload(CAccMgr.Instance.Level);
        });
        EventSys.Instance.AddHander(EEventType.Msg_ShowTips, (EEventType eventID, object p1, object p2) =>
        {
            if (p1 is string)
            {
                WindowRoot.ShowTips((string)p1);
            }
        });
        EventSys.Instance.AddHander(EEventType.CheckDownloadAssetBundleLoaded, (EEventType eventID, object p1, object p2) =>
        {
            CheckAssetBundleLoaded();
        });
    }

    public override void Reset()
    {
        base.Reset();
        IsGetReward = false;
    }

    //初始化异步任务管理
    private void InitAsyncTaskManager()
    {  
        AsyncTaskWorkPlatform platform = AsyncTaskWorkPlatform.Window;
        if (Application.platform == RuntimePlatform.Android)
        {
            platform = AsyncTaskWorkPlatform.Android;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            platform = AsyncTaskWorkPlatform.IOS;
        }
        m_AsyncTaskMgr = new AsyncTaskManager(platform);
    }
    // 初始化不可以点击的按钮
    public void InitCanNotClickBtn()
    {
        string[] btnNameArray = new string[]{
        "btn_gem",             //宝石系统
        "menuBtn_zhenfa",       //阵法
        "btn_shenshou",        //筋脉系统
        "Btn_forge",          //打造
        "menuBtn_jinjichang",  //竞技场
        "menuBtn_juewei",      //爵位
        "menuBtn_guild",       //公会
        "menuBtn_paihang",     //排行
        "ico_actenter",        //日常活动
        "ico_baozang",         //龙宫宝藏
        "ico_store",           //商店
        "7",                   //国王膜拜
        "8",                   //竞技场膜拜
       "step4",               //饰品
       "step3",               //翅膀
        "stronger",            //变强
        "GM",};
        for (int i = 0; i < btnNameArray.Length; i ++ )
        {
            m_unClickBtnList.Add(btnNameArray[i]);
        }
        m_clickCardList.Add(513);      //嫦娥
        m_clickCardList.Add(520);      //沙僧
        m_clickCardList.Add(524);      //牛二
    }

    // 是否可以点击按钮
    public bool CheckCanClickBtn(string name)
    {
        if (m_unClickBtnList.Contains(name))
        {
            return true;
        }
        return false;
    }

    // 可以点击的卡牌
    public bool CheckCanClickCard(int id)
    {
        if (m_clickCardList.Contains(id))
        {
            return true;
        }
        return false;
    }

    public void CheckInitDownLoad()
    {
        if (m_curPackageState != AssetsPackageState.None)//不是初始化状态，说明进入主城后已经检查过了，不然每次进主城都会走下面的逻辑
        {
            return;
        }
        if (Version.Instance.CheckIsPrePublish())
        //if (Version.Instance.CheckIsPrePublish() ||
         //   !CheckStorageSize())
        {
            m_isDecompressSuc = true;
            m_isDownLoadSuc = true;
            m_curPackageState = AssetsPackageState.AssetsReady;
            return;
        }
        if (ClientDataSys.Instance.IsAssetsPackageReady())
        {
            m_isDecompressSuc = true;
            m_isDownLoadSuc = true;
            m_curPackageState = AssetsPackageState.AssetsReady;
        }
        else
        {
            if (m_curPackageState == AssetsPackageState.None)
            {
                LoginCheckDownLoadInfo();
            }
        }
    }

    //用于登录检查下载资源
    private void LoginCheckDownLoadInfo()
    { 
        //创建下载目录
        m_availablePath = EZFunTools.GetPathForPlatform(Application.platform, AssetPathType.TargetAssetsBundlPath);
        if (!Directory.Exists(m_availablePath))
        {
            Directory.CreateDirectory(m_availablePath);
        }
        if (m_updateStruct == null)
        {
            GetUpdateStructForSvr(InitUpateFileInfo);
        }
        else
        {
            InitUpateFileInfo();
        }
       
    }
    //拉取下载文件清单信息
    private void GetUpdateStructForSvr(Action succCB = null)
    {
        var p = Constants.DOWNLOAD_URL + Constants.DOWNLOAD_CONFIG_FILE;
        m_AsyncTaskMgr.AddDownloadSmallFileTask(p, (string text) =>
        {
            //                    Debug.LogError("www download " + p + "success");
            m_updateStruct = JsonMapper.ToObject<XGameUpdateData.UpdateStruct>(text);
            if (m_updateStruct != null)
            {
                if (succCB != null)
                    succCB();
            }
        }, (TaskWorkStatus status) =>
        {
            //                    Debug.LogError("www download "+p+"fail");
        });
        m_AsyncTaskMgr.StartDownloadSmallFile(null, null, CNetSys.Instance.CheckConnected);
    }


    private bool CheckUpdateInfoByPackagetVersion()
    {
        string packageVer = Version.Instance.GetPackagetVersion();

        if (m_updateStruct != null &&
          m_updateStruct.update_package_info != null &&
          m_updateStruct.update_package_info.Count != 0 &&
          m_updateStruct.update_package_info[0].app_update_info != null &&
          m_updateStruct.update_package_info[0].app_update_info[0].UpdateInfo != null)
        {
            m_updateInfo.Clear();
            m_updateInfo.AddRange(m_updateStruct.update_package_info[0].app_update_info[0].UpdateInfo);
            m_downloadPath = Constants.CDNDOWNLOAD_URL + packageVer + "/" +
                Constants.CDN_DOWNLOAD_TESTNUM + "/AssetsBundle/";
            return true;
        }
        else
        {
            return false;
        }
    }

    //初始化更新文件信息
    private void InitUpateFileInfo()
    {
        if (!CheckUpdateInfoByPackagetVersion())
        {
            return;
        }
        //遍历json中的下载信息，统计已下载，总下载，初始化需要下载文件列表
        for (int i = 0; i < m_updateInfo.Count; i++)
        {
            var updateInfo = m_updateInfo[i];
            InitFileReadTask(m_updateInfo[i], m_availablePath);
            m_needDeleteFileList.Add(m_availablePath + m_updateInfo[i].name);

            if (updateInfo.name.Contains("abMap"))
            {
                string name = updateInfo.name.Remove(updateInfo.name.Length - 4);
                m_abMapList.Add(name);
            }
        }

        //开始读取文件任务队列
        m_AsyncTaskMgr.StartFileRead(() =>
        {
            InitFileReadEnd();
        }, () =>
        {
            InitFileReadEnd();
        });
    }

    //初始化文件读取任务
    private void InitFileReadTask(XGameUpdateData.UpdateInfo updateInfo, string path)
    {
        string fileName = path + updateInfo.name;
        string url = Constants.CDNDOWNLOAD_Path + m_downloadPath + updateInfo.name;
        string decompressFileName = fileName.Remove(fileName.Length - compressName.Length);

        //下载文件读取
        m_totalSize += updateInfo.size;
        m_AsyncTaskMgr.AddReadFileTask(fileName, updateInfo.md5, updateInfo.size,
            //读取成功回调
            () =>
            {
                m_curSize += updateInfo.size;
            },
            //读取失败回调
            (TaskWorkStatus status) =>
            {

                m_AsyncTaskMgr.AddDownloadBigFileTask(fileName, url, updateInfo.md5,
                    updateInfo.size,
                    () =>
                    {
                        m_childDownSize = 0;
                        m_curSize += updateInfo.size;
                    }, 
                    (TaskWorkStatus downStatus) =>
                    {
                        m_childDownSize = 0;
                    }, 
                    (long curSize) =>
                    {
                        m_childDownSize = curSize;
                    }
                    );
            }
            );

        //解压后文件读取
        var unDecompressFile = new CUnzipFile(fileName, decompressFileName, updateInfo.desize);
        m_totalDecompressSize += updateInfo.desize;

        m_AsyncTaskMgr.AddReadFileTask(decompressFileName, updateInfo.demd5, updateInfo.desize,
            () =>
            {
                m_curDecompressSize += updateInfo.desize;
            }, 
            (TaskWorkStatus status) =>
            {
                m_AsyncTaskMgr.AddDecompressFileTask(decompressFileName, updateInfo.demd5, 
                updateInfo.desize, unDecompressFile, 
                () =>
                {
                    m_curDecompressSize += updateInfo.desize;
                });
            }
            );
    }

    
    //读取文件结束
    private void InitFileReadEnd()
    {
        m_curPackageState = AssetsPackageState.ReadFileEnd;
        //检查是否所有下载文件成功
        if (m_curSize == m_totalSize)
        {
            m_isDownLoadSuc = true;
            m_curPackageState = AssetsPackageState.DownLoadComplete;
        }
        //检查是否所有解压缩文件成功
        if (m_curDecompressSize == m_totalDecompressSize)
        {
            m_isDecompressSuc = true;
            CheckAndDelExistZipFile();
            m_curPackageState = AssetsPackageState.AssetsReady;
        }
        StartDecompressAssets();
    }

    // 开始下载资源文件
    private void StartDownLoadAssets()
    {
        if (m_isDownLoadSuc)
        {
            return;
        }
        m_AsyncTaskMgr.StartDownloadBigFile(DownLoadAssetsSuccess, () =>
        {
            m_isDownLoadSuc = false;
        }, CheckNetConnected);
    }

    private bool CheckNetConnected()
    {
        return true;
    }

    //下载所有资源成功
    private void DownLoadAssetsSuccess()
    {
        m_isDownLoadSuc = true;
    }

    //开始解压所有文件
    private void StartDecompressAssets()
    {
        if (m_isDecompressSuc)
        {
            return;
        }
 
        m_AsyncTaskMgr.StartDecompressFile(() =>
        {
            
        }, () =>
        {
            m_isDecompressSuc = false;
            m_curPackageState = AssetsPackageState.DecompresFail;
        });
        m_curPackageState = AssetsPackageState.DecompressStart;
    }

    //解压所有资源成功
    private void DecompressAssetsSuccess(float startDecompressSec)
    {
        m_isDecompressSuc = true;
        CheckAndDelExistZipFile();
        m_curPackageState = AssetsPackageState.AssetsReady;
        
    }

    private void CheckAndDelExistZipFile()
    {
        for (int i = 0; i < m_needDeleteFileList.Count; i++)
        {
            if (File.Exists(m_needDeleteFileList[i]))
            {
                File.Delete(m_needDeleteFileList[i]);
            }
        }
    }

    //停止下载
    public void StopDownLoad()
    {
        if (m_curPackageState == AssetsPackageState.DownLoadStart)
        {
            m_AsyncTaskMgr.PauseDownloadBigFile();
        }
        m_curPackageState = AssetsPackageState.DownLoadStop;
    }
    //开始/恢复下载
    public bool ResumeDownLoad()
    {
        if (m_curPackageState == AssetsPackageState.DownLoadStop)
        {
            m_AsyncTaskMgr.ResumeDownloadBigFile();
            m_curPackageState = AssetsPackageState.DownLoadStart;
            return true;
        }
        else if (m_curPackageState == AssetsPackageState.None)
        {
            WindowRoot.ShowTips(TextData.GetText(2000305));
            return true;
        }
        else if (m_curPackageState == AssetsPackageState.ReadFileEnd)
        {
            if (!m_isDownLoadSuc)
            {
                if (CheckEnoughDisk())
                {
                    StartDownLoadAssets();
                    return true;
                }
            }
            else if (!m_isDecompressSuc)  //没有解压完直接开始解压
            {
                StartDecompressAssets();
                return true;
            }
        }
        return false;
    }

    //退出游戏
    void OnApplicationQuit()
    {
        if (m_AsyncTaskMgr != null)
        {
            m_AsyncTaskMgr.Abort();
        }
    }
   

    public bool CheckEnoughDisk()
    {
        if(Application.platform == RuntimePlatform.WindowsEditor)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 资源校验
    /// </summary>
    public void CheckDownLoadAssets()
    {
        OwnCheckResourceLegal();
    }

        
#region Own Check Resource Legal
    private bool m_checkResourceIsLegal = true;
    /// <summary>
    /// 主动检查下载资源是否合法，用于下载成功后的玩家主动选择校验
    /// </summary>
    private void OwnCheckResourceLegal()
    {
        if (m_curPackageState == AssetsPackageState.AssetsCheck)
        {
            WindowRoot.ShowTips(TextData.GetText(2000308));
            return;
        }
        else if (m_curPackageState != AssetsPackageState.AssetsReady)//其他状态不能检查
        {
            WindowRoot.ShowTips(TextData.GetText(EnumText.ET_DownLoadCheckIn));
            return;
        }
        //创建下载目录
        m_availablePath = EZFunTools.GetPathForPlatform(Application.platform, AssetPathType.TargetAssetsBundlPath);
        if (!Directory.Exists(m_availablePath))
        {
            Directory.CreateDirectory(m_availablePath);
        }
        m_curPackageState = AssetsPackageState.AssetsCheck;
        m_checkResourceIsLegal = true;
        GetUpdateStructForSvr(OwnCheckDownloadedAssetFileLegal);
    }
    //检查下载解压后的asset是否合法
    private void OwnCheckDownloadedAssetFileLegal()
    {
        if (!CheckUpdateInfoByPackagetVersion())
        {
            return;
        }
        m_totalSize = 0;
        m_totalDecompressSize = 0;
        m_abMapList.Clear();
        m_curSize = 0;
        m_curDecompressSize = 0;
        m_childDownSize = 0;
        //遍历json中的下载信息，统计已下载，总下载，初始化需要下载文件列表
        for (int i = 0; i < m_updateInfo.Count; i++)
        {
            var updateInfo = m_updateInfo[i];
            InitOwnCheckFileReadTask(m_updateInfo[i], m_availablePath);
            m_needDeleteFileList.Add(m_availablePath + m_updateInfo[i].name);

            if (updateInfo.name.Contains("abMap"))
            {
                string name = updateInfo.name.Remove(updateInfo.name.Length - 4);
                m_abMapList.Add(name);
            }
        }

        //开始读取文件任务队列
        m_AsyncTaskMgr.StartFileRead(() =>
        {
            InitCheckReadFileEnd();
        }, () =>
        {
            InitCheckReadFileEnd();
        });
    }

    //初始化校验文件读取任务
    private void InitOwnCheckFileReadTask(XGameUpdateData.UpdateInfo updateInfo, string path)
    {
        string fileName = path + updateInfo.name;
        string url = Constants.CDNDOWNLOAD_Path + m_downloadPath + updateInfo.name;

        //下载文件读取
        m_totalSize += updateInfo.size;
       
        //解压后文件读取
        string decompressFileName = fileName.Remove(fileName.Length - compressName.Length);
        var unDecompressFile = new CUnzipFile(fileName, decompressFileName, updateInfo.desize);
        m_totalDecompressSize += updateInfo.desize;

        m_AsyncTaskMgr.AddReadFileTask(decompressFileName, updateInfo.demd5, updateInfo.desize,
            () =>
            {
                m_curDecompressSize += updateInfo.desize;
                m_curSize += updateInfo.size;
            },
            (TaskWorkStatus status) =>
            {
                m_checkResourceIsLegal = false;
                //删除非法asset文件
                if (File.Exists(decompressFileName))
                {
                    File.Delete(decompressFileName);
                }
                //删除非法的下载zip文件
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                //校验失败添加到解压缩队列
                m_AsyncTaskMgr.AddDecompressFileTask(decompressFileName, updateInfo.demd5,
                updateInfo.desize, unDecompressFile,
                () =>
                {
                    m_curDecompressSize += updateInfo.desize;
                });

                //重新添加到下载队列
                m_AsyncTaskMgr.AddDownloadBigFileTask(fileName, url, updateInfo.md5,
                   updateInfo.size,
                   () =>
                   {
                       m_childDownSize = 0;
                       m_curSize += updateInfo.size;
                   },
                   (TaskWorkStatus downStatus) =>
                   {
                       m_childDownSize = 0;
                   },
                   (long curSize) =>
                   {
                       m_childDownSize = curSize;
                   }
                   );

            }
            );
    }

    //校验文件读取结束处理
    private void InitCheckReadFileEnd()
    {
        if (!m_checkResourceIsLegal)
        {
            InitFileReadEnd();
        }
    }

#endregion
}
