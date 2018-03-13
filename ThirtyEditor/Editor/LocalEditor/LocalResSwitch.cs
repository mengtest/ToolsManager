using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading;
public enum LocaResType
{
    zn,
    tw,
    vi,
    kr,
    en,
    th,
}
public class FileThread
{
    public string path;
    public Dictionary<string, CFileInfo> m_dic = new Dictionary<string, CFileInfo>();

    public volatile bool isFinish = false;

    public void Start()
    {
        isFinish = false;
        Thread thread = new Thread(Run);
        thread.Start();
    }

    void Run()
    {
        LocalResSwitch.CalFolderMD5(path, m_dic);
        isFinish = true;
    }
}
public class CFileInfo
{
    public string path;
    public string md5;
    public long times;
}
public class LocalResSwitch
{

    private static string m_dataPath = Application.dataPath + "/XGame";

    private static string m_toolsAddPath = "/../../tools/local/";

    [MenuItem("Local/本地化切换/切换台湾")]
    public static void SwitchTaiWan()
    {
        SwitchLocalRes(LocaResType.tw);
    }

    [MenuItem("Local/本地化切换/切换越南")]
    public static void SwitchVi()
    {
        SwitchLocalRes(LocaResType.vi);
    }
    [MenuItem("Local/本地化切换/切换大陆")]
    public static void SwitchZN()
    {
        SwitchLocalRes(LocaResType.zn);
    }
    [MenuItem("Local/本地化切换/切换韩国")]
    public static void SwitchKR()
    {
        SwitchLocalRes(LocaResType.kr);
    }


    [MenuItem("Local/SMT/资源切换/切换大陆")]
    public static void SwitchSMT_ZN()
    {
        MultiLangSwitch.GetInstance().SwitchRes(LocaResType.zn);
    }

    [MenuItem("Local/SMT/资源切换/切换英文")]
    public static void SwitchSMT_EN()
    {
        MultiLangSwitch.GetInstance().SwitchRes(LocaResType.en);
    }
    [MenuItem("Local/SMT/资源切换/切换泰语")]
    public static void SwitchSMT_TH()
    {
        MultiLangSwitch.GetInstance().SwitchRes(LocaResType.th);
    }

    #region 切换平台资源
    private static void SwitchLocalRes(LocaResType type)
    {
        string dataPath = Application.dataPath;
        string curDataPath = dataPath + "/../tools/local/cur.res";
        string curLocal = "";
        using (StreamReader reader = new StreamReader(curDataPath, System.Text.Encoding.GetEncoding("utf-8")))
        {
            curLocal = reader.ReadToEnd().Trim();
        }
        Debug.Log("curLocal:" + curLocal);
        Debug.Log(dataPath);
        if (type.ToString() == curLocal)
        {
            Debug.Log("is CurLocal" + type.ToString());
            return;
        }
        //因为所有的版本比较的都是简体中文版，所以所有版本从不同版本切换都需要中转到大陆简体
        if (type.ToString() != LocaResType.zn.ToString() && curLocal != LocaResType.zn.ToString())
        {
            //Copy2File(dataPath + "/../tools/local/dll/" + LocaResType.zn.ToString() + "/xgame.ezfun", dataPath + "/StreamingAssets/xgame.ezfun", dataPath + "/../tools/local/dll/" + curLocal.ToString() + "/xgame.ezfun");
            //当前的
            Folder2Folder(dataPath + "/../tools/local/" + LocaResType.zn.ToString() + "/", dataPath + "/XGame/", dataPath + "/../tools/local/" + curLocal.ToString() + "/");
            //转表格
            Folder2Folder(dataPath + "/../tools/local/tableData/" + LocaResType.zn.ToString() + "/", dataPath + "/StreamingAssets/Table/", dataPath + "/../tools/local/Table/" + curLocal.ToString() + "/");


            //Copy2File(dataPath + "/../tools/local/dll/" + type.ToString() + "/xgame.ezfun", dataPath + "/StreamingAssets/xgame.ezfun", dataPath + "/../tools/local/dll/" + LocaResType.zn.ToString() + "/xgame.ezfun");
            //中文的移动到
            Folder2Folder(dataPath + "/../tools/local/" + type.ToString() + "/", dataPath + "/XGame/", dataPath + "/../tools/local/" + LocaResType.zn.ToString() + "/");
            string targetTablePath = dataPath + "/StreamingAssets/Table/";
            Folder2Folder(dataPath + "/../tools/local/Table/" + type.ToString() + "/", targetTablePath, dataPath + "/../tools/local/Table/" + LocaResType.zn.ToString() + "/");
        }
        else
        {
            //将dll移动一下
           // Copy2File(dataPath + "/../tools/local/dll/" + type.ToString() + "/xgame.ezfun", dataPath + "/StreamingAssets/xgame.ezfun", dataPath + "/../tools/local/dll/" + curLocal.ToString() + "/xgame.ezfun");
            Folder2Folder(dataPath + "/../tools/local/" + type.ToString() + "/", dataPath + "/XGame/", dataPath + "/../tools/local/" + curLocal.ToString() + "/");
            string targetTablePath = dataPath + "/StreamingAssets/Table/";
            Folder2Folder(dataPath + "/../tools/local/Table/" + type.ToString() + "/", dataPath + "/StreamingAssets/Table/", dataPath + "/../tools/local/tableData/" + curLocal.ToString() + "/");
        }



        File.Delete(curDataPath);
        using (Stream writer = new FileStream(curDataPath, FileMode.OpenOrCreate))
        {
            StreamWriter wr = new StreamWriter(writer);
            wr.Write(type.ToString());
            wr.Close();
        }
    }

