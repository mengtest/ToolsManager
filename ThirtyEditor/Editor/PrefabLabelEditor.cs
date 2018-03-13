using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using ProtoBuf;
using ComponentAce.Compression.Libs.zlib;
using ezfun_resource;
/************************************************************************/
/* @created shandong
 * 【文本】文本替换代码，这个脚本会自动去把prefab中的汉字查找出来，然后导出成一个html文件，然后copy到text.xls上面，并且把所有的文本存储在一个bytes文件中，
 * 以后如果有新的prefab加入，或者节点修改什么的都先跑一下这个脚本。以下是这个执行流程：

1 在unity编辑器菜单中，选择ezfun->ReplacePrefabChinese
2 用浏览器打开assets/ChineseStr.html文件
3 copy浏览器中所有内容，覆盖掉text.xls中文本id从300000开始的格，注意：千万不要覆盖了其他部分的
4 svn上传assets/PrefabAndLabel.bytes*/
/************************************************************************/
public class LabelNumMessage
{
    public int labelNum;
    public string transName;
}
public class PrefabLabelEditor
{
    const string UI_PATH = "XGame/Resources/EZFunUI/Window";

    const string UIITEM_PATH = "XGame/Resources/EZFunUI/Item";

    static int PREFAB_LABELNUM_START = 300000;

    const int PREFAB_NUM_MAX = 1000;

    static Dictionary<string, int> m_labelMaps = new Dictionary<string, int>();




