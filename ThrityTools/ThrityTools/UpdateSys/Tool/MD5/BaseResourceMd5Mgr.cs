/************************************************************
//     文件名      : ResourceMd5Mgr.cs
//     功能描述    : 
//     负责人      : guoliang
//     参考文档    : 无
//     创建日期    : 05/11/2017
//     Copyright  : 
**************************************************************/

using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;
using System.Collections.Generic;
using UpdateDefineSpace;

public class BaseResourceMd5Mgr
{

    private JsonData m_localMD5Json = null;

    protected virtual string GetFilePath()
    {
        string path = Application.persistentDataPath + "/ress.cf";
        return path;
    }

    public void Load()
    {
        string path = GetFilePath();
        if (File.Exists(path))
        {
            using (Stream file = File.OpenRead(path))
            {
                var b = new byte[file.Length];
                file.Read(b, 0, b.Length);
                file.Close();

                m_localMD5Json = JsonMapper.ToObject(System.Text.Encoding.UTF8.GetString(b));
            }
        }
        else
        {
            m_localMD5Json = new JsonData();
        }
    }

    protected JsonData Get(string key)
    {
        if (m_localMD5Json == null)
            return null;

        if (DWTools.JsonDataContainsKey(m_localMD5Json, key))
        {
            return m_localMD5Json[key];
        }
        else
        {
            return null;
        }

    }

    protected void Set(string key, string md5, bool autoSave = true, List<string> fileList = null)
    {
        JsonData item = new JsonData();
        item["md5"] = md5;
        if (fileList != null)
        {
            item["fileList"] = JsonMapper.ToJson(fileList);
        }
        m_localMD5Json[key] = item;

        if (autoSave)
        {
            Save();
        }
    }

    public void Set(BaseResInfo info, bool autoSave = true, List<string> fileList = null)
    {
        var key = GetKeyForRes(info);

        Set(key, info.md5, autoSave, fileList);
    }

    public void Save()
    {
        try
        {
            StreamWriter sw = new StreamWriter(GetFilePath());
            var js = JsonMapper.ToJson(m_localMD5Json);
            sw.Write(js);
            sw.Close();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("save resource md5 file error:" + ex.Message);
        }
    }


    /// <summary>
    /// 更多的检测 因为文件放进来Cache  更容易被删掉
    /// </summary>
    /// <param name="resInfo"></param>
    /// <returns></returns>
	public bool IsResourceExisted(BaseResInfo resInfo)
    {
        var key = GetKeyForRes(resInfo);
        var zipInfo = Get(key);
        if (!DWTools.JsonDataContainsKey(zipInfo, "md5"))
        {
            return false;
        }
        if (!resInfo.md5.Equals(zipInfo["md5"].ToString()))
        {
            return false;
        }
        if (!DWTools.JsonDataContainsKey(zipInfo, "fileList"))
        {
            return false;
        }
        var fileList = zipInfo["fileList"];
        JsonData data = JsonMapper.ToObject(fileList.ToString());
        if (data.IsArray)
        {
            for (int fi = 0; fi < data.Count; fi++)
            {
                var str = data[fi].ToString();
                //如果这个更新需要的文件不存在  那么就需要更新
                if (!File.Exists(str))
                {
                    return false;
                }
            }
        }
        else
        {
            return true;
        }

        return true;
    }

    protected virtual string GetKeyForRes(BaseResInfo resInfo)
    {
        return "";
    }

    public void Clean()
    {
        m_localMD5Json = new JsonData();

        var path = GetFilePath();
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
