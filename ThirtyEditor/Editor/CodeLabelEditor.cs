using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using TableType = System.Collections.Generic.Dictionary<object, global::ProtoBuf.IExtensible>;
[InitializeOnLoad]
public class CodeLabelEditor
{
    const string UI_PATH = "XGame/Script";

    //static int PREFAB_LABELNUM_START = 400000;

    static int m_startIndex = 400000;

    //static int PREFAB_NUM_MAX = 1000;

    static Dictionary<string, int> m_labelMaps = new Dictionary<string, int>();

    //[MenuItem("EZFun/ReplaceTemplate")]
    public static void ReplaceTemplate()
    {
        DirectoryInfo direct = new DirectoryInfo(Application.dataPath + "/" + UI_PATH);
        FileInfo[] files = direct.GetFiles("*.cs", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            HandleTemplateFile(files[i]);
        }
    }

    private static void HandleTemplateFile(FileInfo fileInfo)
    {
        string str = System.IO.File.ReadAllText(fileInfo.FullName);
        bool isChanged = false;
        str = HandleTemplateString(str, out isChanged);
        if (!isChanged)
        {
            return;
        }
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        var utf8WithBom = new System.Text.UTF8Encoding(false);
        //System.IO.File.WriteAllBytes(child.FullName, Encoding.UTF8.GetBytes(str));
        StreamWriter swr = null;
        try
        {
            swr = new StreamWriter(fileInfo.FullName, false, utf8WithBom);
            swr.Write(str);
        }
        catch (Exception)
        {
        }
        finally
        {
            if (swr != null)
            {
                swr.Close();
                swr.Dispose();
            }
        }
    }

    private static string HandleTemplateString(string data, out bool isChanged)
    {
        data = ReplaceFormat(data, ".GetTable<{0}>()", ".GetTable(typeof({0}))", out isChanged);
        data = ReplaceFormat(data, ".GetEntry<{0}>({1})", ".GetEntry({1}, typeof({0}))", out isChanged);
        return data;
    }

    /// <summary>
    /// 暂时支持替换一个参数
    /// </summary>
    /// <param name="srcData"></param>
    /// <param name="matchFormat">他的顺序只能0,1,2</param>
    /// <param name="templateFomat"></param>
    /// <returns></returns>
    private static string ReplaceFormat(string srcData, string matchFormat,
        string templateFomat, out bool isChanged)
    {
        isChanged = false;
        List<string> matchsList = new List<string>();
        int count = 0;
        matchsList.Add(matchFormat.Substring(count,
            matchFormat.IndexOf("{", count) - count));
        count = matchFormat.IndexOf("{", count) + 1;
        while (count < matchFormat.Length - 1)
        {
            var index = matchFormat.IndexOf("}", count);
            var nextIndex = matchFormat.IndexOf("{", index);
            if (index == -1)
            {
                Debug.Log("Format error:" + matchFormat);
                throw new Exception("Format error:" + matchFormat);
            }
            if (index >= matchFormat.Length - 1)
            {
                matchsList.Add("");
                break;
            }
            else if (matchFormat.IndexOf("{", index) == -1)
            {
                matchsList.Add(matchFormat.Substring(index + 1));
                break;
            }
            else
            {
                matchsList.Add(matchFormat.Substring(index + 1, nextIndex - 1 - index));
                count = nextIndex;
            }
        }

        count = 0;

        List<string> dstList = new List<string>();
        dstList.Add(templateFomat.Substring(count, templateFomat.IndexOf("{", count) - count));
        count = templateFomat.IndexOf("{", count) + 1;
        while (count < templateFomat.Length - 1)
        {
            var index = templateFomat.IndexOf("}", count);
            var nextIndex = templateFomat.IndexOf("{", index);
            if (index == -1)
            {
                Debug.Log("Format error:" + templateFomat);
                throw new Exception("Format error:" + matchFormat);
            }
            if (index >= templateFomat.Length - 1)
            {
                dstList.Add("");
                break;
            }
            else if (templateFomat.IndexOf("{", index) == -1)
            {
                dstList.Add(templateFomat.Substring(index + 1));
                break;
            }
            else
            {
                dstList.Add(templateFomat.Substring(index + 1, nextIndex - 1 - index));
                count = nextIndex;
            }
        }
        List<string> machContentList = new List<string>();
        bool isOk = true;
        while (srcData.IndexOf(matchsList[0]) >= 0)
        {
            int index = 0;
            for (int i = 0; i < matchsList.Count; i++)
            {
                index = srcData.IndexOf(matchsList[i], index);
                if (index == -1)
                {
                    isOk = false;
                    break;
                }
            }
            if (!isOk)
            {
                break;
            }
            machContentList.Clear();
            index = 0;
            for (int i = 0; i < matchsList.Count - 1; i++)
            {
                int newindex = srcData.IndexOf(matchsList[i], index);
                if (newindex == -1)
                {
                    Debug.Log("substring:  " + srcData.Substring(index,40) + "  matchIndex:" + matchsList[i]);
                }
                int nextIndex = srcData.IndexOf(matchsList[i + 1], newindex);
                machContentList.Add(srcData.Substring(newindex + matchsList[i].Length,
                    nextIndex - newindex - matchsList[i].Length));
                index = nextIndex;
            }
            string orginStr = string.Format(matchFormat, machContentList.ToArray());
            string dstStr = string.Format(templateFomat, machContentList.ToArray());
            srcData = srcData.Replace(orginStr, dstStr);
            isChanged = true;
        }
        return srcData;
    }

