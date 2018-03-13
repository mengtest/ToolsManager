/************************************************************
     File      : DecompressFileTask.cs
     author    : guoliang
     function  : 文件解压任务
     version   : 1.0
     date      : 11/29/2016.
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/
using System.Collections;
using System;
using System.IO;

namespace ThrityTools
{
    public class DecompressFileTask : BaseTask
    {
        CUnzipFile m_unDecompressFile = null;

        public DecompressFileTask() { }

        public DecompressFileTask(string fileName, string md5, long fileLength, AsyncTaskWorkPlatform platform,
            CUnzipFile unDecompressFile = null, Action successCB = null,
            Action<TaskWorkStatus> failCB = null, Action<long> updateCB = null)
        {
            m_fileName = fileName;
            m_fileLength = fileLength;
            m_md5 = md5;
            m_unDecompressFile = unDecompressFile;
            m_platform = platform;
            m_sucCB = successCB;
            m_failCB = failCB;
            m_updateCB = updateCB;
        }

        public override void WorkStart()
        {
            try
            {
                if (m_unDecompressFile != null)
                {
                    m_unDecompressFile.LZ4UnCompress();

                    if (m_unDecompressFile.m_unzipState == CUnzipFile.EnumUnzipStatus.EUS_SUCCESS)
                    {
                        WorkEndSuccess();
                    }
                    else
                    {
                        WorkEndFail();
                    }
                }
                else
                {
                    //            CDebug.LogError("unDecompressFile is null");
                    WorkEndFail();
                }
            }
            catch(Exception ex)
            {
                 if (ex != null)
                 {
 //                    UnityEngine.Debug.LogError(ex.ToString());
                 }
            }
        }

        //当前已解压缩大小
        public int CurrentDecompressSize
        {
            get
            {
                if (m_unDecompressFile != null)
                    return m_unDecompressFile.m_currentSize;

                return 0;
            }
        }
    }
}