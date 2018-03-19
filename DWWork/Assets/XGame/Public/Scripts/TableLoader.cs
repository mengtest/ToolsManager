/************************************************************
//     文件名      : TableLoader.cs
//     功能描述    : 资源管理类
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/07/11.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ezfun_resource;

using TableType = System.Collections.Generic.Dictionary<object, global::ProtoBuf.IExtensible>;

public class TableLoader
{
    class MapDefine
    {
        public string[] keys;
    }

    class MapData
    {
        public Type type;
        public Dictionary<object, object> map = new Dictionary<object, object>();
    }

    private Dictionary<Type, TableType> m_systemMap = new Dictionary<Type, TableType>();
    private Dictionary<Type, Dictionary<string, MapDefine>> m_mapDefines = new Dictionary<Type, Dictionary<string, MapDefine>>();
    private Dictionary<string, MapData> m_map = new Dictionary<string, MapData>();

    public delegate void TableLoadedCallback(ProtoBuf.IExtensible table, string error);
    public volatile bool m_preloadTable = false;

    private static string m_streamAssets;
    /// <summary>
    /// 因为苹果要求，在iphone下 此目录在Application.temporaryCachePath 
    /// 其他情况在Application.persistentDataPath
    /// </summary>
    private static string m_persistPath;

    public class LoadTableParam
    {
        public string m_strFilePath;
        public TableLoadedCallback m_LoadedCallback;
    }
    private bool hasInit = false;
    public static object changeKey(object key)
    {
        Type tkey = key.GetType();
        object nkey = null;
        if (tkey == typeof(int))
        {
            nkey = (double)(int)key;
        }
        else
        {
            nkey = key;
        }
        return nkey;
    }
    private static TableLoader m_instance;

    public static TableLoader Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new TableLoader();
                m_instance.Init();
            }
            return m_instance;
        }
    }

    public void Init()
    {
        if (hasInit == true)
            return;
        hasInit = true;
        m_streamAssets = Application.streamingAssetsPath;
        //if (Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    m_persistPath = Application.temporaryCachePath;
        //}
        //else
        {
            m_persistPath = Application.persistentDataPath;
        }
    }
    public void Release()
    {

    }

    public void UnLoadAllTable()
    {
        m_map.Clear();
        m_mapDefines.Clear();
        m_systemMap.Clear();
        m_type2MapNamesDict.Clear();
    }

    public void LoadAllTable()
    {
        m_preloadTable = true;
    }

    /// <summary>
    /// 在实际游戏中请不要调用这个接口，只在编辑器中调用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public object Load(Type type)
    {
        return Load("Table/" + type.Name + ".bytes", true, type);
    }

    public object Load(string fileName, bool isDeCry = true
        , Type type = null)
    {
        Debug.Log("Load Table:" + fileName);
        string plocal = (m_streamAssets + "/" + fileName);
        string ulocal = (m_persistPath + "/" + fileName);
        byte[] b = null;
        if (File.Exists(ulocal))
        {
            b = ReadFileStream(ulocal);
        }
        else
        {
            b = ReadFile(plocal);
        }
        if (b == null)
        {
            return null;
        }
        byte[] zdata = b;
        int len = zdata.Length;
        if (isDeCry)
        {
            zdata = GlobalCrypto.Decrypte(b, out len);
        }
        var data = zdata;
        //if (isDecompress)
        //{
        //    data = GlobalCrypto.Decompress(zdata);
        //}
        MemoryStream ms = new MemoryStream(data, 0, len);
        object res = ProtoBuf.Serializer.Deserialize(ms, type);
        //Debug.Log ("load table1 :" + t.Name);
        return res;
    }

    static public TableType InitTable(ProtoBuf.IExtensible table)
    {
        TableType dictTable = new TableType();
        Type t = table.GetType();
        System.Reflection.PropertyInfo p = t.GetProperty("list");
        IList ol = (IList)p.GetValue(table, null);
        var enumerator = ol.GetEnumerator();
        while (enumerator.MoveNext())
        {
            ProtoBuf.IExtensible obj = enumerator.Current as ProtoBuf.IExtensible;
            Type objType = obj.GetType();
            System.Reflection.PropertyInfo objProperty = objType.GetProperty("ID");
            object key = objProperty.GetValue(obj, null);
            object nkey = changeKey(key);

            dictTable[nkey] = obj;
        }
        return dictTable;
    }

    public void AddTable(ProtoBuf.IExtensible res)
    {
        if (res == null)
        {
            return;
        }
        TableType dictTable = InitTable(res);
        m_systemMap[res.GetType()] = dictTable;
        if (m_mapDefines.ContainsKey(res.GetType()))
        {
            Dictionary<string, MapDefine> maps = m_mapDefines[res.GetType()];
            var enumerator = maps.GetEnumerator();
            while (enumerator.MoveNext())
            {
                _GenMap(res.GetType(), enumerator.Current.Key, enumerator.Current.Value);
            }
        }
    }

    public TableType GetTable<T>()
    {
        return GetTable(typeof(T));
    }

    public void UnloadTable<T>()
    {
        var t = typeof(T);
        if (m_systemMap.ContainsKey(t))
        {
            m_systemMap.Remove(t);
        }

        if (m_mapDefines.ContainsKey(t))
        {
            m_mapDefines.Remove(t);
        }

        if (m_type2MapNamesDict.ContainsKey(t))
        {
            var nameList = m_type2MapNamesDict[t];
            for (int i = 0; i < nameList.Count; i++)
            {
                var k = nameList[i];
                if (m_map.ContainsKey(k))
                {
                    m_map.Remove(k);
                }
            }

            m_type2MapNamesDict.Remove(t);
        }
    }

    public TableType GetTable(Type type)
    {
        if (m_systemMap.ContainsKey(type))
        {
            return m_systemMap[type];
        }
        object data = Load(type);
        if (data == null)
        {
            return null;
        }
        TableType dictTable = InitTable(data as ProtoBuf.IExtensible);
        m_systemMap[type] = dictTable;
        if (m_mapDefines.ContainsKey(type))
        {
            Dictionary<string, MapDefine> maps = m_mapDefines[type];
            var enm = maps.GetEnumerator();
            while (enm.MoveNext())
            {
                var entry = enm.Current;
                _GenMap(type, entry.Key, entry.Value);
            }
        }
        return dictTable;
    }

    public ProtoBuf.IExtensible GetEntry<T>(object key)
        where T : ProtoBuf.IExtensible
    {
        return GetEntry(key, typeof(T));
    }

    public ProtoBuf.IExtensible GetEntry(object key, Type type)
    {
        TableType table = GetTable(type);
        object nkey = changeKey(key);
        if (table == null)
        {
            return null;
        }
        if (table.ContainsKey(nkey))
        {
            return table[nkey];
        }
        return null;
    }

    private Dictionary<Type, List<string>> m_type2MapNamesDict = new Dictionary<Type, List<string>>();

    void _GenMap(Type type, string mapName, MapDefine mapDefine)
    {
        List<string> mapNameList = null;
        if (!m_type2MapNamesDict.ContainsKey(type))
        {
            mapNameList = new List<string>(8);
            m_type2MapNamesDict[type] = mapNameList;
        }
        else
        {
            mapNameList = m_type2MapNamesDict[type];
        }
        mapNameList.Add(mapName);

        TableType table = m_systemMap[type];
        MapData mapData = new MapData();
        mapData.type = type;
        Dictionary<object, object> mapDic = mapData.map;
        m_map[mapName] = mapData;
        var enm = table.GetEnumerator();
        while (enm.MoveNext())
        {
            var key = enm.Current.Key;
            Dictionary<object, object> dic = mapDic;
            object entry = enm.Current.Value;
            Type entryType = entry.GetType();
            for (int i = 0; i < mapDefine.keys.Length; ++i)
            {
                string cellName = mapDefine.keys[i];
                System.Reflection.PropertyInfo objProperty = entryType.GetProperty(cellName);
                object cellValue = objProperty.GetValue(entry, null);
                if (i == mapDefine.keys.Length - 1)
                {
                    List<object> list = null;
                    if (dic.ContainsKey(cellValue))
                    {
                        list = (List<object>)dic[cellValue];
                    }
                    else
                    {
                        list = new List<object>();
                        dic[cellValue] = list;
                    }
                    list.Add(key);
                }
                else
                {
                    if (!dic.ContainsKey(cellValue))
                    {
                        dic[cellValue] = new Dictionary<object, object>();
                    }
                    dic = (Dictionary<object, object>)dic[cellValue];
                }
            }
        }
    }

    private void GenMap(string mapName, string[] keys, Type type)
    {
        MapDefine mapDefine = new MapDefine();
        mapDefine.keys = keys;
        GetTable(type);

        if (!m_mapDefines.ContainsKey(type))
        {
            m_mapDefines[type] = new Dictionary<string, MapDefine>();
        }
        m_mapDefines[type][mapName] = mapDefine;
        if (m_systemMap.ContainsKey(type))
        {
            _GenMap(type, mapName, mapDefine);
        }
    }
    public void GenMap<T>(string mapName, params string[] args)
        where T : ProtoBuf.IExtensible
    {
        GenMap(mapName, args, typeof(T));
    }

    public void GenMap(string mapName, Type type, params string[] args)
    {
        GenMap(mapName, args, type);
    }

    public List<object> GetListFromMap(string mapName, object[] keys)
    {
        MapData mapData = m_map[mapName];
        Dictionary<object, object> dic = mapData.map;
        for (int i = 0; i < keys.Length; ++i)
        {
            object cellValue = keys[i];
            if (!dic.ContainsKey(cellValue))
            {
                return null;
            }
            if (i == keys.Length - 1)
            {
                List<object> list = (List<object>)dic[cellValue];
                return list;
            }
            else
            {
                dic = (Dictionary<object, object>)dic[cellValue];
            }
        }
        return null;
    }

    public List<object> GetListFromMap(string mapName, object key1)
    {
        object[] keys = { key1 };
        return GetListFromMap(mapName, keys);
    }
    public List<object> GetListFromMap(string mapName, object key1, object key2)
    {
        object[] keys = { key1, key2 };
        return GetListFromMap(mapName, keys);
    }
    public List<object> GetListFromMap(string mapName, object key1, object key2, object key3)
    {
        object[] keys = { key1, key2, key3 };
        return GetListFromMap(mapName, keys);
    }

    public ProtoBuf.IExtensible GetEntryFromMap(string mapName, object[] keys)
    {
        List<object> list = GetListFromMap(mapName, keys);
        if (list == null)
        {
            return null;
        }
        MapData mapData = m_map[mapName];
        return m_systemMap[mapData.type][list[0]];
    }
    public ProtoBuf.IExtensible GetEntryFromMap(string mapName, object key1)
    {
        object[] keys = { key1 };
        return GetEntryFromMap(mapName, keys);
    }
    public ProtoBuf.IExtensible GetEntryFromMap(string mapName, object key1, object key2)
    {
        object[] keys = { key1, key2 };
        return GetEntryFromMap(mapName, keys);
    }

    public ArrayList GetKeysFromMap(string mapName, object[] keys = null)
    {
        MapData mapData = m_map[mapName];

        Dictionary<object, object> dic = mapData.map;
        if (keys != null)
        {
            for (int i = 0; i < keys.Length; ++i)
            {
                object cellValue = keys[i];
                if (!dic.ContainsKey(cellValue))
                {
                    return null;
                }
                dic = (Dictionary<object, object>)dic[cellValue];
            }
        }
        var array = new ArrayList();
        var enm = dic.GetEnumerator();
        while (enm.MoveNext())
        {
            array.Add(enm.Current.Key);
        }
        return array;
    }

    public ArrayList GetKeysFromMap(string mapName, object key1)
    {
        object[] keys = { key1 };
        return GetKeysFromMap(mapName, keys);
    }
    public ArrayList GetKeysFromMap(string mapName, object key1, object key2)
    {
        object[] keys = { key1, key2 };
        return GetKeysFromMap(mapName, keys);
    }
    #region read file 

    // 读取streamassets目录中的文件
    public static byte[] ReadFile(string path)
    {
        byte[] b = null;
        if (Application.platform == RuntimePlatform.Android && path.Contains(m_streamAssets))
        {
            b = EZFunFileUtil.ReadFile(path);
        }
        else
        {
            b = ReadFileStream(path);
        }

        return b;
    }

    public static byte[] ReadFileStream(string path)
    {
        byte[] b = null;
        if (File.Exists(path))
        {
            using (Stream file = File.OpenRead(path))
            {
                b = new byte[(int)file.Length];
                file.Read(b, 0, b.Length);
                file.Close();
                file.Dispose();
            }
        }
        if (b == null)
            Debug.LogError("ReadFileStream Read file failed! " + path);
        return b;
    }
    #endregion
}


