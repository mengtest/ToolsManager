/************************************************************
     File      : ThreadImpManager.cs
     author    : lezen   lezen@ezfun.cn
     function  : 线程工具管理
     version   : 1.0
     date      : 12/1/2016.
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace ThrityTools
{
    public enum AsyncTaskWorkPlatform
    {
        Window,
        IOS,
        Android,
    }
    public class AsyncTaskManager
    {
        private AsyncTaskWorkPlatform m_platform = AsyncTaskWorkPlatform.Window;
        private List<BaseTask> m_readFileTaskList = new List<BaseTask>();
        private List<BaseTask> m_downloadBigFileTaskList = new List<BaseTask>();
        private List<BaseTask> m_decompressFileTaskList = new List<BaseTask>();
        private List<BaseTask> m_downloadSmallFileList = new List<BaseTask>();
        private BaseTaskImp m_readFileTaskImp = null;
        private DownloadFileTaskImp m_downloadBigFileTaskImp = null;
        private DownloadFileTaskImp m_downloadSmalFileImp = null;
        private DecompressFileTaskImp m_decompressFileTaskImp = null;
        private const string compressName = ".zip";
        public AsyncTaskManager(AsyncTaskWorkPlatform platform)
        {
            m_platform = platform;
        }

        //获取当前解压文件的进度
        public float CurrentDecompressTaskDecompressSize
        {
            get
            {
                if (m_decompressFileTaskImp != null)
                {
                    return m_decompressFileTaskImp.CurrentTaskDecompressSize;
                }
                else
                {
                    //               CDebug.LogError("FileDecompressTaskImp is not init");
                    return 0;
                }
            }
        }

        /// <summary>
        /// 添加文件读取任务
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="md5">MD5码</param>
        /// <param name="fileLength">文件内存大小</param>
        /// <param name="successCB">单个任务执行成功回调</param>
        /// <param name="failCB">单个任务执行失败回调</param>
        /// <param name="updateCB">单个任务执行进度更新回调</param>
        public void AddReadFileTask(string fileName, string md5 = "", long fileLength = 0,
            Action successCB = null, Action<TaskWorkStatus> failCB = null,
            Action<long> updateCB = null)
        {
            ReadFileTask task = new ReadFileTask(fileName, md5, fileLength, m_platform,
                successCB, failCB, updateCB);
            m_readFileTaskList.Add(task);
        }

        /// <summary>
        /// 添加文件下载任务
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="url">下载地址</param>
        /// <param name="md5">MD5码</param>
        /// <param name="length">文件内存大小</param>
        /// <param name="sucCB">单个任务执行成功回调</param>
        /// <param name="failCB">单个任务执行失败回调</param>
        /// <param name="updateCB">单个任务执行进度更新回调</param>
        public void AddDownloadBigFileTask(string fileName, string url, string md5 = "",
            long length = 0, Action sucCB = null, Action<TaskWorkStatus> failCB = null,
            Action<long> updateCB = null)
        {
            DownloadBigFileTask task = new DownloadBigFileTask(fileName, url, md5, length,
                m_platform, sucCB, failCB, updateCB);
            m_downloadBigFileTaskList.Add(task);
        }
        /// <summary>
        /// 添加文件解压任务
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="md5">MD5码</param>
        /// <param name="fileLength">文件内存大小</param>
        /// <param name="unDecompressFile">未解压文件封装对象</param>
        /// <param name="successCB">单个任务执行成功回调</param>
        /// <param name="failCB">单个任务执行失败回调</param>
        /// <param name="updateCB">单个任务执行进度更新回调</param>
        public void AddDecompressFileTask(string fileName, string md5 = "", long fileLength = 0,
            CUnzipFile unDecompressFile = null, Action successCB = null,
            Action<TaskWorkStatus> failCB = null, Action<long> updateCB = null)
        {
            if (unDecompressFile == null)
            {
                //            CDebug.LogError("unDecompressFile is illegal");
                return;
            }

            if (!unDecompressFile.m_direc.EndsWith(compressName))
            {
                //           CDebug.LogError("unDecompressFile is not end with .zip");
                return;
            }

            DecompressFileTask task = new DecompressFileTask(fileName, md5, fileLength,
                m_platform, unDecompressFile, successCB, failCB, updateCB);

            m_decompressFileTaskList.Add(task);
        }

        /// <summary>
        /// 添加轻量级文件下载任务
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="sucCB">单个任务执行成功回调</param>
        /// <param name="failCB">单个任务执行失败回调</param>
        public void AddDownloadSmallFileTask(string url, Action<string> sucCB = null,
            Action<TaskWorkStatus> failCB = null)
        {
            DownloadSmallFileTask task = new DownloadSmallFileTask(url, m_platform,
                sucCB, failCB);
            m_downloadSmallFileList.Add(task);
        }

        /// <summary>
        /// 开始文件读取
        /// </summary>
        /// <param name="succCB">批次任务执行成功回调</param>
        /// <param name="failCB">批次任务执行失败回调</param>
        public void StartFileRead(Action succCB = null, Action failCB = null)
        {
            if (m_readFileTaskImp != null && !m_readFileTaskImp.IsFinish)
            {
                //            Debug.LogError("last file read task is not finish now");
                return;
            }
            if (m_readFileTaskList.Count == 0)
            {
                if (succCB != null)
                {
                    succCB();
                }
                return;
            }

            m_readFileTaskImp = new BaseTaskImp(m_platform);
            m_readFileTaskImp.Init(succCB, failCB);
            m_readFileTaskImp.AddTask(m_readFileTaskList);
            m_readFileTaskImp.Start();

            m_readFileTaskList.Clear();
        }

        /// <summary>
        /// 开始下载任务(有文件读写，支持文件校验)
        /// </summary>
        /// <param name="succCB">批次下载任务执行成功回调</param>
        /// <param name="failCB">批次下载任务执行失败回调</param>
        /// <param name="netConnectCheckCB">网络连接检查回调</param>
        /// <param name="tryTimeLimit">任务失败后尝试次数上限,value=0默认没有上限，不为0时，超过上限后结束任务执行</param>
        public void StartDownloadBigFile(Action succCB = null, Action failCB = null,
            NetConnectCheckCB netConnectCheckCB = null, int tryTimeLimit = 0)
        {
            if (m_downloadBigFileTaskImp != null && !m_downloadBigFileTaskImp.IsFinish)
            {
                //            CDebug.LogError("last file download task is not finish now");
                return;
            }

            if (m_downloadBigFileTaskList.Count == 0)
            {
                if (succCB != null)
                {
                    succCB();
                }
                return;
            }
            m_downloadBigFileTaskImp = new DownloadFileTaskImp(m_platform, tryTimeLimit);
            m_downloadBigFileTaskImp.Init(succCB, failCB);
            m_downloadBigFileTaskImp.AddTask(m_downloadBigFileTaskList);
            m_downloadBigFileTaskImp.SetNetConnectCheckCB(netConnectCheckCB);
            m_downloadBigFileTaskImp.Start();

            m_downloadBigFileTaskList.Clear();
        }

        /// <summary>
        /// 开始文件解压
        /// </summary>
        /// <param name="succCB">批次解压任务执行成功回调</param>
        /// <param name="failCB">批次解压任务执行失败回调</param>
        public void StartDecompressFile(Action succCB = null, Action failCB = null)
        {
            if (m_decompressFileTaskImp != null && !m_decompressFileTaskImp.IsFinish)
            {
                //            CDebug.LogError("last file decompress task is not finish now");
                return;
            }

            if (m_decompressFileTaskList.Count == 0)
            {
                if (succCB != null)
                {
                    succCB();
                }
                return;
            }
            m_decompressFileTaskImp = new DecompressFileTaskImp(m_platform);
            m_decompressFileTaskImp.Init(succCB, failCB);
            m_decompressFileTaskImp.AddTask(m_decompressFileTaskList);
            m_decompressFileTaskImp.Start();

            m_decompressFileTaskList.Clear();
        }

        /// <summary>
        /// 开始小文件下载任务(无文件读写，不支持文件校验)
        /// </summary>
        /// <param name="succCB">批次任务执行成功回调</param>
        /// <param name="failCB">批次任务执行失败回调</param>
        /// <param name="netConnectCheckCB">网络连接检查回调</param>
        /// <param name="tryTimeLimit">任务失败后尝试次数上限,value=0默认没有上限，不为0时，超过上限后结束任务执行</param>
        public void StartDownloadSmallFile(Action succCB = null, Action failCB = null, NetConnectCheckCB netConnectCheckCB = null, int tryTimeLimit = 0)
        {
            if (m_downloadSmalFileImp != null && !m_downloadSmalFileImp.IsFinish)
            {
                //            CDebug.LogError("last www down task is not finish now");
                return;
            }
            if (m_downloadSmallFileList.Count == 0)
            {
                if (succCB != null)
                {
                    succCB();
                }
                return;
            }
            m_downloadSmalFileImp = new DownloadFileTaskImp(m_platform, tryTimeLimit);
            m_downloadSmalFileImp.Init(succCB, failCB);
            m_downloadSmalFileImp.AddTask(m_downloadSmallFileList);
            m_downloadSmalFileImp.SetNetConnectCheckCB(netConnectCheckCB);
            m_downloadSmalFileImp.Start();

            m_downloadSmallFileList.Clear();
        }

        /// <summary>
        /// 暂停文件下载
        /// </summary>
        public void PauseDownloadBigFile()
        {
            if (m_downloadBigFileTaskImp != null)
            {
                m_downloadBigFileTaskImp.Pause();
            }
        }

        /// <summary>
        /// 暂停文件读取
        /// </summary>
        public void PauseReadFile()
        {
            if (m_readFileTaskImp != null)
            {
                m_readFileTaskImp.Pause();
            }
        }

        /// <summary>
        /// 暂停文件解压
        /// </summary>
        public void PauseDecompressFile()
        {
            if (m_decompressFileTaskImp != null)
            {
                m_decompressFileTaskImp.Pause();
            }
        }

        /// <summary>
        /// 暂停小文件下载
        /// </summary>
        public void PauseDownloadSmallFile()
        {
            if (m_downloadSmalFileImp != null)
            {
                m_downloadSmalFileImp.Pause();
            }
        }

        /// <summary>
        /// 恢复文件下载
        /// </summary>
        public void ResumeDownloadBigFile()
        {
            if (m_downloadBigFileTaskImp != null)
            {
                m_downloadBigFileTaskImp.Resume();
            }
        }

        /// <summary>
        /// 恢复文件读
        /// </summary>
        public void ResumeReadFile()
        {
            if (m_readFileTaskImp != null)
            {
                m_readFileTaskImp.Resume();
            }
        }

        /// <summary>
        /// 恢复文件解压
        /// </summary>
        public void ResumeDecompressFile()
        {
            if (m_decompressFileTaskImp != null)
            {
                m_decompressFileTaskImp.Resume();
            }
        }

        /// <summary>
        /// 恢复小文件下载
        /// </summary>
        public void ResumeDownloadSmallFile()
        {
            if (m_downloadSmalFileImp != null)
            {
                m_downloadSmalFileImp.Resume();
            }
        }

        /// <summary>
        /// 恢复所有操作
        /// </summary>
        public void ResumeAll()
        {
            ResumeReadFile();
            ResumeDownloadSmallFile();
            ResumeDecompressFile();
            ResumeDownloadBigFile();
        }

        /// <summary>
        /// 暂停所有操作
        /// </summary>
        public void PauseAll()
        {
            PauseDecompressFile();
            PauseDownloadBigFile();
            PauseDownloadSmallFile();
            PauseReadFile();
        }
        /// <summary>
        /// 关闭所有线程
        /// </summary>
        public void Abort()
        {
            if (m_platform != AsyncTaskWorkPlatform.IOS)
            {
                if (m_readFileTaskImp != null)
                {
                    m_readFileTaskImp.Abort();
                }
                if (m_downloadBigFileTaskImp != null)
                {
                    m_downloadBigFileTaskImp.Abort();
                }
                if (m_decompressFileTaskImp != null)
                {
                    m_decompressFileTaskImp.Abort();
                }
                if (m_downloadSmalFileImp != null)
                {
                    m_downloadSmalFileImp.Abort();
                }
            }
        }
    }
}
