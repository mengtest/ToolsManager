// /************************************************************
//     File      : CUnzipFile.cs
//     brief     : 异步解压缩
//     author    : JanusLiu   janusliu@ezfun.cn
//     version   : 1.0
//     date      : 2015/11/19 11:28:45
//     copyright : Copyright 2014 EZFun Inc.
// **************************************************************/

using System;
using UnityEngine;
using System.IO;
using System.Collections;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;


public class CUnzipFile
{
    public enum EnumUnzipStatus
    {
        EUS_NONE,               //解压缩中
        EUS_SUCCESS,            //解压成功
        EUS_FileNotFound,       //文件不存在
        EUS_ERROR,              //未知错误
        EUS_CANCELL,            //手动取消
    }

    public volatile EnumUnzipStatus m_unzipState = EnumUnzipStatus.EUS_NONE;
    public float m_rate = 0f;
    /// <summary>
    /// 这个size 取自CDownloadFile中下载下来的长度
    /// </summary>
    public long m_totalSize = 0;

    public string m_direc = "";
    private string m_fileName = null;
    private bool m_cancell = false;

    public List<string> m_unpackFileList = new List<string>();

    public CUnzipFile(string direc, string fileName, long unzipSize)
    {
        m_direc = direc;
        m_fileName = fileName;
        m_unzipState = EnumUnzipStatus.EUS_NONE;
        m_rate = 0;
        m_cancell = false;
        m_totalSize = unzipSize;
    }

    public void BeginUnzip()
    {
        m_cancell = false;
        m_unzipState = EnumUnzipStatus.EUS_NONE;
        try
        {
            if (!File.Exists(m_fileName))
            {
                m_unzipState = EnumUnzipStatus.EUS_FileNotFound;
                return;
            }
            //这里不需要先读一遍了  这个size 取自CDownloadFile中下载下来的长度
            //FileStream tempMemory = new FileStream(m_fileName, FileMode.Open);
            //ZipInputStream zipS = new ZipInputStream(tempMemory);
            //         m_totalSize = 0;
            //ZipEntry zipItem = zipS.GetNextEntry();
            //while(zipItem != null)
            //{
            //	m_totalSize += zipItem.Size;
            //	zipItem = zipS.GetNextEntry();
            //}
            //zipS.Close();
            //tempMemory.Close();

            FileStream tempMemory = new FileStream(m_fileName, FileMode.Open);
            ZipInputStream zipS = new ZipInputStream(tempMemory);
            ZipEntry zipItem = zipS.GetNextEntry();
            int unzipSize = 0;
            while (zipItem != null && !m_cancell)
            {
                byte[] temp = new byte[1024 * 1024];
                int len = 0;
                DirectoryInfo dir = new DirectoryInfo(m_direc);
                if (!dir.Exists)
                {
                    dir.Create();
                }

                //现在分目录了 新加创建目录
                int findIndex = zipItem.Name.LastIndexOf("/");
                if (findIndex != -1)
                {
                    string newDirName = m_direc + "/" + zipItem.Name.Substring(0, findIndex);
                    dir = new DirectoryInfo(newDirName);
                    if (!dir.Exists)
                    {
                        dir.Create();
                    }
                }

                if (zipItem.IsFile)
                {
                    FileStream save = new FileStream(m_direc + "/" + zipItem.Name, FileMode.Create);
                    while ((len = zipS.Read(temp, 0, temp.Length)) > 0 && !m_cancell)
                    {
                        save.Write(temp, 0, len);
                        unzipSize += len;
                        m_rate = unzipSize * 1f / m_totalSize;
                        if (m_rate >= 1)
                        {
                            m_rate = 1;
                        }
                    };
                    m_unpackFileList.Add(m_direc + "/" + zipItem.Name);
                    save.Flush();
                    save.Close();
                }
                else if (zipItem.IsDirectory)
                {
                    DirectoryInfo childdir = new DirectoryInfo(m_direc + "/" + zipItem.Name);
                    if (!childdir.Exists)
                    {
                        childdir.Create();
                    }
                }
                zipItem = zipS.GetNextEntry();
            }

            zipS.Close();
            tempMemory.Close();
            if (m_cancell)
            {
                m_unzipState = EnumUnzipStatus.EUS_CANCELL;
            }
            else
            {
                m_unzipState = EnumUnzipStatus.EUS_SUCCESS;
                File.Delete(m_fileName);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("unzip save file:" + e.ToString());
            m_unzipState = EnumUnzipStatus.EUS_ERROR;
        }
    }

    public void Cancell()
    {
        m_cancell = true;
    }
}
