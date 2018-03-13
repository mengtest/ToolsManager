/************************************************************
     File      : WWWDownTask.cs
     author    : guoliang
     version   : 1.0
     function  : www下载任务，不需要校验
     date      : 12/1/2016.
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/

using System.Collections;
using System;
using System.Text;
using System.Net;
using System.IO;

namespace ThrityTools
{
    public class DownloadSmallFileTask : BaseTask
    {
        public string m_url;
        private Action<string> m_sucCB_string = null;

        public DownloadSmallFileTask(string url, AsyncTaskWorkPlatform platform,
            Action<string> sucCB = null, Action<TaskWorkStatus> failCB = null)
        {
            m_url = url;
            m_sucCB_string = sucCB;
            m_failCB = failCB;
            m_platform = platform;
        }

        public DownloadSmallFileTask() { }

        public override void WorkStart()
        {
            try
            {
                CreateHttpWebRequest(m_url);

                WorkEndSuccess();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    //                Debug.LogError(ex.Message);
                }
                WorkEndFail();
            }
        }
        protected override void WorkEndSuccess()
        {
            m_workStatus = TaskWorkStatus.Success;

            string reader = new StreamReader(m_reqStream,
                Encoding.GetEncoding("utf-8")).ReadToEnd();

            if (m_sucCB_string != null)
                m_sucCB_string(reader);

            CloseHttpWepRequest();
        }
        protected override void WorkEndFail()
        {
            m_workStatus = TaskWorkStatus.Fail;

            if (m_failCB != null)
                m_failCB(m_workStatus);

            CloseHttpWepRequest();
        }
    }
}
