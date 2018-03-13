/************************************************************
     File      : DownloadBigFileTask.cs
     author    : guoliang
     function  : 文件下载任务,需要校验
     version   : 1.0
     date      : 11/29/2016.
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/

using System.Collections;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace ThrityTools
{
    public class DownloadBigFileTask : BaseTask
    {
        public string m_url;
        private FileStream m_curFileStream;
        private long m_fileSeekPos = 0;
        private bool m_isPause = false;
        private long m_hasDownBits = 0;

        public DownloadBigFileTask(string fileName, string url, string md5, long length,
            AsyncTaskWorkPlatform platform, Action sucCB = null,
            Action<TaskWorkStatus> failCB = null, Action<long> updateCB = null)
        {
            m_fileName = fileName;
            m_url = url;
            m_md5 = md5;
            m_platform = platform;
            m_sucCB = sucCB;
            m_failCB = failCB;
            m_updateCB = updateCB;
            m_fileLength = length;
            m_hasDownBits = 0;
        }

        public DownloadBigFileTask() { }

        public override void WorkStart()
        {
            InitFileStream();
            
            m_hasDownBits = m_fileSeekPos;

            if (m_updateCB != null)
            {
                m_updateCB(m_hasDownBits);
            }

            try
            {
                CreateHttpWebRequest(m_url, (int)m_fileSeekPos);

                CopyNetStreamToFileStream();

                if (m_curFileStream != null)
                    m_curFileStream.Close();

                if (CheckMd5(m_fileName))
                {
                    WorkEndSuccess();
                }
                else
                {
                    WorkEndFail();
                    File.Delete(m_fileName);//文件不合法，删除
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    //CDebug.LogError(ex.Message);
                }
                if (m_curFileStream != null)
                    m_curFileStream.Close();

                WorkEndFail();
            }
        }

        //复制流文件操作
        protected void CopyNetStreamToFileStream()
        {
            byte[] nbytes = new byte[1024 * 1024];
            int nReadSize = 0;
            nReadSize = m_reqStream.Read(nbytes, 0, 1024 * 1024);

            while (nReadSize > 0)
            {
                if (!m_isPause)
                {
                    m_curFileStream.Write(nbytes, 0, nReadSize);
                    m_hasDownBits += nReadSize;

                    if (m_updateCB != null)
                        m_updateCB(m_hasDownBits);

                    nReadSize = m_reqStream.Read(nbytes, 0, 1024 * 1024);
                }

                if (m_isPause)
                    Thread.Sleep(10);
            }
        }

        //初始化文件流
        protected void InitFileStream()
        {
            if (File.Exists(m_fileName))
            {
                if (CheckMd5(m_fileName))
                {
                    m_workStatus = TaskWorkStatus.Success;
                    return;
                }

                m_curFileStream = new FileStream(m_fileName, FileMode.Open, FileAccess.ReadWrite,
                    FileShare.Read);

                if (m_curFileStream.Length > m_fileLength)//文件长度已经非法，需要删除文件
                {
                    m_curFileStream.Close();
                    File.Delete(m_fileName);

                    m_curFileStream = new FileStream(m_fileName, FileMode.Create,
                        FileAccess.ReadWrite, FileShare.Read);
                    m_fileSeekPos = 0;
                }
                else
                {
                    m_fileSeekPos = m_curFileStream.Length;//通过字节流的长度确定当前的下载位置
                    m_curFileStream.Seek(m_fileSeekPos, SeekOrigin.Current); //移动文件流中的当前指针
                }
            }
            else
            {
                m_curFileStream = new FileStream(m_fileName, FileMode.Create,
                    FileAccess.ReadWrite, FileShare.Read);
                m_fileSeekPos = 0;
            }
        }

        protected override void WorkEndSuccess()
        {
            CloseHttpWepRequest();

            m_workStatus = TaskWorkStatus.Success;

            if (m_sucCB != null)
                m_sucCB();
        }

        protected override void WorkEndFail()
        {
            CloseHttpWepRequest();

            m_workStatus = TaskWorkStatus.Fail;

            if (m_failCB != null)
                m_failCB(m_workStatus);
        }

        public override void Pasue()
        {
            m_isPause = true;
        }

        public override void Resume()
        {
            m_isPause = false;
        }
    }
}