    //[MenuItem("EZFun/ReplacePrefabChinese")]
    public static void RePlaceChinese()
    {
        //ChineseStr 这个文件用来导出给text.xls
        using (FileStream fileStram = new FileStream(Application.dataPath + "/ChineseStr.html", FileMode.OpenOrCreate, FileAccess.Write))
        {
            var data = TableLoader.Instance.Load(typeof(ezfun_resource.ResTextList));
            ezfun_resource.ResTextList m_TextMessage = null;
            if (data is ResTextList)
            {
                m_TextMessage = (ResTextList)data;
            }
            Dictionary<int, ezfun_resource.ResText> tempDic = new Dictionary<int, ezfun_resource.ResText>();

            for (int i = 0; i < m_TextMessage.list.Count; i++)
            {
                tempDic[m_TextMessage.list[i].ID] = m_TextMessage.list[i];
            }

            //for (int i = 300000; i < 310000; i++)
            //{
            //    if (tempDic.ContainsKey(i))
            //    {
            //        m_labelMaps[tempDic[i].constantText] = i;
            //        if (i > PREFAB_LABELNUM_START)
            //        {
            //            PREFAB_LABELNUM_START = i;
            //        }
            //    }
            //}
   
            //用这些html的格式，主要为了到处方便，能直接copy到text.xls
            WriteStr("<html>", fileStram);
            WriteStr("<head><meta charset='UTF-8'></meta></head>", fileStram);
            WriteStr("<body>", fileStram);
            WriteStr("<table>", fileStram);

            ReplaceFile(Application.dataPath + "/" + UI_PATH, fileStram);
            ReplaceFile(Application.dataPath + "/" + UIITEM_PATH, fileStram);
         
            //int ineedKey = 301586;
            List<LabelNumMessage> list = new List<LabelNumMessage>();
            foreach (var keyvalue in m_labelMaps)
            {
                LabelNumMessage mes = new LabelNumMessage();
                mes.labelNum = keyvalue.Value;
                mes.transName = keyvalue.Key;
                list.Add(mes);
                //if (ineedKey < keyvalue.Value)
            }
            list.Sort((LabelNumMessage a, LabelNumMessage b) =>
                {
                    return a.labelNum - b.labelNum;
                });
            for (int i = 0; i < list.Count; i++)
            {
                {
                    WriteStr("<tr>\n<th>" + list[i].labelNum + "</th>\n<th>" + list[i].transName + "</th></tr>\n", fileStram);
                }
            }
                WriteStr("</table>", fileStram);
            WriteStr("</body>", fileStram);
            WriteStr("</html>", fileStram);
            fileStram.Flush();
            //

            

            //SaveStreamFile<LabeNumTotalMessage>("LabeNumTotalMessage.bytes", m_TextMessage, resSys);
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
        Regex reg = new Regex("[\u4e00-\u9fa5]");
        foreach (var child in files)
        {
            if (File.Exists(child.FullName))
            {
                if (child.FullName.Contains(".prefab") && !child.FullName.Contains(".meta"))
                {
                    string fileName = TrimStr(child.FullName);
                    fileName = fileName.Replace('\\', '/');
                    UnityEngine.Object prefab = Resources.Load(fileName);
                    GameObject gb = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    gb.name = child.Name.Substring(0, child.Name.IndexOf(".prefab"));
                    //EditorGUILayout.getproperty(position, label, property);
                    //m_obj.FindProperty("m_ScaleInLightmap"). = m_nScaleInLightmap;
                    //SetActive(gb.transform);
                    //EditorUtility.ReplacePrefab(gb, EditorUtility.InstantiatePrefab((fileName)));
                    //GameObject gb = EditorUtility.CreateEmptyPrefab(Resources.Load(fileName)) as GameObject;
                    UILabel[] label = gb.GetComponentsInChildren<UILabel>(true);
                    bool needSave = false;
                    foreach (var tempLabel in label)
                    {
                        string lableStr = tempLabel.text;
                        lableStr = lableStr.Replace("\n", "\\n");
                        int labelNum = 0;
                        if (reg.IsMatch(lableStr))
                        {
                            if (!m_labelMaps.ContainsKey(lableStr))
                            {
                                m_labelMaps.Add(lableStr, ++PREFAB_LABELNUM_START);
                               // WriteStr("<tr>\n<th>" + m_TextMessage.seqIndex + "</th>\n<th>" + lableStr + "</th></tr>\n", fileStram);
                               // m_TextMessage.seqIndex++;
                            }
                            labelNum = m_labelMaps[lableStr];
                            //EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
                            Transform trans = tempLabel.transform;
                            string pathStr = trans.name;
                            trans = trans.parent;
                            while (trans != null)
                            {
                                pathStr = trans.name + "/" + pathStr;
                                trans = trans.parent;
                            }
                            //tempLabel.text = "~~" + labelNum;

                            //WriteStr("<tr>\n<th>" + pathStr + "</th>\n<th>" + lableStr + "</th></tr>\n", fileStram);
                            fileStram.Flush();
                            needSave = true;
                        }
                    }
                    if (needSave)
                    {
                        //PrefabUtility.ReplacePrefab(gb, prefab);
                    }

                    PrefabUtility.DisconnectPrefabInstance(gb);
                    GameObject.DestroyImmediate(gb);

                    //PrefabUtility.DisconnectPrefabInstance(gb);
                    //GameObject.DestroyImmediate(gb);
                }
            }
        }

        DirectoryInfo[] directes = direct.GetDirectories();
        foreach (var child in directes)
        {
            ReplaceFile(child.FullName, fileStram);
        }
    }

    static void SetActive(Transform trans)
    {
        NGUITools.SetActive(trans.gameObject, true);
        for (int i = 0; i < trans.childCount; i++)
        {
            SetActive(trans.GetChild(i));
        }
    }
    static string TrimStr(string fullName)
    {
        int extendLength = (Application.dataPath + "XGame/Resources/").Length;
        return fullName.Substring(extendLength + 1, fullName.Length - extendLength - ".prefab".Length - 1);
    }

    /// <summary>
    /// 把原本没有加密的文件加密，并且放到StreamingAssets目录下
    /// </summary>
    //[MenuItem("EZFun/EncrypteLable")]
    static public void EncrypteLable()
    {
        //if (File.Exists(Application.dataPath + "/PrefabAndLabel.bytes"))
        //{
        //    Stream s = new FileStream(Application.dataPath + "/PrefabAndLabel.bytes", FileMode.OpenOrCreate);
        //    byte[] bytes = new byte[s.Length];
        //    bytes[0] = (byte)1;
        //    s.Read(bytes, 0, (int)s.Length);
        //    Stream ws = null;
        //    byte[] compressBytes = Compress(bytes);

        //    byte[] Encryptebytes = GlobalCrypto.Encrypte(compressBytes);



        //    if (!File.Exists(Application.streamingAssetsPath + "/TableData/" + "/LabeNumTotalMessage.bytes"))
        //    {
        //        ws = new FileStream(Application.streamingAssetsPath + "/TableData/" + "/LabeNumTotalMessage.bytes", FileMode.OpenOrCreate);
        //    }
        //    else
        //    {
        //        ws = new FileStream(Application.streamingAssetsPath + "/TableData/" + "/LabeNumTotalMessage.bytes", FileMode.Truncate);
        //    }
        //    ws.Write(Encryptebytes, 0, Encryptebytes.Length);
        //    ws.Flush();
        //    ws.Close();
        //    s.Close();
        //}
    }

    #region 存储文件到StreamAssets目录
    static public void SaveStreamFile<T>(string fileName, T intance, TableLoader resSys)
    {
        try
        {
            MemoryStream ms = new MemoryStream();
            ProtoBuf.Serializer.Serialize(ms, intance);
            byte[] bytes = ms.ToArray();
            //bytes = Compress(bytes);
            bytes = GlobalCrypto.Encrypte(bytes);
            Stream s = null;
            if (!File.Exists(Application.streamingAssetsPath + "/Table/" + fileName))
            {
                s = new FileStream(Application.streamingAssetsPath + "/Table/" + fileName, FileMode.Truncate);
            }
            else
            {
                s = new FileStream(Application.streamingAssetsPath + "/Table/" + fileName, FileMode.OpenOrCreate);
            }
            s.Write(bytes, 0, bytes.Length);
            s.Flush();
            s.Close();
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
        }
    }
    #endregion

    #region 解压函数


    public static byte[] Compress(byte[] input)
    {
        MemoryStream output = new MemoryStream();
        var zos = new ZOutputStream(output, 2);
        zos.Write(input, 0, input.Length);
        zos.finish();
        zos.Close();
        return output.ToArray();
    }
    #endregion

    // label字体清晰
    [MenuItem("EZFun/label字体清晰")]
    static void ReplaceLabelSize()
    {
        var directory = EditorUtility.OpenFolderPanel("选择prefab路径", "", "prefab");
        GenUpdatePack.ForeachFile(directory, (string f) =>
        {
            if (f.Contains(".prefab") && !f.Contains(".meta"))
            {
                string fileName = TrimStr(f);
                fileName = fileName.Replace('\\', '/');
                UnityEngine.Object prefab = Resources.Load(fileName);
                GameObject gb = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                var label = gb.GetComponentsInChildren<UILabel>(true);
                var logstr = "replace label size " + fileName + ": ";
                for (int i = 0; i < label.Length; ++i)
                {
                    var tempLabel = label[i];
                    if(tempLabel == null || tempLabel.ambigiousFont == null)
                    {
                        continue;
                    }
                    if (!(tempLabel.ambigiousFont.name.Equals("arial") ||
                        tempLabel.ambigiousFont.name.Equals("ARIALN") ||
                        tempLabel.ambigiousFont.name.Equals("ARIALN2")))
                    {
                        continue;
                    }
                    if (tempLabel.transform.localScale.x == 0.5F &&
                        tempLabel.transform.localScale.y == 0.5F &&
                        tempLabel.transform.localScale.z == 0.5F)
                    {
                        continue;
                    }
                    tempLabel.fontSize *= 2;
                    tempLabel.transform.localScale = new Vector3(
                        tempLabel.transform.localScale.x / 2,
                        tempLabel.transform.localScale.y / 2,
                        tempLabel.transform.localScale.z / 2
                        );
                    logstr += " " + tempLabel.name;
                }
                Debug.Log(logstr);
                PrefabUtility.ReplacePrefab(gb, prefab);
                PrefabUtility.DisconnectPrefabInstance(gb);
                GameObject.DestroyImmediate(gb);
            }
            AssetDatabase.SaveAssets();
        });
    }
}