    public static string GetCurRes()
    {
        string dataPath = Application.dataPath;
        string curDataPath = dataPath + "/../tools/local/cur.res";
        string curLocal = "";
        using (StreamReader reader = new StreamReader(curDataPath, System.Text.Encoding.GetEncoding("utf-8")))
        {
            curLocal = reader.ReadToEnd().Trim();
        }
        return curLocal;
    }

    private static void CleanFolder(string folderPath)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
        List<DirectoryInfo> childFolders = new List<DirectoryInfo>(dirInfo.GetDirectories());
        for (int i = 0; i < childFolders.Count; i++)
        {
            childFolders[i].Delete(true);
        }
    }

    public static void Folder2Folder(string srcFolderPath, string dstFolderPath, string removePath, Action<int, string, string> preFileAction = null)
    {
        if (!Directory.Exists(dstFolderPath))
        {
            //Debug.Log("dstFolderPath create :" + dstFolderPath);
            Directory.CreateDirectory(dstFolderPath);
        }
        if (!Directory.Exists(removePath) && removePath != "")
        {
            //Debug.Log("removePath create :" + removePath);
            Directory.CreateDirectory(removePath);
        }
        DirectoryInfo dirInfo = new DirectoryInfo(srcFolderPath);
        List<FileInfo> needFileList = new List<FileInfo>(dirInfo.GetFiles());
        for (int m = 0; m < needFileList.Count; m++)
        {
            FileInfo nextFile = needFileList[m];
            string moveToProjPath = dstFolderPath + "/" + nextFile.Name;
            int uniqueST = 1;
            if (File.Exists(moveToProjPath))
            {
                FileInfo fileInfo = new FileInfo(moveToProjPath);
                if (File.Exists(removePath + "/" + nextFile.Name))
                {
                    File.Delete(removePath + "/" + nextFile.Name);
                }
                fileInfo.MoveTo(removePath + "/" + nextFile.Name);
                //Debug.Log("curPath" + nextFile.FullName + " removePath:" + removePath);
                uniqueST = 0;
            }
            //Debug.Log("curPath" + nextFile.FullName + " moveToProjPath:" + moveToProjPath);
            if (preFileAction != null)
            {
                preFileAction(uniqueST, moveToProjPath, nextFile.FullName);
            }
            nextFile.MoveTo(moveToProjPath);
            
            //Debug.Log("preFileAction ST = " + uniqueST);
        }
        List<DirectoryInfo> needFolderList = new List<DirectoryInfo>(dirInfo.GetDirectories());
        for (int m = 0; m < needFolderList.Count; m++)
        {
            string subRemovePath = removePath;
            if (subRemovePath != "")
            {
                subRemovePath = removePath + "/" + needFolderList[m].Name;
            }
            //Debug.Log("pass removePath path:" + needFolderList[m].Name);
            Folder2Folder(needFolderList[m].FullName, dstFolderPath + "/" + needFolderList[m].Name, subRemovePath, preFileAction);
        }
    }

    static void Copy2File(string srcStr, string dstStr, string tempStr)
    {
        if (!File.Exists(srcStr))
        {
            return;
        }
        if (File.Exists(tempStr))
        {
            File.Delete(tempStr);
        }
        if (File.Exists(dstStr))
        {
            FileInfo info = new FileInfo(dstStr);
            info.MoveTo(tempStr);
        }
        FileInfo srcFil = new FileInfo(srcStr);
        srcFil.MoveTo(dstStr);
    }
    #endregion

    #region 保存某语言资源
    [MenuItem("Local/SMT/保存当前资源")]
    public static void SaveCurLocalResSMT()
    {
        MultiLangSwitch.GetInstance().SaveCurLocalRes();
    }

    [MenuItem("Local/本地化切换/保存当前资源")]
    public static void SaveCurLocalResToZN()
    {
        SaveCurLocalRes();
    }

    public static void SaveCurLocalRes()
    {
        string dataPath = Application.dataPath;
        string curDataPath = dataPath + "/../tools/local/cur.res";
        string curLocal = "";
        using (StreamReader reader = new StreamReader(curDataPath, System.Text.Encoding.GetEncoding("utf-8")))
        {
            curLocal = reader.ReadToEnd().Trim();
        }

        string resJsonFile = dataPath + "/../tools/local/" + curLocal + ".json";

        Debug.Log("curLocal" + curLocal);
        Dictionary<string, CFileInfo> znResMap = new Dictionary<string, CFileInfo>();


        Debug.Log("begin read zn.json :" + DateTime.Now.TimeOfDay.TotalSeconds);
        if (curLocal != LocaResType.zn.ToString())
        {
            ReadZnFile(znResMap);
        }
        ///等待多线程跑完
        //while (!m_isLoad) ;

        Debug.Log("End read zn.json :" + DateTime.Now.TimeOfDay.TotalSeconds);

        Debug.Log("StartTime:" + DateTime.Now.TimeOfDay.TotalSeconds);

        //ReadZnFile();

        Dictionary<string, CFileInfo> resMap = new Dictionary<string, CFileInfo>();
        //CalFolderMD5(dataPath + "/XGame/Scene/", resMap);

        //暴力开线程
        FileThread fileData = new FileThread();
        fileData.path = dataPath + "/XGame/Data/";
        fileData.m_dic = new Dictionary<string, CFileInfo>();
        FileThread fileData1 = new FileThread();
        fileData1.path = dataPath + "/XGame/Resources/";
        fileData1.m_dic = new Dictionary<string, CFileInfo>();
        FileThread fileData2 = new FileThread();
        fileData2.path = dataPath + "/XGame/Scene/";
        fileData2.m_dic = new Dictionary<string, CFileInfo>();

        fileData.Start();
        fileData1.Start();
        fileData2.Start();

        while (!fileData2.isFinish || !fileData.isFinish || !fileData1.isFinish)
        {
            Thread.Sleep(1);
        }
        Debug.Log("EndScan XGame assets Time:" + DateTime.Now.TimeOfDay.TotalSeconds);
        resMap = fileData.m_dic;

        foreach (var entry in fileData1.m_dic)
        {
            resMap.Add(entry.Key, entry.Value);
        }

        foreach (var entry in fileData2.m_dic)
        {
            resMap.Add(entry.Key, entry.Value);
        }


        Dictionary<string, CFileInfo> difResMap = new Dictionary<string, CFileInfo>();



        //如果是中文的话，就把这个文件存储就好了，如果是其他语言的话，那么就需要与中文版本对比了
        if (curLocal != LocaResType.zn.ToString())
        {
            foreach (var entry in resMap)
            {
                if (znResMap.ContainsKey(entry.Key) && znResMap[entry.Key].times != entry.Value.times)
                {
                    MoveToFile(entry.Key, curLocal);
                    //difResMap.Add(entry.Key, entry.Value);
                }
                else if (!znResMap.ContainsKey(entry.Key))
                {
                    MoveToFile(entry.Key, curLocal);
                    // difResMap.Add(entry.Key, entry.Value);
                }
            }

            DirectoryInfo directInfo = new DirectoryInfo(Application.dataPath + "/StreamingAssets/Table");
            FileInfo[] files = directInfo.GetFiles(curLocal + "_*.bytes", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string dstPath = Application.dataPath + "/../tools/local/tableData/" + curLocal;
                if (File.Exists(dstPath + "/" + files[i].Name))
                {
                    File.Delete(dstPath + "/" + files[i].Name);
                }
                files[i].MoveTo(dstPath + "/" + files[i].Name);
            }

        }
        else
        {
            difResMap = resMap;
        }
        Debug.Log("End move files :" + DateTime.Now.TimeOfDay.TotalSeconds);
        if (File.Exists(resJsonFile))
        {
            File.Delete(resJsonFile);
        }
        WriteFile(resJsonFile, difResMap);
        Debug.Log("End write dif files :" + DateTime.Now.TimeOfDay.TotalSeconds);

        WriteLocalRes(LocaResType.zn);
        SaveLocalLangFile(LocaResType.zn.ToString());
    }

    public static void CalFolderMD5(string dir, Dictionary<string, CFileInfo> resMap)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        List<FileInfo> needFileList = new List<FileInfo>(dirInfo.GetFiles());
        for (int m = 0; m < needFileList.Count; m++)
        {
            FileInfo nextFile = needFileList[m];

            CFileInfo fileInfo = new CFileInfo();
            fileInfo.path = nextFile.FullName;
            fileInfo.times = nextFile.LastWriteTime.ToFileTime();
            resMap[nextFile.FullName] = fileInfo;
        }
        List<DirectoryInfo> needFolderList = new List<DirectoryInfo>(dirInfo.GetDirectories());
        for (int m = 0; m < needFolderList.Count; m++)
        {
            CalFolderMD5(needFolderList[m].FullName, resMap);
        }
    }


    static public string GenMD5(string fn)
    {
        var md5CSP = new System.Security.Cryptography.MD5CryptoServiceProvider();
        var fs = new System.IO.FileStream(fn, FileMode.Open, FileAccess.Read);
        var arrbytHashValue = md5CSP.ComputeHash(fs);
        var strHashData = System.BitConverter.ToString(arrbytHashValue).Replace("-", "");
        fs.Close();
        return strHashData;
    }


    private static void ReadFile(string md5Path, Dictionary<string, CFileInfo> dic)
    {
        using (Stream writer = new FileStream(md5Path, FileMode.OpenOrCreate))
        {
            StreamReader wr = new StreamReader(writer);
            string str = wr.ReadLine();
            while (!string.IsNullOrEmpty(str))
            {
                string[] entry = str.Split(':');
                if (entry.Length < 2)
                {
                    break;
                }
                CFileInfo fileInfo = new CFileInfo();
                fileInfo.times = long.Parse(entry[3]);
                fileInfo.path = entry[0];
                dic[entry[0] + ":" + entry[1]] = fileInfo;
                str = wr.ReadLine();
            }
            wr.Close();
        }
    }


    #region 多线程
    private static volatile bool m_isLoad = false;

    private static void ReadZnFile(Dictionary<string, CFileInfo> znResMap)
    {
        ReadFile(m_dataPath + "/../../tools/local/zn.json", znResMap);
        m_isLoad = true;
    }
    #endregion


    private static void WriteFile(string md5Path, Dictionary<string, CFileInfo> resMap)
    {
        using (Stream writer = new FileStream(md5Path, FileMode.OpenOrCreate))
        {
            StreamWriter wr = new StreamWriter(writer);
            foreach (var entry in resMap)
            {
                wr.WriteLine(entry.Key + ":" + entry.Value.md5 + ":" + entry.Value.times);
            }
            //wr.Flush();
            wr.Close();
        }
    }

    /// <summary>
    ///写死的，都是从assets文件夹，移到对应的localresTpye,路径下，然后把tools/local/zn路径下的资源移到Assets下
    /// </summary>
    /// <param name="srcFilePath"></param>
    /// <param name="dstFilePath"></param>
    private static void MoveToFile(string srcFilePath, string resType)
    {
        string relativePath = srcFilePath.Substring(m_dataPath.Length);

        string resTypePath = m_dataPath + m_toolsAddPath + resType + relativePath;

        string[] folders = relativePath.Split('\\');

        string parentPath = m_dataPath + m_toolsAddPath + resType;
        for (int i = 0; i < folders.Length - 1; i++)
        {
            parentPath = parentPath + "/" + folders[i];
            if (!Directory.Exists(parentPath))
            {
                Directory.CreateDirectory(parentPath);
            }
        }
        // if (File.Exists(parentPath))
        {
            //    File.Delete(parentPath);
        }

        FileInfo fileInfo = new FileInfo(srcFilePath);
        resTypePath = resTypePath.Replace('\\', '/');
        if (File.Exists(resTypePath))
        {
            File.Delete(resTypePath);
        }
        fileInfo.MoveTo(resTypePath);


        string znResTypePath = m_dataPath + m_toolsAddPath + LocaResType.zn.ToString() + relativePath;
        znResTypePath = znResTypePath.Replace('\\', '/');
        if (File.Exists(znResTypePath))
        {
            FileInfo znFile = new FileInfo(znResTypePath);
            if (File.Exists(srcFilePath))
            {
                File.Delete(srcFilePath);
            }
            znFile.MoveTo(srcFilePath);
        }
    }



    #endregion

    private static void WriteLocalRes(LocaResType type)
    {
        string curDataPath = Application.dataPath + "/../tools/local/cur.res";
        File.Delete(curDataPath);
        using (Stream writer = new FileStream(curDataPath, FileMode.OpenOrCreate))
        {
            StreamWriter wr = new StreamWriter(writer);
            wr.Write(type.ToString());
            wr.Close();
        }
    }






    [MenuItem("Local/输出当前版本地区编码")]
    public static void PrintCurLocalRes()
    {
        string dataPath = Application.dataPath;
        string curDataPath = dataPath + "/../tools/local/cur.res";
        string curLocal = "";
        using (StreamReader reader = new StreamReader(curDataPath, System.Text.Encoding.GetEncoding("utf-8")))
        {
            curLocal = reader.ReadToEnd().Trim();
        }

        Debug.LogError("当前版本 (zn)简体中文,, (th)(泰语)，(en)英文.  当前地区代码:" + curLocal);
    }


    public class MultiLangSwitch
    {
        private static MultiLangSwitch _instance = null;
        public static MultiLangSwitch GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MultiLangSwitch();
            }
            _instance.ReadCurType();
            return _instance;
        }
        string m_curLocal = "";
        Dictionary<string, string> m_beSaveUniqueResList = new Dictionary<string, string>(); //版本独有资源列表
        Dictionary<string, CFileInfo> m_difResMap = new Dictionary<string, CFileInfo>(); //和简体中文版差异的map，对于当前是简体中文版的话就是所有res的MD5
        string m_uniqueResListPath = Application.dataPath + "/../tools/local/unique.res";
        string curTypePath = Application.dataPath + "/../tools/local/cur.res";
        public void SwitchRes(LocaResType ty)
        {
            if (ty.ToString() == m_curLocal)
            {
                Debug.LogError("can not switch " + m_curLocal + " to " + ty.ToString());//同版本不允许切换
                return;
            }
            Debug.Log("begin SwitchRes to -> " + ty.ToString());
            //表格拷贝，把所有语言版本的表格都拷贝都xgame的表格目录底下。不需要考虑，差异的问题。
            string dataPath = Application.dataPath;
            Folder2Folder(dataPath + "/../tools/local/Table/smt/", dataPath + "/StreamingAssets/Table/", "");
            //资源拷贝，保存独有资源列表。两个非简体中文版之间的切换需要一简体中文版作为中介。
            if (ty.ToString() != LocaResType.zn.ToString() && m_curLocal != LocaResType.zn.ToString())
            {
                Folder2Folder(dataPath + "/../tools/local/" + LocaResType.zn.ToString() + "/", dataPath + "/XGame/", dataPath + "/../tools/local/" + m_curLocal + "/");
                DeleteUniqueResToZN();//切了中文之后删除m_curLocal 的独有资源。

                m_beSaveUniqueResList.Clear();
                Folder2Folder(dataPath + "/../tools/local/" + ty.ToString() + "/", dataPath + "/XGame/", dataPath + "/../tools/local/" + LocaResType.zn.ToString() + "/", CheckAddUniqueRes);
                WriteUniqueFile(m_uniqueResListPath, m_beSaveUniqueResList);//切换独有版本之后去保存独有资源。
            }
            else if (ty == LocaResType.zn)
            {
                //切了中文之后删除m_curLocal 的独有资源
                Folder2Folder(dataPath + "/../tools/local/" + ty.ToString() + "/", dataPath + "/XGame/", dataPath + "/../tools/local/" + m_curLocal + "/");
                DeleteUniqueResToZN();
                m_beSaveUniqueResList.Clear();
                WriteUniqueFile(m_uniqueResListPath, m_beSaveUniqueResList);
            }
            else
            {
                //中文切换到别的版本
                m_beSaveUniqueResList.Clear();
                Folder2Folder(dataPath + "/../tools/local/" + ty.ToString() + "/", dataPath + "/XGame/", dataPath + "/../tools/local/" + m_curLocal + "/", CheckAddUniqueRes);
                WriteUniqueFile(m_uniqueResListPath, m_beSaveUniqueResList);
            }
            Debug.Log("end SwitchRes to -> " + ty.ToString());
            Debug.Log("begin write cur.res -> " + ty.ToString());
            WriteLocalRes(ty);
            SaveLocalLangFile(ty.ToString());
            Debug.Log("end write cur.res -> " + ty.ToString());
        }

        public void SaveCurLocalRes()
        {
            //先拷贝md5不一样的文件，此时会切换到中文。
            LocalResSwitch.SaveCurLocalRes();
        }

        private void ReadCurType()
        {
            using (StreamReader reader = new StreamReader(curTypePath, System.Text.Encoding.GetEncoding("utf-8")))
            {
                m_curLocal = reader.ReadToEnd().Trim();
            }
        }

        private void CheckAddUniqueRes(int st, string path, string localPath)
        {
            if (st == 1)
            {
                //Debug.Log("st == 1 Path: " + path);
                if(!m_beSaveUniqueResList.ContainsKey(path))
                {
                    m_beSaveUniqueResList.Add(path, localPath);
                }
                else
                {
                    m_beSaveUniqueResList[path] = localPath;
                }
            }
        }

        //当切换到简体中文版的时候需要去当前版本的删除独有资源。并移动到Local
        private void DeleteUniqueResToZN()
        {
            Dictionary<string, string> fileList = ReadUniqueFile(m_uniqueResListPath);
            foreach (var fP in fileList)
            {
                if (File.Exists(fP.Value))
                {
                    File.Delete(fP.Value);
                }
                FileInfo sInfo = new FileInfo(fP.Key);
                sInfo.MoveTo(fP.Value);
            }
        }

        private Dictionary<string, string> ReadUniqueFile(string path)
        {
            Dictionary<string, string> resList = new Dictionary<string, string>();
            using (Stream writer = new FileStream(path, FileMode.OpenOrCreate))
            {
                StreamReader wr = new StreamReader(writer);
                string str = wr.ReadLine();
                while (!string.IsNullOrEmpty(str))
                {
                    var split = str.Split(new string[] { "$S$" }, StringSplitOptions.None);
                    if (split.Length < 2)
                    {
                        break;
                    }
                    if (resList.ContainsKey(split[0]))
                    {
                        Debug.LogError(split[0]);
                    }
                    else
                    {
                        resList.Add(split[0], split[1]);
                    }
                    str = wr.ReadLine();
                }
                wr.Close();
            }

            return resList;
        }

        private void WriteUniqueFile(string path, Dictionary<string, string> uniqueRes)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (Stream writer = new FileStream(path, FileMode.OpenOrCreate))
            {
                StreamWriter wr = new StreamWriter(writer);
                foreach (var entry in uniqueRes)
                {
                    wr.WriteLine(entry.Key + "$S$" + entry.Value);
                }
                wr.Close();
            }
        }
    }

    //设置客户端语言
    static void SaveLocalLangFile(string curLang)
    {
        var m_localSetPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "curLang";
        try
        {
            FileStream fs = new FileStream(m_localSetPath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(curLang.ToLower());
            sw.Close();
            fs.Close();
        }
        catch (Exception)
        {
            Debug.LogError("save SaveLocalLangFile error");
        }

    }
}
