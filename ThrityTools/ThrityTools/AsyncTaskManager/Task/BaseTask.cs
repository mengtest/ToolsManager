/************************************************************
     File      : BaseTask.cs
     author    : guoliang
     fuction   : 任务基类
     version   : 1.0
     date      : 11/29/2016.
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/
using System.Collections;
using System;
using System.IO;
using System.Net;

namespace ThrityTools
{
    public enum TaskWorkStatus
    {
        Working,           //工作中
        Success,			//成功
        Fail,			//失败
    }
    public class BaseTask
    {
        //工作平台
        protected AsyncTaskWorkPlatform m_platform = AsyncTaskWorkPlatform.Window;
        //文件名
        public string m_fileName;
        //MD5
        public string m_md5 = "";//为空时默认不校验
        //文件校验长度
        public long m_fileLength = 0;//为0时默认不校验
        //任务执行成功回调
        public Action m_sucCB = null;
        //任务执行失败回调
        public Action<TaskWorkStatus> m_failCB = null;
        //任务执行更新回调
        public Action<long> m_updateCB = null;
        //任务工作状态
        public TaskWorkStatus m_workStatus = TaskWorkStatus.Working;

        //网络下载相关变量
        protected HttpWebRequest m_webRequest;
        protected HttpWebResponse m_webResponse;
        protected Stream m_reqStream;


        public BaseTask() { }

        public virtual void WorkStart() { }

        protected virtual void WorkEndSuccess()
        {
            m_workStatus = TaskWorkStatus.Success;
            if (m_sucCB != null)
            {
                m_sucCB();
            }
        }

        protected virtual void WorkEndFail()
        {
            m_workStatus = TaskWorkStatus.Fail;
            if (m_failCB != null)
            {
                m_failCB(m_workStatus);
            }
        }

        public virtual void Pasue()
        {

        }

        public virtual void Resume()
        {

        }

        protected bool CheckMd5(string fileName)
        {
            if (m_platform == AsyncTaskWorkPlatform.IOS)
            {
                FileStream curFileStream = new FileStream(fileName, FileMode.Open,
                    FileAccess.Read, FileShare.Read);
                if (curFileStream != null && (m_fileLength == 0 || m_fileLength == curFileStream.Length))
                {
                    curFileStream.Close();
                    return true;
                }
                if (curFileStream != null)
                    curFileStream.Close();
                return false;
            }
            else
            {
                string md5 = GenMD5(fileName);
                return (m_md5 != "") ? m_md5 == md5 : true;
            }
        }

        protected string GenMD5(string fn)
        {
            var md5CSP = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var fs = new System.IO.FileStream(fn, FileMode.Open, FileAccess.Read);
            var arrbytHashValue = md5CSP.ComputeHash(fs);
            var strHashData = System.BitConverter.ToString(arrbytHashValue).Replace("-", "");
            fs.Close();
            return strHashData;
        }

        //建立http连接
        protected void CreateHttpWebRequest(string url, int range = 0)
        {
            m_webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            m_webRequest.Timeout = 10000;
            if (range > 0)
                m_webRequest.AddRange(range); //设置Range值

            m_webResponse = (HttpWebResponse)m_webRequest.GetResponse();

            m_reqStream = m_webRequest.GetResponse().GetResponseStream();
            m_reqStream.ReadTimeout = 10000;
        }

        //关闭http连接
        protected void CloseHttpWepRequest()
        {
            if (m_reqStream != null)
                m_reqStream.Close();

            if (m_webResponse != null)
                m_webResponse.Close();

            if (m_platform != AsyncTaskWorkPlatform.IOS)
            {
                if (m_webRequest != null)
                    m_webRequest.Abort();
            }
        }
    }
}
