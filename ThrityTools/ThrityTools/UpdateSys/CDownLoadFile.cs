// /************************************************************
//     File      : CDownLoadFile.cs
//     brief     : 断点续传
//     author    : JanusLiu   janusliu@ezfun.cn
//     version   : 1.0
//     date      : 2015/11/19 11:28:51
//     copyright : Copyright 2014 EZFun Inc.
// **************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Net;
using System;

public class CDownLoadFile
{
	public enum EnumDownLoadStatus
	{
		EDLS_NONE, 				//下载状态
		EDLS_SUCCESS,			//下载成功
		EDLS_NETERROR,			//链接网络失败
		EDLS_MD5WRONG,			//md5校验失败
		EDLS_STOP,				//手动停止下载
	}

	public volatile EnumDownLoadStatus m_downLoadStatus = EnumDownLoadStatus.EDLS_NONE;
	public long m_hasDownLoadBits = 0;

	private string m_fileName;
	private string m_url;
	private string m_md5;
	private bool m_stop = false;
	public CDownLoadFile(string fileName, string url, string md5)
	{
		m_fileName = fileName;
		m_url = url;
		m_md5 = md5;
		m_stop = false;
	}

	//开始下载
	public void BeginDownLoad()
	{
		m_stop = false;
		m_hasDownLoadBits = 0;//用于显示当前的进度
		long lStartPos = 0; //打开上次下载的文件或新建文件 
		FileStream fs;
		if (File.Exists(m_fileName))
		{
			var md5 = DWTools.GenMD5(m_fileName);
			Debug.Log ("get md5:" + md5);
			Debug.Log ("need md5:" + m_md5);
			if(m_md5.ToLower() == md5.ToLower())
			{
				m_downLoadStatus = EnumDownLoadStatus.EDLS_SUCCESS;
				return;
			}

			fs = File.OpenWrite(m_fileName);//打开流
			lStartPos = fs.Length;//通过字节流的长度确定当前的下载位置
			fs.Seek(lStartPos, SeekOrigin.Current); //移动文件流中的当前指针 
		}
		else
		{
			fs = new FileStream(m_fileName, FileMode.Create);
			lStartPos = 0;
		}

		try
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(m_url);
			request.Timeout = 10000;
			if (lStartPos > 0)
			{
				request.AddRange((int)lStartPos); //设置Range值
				m_hasDownLoadBits += lStartPos;
			}
			
			//向服务器请求，获得服务器回应数据流 
			Stream ns = request.GetResponse().GetResponseStream();
			ns.ReadTimeout = 10000;
			byte[] nbytes = new byte[1024*1024];
			int nReadSize = 0;
			nReadSize = ns.Read(nbytes, 0, 1024*1024);
			while (nReadSize > 0 || m_stop)
			{
				fs.Write(nbytes, 0, nReadSize);
				m_hasDownLoadBits = m_hasDownLoadBits + nReadSize;
				nReadSize = ns.Read(nbytes, 0, 1024*1024);
			}
			ns.Close();
            fs.Flush();
            if (m_stop)
			{
				Debug.LogError("Stop Success");
				m_downLoadStatus = EnumDownLoadStatus.EDLS_STOP;
				fs.Close();
			}
			else
			{
				//检测MD5
				fs.Close();
				var md5 = DWTools.GenMD5(m_fileName);

				Debug.Log ("get md5:" + md5);
				Debug.Log ("need md5:" + m_md5);

				if(m_md5.ToLower() == md5.ToLower())
				{
					m_downLoadStatus = EnumDownLoadStatus.EDLS_SUCCESS;
				}
				else
				{
					File.Delete(m_fileName);
					m_downLoadStatus = EnumDownLoadStatus.EDLS_MD5WRONG;
				}
			}
		}
		catch (Exception ex)
		{
			fs.Close();
			m_downLoadStatus = EnumDownLoadStatus.EDLS_NETERROR;
			Debug.LogError(ex.ToString());
		}
	}

	public void Stop()
	{
		Debug.LogError("stop");
		m_stop = true;
	}
}
