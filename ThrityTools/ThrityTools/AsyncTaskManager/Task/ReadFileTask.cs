/************************************************************
     File      : FileReadTask.cs
     author    : lezen   lezen@ezfun.cn
     function  : 文件读取任务
     version   : 1.0
     date      : 11/29/2016.
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/

using System.Collections;
using System;
using System.IO;
namespace ThrityTools
{
    public class ReadFileTask : BaseTask
    {
        public ReadFileTask() { }

        public ReadFileTask(string fileName, string md5, long fileLength,
            AsyncTaskWorkPlatform platform, Action successCB = null,
            Action<TaskWorkStatus> failCB = null, Action<long> updateCB = null)
        {
            m_fileName = fileName;
            m_fileLength = fileLength;
            m_md5 = md5;
            m_platform = platform;
            m_sucCB = successCB;
            m_failCB = failCB;
            m_updateCB = updateCB;
        }

        public override void WorkStart()
        {
            if (File.Exists(m_fileName))
            {
                if (m_platform == AsyncTaskWorkPlatform.IOS)
                {
                    FileStream fs = File.OpenRead(m_fileName);
                    long fileSize = fs.Length;
                    fs.Close();

                    if (fileSize == m_fileLength || m_fileLength == 0)//未传入文件长度校验
                        WorkEndSuccess();
                    else
                        WorkEndFail();
                }
                else
                {
                    var md5 = GenMD5(m_fileName);
                    if (m_md5 == md5 || m_md5 == "")//未传入md5校验
                        WorkEndSuccess();
                    else
                        WorkEndFail();
                }
            }
            else
            {
                WorkEndFail();
            }
        }
    }
}