    //[MenuItem("EZFun/ReplaceCodeChinese")]
    public static void RePlaceChinese()
    {
        m_startIndex = 400000;
        using (FileStream fileStram = new FileStream(Application.dataPath + "/CodeChineseStr.html", FileMode.Truncate, FileAccess.Write))
        {
            m_labelMaps.Clear();
            while (!string.IsNullOrEmpty(TextData(m_startIndex)))
            {
                m_labelMaps[TextData(m_startIndex)] = m_startIndex++;
            }
            WriteStr("<html>", fileStram);
            WriteStr("<head><meta charset='UTF-8'></meta></head>", fileStram);
            WriteStr("<body>", fileStram);
            WriteStr("<table>", fileStram);
            ReplaceFile(Application.dataPath + "/" + UI_PATH, fileStram);
            WriteStr("</table>", fileStram);
            WriteStr("</body>", fileStram);
            WriteStr("</html>", fileStram);
            fileStram.Flush();
        }
    }

    static void WriteStr(string str, FileStream fileStram)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        fileStram.Write(bytes, 0, bytes.Length);
    }

    static void ReplaceFile(string file, FileStream fileStram)
    {
        DirectoryInfo direct = new DirectoryInfo(file);
        FileInfo[] files = direct.GetFiles();
        Regex reg = new Regex("\"[^\"/#]*[\u4e00-\u9fa5][^\"//#]*\"");
        foreach (var child in files)
        {
            if (File.Exists(child.FullName))
            {
                if (child.FullName.Contains(".cs") && !child.FullName.Contains(".meta") && !child.FullName.Contains("GameRoot.cs")
                    && !child.FullName.Contains("LoginWindow.cs")
                     && !child.FullName.Contains("EValue.cs")
                    && !child.FullName.Contains("WeaponSys.cs")
                        && !child.FullName.Contains("LJSDKSys.cs")
                            && !child.FullName.Contains("ResourceSys.cs")
                                && !child.FullName.Contains("CAttributeDataMgr.cs")
                                    && !child.FullName.Contains("PhalanxSys.cs")
                                        && !child.FullName.Contains("NewPlayerGuideSys.cs")
                                            && !child.FullName.Contains("NewPlayerGuideStep.cs")
                                                && !child.FullName.Contains("NewPlayerGuideModule.cs")
                    && !child.FullName.Contains("DropItemMgr.cs")
                    && !child.FullName.Contains("CGuildWarSys_Window.cs")
                     && !child.FullName.Contains("CFightInfoSys.cs")
                     && !child.FullName.Contains("EeventDefine.cs")
                         && !child.FullName.Contains("DataEyeStaticSys.cs")
                     && !child.FullName.Contains("DataEyeSys.cs")
                       && !child.FullName.Contains("HandleTaskResultWindow.cs")
                    && !child.FullName.Contains("HandleErrorWindow")
                    )
                {
                    string str = System.IO.File.ReadAllText(child.FullName);
                    var en = reg.Matches(str).GetEnumerator();
                    bool isNeed = false;
                    while (en.MoveNext())
                    {
                        var matchStr = en.Current;
                        string tostring = matchStr.ToString();
                        string tempS = tostring.Replace("\n", "\\n");
                        tempS = tempS.Replace("\"", "");
                        if (!m_labelMaps.ContainsKey(tempS))
                        {
                            m_labelMaps.Add(tempS, m_startIndex);
                            WriteStr("<tr>\n <th>" + m_startIndex + "</th>\n<th>" + tempS + "</th></tr>\n", fileStram);
                            m_startIndex++;
                        }
                        int labelNum = m_labelMaps[tempS];
                        str = str.Replace(tostring, "TextData.GetText(" + labelNum + ")");
                        isNeed = true;
                    }
                    if (isNeed)
                    {
                        var utf8WithBom = new System.Text.UTF8Encoding(false);
                        //System.IO.File.WriteAllBytes(child.FullName, Encoding.UTF8.GetBytes(str));
                        StreamWriter swr = null;
                        try
                        {
                            swr = new StreamWriter(child.FullName, false, utf8WithBom);
                            swr.Write(str);
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            if (swr != null)
                            {
                                swr.Close();
                                swr.Dispose();
                            }
                        }
                    }
                }
            }
        }

        DirectoryInfo[] directes = direct.GetDirectories();
        foreach (var child in directes)
        {
            ReplaceFile(child.FullName, fileStram);
        }
    }




    public static string TextData(int textId)
    {
        string text = "";

        ezfun_resource.ResText resText = (ezfun_resource.ResText)TableLoader.Instance.GetEntry<ezfun_resource.ResTextList>(textId);
        if (resText != null)
        {
            text = resText.text.Replace("\\n", "\n");
        }
        else
        {
            text = "";
        }
        return text;
    }
}

