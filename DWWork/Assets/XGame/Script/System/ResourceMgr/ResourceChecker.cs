//========================================================================
// Copyright(C): EZFun
//
// CLR Version : 4.0.30319.34209
// NameSpace : Assets.XGame.Script.Util
// FileName : ResourceChecker
//
// Created by : dhf at 1/15/2015 9:57:01 PM
//
// Function : 用于检查资源重复
//
//========================================================================

using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceChecker 
{
    //之前用list，这个list大了之后，查找效率特别差，换成hashset
    static public HashSet<string> resPaths = new HashSet<string>();
    static public string LogFile = EZFunTools.AvailablePath + Path.DirectorySeparatorChar + "reslog";

    static public void LogUsedRes(string res)
    {
        return;
//#if UNITY_EDITOR 
//        if (CheckRepeat(res))
//        {
//            return;
//        }

//        using (StreamWriter sw = new StreamWriter(LogFile, true))
//        {
//            sw.WriteLine(res);
//        }
//#endif
    }

    static private bool CheckRepeat(string res)
    {
        if (resPaths.Contains(res))
        {
            return true;
        }
        else
        {
            resPaths.Add(res);
            return false;
        }
    }

    static public void SendLog()
    {
        return;
//#if UNITY_EDITOR
//        if (!CheckLogFile())
//        {
//            return;
//        }
//        ezfun.CSPackageBody body = new ezfun.CSPackageBody();
//        body.loadUpdateReq = new ezfun.CSLoadUpdateReq();
//        body.loadUpdateReq.gameVersion = Version.Instance.GetVersion(VersionType.App);
//        body.loadUpdateReq.sourceVersion = Version.Instance.GetVersion(VersionType.Resource);
//        body.loadUpdateReq.time = (ulong)DateTime.Now.ToFileTimeUtc();
//        using (StreamReader s = File.OpenText(LogFile))
//        {
//            string fileContent = null;
//            while ((fileContent = s.ReadLine()) != null)
//            {
//                body.loadUpdateReq.files.Add(fileContent);
//            }
//        }
//        CNetSys.Instance.SendNetMsg(ezfun.CS_CMD.CS_LOGIC_CMD_LOAD_UPDATE, body,
//            (ezfun.SCPackage msg) => 
//            {
//                Debug.Log("send res load file suc");
//            },
//            () =>
//            {
//                Debug.Log("send res load file failed");
//            });

//        FileStream fs = null;
//        try
//        {
//            fs = new FileStream(LogFile, FileMode.Truncate, FileAccess.ReadWrite);
//        }
//        catch (Exception ex)
//        {
//            Debug.LogError("clean res load file error:" + ex.Message);
//        }
//        finally
//        {
//            fs.Close();
//        }
//#endif
    }

    static private bool CheckLogFile()
    {
        if (!File.Exists(LogFile))
        {
            try
            {
                File.Create(LogFile);
            }
            catch (Exception ex)
            {
                Debug.LogError("create res load file error:" + ex.Message);
            }
            return false;
        }
        return true;
    }
}
